using Diligent;
using System;
using VrmacVideo.Linux;

namespace VrmacVideo.IO
{
	/// <summary>Encoded buffers do streaming I/O with memory mapping</summary>
	sealed class EncodedBuffer: BufferBase, IDisposable
	{
		readonly int mappedLength;
		IntPtr mappedAddress;
		sBuffer buffer;
		PlanesArray planes;

		public EncodedBuffer( FileHandle videoDevice, int bufferIndex ) : base( bufferIndex )
		{
			// https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/mmap.html#example-mapping-buffers-in-the-single-planar-api
			buffer = default;
			buffer.type = eBufferType.VideoOutputMPlane;
			buffer.memory = eMemory.MemoryMap;
			buffer.index = bufferIndex;
			buffer.length = 1;
			planes = new PlanesArray( 1 );
			buffer.m.planes = planes;
			videoDevice.call( eControlCode.QUERYBUF, ref buffer );
			// Logger.logVerbose( "eControlCode.QUERYBUF completed.\n\tbuffer: {0}\n\tplane: {1}", buffer, plane );

			ref sPlane plane = ref planes.span[ 0 ];
			mappedAddress = videoDevice.memoryMapInput( plane.union.memoryOffset, plane.length );
			// Logger.logVerbose( "Mapped encoded buffer #{0}: offset {1}, size {2}, mapped pointer 0x{3}", bufferIndex, plane.union.memoryOffset, plane.length, mappedAddress.ToString( "x" ) );

			mappedLength = plane.length;
		}

		void unmapBuffer()
		{
			planes.finalize();
			if( mappedAddress == default )
				return;
			int returnedValue = LibC.munmap( mappedAddress, (UIntPtr)mappedLength );
			mappedAddress = default;
			if( 0 == returnedValue )
				return;
			var ex = LibC.exception( "EncodedBuffer.unmapBuffer", returnedValue );
			Logger.logWarning( "{0}", ex.Message );
		}

		void unmap()
		{
			unmapBuffer();
			GC.SuppressFinalize( this );
		}
		void IDisposable.Dispose()
		{
			unmapBuffer();
			GC.SuppressFinalize( this );
		}
		~EncodedBuffer()
		{
			unmapBuffer();
		}

		/// <summary>Span to write these h264 frames, backed by memory mapping.</summary>
		public Span<byte> span
		{
			get
			{
				if( state != eBufferState.User )
					throw new ApplicationException( $"Buffer #{ bufferIndex } ain’t currently writable, it’s being processed by OS kernel" );
				return Unsafe.writeSpan<byte>( mappedAddress, mappedLength );
			}
		}

		/// <summary>Set count of bytes written into the span for the frame</summary>
		public void setLength( int bytesUsed )
		{
			if( bytesUsed <= 0 || bytesUsed > mappedLength )
				throw new ArgumentOutOfRangeException();
			planes.setBytesUsed( bytesUsed );
		}

		/* const eBufferFlags naluTypeFlags = eBufferFlags.KeyFrame | eBufferFlags.PFrame | eBufferFlags.BFrame;

		public void setType( eNaluType naluType )
		{
			buffer.flags &= ~naluTypeFlags;
			switch( naluType )
			{
				case eNaluType.IDR:
					buffer.flags |= eBufferFlags.KeyFrame;
					break;
			}
		} */

		public void setFlags( eBufferFlags f ) => buffer.flags = f;

		protected override void enqueue( FileHandle videoDevice )
		{
			if( planes.getBytesUsed() <= 0 )
				throw new ApplicationException( "Can’t enqueue an empty buffer; you prolly forgot to call EncodedBuffer.setLength()" );

			// https://www.kernel.org/doc/html/v4.10/media/uapi/v4l/vidioc-qbuf.html
			// When the buffer is intended for output.. applications must also initialize the bytesused, field and timestamp fields,
			buffer.field = eField.Progressive;
			// buffer.timestamp = LibC.gettime( eClock.Monotonic );
			// buffer.flags |= eBufferFlags.TimestampMonotonic;

			// Logger.logVerbose( "QBUF: {0}", buffer );
			videoDevice.call( eControlCode.QBUF, ref buffer );
		}

		protected override void dequeue( FileHandle videoDevice )
		{
			int idx = buffer.index;
			videoDevice.call( eControlCode.DQBUF, ref buffer );
			if( buffer.index != idx )
				throw new ApplicationException( $"EncodedBuffer.dequeue: expecting buffer #{ idx }, got #{ buffer.index }" );

			if( buffer.flags.HasFlag( eBufferFlags.Error ) )
				Logger.logError( "Encoded buffer #{0}: error in the bitstream", buffer.index );
			buffer.flags = default;
			// Logger.logVerbose( "EncodedBuffer.dequeue: {0}", buffer.index );
		}

		public void setTimestamp( TimeSpan timestamp )
		{
			buffer.flags |= eBufferFlags.TimestampCopy;
			buffer.timestamp = timestamp;
		}

		public static void dispose( EncodedBuffer[] buffers )
		{
			if( null == buffers )
				return;
			for( int i = 0; i < buffers.Length; i++ )
			{
				var b = buffers[ i ];
				if( null == b )
					continue;
				b.unmap();
				buffers[ i ] = null;
			}
		}

		public int index => buffer.index;

		public override string queryStatus( FileHandle videoDevice )
		{
			videoDevice.call( eControlCode.QUERYBUF, ref buffer );
			return buffer.ToString();
		}
	}
}