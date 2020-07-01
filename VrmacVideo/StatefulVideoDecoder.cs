using Diligent.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Vrmac;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	/// <summary>Memory-to-Memory Stateful Video Decoder</summary>
	/// <remarks>There’s a spec on kernel.org, unfortunately it differs from reality, dramatically so.
	/// Setup workflow is complete BS, resolution change event never arrives before the output queue is streaming.
	/// Pi4 decoder requires multi-planar API and fails at once with single-plane APIs, despite both input and output queues only using 1 plane / each.
	/// And so on.</remarks>
	/// <seealso href="https://www.kernel.org/doc/html/latest/media/uapi/v4l/dev-decoder.html" />
	sealed class StatefulVideoDecoder: IDisposable, iDecoderEvents
	{
		readonly VideoDevice device;
		readonly int shutdownHandle;
		EncodedQueue encoded;
		IDisposable eventSubscription;
		DecodedQueue decoded;
		DecoderThread m_thread;
		public iDecoderThread thread => m_thread;
		public sDecodedVideoSize decodedSize { get; private set; }

		// The following 2 values only arrive after resolution change event is handled.
		public sPixelFormatMP decodedPixelFormat { get; private set; }
		public Diligent.Graphics.Video.ColorFormat colorFormat { get; private set; }

		static TimeSpan waitForResolutionTimeout => TimeSpan.FromSeconds( 2 );

		public StatefulVideoDecoder( VideoDevice device, int shutdownHandle )
		{
			this.device = device;
			this.shutdownHandle = shutdownHandle;
		}

		public void Dispose()
		{
			eventSubscription?.Dispose();

			// dbgSaveTga?.Dispose();
			decoded?.Dispose();
			decoded = null;

			encoded?.Dispose();
			encoded = null;
		}

		void dbgPrintEncodedFormats()
		{
			Logger.logVerbose( "Video output formats:" );
			foreach( var f in device.enumerateFormats( eBufferType.VideoOutputMPlane ) )
				Logger.logVerbose( "{0}", f );
		}

		public void initialize( iVideoTrack videoTrack, int encodedBuffersCount )
		{
			// dbgPrintEncodedFormats();

			// Determine format of the encoded video
			sPixelFormatMP encodedFormat = videoTrack.getEncodedFormat();
			// Set format of the first and the only plane of the compressed video.
			encodedFormat.numPlanes = 1;
			encodedFormat.setPlaneFormat( 0, new sPlanePixelFormat() { sizeImage = EncodedQueue.encodedVideoBufferSize( videoTrack ) } );

			// 4.5.1.5. Initialization

			// 1. Set the coded format on OUTPUT via VIDIOC_S_FMT()
			sStreamDataFormat sdf = new sStreamDataFormat()
			{
				bufferType = eBufferType.VideoOutputMPlane,
				pix_mp = encodedFormat
			};
			device.file.call( eControlCode.S_FMT, ref sdf );

			// Logger.logVerbose( "eControlCode.S_FMT completed OK for encoded format: {0}", sdf.pix_mp );

			// 2 Allocate source (bytestream) buffers via VIDIOC_REQBUFS() on OUTPUT
			encoded = new EncodedQueue( device, encodedBuffersCount, ref encodedFormat );

			// 3 Start streaming on the OUTPUT queue via VIDIOC_STREAMON()
			device.startStreaming( eBufferType.VideoOutputMPlane );

			// Continue queuing/dequeuing bytestream buffers to/from the OUTPUT queue via VIDIOC_QBUF() and VIDIOC_DQBUF().
			// The buffers will be processed and returned to the client in order, until required metadata to configure the CAPTURE queue are found.
			// This is indicated by the decoder sending a V4L2_EVENT_SOURCE_CHANGE event with changes set to V4L2_EVENT_SRC_CH_RESOLUTION.
			eventSubscription = new EventSubscription( device );

			// Linux kernel on Pi4 appears to be too old and does not implement that spec. The event never arrives, while the encoded buffers are stuck in the Queued state.
			// For this reason, we have to deal with dynamic resolution changes instead :-(
			// WaitForResolution.wait( device, encoded, reader, waitForResolutionTimeout );
		}

		static sPixelFormatMP computeDecodedFormat( ref sDecodedVideoSize decodedSize )
		{
			if( decodedSize.chromaFormat != eChromaFormat.c420 )
				throw new NotImplementedException( "So far, the library only supports 4:2:0 chroma sampling" );

			// Apparently, the hardware decoder of the Pi4 can't crop video. Not a huge deal, will crop while rendering NV12 into RGB.
			// You would expect you need to pass decodedSize.size here, but no, Linux only plays the video when cropped size is passed there.
			// The size of the output buffers actually created by that Linux ain't cropped. Crazy stuff.
			CSize px = decodedSize.cropRect.size;

			// Set stride to be a multiple of 4 bytes, GLES requirement on Pi4
			int stride = ( px.cx + 3 ) & ( ~3 );

			sPixelFormatMP pmp = new sPixelFormatMP()
			{
				size = px,
				pixelFormat = ePixelFormat.NV12,
				field = eField.Progressive,
				colorSpace = eColorSpace.BT709,
				numPlanes = 2,
				encoding = (byte)eYCbCrEncoding.BT709,
				quantization = eQuantization.FullRange,
				transferFunction = eTransferFunction.BT_709,
			};

			pmp.setPlaneFormat( 0, new sPlanePixelFormat() { sizeImage = px.cy * stride, bytesPerLine = stride } );
			pmp.setPlaneFormat( 1, new sPlanePixelFormat() { sizeImage = px.cy * stride / 2, bytesPerLine = stride } );
			return pmp;
		}

		public void captureSetup( iVideoTrack videoTrack, int decodedBuffersCount, sDecodedVideoSize decodedSize )
		{
			this.decodedSize = decodedSize;
			if( pendingFrames.capacity != decodedBuffersCount )
				pendingFrames = new PendingFrames( decodedBuffersCount );

			// Set decoded format. Pi4 Linux failed to implement V4L2 stateful decoder setup workflow, instead computing everything manually, from parsed SPS.
			sPixelFormatMP sdf = computeDecodedFormat( ref decodedSize );
			device.setDataFormat( eBufferType.VideoCaptureMPlane, ref sdf );

			// Apparently, Pi4 hardware or drivers is unable to process S_SELECTION request and crop the video. Cropping it later while rendering NV12 into RGB.
			/* sSelection selection = default;
			selection.type = eBufferType.VideoCaptureMPlane;
			selection.target = eSelectionTarget.Compose;
			selection.flags = eSelectionFlags.LesserOrEqual;
			selection.rect = decodedSize.cropRect;
			device.file.call( eControlCode.S_SELECTION, ref selection );
			device.file.call( eControlCode.G_SELECTION, ref selection );
			CRect selectedRect = selection.rect;
			if( selectedRect == decodedSize.cropRect )
				Logger.logVerbose( "Video cropping: decoded size {0}, cropped to {1}", decodedSize.size, selectedRect );
			else
				Logger.logInfo( "Video cropping: decoded size {0}, asked to crop to {1}, GPU driver replaced with {2}", decodedSize.size, decodedSize.cropRect, selectedRect ); */

			sdf = device.getDataFormat( eBufferType.VideoCaptureMPlane );
			decoded = new DecodedQueue( device, decodedBuffersCount );
			// decoded.exportTextures( renderDev, device, ref sdf );

			// Start streaming of the output queue
			device.startStreaming( eBufferType.VideoCaptureMPlane );
		}

		public void startDecoder( iMediaFile media, Audio.iDecoderQueues audioQueues )
		{
			iVideoTrackReader reader = encoded.enqueueInitial( media.videoTrack );
			var audioReader = media.audioTrack.createReader();

			// Launch the thread
			m_thread = new DecoderThread( device, reader, encoded, decoded, shutdownHandle, this,
				audioReader, audioQueues );
		}

		readonly object syncRoot = new object();
		PendingFrames pendingFrames;

		void iDecoderEvents.onFrameDecoded( DecodedBuffer buffer )
		{
			lock( syncRoot )
			{
				pendingFrames.insert( buffer.timestamp, buffer.bufferIndex );
			}
		}

		void iDecoderEvents.onEndOfStream()
		{
			Logger.logInfo( "iDecoderEvents.onEndOfStream" );
		}

		// Called on decoder thread
		void iDecoderEvents.onDynamicResolutionChange()
		{
			sSelection selection = default;
			selection.type = eBufferType.VideoCaptureMPlane;
			selection.target = eSelectionTarget.Compose;
			device.file.call( eControlCode.G_SELECTION, ref selection );
			// Logger.logInfo( "selection: {0}", selection );  // Appears to be correct, i.e. matches what's in the PPS of the video
			if( selection.rect != decodedSize.cropRect )
				throw new ApplicationException( $"Linux failed to decode SPS from the video; SPS says the crop rectangle is { decodedSize.cropRect }, Linux decoded as { selection.rect }" );

			sStreamDataFormat sdf = new sStreamDataFormat { bufferType = eBufferType.VideoCaptureMPlane };
			device.file.call( eControlCode.G_FMT, ref sdf );
			Logger.logVerbose( "Automatically selected format: {0}", sdf );
			decodedPixelFormat = sdf.pix_mp;
			colorFormat = sdf.pix_mp.colorFormat();
			Logger.logInfo( "Dynamic resolution change: {0}", colorFormat );
			return;
			// The following code causes endless loop of resolution changes, despite nothing being changed, really

			// state = eDecoderState.DrainRezChange;

			// The setup workflow in that V4L spec is BS, unfortunately :-(

			device.stopStreaming( eBufferType.VideoCaptureMPlane );
			int decodedBuffersCount = decoded.buffersCount;
			decoded.Dispose();
			decoded = null;

			sImageFormatDescription format = device.findOutputFormat();
			Logger.logVerbose( "Picked the format \"{0}\", {1}, flags {2}", format.description, format.pixelFormat, format.flags );


			// Destroy the old decoded buffers
			sRequestBuffers rbDecoded = new sRequestBuffers()
			{
				type = eBufferType.VideoCaptureMPlane,
				memory = eMemory.MemoryMap
			};
			device.file.call( eControlCode.REQBUFS, ref rbDecoded );

			// Fix a few things there
			sdf.pix_mp.pixelFormat = ePixelFormat.NV12; // This one is actually pre-selected, prolly because of the initial one we set in the captureSetup method
			sdf.pix_mp.quantization = eQuantization.FullRange;  // Linux defaults to limited range, not what we want.
			device.file.call( eControlCode.S_FMT, ref sdf );
			// Logger.logVerbose( "Set format: {0}", sdf );

			// Create new set of decoded buffers, same count as before
			decoded = new DecodedQueue( device, decodedBuffersCount );

			// Finally, resume the video
			device.startStreaming( eBufferType.VideoCaptureMPlane );
			decoded.enqueueAll();
		}

		/// <summary>Called by GUI thread after it rendered a frame, to post the buffer back to the V4L output queue</summary>
		public void enqueueDecodedBuffer( int index ) => decoded.enqueueByIndex( index );

		public bool needsTextures => null == decoded.textures;

		// public SaveTgaFrames dbgSaveTga { get; private set; }

		public Nv12Texture[] exportTextures( IRenderDevice renderDevice )
		{
			var fmt = decodedPixelFormat;
			var color = colorFormat;
			decoded.exportTextures( renderDevice, device, ref fmt, ref color );

			// dbgSaveTga = decoded.dbgMapOutput( device, ref fmt );
			return decoded.textures;
		}

		/// <summary>If there’s a decoded frame available return it’s presentation time, otherwise null.</summary>
		public TimeSpan? nextFramePresentationTime
		{
			get
			{
				lock( syncRoot )
				{
					if( pendingFrames.any )
						return pendingFrames.firstTimestamp();
					return null;
				}
			}
		}

		/// <summary>If there’s a decoded frame available, remove it from the ready queue and return the buffer index.
		/// Otherwise throw an exception.</summary>
		public int dequeDecoded()
		{
			lock( syncRoot )
			{
				if( pendingFrames.any )
					return pendingFrames.removeFirst();
				throw new ApplicationException( "You should only call iMediaEngine.transferVideoFrame or iVideoRenderState.render after you got the `true` return value from onVideoStreamTick() method." );
			}
		}

		public ITextureView textureView( int frame ) => decoded.textures[ frame ].view;

		void iDecoderEvents.discardDecoded()
		{
			lock( syncRoot )
			{
				foreach( var v in pendingFrames.Values )
					decoded.enqueueByIndex( v );
				pendingFrames.clear();
			}
		}
	}
}