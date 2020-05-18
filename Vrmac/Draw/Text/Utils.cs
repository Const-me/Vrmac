using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw.Text
{
	static class Utils
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static int length( this string s )
		{
			if( string.IsNullOrEmpty( s ) )
				return 0;
			ReadOnlySpan<char> chars = s;
			int len = chars.Length;
			int res = 0;
			for( int i = 0; i < len; i++ )
			{
				char c = chars[ i ];
				if( char.IsLowSurrogate( c ) )
					throw new ArgumentException();

				if( !char.IsHighSurrogate( c ) )
				{
					res++;
					continue;
				}
				if( i + 1 >= len )
					throw new ArgumentException();
				char c2 = chars[ i + 1 ];
				if( !char.IsLowSurrogate( c2 ) )
					throw new ArgumentException();
				res++;
				i++;
			}
			return res;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static sMeshDataSize meshDataSize( string str )
		{
			int chars = str.length();
			return new sMeshDataSize( chars * 4, chars * 2 );
		}

		/// <summary>Convert font size into pixels</summary>
		public static uint computeFontSize( float fontSizePt, float dpiScaling )
		{
			if( fontSizePt < 4 || fontSizePt > 288 )
				throw new ArgumentOutOfRangeException();

			float dpi = 96.0f * dpiScaling;
			// https://stackoverflow.com/a/139712/126995
			float pixels = fontSizePt * dpi / 72.0f;
			return (uint)MathF.Round( pixels );
		}

		/// <summary>Produce 16-bit indices of a glyph run mesh</summary>
		public static void writeIndices( Span<ushort> destSpan, int baseVertex, int countTriangles )
		{
			Debug.Assert( 0 == countTriangles % 2 );
			byte[] src = RectangleMesh.indexBufferFilled;
			int destIndexEnd = countTriangles * 3;

			for( int destIndex = 0; destIndex < destIndexEnd; destIndex += 6, baseVertex += 4 )
			{
				destSpan[ destIndex ] = (ushort)( src[ 0 ] + baseVertex );
				destSpan[ destIndex + 1 ] = (ushort)( src[ 1 ] + baseVertex );
				destSpan[ destIndex + 2 ] = (ushort)( src[ 2 ] + baseVertex );
				destSpan[ destIndex + 3 ] = (ushort)( src[ 3 ] + baseVertex );
				destSpan[ destIndex + 4 ] = (ushort)( src[ 4 ] + baseVertex );
				destSpan[ destIndex + 5 ] = (ushort)( src[ 5 ] + baseVertex );
			}
		}

		/// <summary>Produce 32-bit indices of a glyph run mesh</summary>
		public static void writeIndices( Span<uint> destSpan, int baseVertex, int countTriangles )
		{
			Debug.Assert( 0 == countTriangles % 2 );
			byte[] src = RectangleMesh.indexBufferFilled;
			int destIndexEnd = countTriangles * 3;

			for( int destIndex = 0; destIndex < destIndexEnd; destIndex += 6, baseVertex += 4 )
			{
				destSpan[ destIndex ] = (uint)( src[ 0 ] + baseVertex );
				destSpan[ destIndex + 1 ] = (uint)( src[ 1 ] + baseVertex );
				destSpan[ destIndex + 2 ] = (uint)( src[ 2 ] + baseVertex );
				destSpan[ destIndex + 3 ] = (uint)( src[ 3 ] + baseVertex );
				destSpan[ destIndex + 4 ] = (uint)( src[ 4 ] + baseVertex );
				destSpan[ destIndex + 5 ] = (uint)( src[ 5 ] + baseVertex );
			}
		}

		public static bool isGrayscale( this eTextRendering textRendering )
		{
			return (byte)textRendering < (byte)eTextRendering.ClearTypeHorizontal;
		}
	}
}