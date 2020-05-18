using System;

namespace Vrmac.FreeType
{
	/// <summary>Render modes supported by FreeType2. Each mode corresponds to a specific type of scanline conversion performed on the outline.</summary>
	public enum eRenderMode: int
	{
		/// <summary>Default render mode; it corresponds to 8-bit anti-aliased bitmaps.</summary>
		Normal = 0,

		/// <summary>This is equivalent to Normal. It is only defined as a separate value because render modes are also used indirectly to define hinting algorithm selectors.</summary>
		Light = 1,

		/// <summary>This mode corresponds to 1-bit bitmaps (with 2~levels of opacity).</summary>
		Monochrome = 2,

		/// <summary>This mode corresponds to horizontal RGB and BGR subpixel displays like LCD screens.</summary>
		/// <remarks>It produces 8-bit bitmaps that are 3 times the width of the original glyph outline in pixels</remarks>
		ClearTypeHorizontal = 3,

		/// <summary>This mode corresponds to vertical RGB and BGR subpixel displays (like PDA screens, rotated LCD displays, etc.).</summary>
		/// <remarks>It produces 8-bit bitmaps that are 3 times the height of the original glyph outline in pixels</remarks>
		ClearTypeVertical = 4
	}

	/// <summary>Flags for <see cref="iFont.loadGlyph(int, eLoadGlyphFlags, uint)" /> method</summary>
	[Flags]
	public enum eLoadGlyphFlags: int
	{
		/// <summary>this value is used as the default glyph load operation.</summary>
		/// <remarks>
		/// <list type="number">
		/// <item>FreeType looks for a bitmap for the glyph corresponding to the face's current size. If one is found, the function returns. The bitmap data can be accessed from the glyph slot.</item>
		/// <item>If no embedded bitmap is searched for or found, FreeType looks for a scalable outline.
		/// If one is found, it is loaded from the font file, scaled to device pixels, then 'hinted' to the pixel grid in order to optimize it. 
		/// The outline data can be accessed from the glyph slot.</item>
		/// </list>
		/// <para>Note that by default the glyph loader doesn't render outlines into bitmaps. The following flags are used to modify this default behaviour to more specific and useful cases.</para>
		/// </remarks>
		Default = 0,

		/// <summary>Don't scale the loaded outline glyph but keep it in font units. This flag implies <see cref="NoHinting" /> and <see cref="NoBitmap" />, and unsets <see cref="Render" />.</summary>
		/// <remarks>
		/// If the font is 'tricky', using `NoScale` usually yields meaningless outlines because the subglyphs must be scaled and positioned with hinting instructions.
		/// This can be solved by loading the font without `NoScale` and setting the character size to <see cref="sFontInfo.unitsPerEM" />.
		/// </remarks>
		NoScale = 1,

		/// <summary>Disable hinting.  This generally generates 'blurrier' bitmap glyphs when the glyph are rendered in any of the anti-aliased modes.</summary>
		/// <remarks>This flag is implied by <see cref="NoScale" /></remarks>
		NoHinting = 2,

		/// <summary>Call FT_Render_Glyph after the glyph is loaded. By default, the glyph is rendered in <see cref="eRenderMode.Normal" /> mode.
		/// This can be overridden by TargetXXX or Monochrome.</summary>
		Render = 4,

		/// <summary>Ignore bitmap strikes when loading. Bitmap-only fonts ignore this flag.</summary>
		NoBitmap = 8,

		/// <summary>Load the glyph for vertical text layout. In particular, the `advance` value in the FT_GlyphSlotRec structure is set to the `vertAdvance` value of the `metrics` field.</summary>
		VerticalLayout = 1 << 4,

		/// <summary>Prefer the auto-hinter over the font's native hinter.</summary>
		ForceAutohint = 1 << 5,

		/// <summary>Make the font driver perform pedantic verifications during glyph loading and hinting. This is mostly used to detect broken glyphs in fonts. By default, FreeType tries to handle broken fonts also.</summary>
		/// <remarks>In particular, errors from the TrueType bytecode engine are not passed to the application if this flag is not set; this might result in partially hinted or distorted glyphs in case a glyph's bytecode is buggy.</remarks>
		Pedantic = 1 << 7,

		/// <summary>Don't load composite glyphs recursively.</summary>
		NoRecurse = 1 << 10,

		/// <summary>Ignore the transform matrix</summary>
		IgnoreTransform = 1 << 11,

		/// <summary>Indicates that you want to render an outline glyph to a 1-bit monochrome bitmap glyph, with 8 pixels packed into each byte of the bitmap data.</summary>
		Monochrome = 1 << 12,

		/// <summary>Keep `linearHoriAdvance` and `linearVertAdvance` fields of @FT_GlyphSlotRec in font units.</summary>
		LinearDesign = 1 << 13,

		/// <summary>Disable the auto-hinter.</summary>
		NoAutohint = 1 << 15,

		/// <summary>Load colored glyphs.  There are slight differences depending on the font format.</summary>
		Color = 1 << 20,

		/// <summary>[Since 2.6.1] Compute glyph metrics from the glyph data, without the use of bundled metrics tables (for example, the 'hdmx' table in TrueType fonts).</summary>
		/// <remarks>This flag is mainly used by font validating or font editing applications, which need to ignore, verify, or edit those tables.</remarks>
		ComputeMetrics = 1 << 21,

		/// <summary>[Since 2.7.1] Request loading of the metrics and bitmap image information of a (possibly embedded) bitmap glyph without allocating or copying the bitmap image data itself.</summary>
		/// <remarks>No effect if the target glyph is not a bitmap image.</remarks>
		BitmapMetricsOnly = 1 << 22,

		/// <summary>The default hinting algorithm, optimized for standard gray-level rendering.</summary>
		TargetNormal = 0,

		/// <summary>A lighter hinting algorithm for gray-level modes. Many generated glyphs are fuzzier but better resemble their original shape.</summary>
		/// <remarks>
		/// <para>This is achieved by snapping glyphs to the pixel grid only vertically (Y-axis), as is done by FreeType's new CFF engine or Microsoft's ClearType font renderer.
		/// This preserves inter-glyph spacing in horizontal text. The snapping is done either by the native font driver, if the driver itself and the font support it, or by the auto-hinter.</para>
		/// <para>Advance widths are rounded to integer values; however, using the `lsb_delta` and `rsb_delta` fields of @FT_GlyphSlotRec, it is possible to get fractional advance widths for subpixel positioning (which is recommended to use).</para>
		/// </remarks>
		TargetLight = eRenderMode.Light << 16,

		/// <summary>Strong hinting algorithm that should only be used for monochrome output. The result is probably unpleasant if the glyph is rendered in non-monochrome modes.</summary>
		/// <remarks>Note that for outline fonts only the TrueType font driver has proper monochrome hinting support, provided the TTFs contain hints for B/W rendering (which most fonts no longer provide).
		/// If these conditions are not met it is very likely that you get ugly results at smaller sizes.</remarks>
		TargetMonochrome = eRenderMode.Monochrome << 16,

		/// <summary>A variant of <see cref="TargetLight" /> optimized for horizontally decimated LCD displays.</summary>
		TargetClearTypeHorizontal = eRenderMode.ClearTypeHorizontal << 16,

		/// <summary>A variant of <see cref="TargetLight" /> optimized for vertically decimated LCD displays.</summary>
		TargetClearTypeVertical = eRenderMode.ClearTypeVertical << 16,
	}
}