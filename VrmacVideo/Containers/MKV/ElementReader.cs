using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace VrmacVideo.Containers.MKV
{
	ref struct ElementReader
	{
		public readonly Stream stream;
		public readonly long startPosition;
		public readonly long endPosition;
		public bool EOF => stream.Position >= endPosition;
		public ElementReader( Stream stream )
		{
			this.stream = stream;
			long bytesLeft = (long)stream.readUint8();
			startPosition = stream.Position;
			endPosition = startPosition + bytesLeft;
		}

		public eElement readElementId()
		{
			var res = stream.readElementId();
			// Console.WriteLine( res );
			return res;
		}

		public uint readUint( uint defaultValue = 0 )
		{
			uint cb = stream.readUint4();
			if( cb <= 0 )
				return defaultValue;
			if( cb > 4 )
				throw new ArgumentException();

			Span<byte> span4 = stackalloc byte[ 4 ];
			BinaryPrimitives.WriteInt32LittleEndian( span4, 0 );
			stream.read( span4.Slice( 4 - (int)cb ) );
			return BinaryPrimitives.ReadUInt32BigEndian( span4 );
		}

		public byte readByte( byte defaultValue = 0 )
		{
			uint cb = stream.readUint4();
			if( cb <= 0 )
				return defaultValue;
			if( cb > 1 )
				throw new ArgumentOutOfRangeException();
			int res = stream.ReadByte();
			if( res < 0 )
				throw new EndOfStreamException();
			return (byte)res;
		}

		public ulong readUlong( ulong defaultValue = 0 )
		{
			uint cb = stream.readUint4();
			if( cb <= 0 )
				return defaultValue;
			if( cb > 8 )
				throw new ArgumentException();

			Span<byte> span8 = stackalloc byte[ 8 ];
			BinaryPrimitives.WriteInt64LittleEndian( span8, 0 );
			stream.read( span8.Slice( 8 - (int)cb ) );
			return BinaryPrimitives.ReadUInt64BigEndian( span8 );
		}

		public string readUtf8()
		{
			int cb = (int)stream.readUint4();
			if( cb > 2048 )
				throw new ArgumentOutOfRangeException();
			Span<byte> span = stackalloc byte[ cb ];
			stream.read( span );
			return Encoding.UTF8.GetString( span );
		}

		public string readAscii()
		{
			int cb = (int)stream.readUint4();
			if( cb > 2048 )
				throw new ArgumentOutOfRangeException();
			Span<byte> span = stackalloc byte[ cb ];
			stream.read( span );
			return Encoding.ASCII.GetString( span );
		}

		public void skipElement()
		{
			ulong cb = stream.readUint8();
			stream.Seek( (long)cb, SeekOrigin.Current );
		}

		public byte[] readByteArray()
		{
			uint cb = stream.readUint4();
			if( cb > 0x8000 )
				throw new ArgumentOutOfRangeException();
			byte[] result = new byte[ cb ];
			stream.read( result.AsSpan() );
			return result;
		}

		public double readFloat( double defaultValue = 0 )
		{
			uint cb = stream.readUint4();
			switch( cb )
			{
				case 0:
					return defaultValue;
				case 4:
					Span<byte> span4 = stackalloc byte[ 4 ];
					stream.read( span4 );
					int i4 = BinaryPrimitives.ReadInt32BigEndian( span4 );
					return BitConverter.Int32BitsToSingle( i4 );
				case 8:
					Span<byte> span8 = stackalloc byte[ 8 ];
					stream.read( span8 );
					long i8 = BinaryPrimitives.ReadInt64BigEndian( span8 );
					return BitConverter.Int64BitsToDouble( i8 );
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public int readInt( int defaultValue = 0 )
		{
			uint cb = stream.readUint4();
			if( cb <= 0 )
				return defaultValue;
			if( cb > 4 )
				throw new ArgumentException();

			// Read uint32
			Span<byte> span4 = stackalloc byte[ 4 ];
			BinaryPrimitives.WriteInt32LittleEndian( span4, 0 );
			stream.read( span4.Slice( 4 - (int)cb ) );
			uint ui = BinaryPrimitives.ReadUInt32BigEndian( span4 );

			// Sign extend
			uint signMask = uint.MaxValue << (int)( cb * 8 - 1 );
			if( 0 != ( ui | signMask ) )
				ui |= signMask;

			return unchecked((int)ui);
		}

		public uint readColorSpace()
		{
			uint cb = stream.readUint4();
			if( cb != 4 )
				throw new ArgumentException();
			Span<byte> span4 = stackalloc byte[ 4 ];
			stream.read( span4 );
			return BinaryPrimitives.ReadUInt32LittleEndian( span4 );
		}

		public Guid readGuid()
		{
			uint cb = stream.readUint4();
			if( cb != 16 )
				throw new ArgumentException();

			Span<byte> span = stackalloc byte[ 16 ];
			stream.read( span );
			return new Guid( span );
		}

		static readonly DateTime epoch = new DateTime( 2001, 1, 1, 0, 0, 0, DateTimeKind.Utc );

		public DateTime readDate()
		{
			uint cb = stream.readUint4();
			if( 0 == cb )
				return epoch;
			if( 8 != cb )
				throw new ArgumentException();
			Span<byte> span = stackalloc byte[ 8 ];
			stream.read( span );
			long nano = BinaryPrimitives.ReadInt64BigEndian( span );
			long ticks = nano / 100;
			return epoch + TimeSpan.FromTicks( ticks );
		}
	}
}