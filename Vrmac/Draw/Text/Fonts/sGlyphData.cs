using Vrmac.FreeType;

namespace Vrmac.Draw.Text
{
	/// <summary>Cached per glyph * font * size tuple</summary>
	struct sGlyphData
	{
		/// <summary>The advance value of a given glyph outline, 26.6 fixed point, should be positive.</summary>
		public readonly int advance;
		/// <summary>Left side bearing, 10.6 fixed point, signed</summary>
		public readonly short lsbDelta;
		/// <summary>Right side bearing, 10.6 fixed point, signed</summary>
		public readonly short rsbDelta;

		/// <summary>Position of the sprite relative to the top left corner of the glyph rectangle.</summary>
		/// <remarks>FreeType measures everything relative to baseline. Took surprising amount of time to convert that typographic BS to normal units.</remarks>
		public readonly short left, top;

		/// <summary>Index of sprite in the atlas, or -1 if there's none like it happens with space, tab, dozen of various Unicode extended spaces, and other characters.</summary>
		public readonly int spriteIndex;

		// No size is here, sprite atlas already keeps that info, and we need to lookup that data anyway to get UV rectangles.

		public sGlyphData( int baselinePosition, ref sGlyphInfo glyph )
		{
			advance = glyph.advance;
			lsbDelta = glyph.lsbDelta;
			rsbDelta = glyph.rsbDelta;
			left = (short)glyph.rect.left;
			top = (short)( glyph.rect.top + baselinePosition );
			spriteIndex = -1;
		}

		public sGlyphData( int baselinePosition, ref sGlyphInfo glyph, int idxSprite ) : this( baselinePosition, ref glyph )
		{
			spriteIndex = idxSprite;
		}

		public bool hasSprite => spriteIndex >= 0;
	}
}