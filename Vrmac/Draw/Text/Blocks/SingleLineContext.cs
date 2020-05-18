using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw.Text
{
	/// <summary>Lays out glyphs in a single line, without any wrapping, line breaks or alignment. Embedded newlines are ignored.</summary>
	ref struct SingleLineContext
	{
		GlyphLayout glyphLayout;
		readonly uint id;
		int destIndex;
		readonly Span<sGlyphVertex> span;

		public SingleLineContext( Span<sGlyphVertex> destSpan, CPoint start, uint id )
		{
			glyphLayout = new GlyphLayout( start );
			this.id = id;
			destIndex = 0;
			span = destSpan;
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code for glyphs without bitmaps. All arguments are compile-time constants.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void skipGlyph( int advance, short lsbDelta, short rsbDelta )
		{
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );
			glyphLayout.advance( advance );
		}

		/// <summary>Called by <see cref="Kompiler" />-generated code to actually emit the quad. All arguments are compile-time constants.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void emitGlyph( int advance, short lsbDelta, short rsbDelta,
			short spriteLeft, short spriteTop, ushort sx, ushort sy, uint uvTopLeft, uint uvBottomRight, byte layer )
		{
			glyphLayout.adjustPosition( lsbDelta, rsbDelta );

			uint misc = id | layer;
			glyphLayout.emitGlyph( span, destIndex, misc, spriteLeft, spriteTop, sx, sy, uvTopLeft, uvBottomRight );

			destIndex += 4;
			glyphLayout.advance( advance );
		}

		public sMeshDataSize actualMeshSize()
		{
			int vertices = destIndex;
			Debug.Assert( 0 == ( vertices % 4 ) );
			int quads = vertices / 4;
			return new sMeshDataSize( quads * 4, quads * 2 );
		}
	}
}