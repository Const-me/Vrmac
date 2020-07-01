using Diligent.Graphics;
using Diligent.Graphics.Video;
using System;
using Vrmac;
using VrmacVideo.Linux;

namespace VrmacVideo.IO
{
	/// <summary>Decoding streams frames into GL textures, shared with DMA buffer import/export</summary>
	sealed class DecodedBuffer: BufferBase, IDisposable
	{
		sBuffer buffer;
		PlanesArray planes;

		public DecodedBuffer( int bufferIndex, VideoDevice device )
			: base( bufferIndex )
		{
			// Create small unmanaged buffer for our 2 planes
			planes = new PlanesArray( 2 );

			buffer.index = bufferIndex;
			buffer.type = eBufferType.VideoCaptureMPlane;
			buffer.field = eField.Progressive;
			buffer.memory = eMemory.MemoryMap;
			buffer.length = 2;
			buffer.m.planes = planes;

			device.file.call( eControlCode.QUERYBUF, ref buffer );
			// Logger.logVerbose( "Decoded buffer: {0}", buffer );

			buffer.flags |= eBufferFlags.NoCacheInvalidate;
		}

		static CSize chromaSize( CSize lumaSize ) =>
			new CSize( ( lumaSize.cx + 1 ) / 2, ( lumaSize.cy + 1 ) / 2 );

		public VideoTextures exportTextures( iGlesRenderDevice gles, VideoDevice device, ref sPixelFormatMP pixelFormat )
		{
			sExportBuffer eb = new sExportBuffer()
			{
				type = eBufferType.VideoCaptureMPlane,
				index = bufferIndex,
				plane = 0,
				flags = eFileFlags.O_RDONLY | eFileFlags.O_CLOEXEC
			};
			device.file.call( eControlCode.EXPBUF, ref eb );

			sPlanePixelFormat planeFormat = pixelFormat.getPlaneFormat( 0 );

			sDmaBuffer dma = new sDmaBuffer()
			{
				fd = eb.fd,
				offset = 0,
				stride = planeFormat.bytesPerLine,
				imageSize = planeFormat.bytesPerLine * pixelFormat.size.cy,
				sizePixels = pixelFormat.size,
				bufferIndex = bufferIndex
			};
			ITexture luma = gles.importLumaTexture( ref dma );
			Logger.logVerbose( "Exported luma texture: {0}", dma );

			// I asked V4L2 for 2 planes, however QUERYBUF returned a single plane, with the complete NV12 image in it.
			// No big deal, we have EGL_DMA_BUF_PLANE0_OFFSET_EXT for that; that's where the sDmaBuffer.offset field goes.
			dma.offset = dma.imageSize;
			dma.sizePixels = chromaSize( pixelFormat.size );
			dma.imageSize = dma.stride * dma.sizePixels.cy;
			ITexture chroma = gles.importChromaTexture( ref dma );
			Logger.logVerbose( "Exported chroma texture: {0}", dma );

			return new VideoTextures( luma, chroma );
		}

		public sDmaBuffer exportOutputBuffer( VideoDevice device, ref sPixelFormatMP pixelFormat )
		{
			sExportBuffer eb = device.exportOutputBuffer( bufferIndex );
			sPlanePixelFormat planeFormat = pixelFormat.getPlaneFormat( 0 );

			sDmaBuffer dma = new sDmaBuffer()
			{
				fd = eb.fd,
				offset = 0,
				stride = planeFormat.bytesPerLine,
				imageSize = planeFormat.bytesPerLine * pixelFormat.size.cy,
				sizePixels = pixelFormat.size,
				bufferIndex = bufferIndex
			};
			return dma;
		}

		public Nv12Texture exportNv12( iGlesRenderDevice gles, VideoDevice device, ref sPixelFormatMP pixelFormat, ref ColorFormat colorFormat )
		{
			device.file.call( eControlCode.QUERYBUF, ref buffer );

			sDmaBuffer dma = exportOutputBuffer( device, ref pixelFormat );
			ITexture texture = gles.importNv12Texture( ref dma, ref colorFormat );
			// Logger.logVerbose( "Exported NV12 texture: {0}", dma );
			return new Nv12Texture( texture );
		}

		/* public MappedOutput dbgMapOutput( VideoDevice device, ref sPixelFormatMP pixelFormat )
		{
			sPlane plane = planes.span[ 0 ];
			sPlanePixelFormat planeFormat = pixelFormat.getPlaneFormat( 0 );
			IntPtr pointer;
			try
			{
				pointer = device.file.memoryMapOutput( plane.union.memoryOffset, plane.length );
			}
			catch( Exception ex )
			{
				string msg = $"Unable to memory map output buffer: memoryOffset = { plane.union.memoryOffset }, length = { plane.length } ";
				throw new ApplicationException( msg, ex );
			}
			return new MappedOutput( bufferIndex, plane.length, pointer, planeFormat.bytesPerLine, pixelFormat.size );
		} */

		public void Dispose()
		{
			planes.finalize();
			GC.SuppressFinalize( this );
		}

		~DecodedBuffer()
		{
			planes.finalize();
		}

		protected override void enqueue( FileHandle videoDevice )
		{
			videoDevice.call( eControlCode.QBUF, ref buffer );
		}

		protected override void dequeue( FileHandle videoDevice )
		{
			int idx = buffer.index;
			videoDevice.call( eControlCode.DQBUF, ref buffer );
			if( buffer.index != idx )
				throw new ApplicationException( $"DecodedBuffer.dequeue: expecting buffer #{ idx }, got #{ buffer.index }" );
			// Logger.logVerbose( "DecodedBuffer.dequeue: {0}", buffer );
			// Logger.logVerbose( "DecodedBuffer.dequeue: {0}", buffer.index );
		}

		public static void dispose( DecodedBuffer[] buffers )
		{
			if( null == buffers )
				return;
			for( int i = 0; i < buffers.Length; i++ )
			{
				var b = buffers[ i ];
				if( null == b )
					continue;
				b.Dispose();
				buffers[ i ] = null;
			}
		}

		public void logBufferInfo( string what )
		{
			Logger.logVerbose( "{0}: {1}", what, buffer );
		}

		public eBufferFlags flags => buffer.flags;

		public override string queryStatus( FileHandle videoDevice )
		{
			videoDevice.call( eControlCode.QUERYBUF, ref buffer );
			return buffer.ToString();
		}

		public TimeSpan timestamp => buffer.timestamp;
	}
}