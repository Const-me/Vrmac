using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

		/// <summary>Multiply signed int16 numbers by a specified constant</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void applyPcmVolume( this iSimdUtils utils, Span<short> span, byte volume )
		{
			if( volume == 0xFF )
				return;
			unsafe
			{
				fixed ( short* ptr = span )
					utils.applyPcmVolume( ptr, span.Length, volume );
			}
		}

		/// <summary>Get a function pointer to compute interleaved 16-bit PCM samples from the output of DTS audio decoder</summary>
		public static pfnInterleaveFunc interleaveDts( this iSimdUtils utils, byte channelsCount )
		{
			utils.interleaveDts( out var pfn, channelsCount );
			return Marshal.GetDelegateForFunctionPointer<pfnInterleaveFunc>( pfn );
		}

		/// <summary>Get a function pointer to compute interleaved 16-bit PCM samples from the output of Dolby AC3 decoder</summary>
		public static pfnInterleaveFunc interleaveDolby( this iSimdUtils utils, byte channelsCount )
		{
			utils.interleaveDolby( out var pfn, channelsCount );
			return Marshal.GetDelegateForFunctionPointer<pfnInterleaveFunc>( pfn );
		}
	}
}