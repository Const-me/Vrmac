using System;
using System.Buffers.Binary;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>EBML, short for Extensible Binary Meta Language, specifies a binary and octet (byte) aligned format inspired by the principle of XML (a framework for structuring data).</summary>
	/// <seealso href="https://github.com/cellar-wg/ebml-specification/blob/master/specification.markdown" />
	public static class Primitives
	{
		/// <summary>Read element ID from stream</summary>
		public static eElement readElementId( this Stream stream )
		{
			int i = stream.ReadByte();
			if( i < 0 )
				throw new EndOfStreamException();
			if( 0 != ( i & 0x80 ) )
			{
				// 1 bit header, 7 bits payload
				return (eElement)( i );
			}
			if( ( i & 0xC0 ) == 0x40 )
			{
				// 2 bits header, 14 bits payload
				int low = stream.ReadByte();
				if( low < 0 )
					throw new EndOfStreamException();
				return (eElement)( ( i << 8 ) | low );
			}

			Span<byte> span4 = stackalloc byte[ 4 ];

			if( ( i & 0xE0 ) == 0x20 )
			{
				// 3 bits header, 21 bits payload
				stream.read( span4.Slice( 2 ) );
				span4[ 0 ] = 0;
				span4[ 1 ] = (byte)i;
				return (eElement)BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF0 ) == 0x10 )
			{
				// 4 bits header, 28 bits payload
				stream.read( span4.Slice( 1 ) );
				span4[ 0 ] = (byte)i;
				return (eElement)BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF8 ) == 8 )
			{
				// 5 bits header, 35 bits payload
				if( 0 != ( i & 7 ) )
					throw new ArgumentOutOfRangeException();
				stream.read( span4 );
				return (eElement)BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			throw new ArgumentOutOfRangeException();
		}

		/// <summary>Read 4-bytes unsigned integer</summary>
		public static uint readUint4( this Stream stream )
		{
			int i = stream.ReadByte();
			if( i < 0 )
				throw new EndOfStreamException();
			if( 0 != ( i & 0x80 ) )
			{
				// 1 bit header, 7 bits payload
				return (uint)( i & 0x7F );
			}
			if( ( i & 0xC0 ) == 0x40 )
			{
				// 2 bits header, 14 bits payload
				int low = stream.ReadByte();
				if( low < 0 )
					throw new EndOfStreamException();
				return (uint)( ( ( i & 0x3F ) << 8 ) | low );
			}

			Span<byte> span4 = stackalloc byte[ 4 ];

			if( ( i & 0xE0 ) == 0x20 )
			{
				// 3 bits header, 21 bits payload
				stream.read( span4.Slice( 2 ) );
				span4[ 0 ] = 0;
				span4[ 1 ] = (byte)( i & 0x1F );
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF0 ) == 0x10 )
			{
				// 4 bits header, 28 bits payload
				stream.read( span4.Slice( 1 ) );
				span4[ 0 ] = (byte)( i & 0xF );
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF8 ) == 8 )
			{
				// 5 bits header, 35 bits payload
				if( 0 != ( i & 7 ) )
					throw new ArgumentOutOfRangeException();
				stream.read( span4 );
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			throw new ArgumentOutOfRangeException();
		}

		/// <summary>Read 4-bytes unsigned integer</summary>
		public static uint readUint4( this Stream stream, out int bytesCount )
		{
			int i = stream.ReadByte();
			if( i < 0 )
				throw new EndOfStreamException();
			if( 0 != ( i & 0x80 ) )
			{
				// 1 bit header, 7 bits payload
				bytesCount = 1;
				return (uint)( i & 0x7F );
			}
			if( ( i & 0xC0 ) == 0x40 )
			{
				// 2 bits header, 14 bits payload
				int low = stream.ReadByte();
				if( low < 0 )
					throw new EndOfStreamException();
				bytesCount = 2;
				return (uint)( ( ( i & 0x3F ) << 8 ) | low );
			}

			Span<byte> span4 = stackalloc byte[ 4 ];

			if( ( i & 0xE0 ) == 0x20 )
			{
				// 3 bits header, 21 bits payload
				stream.read( span4.Slice( 2 ) );
				span4[ 0 ] = 0;
				span4[ 1 ] = (byte)( i & 0x1F );
				bytesCount = 3;
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF0 ) == 0x10 )
			{
				// 4 bits header, 28 bits payload
				stream.read( span4.Slice( 1 ) );
				span4[ 0 ] = (byte)( i & 0xF );
				bytesCount = 4;
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF8 ) == 8 )
			{
				// 5 bits header, 35 bits payload
				if( 0 != ( i & 7 ) )
					throw new ArgumentOutOfRangeException();
				stream.read( span4 );
				bytesCount = 5;
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			throw new ArgumentOutOfRangeException();
		}

		/// <summary>Read 8-bytes unsigned integer</summary>
		public static ulong readUint8( this Stream stream )
		{
			int i = stream.ReadByte();
			if( i < 0 )
				throw new EndOfStreamException();
			if( 0 != ( i & 0x80 ) )
			{
				// 1 bit header, 7 bits payload
				return (uint)( i & 0x7F );
			}
			if( ( i & 0xC0 ) == 0x40 )
			{
				// 2 bits header, 14 bits payload
				int low = stream.ReadByte();
				if( low < 0 )
					throw new EndOfStreamException();
				return (uint)( ( ( i & 0x3F ) << 8 ) | low );
			}

			Span<byte> span4 = stackalloc byte[ 4 ];

			if( ( i & 0xE0 ) == 0x20 )
			{
				// 3 bits header, 21 bits payload
				stream.read( span4.Slice( 2 ) );
				span4[ 0 ] = 0;
				span4[ 1 ] = (byte)( i & 0x1F );
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF0 ) == 0x10 )
			{
				// 4 bits header, 28 bits payload
				stream.read( span4.Slice( 1 ) );
				span4[ 0 ] = (byte)( i & 0xF );
				return BinaryPrimitives.ReadUInt32BigEndian( span4 );
			}
			if( ( i & 0xF8 ) == 8 )
			{
				// 5 bits header, 35 bits payload
				stream.read( span4 );
				ulong low = BinaryPrimitives.ReadUInt32BigEndian( span4 );
				ulong high = (ulong)( i & 7 );
				return ( high << 32 ) | low;
			}

			Span<byte> span8 = stackalloc byte[ 8 ];
			if( ( i & 0xFC ) == 4 )
			{
				// 6 bits header, 42 bits payload
				BinaryPrimitives.WriteInt16LittleEndian( span8, 0 );
				span8[ 2 ] = (byte)( i & 3 );
				stream.read( span8.Slice( 3 ) );
				return BinaryPrimitives.ReadUInt64BigEndian( span8 );
			}
			if( ( i & 0xFE ) == 2 )
			{
				// 7 bits header, 49 bits payload
				span8[ 0 ] = 0;
				span8[ 1 ] = (byte)( i & 1 );
				stream.read( span8.Slice( 2 ) );
				return BinaryPrimitives.ReadUInt64BigEndian( span8 );
			}
			if( i == 1 )
			{
				// 8 bits header, 56 bits payload
				span8[ 0 ] = 0;
				stream.read( span8.Slice( 1 ) );
				return BinaryPrimitives.ReadUInt64BigEndian( span8 );
			}
			throw new ArgumentOutOfRangeException();
		}

		/// <summary>Parse 4-bytes unsigned integer from a buffer</summary>
		public static uint parseUint4( this ReadOnlySpan<byte> buffer, int offset, out int bytesCount )
		{
			int i = buffer[ offset ];
			if( 0 != ( i & 0x80 ) )
			{
				// 1 bit header, 7 bits payload
				bytesCount = 1;
				return (uint)( i & 0x7F );
			}
			if( ( i & 0xC0 ) == 0x40 )
			{
				// 2 bits header, 14 bits payload
				uint res = BinaryPrimitives.ReadUInt16BigEndian( buffer.Slice( offset ) );
				res &= 0x3FFF;
				bytesCount = 2;
				return res;
			}

			if( ( i & 0xE0 ) == 0x20 )
			{
				// 3 bits header, 21 bits payload
				Span<byte> span4 = stackalloc byte[ 4 ];
				span4[ 3 ] = 0;
				span4[ 2 ] = (byte)( i & 0x1F );
				span4[ 1 ] = buffer[ offset + 1 ];
				span4[ 0 ] = buffer[ offset + 2 ];
				bytesCount = 3;
				return BinaryPrimitives.ReadUInt32LittleEndian( span4 );
			}
			if( ( i & 0xF0 ) == 0x10 )
			{
				// 4 bits header, 28 bits payload
				uint res = BinaryPrimitives.ReadUInt32BigEndian( buffer.Slice( offset ) );
				res &= 0x0FFFFFFF;
				bytesCount = 4;
				return res;
			}
			if( ( i & 0xF8 ) == 8 )
			{
				// 5 bits header, 35 bits payload
				if( 0 != ( i & 7 ) )
					throw new ArgumentOutOfRangeException();
				uint res = BinaryPrimitives.ReadUInt32BigEndian( buffer.Slice( offset + 1 ) );
				bytesCount = 5;
				return res;
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}