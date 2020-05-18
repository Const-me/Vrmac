using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	static class RoundFloats
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static int log2( float val )
		{
			Debug.Assert( val > 0 );
			// The magic number is exponent bits:
			// https://en.wikipedia.org/wiki/Single-precision_floating-point_format#IEEE_754_single-precision_binary_floating-point_format:_binary32
			return BitConverter.SingleToInt32Bits( val ) & 0x7F800000;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static (float, float) roundBitwise( this float val )
		{
			int floor = log2( val );
			int ceil = floor + 0x00800000;
			return (BitConverter.Int32BitsToSingle( floor ), BitConverter.Int32BitsToSingle( ceil ));
		}

		public static float floorBitwise( this float val )
		{
			int floor = log2( val );
			return BitConverter.Int32BitsToSingle( floor );
		}

		public static float ceilBitwise( this float val )
		{
			int ceil = log2( val ) + 0x00800000;
			return BitConverter.Int32BitsToSingle( ceil );
		}
	}
}