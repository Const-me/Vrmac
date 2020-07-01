using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace VrmacVideo
{
	static class Endian
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static uint endian( this uint v )
		{
			return BinaryPrimitives.ReverseEndianness( v );
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static int endian( this int v )
		{
			return BinaryPrimitives.ReverseEndianness( v );
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static ushort endian( this ushort v )
		{
			return BinaryPrimitives.ReverseEndianness( v );
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static long endian( this long v )
		{
			return BinaryPrimitives.ReverseEndianness( v );
		}
	}
}