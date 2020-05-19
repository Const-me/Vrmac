using System;
using System.Numerics;

namespace Vrmac.Draw
{
	/// <summary>Utility functions related to 2D graphics</summary>
	public static class DrawUtils
	{
		/// <summary>Inflate the rectangle, snapping all 4 edges to integers.</summary>
		public static Rect inflateToIntegers( this Rect pixels )
		{
			Vector2 tl = pixels.topLeft;
			Vector2 br = pixels.bottomRight;
			tl.X = MathF.Floor( tl.X );
			tl.Y = MathF.Floor( tl.Y );
			br.X = MathF.Ceiling( br.X );
			br.Y = MathF.Ceiling( br.Y );
			return new Rect( tl, br );
		}

		/// <summary>Round all 4 values of the rectangle, snapping them to integers.</summary>
		public static Rect roundToIntegers( this Rect pixels )
		{
			Vector2 tl = pixels.topLeft;
			Vector2 br = pixels.bottomRight;
			tl.X = MathF.Round( tl.X );
			tl.Y = MathF.Round( tl.Y );
			br.X = MathF.Round( br.X );
			br.Y = MathF.Round( br.Y );
			return new Rect( tl, br );
		}

		/// <summary>Deflate the rectangle, snapping all 4 edges to integers.</summary>
		public static Rect deflateToIntegers( this Rect pixels )
		{
			Vector2 tl = pixels.topLeft;
			Vector2 br = pixels.bottomRight;
			tl.X = MathF.Ceiling( tl.X );
			tl.Y = MathF.Ceiling( tl.Y );
			br.X = MathF.Floor( br.X );
			br.Y = MathF.Floor( br.Y );

			if( tl.X >= br.X )
				tl.X = br.X = ( pixels.left + pixels.right ) * 0.5f;
			if( tl.Y >= br.Y )
				tl.Y = br.Y = ( pixels.top + pixels.bottom ) * 0.5f;

			return new Rect( tl, br );
		}

		/// <summary>Same as Span.CopyTo but adds an offset.</summary>
		public static void copyTo( this ReadOnlySpan<uint> src, Span<uint> dest, int offset )
		{
			int len = src.Length;
			if( len != dest.Length )
				throw new ArgumentException();
			for( int i = 0; i < len; i++ )
				dest[ i ] = (uint)( src[ i ] + offset );
		}
	}
}