using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw.Text
{
	/// <summary>A kompiler-driven context which doesn't write vertices, measures the text instead.</summary>
	ref struct MeasureBlock
	{
		GlyphLayout glyphLayout;
		readonly ushort lineHeight;
		LineBreaker lineBreaker;
		int maxY, maxLineWidth;

		int firstCharacterInWord;

		public MeasureBlock( int breakWidth, int lineHeight )
		{
			glyphLayout = new GlyphLayout( new CPoint() );
			this.lineHeight = (ushort)lineHeight;
			lineBreaker = new LineBreaker( breakWidth );
			maxY = maxLineWidth = 0;

			firstCharacterInWord = int.MinValue;
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code for '\n' character.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void newline()
		{
			maxLineWidth = Math.Max( maxLineWidth, glyphLayout.currentPositionPixels );
			glyphLayout.newLine( 0, lineHeight, false );
			lineBreaker.newline();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void nonBreakingSpace( int advance, short lsbDelta, short rsbDelta )
		{
			// Same as empty glyph, but does not reset firstCharacterInWord value.
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );
			glyphLayout.advance( advance );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void skipGlyph( int advance, short lsbDelta, short rsbDelta )
		{
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );
			glyphLayout.advance( advance );

			// Reset the first char position
			firstCharacterInWord = int.MinValue;
			eBreakAction ba = lineBreaker.whitespace();
			if( ba == eBreakAction.LineFeed )
				glyphLayout.newLine( 0, lineHeight, false );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void emitGlyph( int advance, short lsbDelta, short rsbDelta,
			short spriteLeft, short spriteTop, ushort sx, ushort sy, uint uvTopLeft, uint uvBottomRight, byte layer )
		{
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );

			if( firstCharacterInWord == int.MinValue )
				firstCharacterInWord = glyphLayout.currentPositionPixels;

			glyphLayout.advance( advance );

			maxY = Math.Max( maxY, glyphLayout.spriteTop( spriteTop ) + sy );

			var ba = lineBreaker.glyph( glyphLayout.currentPositionPixels );
			switch( ba )
			{
				case eBreakAction.None:
					return;
				case eBreakAction.LineFeed:
					newline();
					return;
				case eBreakAction.MoveLastWord:
					break;
				default:
					throw new ApplicationException();
			}

			maxLineWidth = Math.Max( maxLineWidth, firstCharacterInWord );

			int movedWidth = glyphLayout.currentPositionPixels - firstCharacterInWord;
			glyphLayout.newLine( movedWidth, lineHeight, true );    // True to keep the damn RSB delta
			maxY += lineHeight;
		}

		public CSize getResult()
		{
			maxLineWidth = Math.Max( maxLineWidth, glyphLayout.currentPositionPixels );
			return new CSize( maxLineWidth, maxY );
		}
	}
}