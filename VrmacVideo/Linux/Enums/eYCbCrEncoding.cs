namespace VrmacVideo.Linux
{
	// enum v4l2_ycbcr_encoding
	public enum eYCbCrEncoding: byte
	{
		Default = 0,

		/// <summary>ITU-R 601 -- SDTV</summary>
		BT601 = 1,
		/// <summary>Rec. 709 -- HDTV</summary>
		BT709 = 2,
		/// <summary>ITU-R 601/EN 61966-2-4 Extended Gamut -- SDTV</summary>
		XV601 = 3,
		/// <summary>Rec. 709/EN 61966-2-4 Extended Gamut -- HDTV</summary>
		XV709 = 4,
		/// <summary>sYCC (Y'CbCr encoding of sRGB), identical to ENC_601. It was added originally due to a misunderstanding of the sYCC standard. It should not be used, instead use <see cref="eYCbCrEncoding.BT601" />.</summary>
		SYCC = 5,
		/// <summary>BT.2020 Non-constant Luminance Y'CbCr</summary>
		BT2020 = 6,
		/// <summary>BT.2020 Constant Luminance Y'CbcCrc</summary>
		BT2020_CONST_LUM = 7,
		/// <summary>SMPTE 240M -- Obsolete HDTV</summary>
		SMPTE240M = 8,
	}
}