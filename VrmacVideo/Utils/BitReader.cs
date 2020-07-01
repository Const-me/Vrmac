using System;
using System.Diagnostics;
using System.IO;

namespace VrmacVideo
{
	/// <summary>Generic bitstream reader, big endian.</summary>
	ref struct BitReader
	{
		// Inspired by C++ code in Chromium (BSD-like license), mostly these source files:
		// \chromium-master\media\video\h264_bit_reader.cc
		// \chromium-master\media\video\h264_parser.cc
		// Unlike C++, C# is very safe language, no need to check for too many things Chromium does. The .NET will throw runtime exceptions by itself.
		// If you want extra safety e.g. using it for youtube, recompile this DLL with /checked compiler flag.

		ReadOnlySpan<byte> source;
		int offset;
		byte bufferedBits;
		byte bufferedByte;
		ushort previousTwoBytes;

		public BitReader( ReadOnlySpan<byte> source )
		{
			this.source = source;
			offset = 0;
			bufferedBits = 0;
			bufferedByte = 0;
			previousTwoBytes = ushort.MaxValue;
		}

		/// <summary>Read and buffer 8 moar bits from the span.</summary>
		void bufferByte()
		{
			Debug.Assert( bufferedBits == 0 );
			if( offset >= source.Length )
				throw new EndOfStreamException();
			bufferedByte = source[ offset++ ];
			// Deal with these emulation prevention bytes
			if( 3 == bufferedByte && previousTwoBytes == 0 )
			{
				if( offset >= source.Length )
					throw new EndOfStreamException();
				bufferedByte = source[ offset++ ];
				previousTwoBytes = ushort.MaxValue;
			}
			previousTwoBytes = (ushort)( ( previousTwoBytes << 8 ) | bufferedByte );
			bufferedBits = 8;
		}

		/// <summary>Skip and discard bits from the stream</summary>
		public void skipBits( int count )
		{
			while( true )
			{
				if( count <= bufferedBits )
				{
					// We have enough bits buffered. Discard some and return.
					bufferedByte <<= count;
					bufferedBits -= (byte)count;
					return;
				}
				// We don't have enough bits in the buffer. Discard the complete buffer.
				count -= bufferedBits;
				bufferedBits = 0;
				// Read and buffer 8 moar bits.
				bufferByte();
			}
		}

		/// <summary>Read a single bit from the stream</summary>
		public bool readBit()
		{
			if( bufferedBits <= 0 )
				bufferByte();
			bool res = 0 != ( bufferedByte & 0x80 );
			bufferedByte <<= 1;
			bufferedBits--;
			return res;
		}

		/// <summary>Read specified count of bits into integer; the count must be positive, max.count of bits is 32.</summary>
		public int readInt( int countBits )
		{
			if( countBits <= 0 || countBits > 32 )
				throw new ArgumentOutOfRangeException();
			int res = 0;
			while( true )
			{
				if( countBits <= bufferedBits )
				{
					// We have enough bits buffered. Produce the integer value.
					res <<= countBits;
					// While reading stuff, we shift bufferedByte to the left.
					// The following line extracts the topmost `countBits` of that byte
					res |= ( bufferedByte >> ( 8 - countBits ) );

					bufferedByte <<= countBits;
					bufferedBits -= (byte)countBits;
					return res;
				}
				// We don't have enough data bits in the buffer. Consume and flush the buffered byte.
				res <<= bufferedBits;
				res |= ( bufferedByte >> ( 8 - bufferedBits ) );
				countBits -= bufferedBits;
				bufferedBits = 0;
				// Read and buffer 8 moar bits.
				bufferByte();
			}
		}

		/// <summary>Read specified count of bits into integer; the count must be positive, max. 8.</summary>
		public byte readByte( int countBits )
		{
			if( countBits <= 0 || countBits > 8 )
				throw new ArgumentOutOfRangeException();
			return (byte)readInt( countBits );
		}

		/// <summary>h264 and h265 specific: Parse variable-length unsigned Exp-Golomb code</summary>
		public uint unsignedGolomb()
		{
			// Paragraph 9.1 "Parsing process for Exp-Golomb codes" in this spec: https://www.itu.int/rec/T-REC-H.264-201906-I/en
			int leadingZeroBits = 0;
			while( false == readBit() )
				leadingZeroBits++;
			if( 0 == leadingZeroBits )
				return 0;
			uint suffix = (uint)readInt( leadingZeroBits );
			uint baseIndex = ( 1u << leadingZeroBits ) - 1;
			return baseIndex + suffix;
		}

		/// <summary>h264 and h265 specific: Parse variable-length signed Exp-Golomb code</summary>
		public int signedGolomb()
		{
			// Paragraph 9.1.1 "Mapping process for signed Exp-Golomb codes"
			uint uns = unsignedGolomb();
			if( 0 == uns )
				return 0;
			uns++;
			int abs = (int)( uns / 2 );
			if( 0 == ( uns & 1 ) )
				return abs;
			else
				return -abs;
		}

		/// <summary>h264 and h265 specific: skip a variable-length integer</summary>
		public void skipGolomb()
		{
			int leadingZeroBits = 0;
			while( false == readBit() )
				leadingZeroBits++;
			skipBits( leadingZeroBits );
		}

		/// <summary>Discard incomplete buffered byte, making next read call byte-aligned.</summary>
		public void byteAlign()
		{
			bufferedBits = 0;
		}
	}
}