using System;

namespace VrmacVideo.Containers.HEVC
{
	sealed class ScalingList
	{
		public readonly byte[,,] scalingList = new byte[ 4, 6, 64 ];
		public readonly byte[,] dcCoeffs = new byte[ 2, 6 ];

		void setRow( int outer, int inner, int offset, ReadOnlySpan<byte> data )
		{
			for( int i = 0; i < 16; i++ )
				scalingList[ outer, inner, i + offset ] = data[ i ];
		}

		// Table 7-6 – Specification of default values of ScalingList[ 1..3 ][ matrixId ][ i ] with i = 0..63
		static readonly byte[] defaults_012 = new byte[ 64 ]
		{
			16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 17, 16, 17, 16, 17, 18,
			17, 18, 18, 17, 18, 21, 19, 20, 21, 20, 19, 21, 24, 22, 22, 24,
			24, 22, 22, 24, 25, 25, 27, 30, 27, 25, 25, 29, 31, 35, 35, 31,
			29, 36, 41, 44, 41, 36, 47, 54, 54, 47, 65, 70, 65, 88, 88, 115
		};
		static readonly byte[] defaults_345 = new byte[ 64 ]
		{
			16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 17, 17, 17, 17, 17, 18,
			18, 18, 18, 18, 18, 20, 20, 20, 20, 20, 20, 20, 24, 24, 24, 24,
			24, 24, 24, 24, 25, 25, 25, 25, 25, 25, 25, 28, 28, 28, 28, 28,
			28, 33, 33, 33, 33, 33, 41, 41, 41, 41, 54, 54, 54, 71, 71, 91,
		};

		public void setDefault()
		{
			// Table 7-5 – Specification of default values of ScalingList[ 0 ][ matrixId ][ i ] with i = 0..15
			for( int i = 0; i < 6; i++ )
				for( int j = 0; j < 16; j++ )
					scalingList[ 0, i, j ] = 16;

			for( int i = 1; i < 4; i++ )
			{
				for( int j = 0; j < 3; j++ )
					for( int k = 0; k < 64; k++ )
						scalingList[ i, j, k ] = defaults_012[ k ];

				for( int j = 3; j < 6; j++ )
					for( int k = 0; k < 64; k++ )
						scalingList[ i, j, k ] = defaults_345[ k ];
			}

			for( int i = 0; i < 2; i++ )
				for( int j = 0; j < 6; j++ )
					dcCoeffs[ i, j ] = 16;
		}

		static int flatIndex( int size, int matrix )
		{
			if( size >= 0 && size < 4 && matrix >= 0 && matrix < 6 )
				return ( size * 6 + matrix ) * 64;
			throw new IndexOutOfRangeException();
		}

		public void read( ref BitReader reader )
		{
			Span<int> scaling_list_dc_coef = stackalloc int[ 2 * 6 ];   // Actually a 2D array of 2*6 integers.

			// 7.3.4 Scaling list data syntax, page 45
			// For the indexing, Table 7-3 – Specification of sizeId, page 93
			for( int sizeId = 0; sizeId < 4; sizeId++ )
			{
				byte[] diagonalScanX, diagonalScanY;
				int blockSizeMul;
				if( sizeId == 0 )
				{
					diagonalScanX = ScanOrder.diagonalScan_4x4_x;
					diagonalScanY = ScanOrder.diagonalScan_4x4_y;
					blockSizeMul = 4;
				}
				else
				{
					diagonalScanX = ScanOrder.diagonalScan_8x8_x;
					diagonalScanY = ScanOrder.diagonalScan_8x8_y;
					blockSizeMul = 8;
				}

				int step = ( sizeId == 3 ) ? 3 : 1;

				// Table 7-4 – Specification of matrixId according to sizeId, prediction mode and colour component
				// Page 93
				for( int matrixId = 0; matrixId < 6; matrixId += step )
				{
					bool flag = reader.readBit();   // scaling_list_pred_mode_flag[ sizeId ][ matrixId ]
					if( !flag )
					{
						uint delta = reader.unsignedGolomb();
						if( delta > 0 )
						{
							delta *= ( sizeId == 3 ) ? 3u : 1u;
							if( matrixId < delta )
								throw new ArgumentException( "Invalid data in the scaling list" );

							int bytesToCopy = sizeId > 0 ? 64 : 16;
							Array.Copy( scalingList, flatIndex( sizeId, matrixId ),
								scalingList, flatIndex( sizeId, matrixId - (int)delta ),
								bytesToCopy );
							if( sizeId > 2 )
								dcCoeffs[ sizeId - 2, matrixId ] = dcCoeffs[ sizeId - 2, matrixId - delta ];
						}
						continue;
					}

					int nextCoef = 8;
					int coefNum = Math.Min( 64, ( 1 << ( 4 + ( sizeId << 1 ) ) ) );
					if( sizeId > 1 )
					{
						int v = reader.signedGolomb() + 8;
						scaling_list_dc_coef[ ( sizeId - 2 ) * 6 + matrixId ] = nextCoef = v;
						dcCoeffs[ sizeId - 2, matrixId ] = checked((byte)nextCoef);
					}

					for( int i = 0; i < coefNum; i++ )
					{
						int pos = blockSizeMul * diagonalScanY[ i ] + diagonalScanX[ i ];
						int scaling_list_delta_coef = reader.signedGolomb();
						nextCoef = ( nextCoef + scaling_list_delta_coef + 256 ) % 256;
						scalingList[ sizeId, matrixId, pos ] = checked((byte)nextCoef);
					}
				}
			}
		}
	}
}