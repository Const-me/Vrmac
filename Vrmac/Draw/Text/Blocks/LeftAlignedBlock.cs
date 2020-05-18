using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw.Text
{
	/// <summary>Text block that aligns content to the left.</summary>
	/// <remarks>It only needs to buffer the last word.</remarks>
	ref struct LeftAlignedBlock
	{
		GlyphLayout glyphLayout;
		readonly uint id;

		readonly CRect rectangle;
		readonly ushort lineHeight;
		LineBreaker lineBreaker;
		readonly int lineStartPosition;
		int destOffset;
		readonly Span<sVertexWithId> finalDestSpan;
		readonly LineBuffer buffer;

		int firstCharacterInWord;

		public LeftAlignedBlock( Span<sVertexWithId> destSpan, CRect rectangle, int lineHeight, uint id )
		{
			this.rectangle = rectangle;
			glyphLayout = new GlyphLayout( rectangle.topLeft );
			lineStartPosition = glyphLayout.currentPositionPixels;
			lineBreaker = new LineBreaker( rectangle.right );
			this.lineHeight = (ushort)lineHeight;
			this.id = id;
			buffer = LineBuffer.instance;
			buffer.clear();

			firstCharacterInWord = int.MinValue;

			finalDestSpan = destSpan;
			destOffset = 0;
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code for '\n' character.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void newline()
		{
			copyTempBuffer();
			glyphLayout.newLine( lineStartPosition, lineHeight, false );
			lineBreaker.newline();
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code for &amp;nbsp; character.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void nonBreakingSpace( int advance, short lsbDelta, short rsbDelta )
		{
			// Same as empty glyph, but does not copy temp buffer and does not reset firstCharacterInWord value.
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );
			glyphLayout.advance( advance );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void copyTempBuffer()
		{
			if( buffer.length <= 0 )
				return;

			var destSlice = finalDestSpan.Slice( destOffset );
			buffer.copyTo( destSlice );
			destOffset += buffer.length;
			buffer.clear();
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code for glyphs without bitmaps. All arguments are compile-time constants.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void skipGlyph( int advance, short lsbDelta, short rsbDelta )
		{
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );
			glyphLayout.advance( advance );

			// Space character. If the buffer has some glyphs, copy them to the destination.
			copyTempBuffer();
			// Also reset the first char position
			firstCharacterInWord = int.MinValue;
			eBreakAction ba = lineBreaker.whitespace();
			if( ba == eBreakAction.LineFeed )
				glyphLayout.newLine( rectangle.left, lineHeight, false );
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code to actually emit the quad. All arguments are compile-time constants.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void emitGlyph( int advance, short lsbDelta, short rsbDelta,
			short spriteLeft, short spriteTop, ushort sx, ushort sy, uint uvTopLeft, uint uvBottomRight, byte layer )
		{
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );

			if( firstCharacterInWord == int.MinValue )
				firstCharacterInWord = glyphLayout.currentPositionPixels;

			Span<sGlyphVertex> destSpan = buffer.appendGlyph();
			glyphLayout.emitGlyph( destSpan, 0, id | layer,
				spriteLeft, spriteTop, sx, sy, uvTopLeft, uvBottomRight );

			glyphLayout.advance( advance );

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

			CPoint offsetToNextLine = new CPoint();
			int xMovingFrom = firstCharacterInWord;
			int xMovingTo = lineStartPosition;
			offsetToNextLine.x = lineStartPosition - firstCharacterInWord;	// We want this to be negative number, because moving left.
			offsetToNextLine.y = lineHeight;

			var destSlice = finalDestSpan.Slice( destOffset );
			buffer.copyWithOffset( destSlice, offsetToNextLine );
			destOffset += buffer.length;
			buffer.clear();

			int movedWidth = glyphLayout.currentPositionPixels - firstCharacterInWord;
			glyphLayout.newLine( lineStartPosition + movedWidth, lineHeight, true );	// True to keep the damn RSB delta
		}

		/// <summary>Flush incomplete word if any, return rectangle actually occupied by the text. That rectangle is in physical pixels.</summary>
		public CRect flush()
		{
			CRect result = rectangle;
			copyTempBuffer();
			if( destOffset <= 0 )
			{
				// There were no characters
				result.bottom = result.top;
			}
			else
			{
				// Some characters were written. Add height of the final line.
				result.bottom = glyphLayout.y + lineHeight;
			}
			return result;
		}

		/// <summary>Not the case currently, but eventually, after advanced typography features are implemented, the mesh gonna contain slightly less vertices than allocated in the buffer.
		/// This is because soft hyphens, combining characters, ligatures, and other typographic shenanigans.
		/// This method returns count of vertices/triangles actually written to the destination span.</summary>
		/// <remarks>Should be called after the <see cref="flush" /></remarks>
		public sMeshDataSize actualMeshSize()
		{
			int vertices = destOffset;
			Debug.Assert( 0 == ( vertices % 4 ) );
			int quads = vertices / 4;
			return new sMeshDataSize( quads * 4, quads * 2 );
		}
	}
}