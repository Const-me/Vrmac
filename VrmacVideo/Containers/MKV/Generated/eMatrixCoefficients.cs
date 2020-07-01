namespace VrmacVideo.Containers.MKV
{
	/// <summary>The Matrix Coefficients of the video used to derive luma and chroma values from red, green, and blue color primaries. For clarity, the value and meanings for MatrixCoefficients are adopted from Table 4 of ISO/IEC
	/// 23001-8:2016 or ITU-T H.273.</summary>
	public enum eMatrixCoefficients: byte
	{
		/// <summary>Identity</summary>
		Identity = 0,
		/// <summary>ITU-R BT.709</summary>
		BT709 = 1,
		/// <summary>unspecified</summary>
		Unspecified = 2,
		/// <summary>US FCC 73.682</summary>
		FCC73_682 = 4,
		/// <summary>ITU-R BT.470BG</summary>
		BT470BG = 5,
		/// <summary>SMPTE 170M</summary>
		Smpte170m = 6,
		/// <summary>SMPTE 240M</summary>
		Smpte240m = 7,
		/// <summary>YCoCg</summary>
		Ycocg = 8,
		/// <summary>BT2020 Non-constant Luminance</summary>
		BT2020NonConstantLuminance = 9,
		/// <summary>BT2020 Constant Luminance</summary>
		Bt2020ConstantLuminance = 10,
		/// <summary>SMPTE ST 2085</summary>
		SmpteSt2085 = 11,
		/// <summary>Chroma-derived Non-constant Luminance</summary>
		ChromaDerivedNonConstantLuminance = 12,
		/// <summary>Chroma-derived Constant Luminance</summary>
		ChromaDerivedConstantLuminance = 13,
		/// <summary>ITU-R BT.2100-0</summary>
		BT2100 = 14,
	}
}