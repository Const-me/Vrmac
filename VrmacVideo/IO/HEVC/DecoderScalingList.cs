using System;
using System.Runtime.InteropServices;
using VrmacVideo.Containers.HEVC;

namespace VrmacVideo.IO.HEVC
{
	/// <summary>Scaling list from SPS or PPS of the h265 video, transformed into the integer array for the hardware decoder</summary>
	struct DecoderScalingList
	{
		public const int scalingListBytes = 4064;
		readonly uint[] scalingListData;
		public bool enabled { get; private set; }

		public DecoderScalingList( bool unused )
		{
			scalingListData = new uint[ scalingListBytes / 4 ];
			enabled = false;
		}

		static readonly ushort[,] scalingFactorOffsets = new ushort[ 4, 6 ]
		{
			{    0,   0x0010, 0x0020, 0x0030, 0x0040, 0x0050 },
			{ 0x0060, 0x00A0, 0x00E0, 0x0120, 0x0160, 0x01A0 },
			{ 0x01E0, 0x02E0, 0x03E0, 0x04E0, 0x05E0, 0x06E0 },
			{ 0x07E0,    0,      0,   0x0BE0,    0,      0   },
		};

		interface iSizeID
		{
			int computeIndex( int x, int y );
		}
		struct Size0: iSizeID
		{
			public int computeIndex( int x, int y ) => ( y << 2 ) + x;
		}
		struct Size1: iSizeID
		{
			public int computeIndex( int x, int y ) => ( y << 3 ) + x;
		}
		struct Size2: iSizeID
		{
			public int computeIndex( int x, int y ) => ( ( y >> 1 ) << 3 ) + ( x >> 1 );
		}
		struct Size3: iSizeID
		{
			public int computeIndex( int x, int y ) => ( ( y >> 2 ) << 3 ) + ( x >> 2 );
		}

		void update<S>( ScalingList source, byte sizeID, byte matrixID, S s ) where S : struct, iSizeID
		{
			Span<byte> span = MemoryMarshal.Cast<uint, byte>( scalingListData.AsSpan() );

			int indexOffset = scalingFactorOffsets[ sizeID, matrixID ];
			int blockSize = 4 << sizeID;

			for( int x = 0; x < blockSize; x++ )
			{
				for( int y = 0; y < blockSize; y++ )
				{
					int destIdx = indexOffset + x + y * blockSize;
					int sourceIdx = s.computeIndex( x, y );
					span[ destIdx ] = source.scalingList[ sizeID, matrixID, sourceIdx ];
				}
			}
			if( sizeID > 1 )
				span[ indexOffset ] = source.dcCoeffs[ sizeID - 2, matrixID ];
		}

		void update( ScalingList source, byte sizeID, byte matrixID )
		{
			switch( sizeID )
			{
				case 0:
					update( source, sizeID, matrixID, new Size0() );
					break;
				case 1:
					update( source, sizeID, matrixID, new Size1() );
					break;
				case 2:
					update( source, sizeID, matrixID, new Size2() );
					break;
				case 3:
					update( source, sizeID, matrixID, new Size3() );
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void initialize( ScalingList source )
		{
			for( byte size = 0; size < 3; size++ )
				for( byte matrix = 0; matrix < 6; matrix++ )
					update( source, size, matrix );
			enabled = true;
		}

		public void disable()
		{
			enabled = false;
		}

		public bool write( Span<uint> dest )
		{
			if( !enabled )
				return false;
			scalingListData.AsSpan().CopyTo( dest );
			return true;
		}
	}
}