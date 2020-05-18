using System;
using System.Runtime.InteropServices;

namespace Vrmac.FreeType
{
	/// <summary>FreeType root face class structure. A face object models a typeface in a font file.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sFontInfo
	{
		/// <summary>The number of faces in the font file. Some font formats can have multiple faces in a single font file.</summary>
		public readonly int numFaces;

		/// <summary>This field holds two different values. Bits 0-15 are the index of the face in the font file (starting with value 0). They are set to 0 if there is only one face in the font file.
		/// [Since 2.6.1] Bits 16-30 are relevant to GX and OpenType variation fonts only, holding the named instance index for the current face index( starting with value~1;
		/// value 0 indicates font access without a named instance). For non-variation fonts, bits 16-30 are ignored. If we have the third named instance of face~4, say, `face_index` is set to 0x00030004.</summary>
		/// <remarks>Bit 31 is always zero (this is, `faceIndex` is always a positive value).</remarks>
		public readonly int faceIndex;

		/// <summary>A set of bit flags that give important information about the face.</summary>
		public readonly eFaceFlags faceFlags;

		/// <summary>A set of bit flags indicating the style of the face.</summary>
		public readonly Draw.eFontStyleFlags styleFlags;
		byte zzPadding;
		/// <summary>The number of named instances available for the current face if we have a GX or OpenType variation (sub) font.</summary>
		public readonly ushort namedInstancesCount;

		/// <summary>The number of glyphs in the face. If the face is scalable and has sbits( see `num_fixed_sizes`), it is set to the number of outline glyphs.
		/// For CID-keyed fonts( not in an SFNT wrapper) this value gives the highest CID used in the font.</summary>
		public readonly int numGlyphs;

		private readonly IntPtr family_name, style_name;
		/// <summary>The face's family name. This is an ASCII string, usually in English, that describes the typeface's family (like 'Times New Roman', 'Bodoni', 'Garamond', etc).
		/// This is a least common denominator used to list fonts. Some formats (TrueType &amp; OpenType) provide localized and Unicode versions of this string.
		/// Applications should use the format-specific interface to access them. Can be null (e.g., in fonts embedded in a PDF file).</summary>
		/// <remarks>In case the font doesn't provide a specific family name entry, FreeType tries to synthesize one, deriving it from other name entries.</remarks>
		public string familyName => Marshal.PtrToStringUTF8( family_name );

		/// <summary>The face's style name. This is an ASCII string, usually in English, that describes the typeface's style (like 'Italic', 'Bold', 'Condensed', etc).</summary>
		/// <remarks>Not all font formats provide a style name, so this field is optional, and can be set to `NULL`.
		/// As for <see cref="familyName" />, some formats provide localized and Unicode versions of this string. Applications should use the format-specific interface to access them.</remarks>
		public string styleName => Marshal.PtrToStringUTF8( style_name );

		/// <summary>The font bounding box. Coordinates are expressed in font units, <see cref="unitsPerEM" />.
		/// The box is large enough to contain any glyph from the font. Thus, bbox.top can be seen as the 'maximum ascender', and bbox.bottom as the 'minimum descender'.</summary>
		/// <remarks>Only relevant for scalable formats. Note that the bounding box might be off by (at least) one pixel for hinted fonts.</remarks>
		public readonly CRect bbox;

		/// <summary>The number of font units per EM square for this face. This is typically 2048 for TrueType fonts, and 1000 for Type ~1 fonts.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly ushort unitsPerEM;

		/// <summary>The typographic ascender of the face, expressed in font units. For font formats not having this information, it is set to bbox.top.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly short ascender;

		/// <summary>The typographic descender of the face, expressed in font units. For font formats not having this information, it is set to `bbox.bottom`. Note that this field is negative for values below the baseline.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly short descender;

		/// <summary>This value is the vertical distance between two consecutive baselines, expressed in font units. It is always positive.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly short height;

		/// <summary>The maximum advance width, in font units, for all glyphs in this face. This can be used to make word wrapping computations faster.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly short maxAdvanceWidth;

		/// <summary>The maximum advance height, in font units, for all glyphs in this face. This is only relevant for vertical layouts, and is set to `height` for fonts that do not provide vertical metrics.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly short maxAdvanceHeight;

		/// <summary>The position, in font units, of the underline line for this face. It is the center of the underlining stem.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly short underlinePosition;

		/// <summary>The thickness, in font units, of the underline for this face.</summary>
		/// <remarks>Only relevant for scalable formats.</remarks>
		public readonly short underlineThickness;
	};
}