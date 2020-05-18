using System.Runtime.InteropServices;

namespace Vrmac.FreeType
{

	/// <summary>Information about the loaded glyph</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sGlyphInfo
	{
		/// <summary>The advance value of a given glyph outline, in 26.6 fixed point pixels</summary>
		public readonly int advance;
		/// <summary>The difference between hinted and unhinted left side bearing while auto-hinting is active, in 10.6 fixed point pixels. Zero otherwise.</summary>
		public readonly short lsbDelta;
		/// <summary>The difference between hinted and unhinted right side bearing while auto-hinting is active, in 10.6 fixed point pixels. Zero otherwise.</summary>
		public readonly short rsbDelta;
		/// <summary>Size and position of the bitmap in pixels.</summary>
		/// <remarks>X is relative to left edge of the character. Y is relative to baseline, so top is almost always negative.
		/// Size can be 0 if FreeType refused to build a bitmap, like it happenns with space character.</remarks>
		public readonly CRect rect;

		/// <summary>True if the glyph has a bitmap</summary>
		public bool hasBitmap => !rect.size.isEmpty;
	};
}