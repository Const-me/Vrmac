using System;

namespace Vrmac.FreeType
{
	/// <summary>A list of bit flags used in the <see cref="sFontInfo.faceFlags" /> field of the <see cref="sFontInfo" /> structure.
	/// They inform client applications of properties of the corresponding face.</summary>
	[Flags]
	public enum eFaceFlags: uint
	{
		/// <summary>The face contains outline glyphs</summary>
		Scalable = 1,

		/// <summary>The face contains bitmap strikes</summary>
		FixedSizes = 2,

		/// <summary>The face contains fixed-width characters (like Courier, Lucida, MonoType, etc.)</summary>
		FixedWidth = 4,

		/// <summary>The face uses the SFNT storage scheme. For now, this means TrueType and OpenType.</summary>
		SFNT = 8,

		/// <summary>The face contains horizontal glyph metrics. This should be set for all common formats.</summary>
		Horizontal = 0x10,

		/// <summary>The face contains vertical glyph metrics. This is only available in some formats, not all of them.</summary>
		Vertical = 0x20,

		/// <summary>The face contains kerning information. If set, the kerning distance can be retrieved using the function @FT_Get_Kerning.
		/// Otherwise the function always return the vector (0,0).
		/// Note that FreeType doesn't handle kerning data from the SFNT 'GPOS' table(as present in many OpenType fonts).</summary>
		Kerning = 0x40,

		/// <summary>The face contains multiple masters and is capable of interpolating between them. Supported formats are Adobe MM, TrueType GX, and OpenType variation fonts.</summary>
		MultipleMasters = 0x100,

		/// <summary>The face contains glyph names, which can be retrieved using FT_Get_Glyph_Name.
		/// Note that some TrueType fonts contain broken glyph name tables. Use the function @FT_Has_PS_Glyph_Names when needed.</summary>
		GlyphNames = 0x200,

		/// <summary>Used internally by FreeType to indicate that a face's stream was provided by the client application and should not be destroyed when @FT_Done_Face is called. Don't read or test this flag.</summary>
		ExternalStream = 0x400,

		/// <summary>The font driver has a hinting machine of its own.
		/// For example, with TrueType fonts, it makes sense to use data from the SFNT 'gasp' table only if the native TrueType hinting engine (with the bytecode interpreter) is available and active.</summary>
		Hinter = 0x800,

		/// <summary>The face is CID-keyed. In that case, the face is not accessed by glyph indices but by CID values.</summary>
		/// <remarks>
		/// <para>For subsetted CID-keyed fonts this has the consequence that not all index values are a valid argument to @FT_Load_Glyph.
		/// Only the CID values for which corresponding glyphs in the subsetted font exist make `FT_Load_Glyph` return successfully; in all other cases you get an `FT_Err_Invalid_Argument` error.</para>
		/// <para>Note that CID-keyed fonts that are in an SFNT wrapper (this is, all OpenType/CFF fonts) don't have this flag set since the glyphs are accessed in the normal way (using contiguous indices);
		/// the 'CID-ness' isn't visible to the application.</para>
		/// </remarks>
		CidKeyed = 0x1000,

		/// <summary>The face is 'tricky', this is, it always needs the font format's native hinting engine to get a reasonable result. 
		/// A typical example is the old Chinese font `mingli.ttf` (but not `mingliu.ttc`) that uses TrueType bytecode instructions to move and scale all of its subglyphs.</summary>
		Tricky = 0x2000,

		/// <summary>[Since 2.5.1] The face has color glyph tables.</summary>
		Color = 0x4000,

		/// <summary>[Since 2.9] Set if the current face (or named instance) has been altered with @FT_Set_MM_Design_Coordinates, @FT_Set_Var_Design_Coordinates, or @FT_Set_Var_Blend_Coordinates.
		/// This flag is unset by a call to @FT_Set_Named_Instance.</summary>
		Variation = 0x8000,
	}
}