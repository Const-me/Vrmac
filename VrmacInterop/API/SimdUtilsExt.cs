using System;
using System.Runtime.CompilerServices;
using Vrmac.Draw;

namespace Vrmac.Utils
{
	/// <summary>Extension methods for iSimdUtils COM interface</summary>
	/// <remarks>VrmacGraphics compiles without /unsafe switch, unsafe code can only appear here in VrmacInterop.
	/// And due to questionable Microsoft's choices, ReadOnlySpan.GetPinnableReference requires unsafe to compile.</remarks>
	public static class SimdUtilsExt
	{
		/// <summary>Write glyph vertices with the specified offset in pixels</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void offsetGlyphs( this iSimdUtils utils, Span<sVertexWithId> dest, ReadOnlySpan<sGlyphVertex> source, CPoint offsetValue )
		{
			if( source.IsEmpty || dest.IsEmpty || dest.Length < source.Length || 0 != ( source.Length % 4 ) )
				throw new ArgumentException();

			int count = source.Length;
			unsafe
			{
				fixed ( sGlyphVertex* ptr = source )
				{
					IntPtr sourcePtr = (IntPtr)ptr;
					utils.offsetGlyphs( ref dest.GetPinnableReference(), sourcePtr, count, offsetValue );
				}
			}
		}
	}
}