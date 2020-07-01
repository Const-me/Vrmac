using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VrmacVideo.Containers.MP4.ElementaryStream
{
	/// <summary>H264 byte stream reader</summary>
	ref struct Reader
	{
		ReadOnlySpan<byte> source;
		int offset;

		public Reader( ReadOnlySpan<byte> source )
		{
			this.source = source;
			offset = 0;
		}

		public bool EOF => offset >= source.Length;

		public byte readByte()
		{
			if( offset < source.Length )
				return source[ offset++ ];
			throw new EndOfStreamException();
		}

		public eDescriptorTag readTag() => (eDescriptorTag)readByte();

		/// <summary>Read variable-length size.</summary>
		/// <remarks>Mpeg-4 developers have probably copy-pasted MultiByteInt31 structure documented in 2.1.2.4 of [MC-NBFX] ".NET Binary Format: XML Data Structure" spec by Microsoft, flipping endianness to hide that fact.</remarks>
		public int readSize()
		{
			int size = 0;
			for( int i = 0; i < 4; i++ )
			{
				if( offset >= source.Length )
					throw new EndOfStreamException();
				int val = source[ offset++ ];
				size = ( size << 7 ) | ( val & 0x7F );
				if( 0 == ( val & 0x80 ) )
					break;
			}
			return size;
		}

		public T readStructure<T>() where T : unmanaged
		{
			int cb = Marshal.SizeOf<T>();
			if( cb + offset > source.Length )
				throw new EndOfStreamException();

			T result = source.Slice( offset, cb ).cast<T>()[ 0 ];
			offset += cb;
			return result;
		}

		public void readBytes( Span<byte> span )
		{
			int cb = span.Length;
			if( cb + offset > source.Length )
				throw new EndOfStreamException();
			source.Slice( offset, cb ).CopyTo( span );
			offset += cb;
		}

		/// <summary>Create a reader to read sliced portion of the stream.</summary>
		/// <remarks>Also fast forward the current stream by that count of bytes. The payload of the sub-stream is expected to be read by the returned substream reader.</remarks>
		public Reader readSubStream( int cb )
		{
			if( cb + offset > source.Length )
				throw new EndOfStreamException();
			Reader result = new Reader( source.Slice( offset, cb ) );
			offset += cb;
			return result;
		}

		public ReadOnlySpan<byte> subStreamSpan( int cb )
		{
			if( cb + offset > source.Length )
				throw new EndOfStreamException();
			var result = source.Slice( offset, cb );
			offset += cb;
			return result;
		}

		public int bytesLeft => source.Length - offset;

		public override string ToString() => $"ElementaryStream.Reader: offset { offset }, bytesLeft { bytesLeft }";

		public byte[] ToArray() => source.Slice( offset ).ToArray();
	}
}