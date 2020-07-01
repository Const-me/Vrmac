using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Vrmac.Utils;
using VrmacVideo.IO;

namespace VrmacVideo.Linux
{
	public struct sBuffersSet
	{
		public readonly int firstIndex, count;

		public sBuffersSet( int idx, int c )
		{
			firstIndex = idx;
			count = c;
		}
	}

	public sealed class VideoDevice: IDisposable
	{
		public void Dispose()
		{
			file.dispose();
			GC.SuppressFinalize( this );
		}
		~VideoDevice()
		{
			file.finalize();
		}

		const eFileFlags openDeviceFlags = eFileFlags.O_RDWR | eFileFlags.O_NONBLOCK | eFileFlags.O_CLOEXEC;

		public static VideoDevice open( string path )
		{
			FileHandle file = FileHandle.openFile( path, openDeviceFlags );

			try
			{
				return new VideoDevice( path, file );
			}
			catch( Exception )
			{
				file.dispose();
				throw;
			}
		}

		internal FileHandle file { get; private set; }

		VideoDevice( string path, FileHandle file )
		{
			this.file = file;
			sCapability capability = file.read<sCapability>( eControlCode.QUERYCAP );
			unsafe
			{
				driver = StringMarshal.copy( capability.driver, 16 );
				card = StringMarshal.copy( capability.card, 32 );
				busInfo = StringMarshal.copy( capability.bus_info, 32 );
			}
			deviceCapabilities = capability.capabilities;
			endpointCapabilities = capability.device_caps;

			bufferCaps = BufferCapabilities.getBufferCaps( path, this );
		}

		/// <summary>name of the driver module, e.g. "bcm2835-codec"</summary>
		public string driver { get; }
		/// <summary>name of the card, e.g. "bcm2835-codec-decode"</summary>
		public string card { get; }
		/// <summary>name of the bus, e.g. "platform:bcm2835-codec"</summary>
		public string busInfo { get; }

		/// <summary>capabilities of the physical device as a whole</summary>
		eCapabilityFlags deviceCapabilities { get; }
		/// <summary>capabilities accessed via this particular device (node)</summary>
		eCapabilityFlags endpointCapabilities { get; }

		BufferCapabilities bufferCaps { get; }

		IEnumerable<string> details()
		{
			yield return $"driver: \"{ driver }\"";
			yield return $"card: \"{ card }\"";
			yield return $"busInfo: \"{ busInfo }\"";
			yield return $"endpointCapabilities: { endpointCapabilities }";
			yield return $"bufferCaps.videoOutput: { bufferCaps.videoOutput }";
			yield return $"bufferCaps.videoOutputMPlane: { bufferCaps.videoOutputMPlane }";
		}

		/// <summary>A string for debugger</summary>
		public override string ToString() => details().makeLines();

		internal bool queryBufferCaps( eBufferType bufferType, eMemory type )
		{
			// If you want to query the capabilities with a minimum of side-effects, then this can be called with count set to 0, memory set to V4L2_MEMORY_MMAP and type set to the buffer type.
			// This will free any previously allocated buffers, so this is typically something that will be done at the start of the application.
			sRequestBuffers requestBuffers = new sRequestBuffers();
			requestBuffers.memory = type;
			requestBuffers.type = eBufferType.VideoOutputMPlane;
			return file.tryCall( eControlCode.REQBUFS, ref requestBuffers );
		}

		/// <summary>Enumerate image formats</summary>
		public IEnumerable<sImageFormatDescription> enumerateFormats( eBufferType bufferType )
		{
			sImageFormatDescription desc = new sImageFormatDescription();
			desc.type = bufferType;
			for( int i = 0; true; i++ )
			{
				desc.index = i;
				if( !file.enumerate( eControlCode.ENUM_FMT, ref desc ) )
					yield break;
				yield return desc;
			}
		}

		public sFrameSizeEnum frameSizeFirst( ePixelFormat pixelFormat )
		{
			sFrameSizeEnum fse = new sFrameSizeEnum();
			fse.pixelFormat = pixelFormat;
			file.call( eControlCode.ENUM_FRAMESIZES, ref fse );
			return fse;
		}

		public IEnumerable<sFrameSizeEnum> frameSizeEnum( ePixelFormat pixelFormat )
		{
			sFrameSizeEnum fse = new sFrameSizeEnum();
			fse.pixelFormat = pixelFormat;
			for( int i = 0; true; i++ )
			{
				fse.index = i;
				if( !file.enumerate( eControlCode.ENUM_FRAMESIZES, ref fse ) )
					yield break;
				yield return fse;
			}
		}

		public void setDataFormat( eBufferType bufferType, ref sPixelFormatMP pixelFormat )
		{
			Debug.Assert( bufferType.isMultiPlaneBufferType() );
			sStreamDataFormat sdf = default;
			sdf.bufferType = bufferType;
			sdf.pix_mp = pixelFormat;
			file.call( eControlCode.S_FMT, ref sdf );
		}

		public sPixelFormatMP getDataFormat( eBufferType bufferType )
		{
			Debug.Assert( bufferType.isMultiPlaneBufferType() );
			sStreamDataFormat sdf = default;
			sdf.bufferType = bufferType;
			file.call( eControlCode.G_FMT, ref sdf );
			return sdf.pix_mp;
		}

		public bool tryDataFormat( eBufferType bufferType, sPixelFormatMP pixelFormat )
		{
			Debug.Assert( bufferType.isMultiPlaneBufferType() );
			sStreamDataFormat sdf = default;
			sdf.bufferType = bufferType;
			sdf.pix_mp = pixelFormat;

			return file.tryCall( eControlCode.TRY_FMT, ref sdf );
		}

