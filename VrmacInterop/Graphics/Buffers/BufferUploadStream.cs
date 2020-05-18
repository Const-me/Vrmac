using ComLight;
using ComLight.IO;
using System;
using System.IO;

namespace Diligent.Graphics.Buffers
{
	/// <summary>A write-only stream to upload larger buffers to GPU.</summary>
	public class BufferUploadStream: Stream
	{
		readonly iBufferUploadStream stream;

		internal BufferUploadStream( IntPtr nativeComPointer )
		{
			stream = NativeWrapper.wrap<iBufferUploadStream>( nativeComPointer );
		}

		/// <summary>Returns false</summary>
		public override bool CanRead => false;

		/// <summary>Returns true</summary>
		public override bool CanSeek => true;

		/// <summary>Returns true</summary>
		public override bool CanWrite => true;

		/// <summary>Capacity of the native buffer</summary>
		public override long Length
		{
			get
			{
				stream.getLength( out uint length );
				return length;
			}
		}

		/// <summary>Gets or sets the position within the current stream.</summary>
		public override long Position
		{
			get
			{
				stream.getPosition( out uint position );
				return position;
			}
			set => stream.seek( checked((uint)value), eSeekOrigin.Begin );
		}

		/// <summary>Does nothing; this stream doesn't do any IO, it just copies data from manager memory to native</summary>
		public override void Flush()
		{
		}

		/// <summary>The method is not supported</summary>
		public override int Read( byte[] buffer, int offset, int count )
		{
			throw new NotSupportedException();
		}

		/// <summary>Sets the position within the current stream.</summary>
		public override long Seek( long offset, SeekOrigin origin )
		{
			eSeekOrigin so = (eSeekOrigin)(byte)origin;
			stream.seek( checked((uint)offset), so );
			return Position;
		}

		/// <summary>The method is not supported</summary>
		public override void SetLength( long value )
		{
			throw new NotSupportedException();
		}

		/// <summary>Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
		public override void Write( byte[] buffer, int offset, int count )
		{
			unsafe
			{
				fixed ( byte* bufferPointer = buffer )
				{
					byte* destPointer = bufferPointer + offset;
					stream.write( (IntPtr)destPointer, count );
				}
			}
		}

		/// <summary>Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
		public override void Write( ReadOnlySpan<byte> buffer )
		{
			unsafe
			{
				fixed ( byte* bufferPointer = &buffer.GetPinnableReference() )
				{
					stream.write( (IntPtr)bufferPointer, buffer.Length );
				}
			}
		}

		/// <summary> Releases the unmanaged resources used by the System.IO.Stream and optionally releases the managed resources.</summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
				stream?.Dispose();
			base.Dispose( disposing );
		}
	}
}