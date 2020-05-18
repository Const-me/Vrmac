using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Vrmac.FreeType;

namespace Vrmac.Draw.Text
{
	ref struct ConsoleBlock
	{
		GlyphLayout glyphLayout;
		readonly uint id;

		readonly ushort lineHeight;
		readonly ushort characterWidth;
		readonly int lineStartPosition;
		int nextGlyphVertexOffset;
		readonly Span<sGlyphVertex> destSpan;
		int x;
		readonly int width;

		public ConsoleBlock( Span<sGlyphVertex> destSpan, CPoint start, ref sScaledMetrics metrics, uint id, int width )
		{
			glyphLayout = new GlyphLayout( start );
			lineStartPosition = glyphLayout.currentPositionPixels;
			lineHeight = (ushort)metrics.lineHeight;
			characterWidth = (ushort)metrics.maxAdvancePixels;
			this.id = id;

			this.destSpan = destSpan;
			nextGlyphVertexOffset = 0;
			x = 0;
			this.width = width;
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code for '\n' character.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void newline()
		{
			glyphLayout.newLine( lineStartPosition, lineHeight, false );
			x = 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void breakLineIfNeeded()
		{
			if( x < width )
			{
				x++;
				return;
			}
			newline();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void tabulation()
		{
			x = ( x + 4 ) & ( ~3 );
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code for glyphs without bitmaps. All arguments are compile-time constants.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void skipGlyph( int advance, short lsbDelta, short rsbDelta )
		{
			breakLineIfNeeded();

			glyphLayout.adjustPosition( lsbDelta, rsbDelta );
			glyphLayout.advance( advance );
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code to actually emit the quad. All arguments are compile-time constants.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void emitGlyph( int advance, short lsbDelta, short rsbDelta,
			short spriteLeft, short spriteTop, ushort sx, ushort sy, uint uvTopLeft, uint uvBottomRight, byte layer )
		{
			breakLineIfNeeded();

			glyphLayout.emitGlyph( destSpan, nextGlyphVertexOffset, id | layer,
				spriteLeft, spriteTop, sx, sy, uvTopLeft, uvBottomRight );
			nextGlyphVertexOffset += 4;
			glyphLayout.advance( characterWidth * 64 );
		}

		public sMeshDataSize actualMeshSize()
		{
			int vertices = nextGlyphVertexOffset;
			Debug.Assert( 0 == ( vertices % 4 ) );
			int quads = vertices / 4;
			return new sMeshDataSize( quads * 4, quads * 2 );
		}
	}
}