		public int allocateEncodedBuffers( int encodedBuffers )
		{
			sRequestBuffers rbEncoded = new sRequestBuffers()
			{
				count = encodedBuffers,
				type = eBufferType.VideoOutputMPlane,
				memory = eMemory.MemoryMap
			};
			file.call( eControlCode.REQBUFS, ref rbEncoded );
			// Logger.logVerbose( "Requesting {0} encoded buffers, the driver said {1}", encodedBuffers, rbEncoded.count );
			return rbEncoded.count;
		}

		public int allocateDecodedFrames( int frames )
		{
			sRequestBuffers rbDecoded = new sRequestBuffers()
			{
				count = frames,
				type = eBufferType.VideoCaptureMPlane,
				memory = eMemory.MemoryMap
			};
			file.call( eControlCode.REQBUFS, ref rbDecoded );
			return rbDecoded.count;
		}

		/// <summary>Set buffer count = 0, for both queues</summary>
		public void freeAllBuffers()
		{
			sRequestBuffers rbEncoded = new sRequestBuffers()
			{
				type = eBufferType.VideoOutputMPlane,
				memory = eMemory.MemoryMap
			};
			file.tryCall( eControlCode.REQBUFS, ref rbEncoded );

			sRequestBuffers rbDecoded = new sRequestBuffers()
			{
				type = eBufferType.VideoCaptureMPlane,
				memory = eMemory.MemoryMap
			};
			file.tryCall( eControlCode.REQBUFS, ref rbDecoded );
		}

		/* public sBuffersSet allocateEncodedBuffers( int encodedBuffers, ref sPixelFormatMP format )
		{
			sCreateBuffers buffers = new sCreateBuffers()
			{
				count = encodedBuffers,
				memory = eMemory.MemoryMap,
			};
			buffers.format.bufferType = eBufferType.VideoOutputMPlane;
			buffers.format.pix_mp = format;
			Logger.logVerbose( "allocateEncodedBuffers: {0} buffers\n{1}", encodedBuffers, format );
			file.call( eControlCode.CREATE_BUFS, ref buffers );
			return new sBuffersSet( buffers.index, buffers.count );
		} */

		/* internal sBuffersSet allocateDecodedBuffers( int decodedBuffersCount, ref VideoTextureDesc desc, ref DmaBuffers dma )
		{
			sCreateBuffers buffers = new sCreateBuffers()
			{
				count = decodedBuffersCount,
				memory = eMemory.MemoryMap,
			};
			buffers.format.bufferType = eBufferType.VideoCaptureMPlane;

			sPixelFormatMP pf = default;
			pf.size = desc.lumaSize;
			pf.pixelFormat = ePixelFormat.NV12;
			pf.field = eField.Progressive;
			pf.numPlanes = 2;

			sPlanePixelFormat ppf = default;
			ppf.bytesPerLine = dma.luma.stride;
			ppf.sizeImage = dma.luma.stride * desc.lumaSize.cy;
			pf.setPlaneFormat( 0, ppf );

			ppf.bytesPerLine = dma.chroma.stride;
			ppf.sizeImage = dma.chroma.stride * desc.chromaSize.cy;
			pf.setPlaneFormat( 1, ppf );

			buffers.format.pix_mp = pf;
			file.call( eControlCode.CREATE_BUFS, ref buffers );
			return new sBuffersSet( buffers.index, buffers.count );
		} */

		public bool tryGetEvent( out sEvent evt )
		{
			int res;
			unsafe
			{
				fixed ( sEvent* pointer = &evt )
					res = LibC.ioctl( file, (uint)eControlCode.DQEVENT, pointer );
			}
			if( 0 == res )
			{
				// Got an event
				return true;
			}
			// Got no event, check errno
			int errno = Marshal.GetLastWin32Error();
			if( errno == LibC.ENOENT )
			{
				// Undocumented behavior, BTW, kernel.org docs never say the "no events" condition generates ENOENT code.
				evt = default;
				return false;
			}

			string message = NativeErrorMessages.lookupLinuxError( errno );
			if( null != message )
				throw new COMException( $"I/O control code DQEVENT failed: { message }", LibC.hresultFromLinux( errno ) );
			throw new COMException( $"I/O control code DQEVENT failed: undocumented Linux error code { errno }", LibC.hresultFromLinux( errno ) );
		}

		public void startStreaming( eBufferType bt )
		{
			file.call( eControlCode.STREAMON, ref bt );
		}
		public void stopStreaming( eBufferType bt )
		{
			file.call( eControlCode.STREAMOFF, ref bt );
		}

		public sImageFormatDescription findOutputFormat()
		{
			foreach( sImageFormatDescription f in enumerateFormats( eBufferType.VideoCaptureMPlane ) )
			{
				// Logger.logVerbose( "Available format: {0}", f );
				if( f.pixelFormat == ePixelFormat.NV12 )
					return f;
			}
			throw new ApplicationException( $"Linux has not offered NV12 decoded format for that video" );
		}

		public sExportBuffer exportOutputBuffer( int bufferIndex )
		{
			sExportBuffer eb = new sExportBuffer()
			{
				type = eBufferType.VideoCaptureMPlane,
				index = bufferIndex,
				plane = 0,
				flags = eFileFlags.O_RDONLY | eFileFlags.O_CLOEXEC
			};
			file.call( eControlCode.EXPBUF, ref eb );
			return eb;
		}
	}
}