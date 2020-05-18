using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw.Text
{
	ref struct GlyphLayout
	{
		int currentPosition;
		public ushort y { get; private set; }
		short prevRsbDelta;

		public GlyphLayout( CPoint start )
		{
			currentPosition = start.x * 64;
			y = checked((ushort)start.y);
			prevRsbDelta = 0;
		}

		public void reset( int x, int y )
		{
			currentPosition = x * 64;
			this.y = checked((ushort)y);
			prevRsbDelta = 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void newLine( int x, ushort lineHeight, bool keepRsbDelta )
		{
			currentPosition = x * 64;
			y = checked((ushort)( y + lineHeight ));
			if( keepRsbDelta )
				return;
			prevRsbDelta = 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void adjustPosition( short lsbDelta, short rsbDelta )
		{
			// https://www.freetype.org/freetype2/docs/reference/ft2-base_interface.html#ft_glyphslotrec
			if( prevRsbDelta - lsbDelta > 32 )
				currentPosition -= 64;
			else if( prevRsbDelta - lsbDelta < -31 )
				currentPosition += 64;

			prevRsbDelta = rsbDelta;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void advance( int advance )
		{
			currentPosition += advance;
		}

		public int currentPositionPixels => currentPosition / 64;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ushort spriteLeft( short left )
		{
			return (ushort)( currentPositionPixels + left );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public ushort spriteTop( short top )
		{
			return (ushort)( y + top );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void emitGlyph( Span<sGlyphVertex> span, int destIndex, uint misc,
			short spriteLeft, short spriteTop, ushort sx, ushort sy, uint uvTopLeft, uint uvBottomRight )
		{
			ushort x1 = this.spriteLeft( spriteLeft );
			ushort y1 = this.spriteTop( spriteTop );
			ushort x2 = (ushort)( x1 + sx );
			ushort y2 = (ushort)( y1 + sy );

			// left top
			ref sGlyphVertex vert = ref span[ destIndex ];
			vert.x = x1;
			vert.y = y1;
			vert.uv = uvTopLeft;
			vert.misc = misc;

			// left bottom
			vert = ref span[ destIndex + 1 ];
			vert.x = x1;
			vert.y = y2;
			vert.uv = ( uvTopLeft & 0xFFFF ) | ( uvBottomRight & 0xFFFF0000 );
			vert.misc = misc;

			// right bottom
			vert = ref span[ destIndex + 2 ];
			vert.x = x2;
			vert.y = y2;
			vert.uv = uvBottomRight;
			vert.misc = misc;

			// right top
			vert = ref span[ destIndex + 3 ];
			vert.x = x2;
			vert.y = y1;
			vert.uv = ( uvTopLeft & 0xFFFF0000 ) | ( uvBottomRight & 0xFFFF );
			vert.misc = misc;
		}

		public CPoint offsetToNextLine( ushort lineHeight, int left, int x )
		{
			int xPixel = x / 64;
			return new CPoint( xPixel - left, lineHeight );
		}
	}
}