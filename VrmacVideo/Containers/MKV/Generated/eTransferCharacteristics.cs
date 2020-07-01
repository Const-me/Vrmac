namespace VrmacVideo.Containers.MKV
{
	/// <summary>The transfer characteristics of the video. For clarity, the value and meanings for TransferCharacteristics are adopted from Table 3 of ISO/IEC 23091-4 or ITU-T H.273.</summary>
	public enum eTransferCharacteristics: byte
	{
		/// <summary>ITU-R BT.709</summary>
		BT709 = 1,
		/// <summary>unspecified</summary>
		Unspecified = 2,
		/// <summary>Gamma 2.2 curve - BT.470M</summary>
		BT470M = 4,
		/// <summary>Gamma 2.8 curve - BT.470BG</summary>
		BT470BG = 5,
		/// <summary>SMPTE 170M</summary>
		Smpte170m = 6,
		/// <summary>SMPTE 240M</summary>
		Smpte240m = 7,
		/// <summary>Linear</summary>
		Linear = 8,
		/// <summary>Log</summary>
		Log = 9,
		/// <summary>Log Sqrt</summary>
		LogSqrt = 10,
		/// <summary>IEC 61966-2-4</summary>
		IEC61966_2_4 = 11,
		/// <summary>ITU-R BT.1361 Extended Colour Gamut</summary>
		BT1361 = 12,
		/// <summary>IEC 61966-2-1</summary>
		IEC61966_2_1 = 13,
		/// <summary>ITU-R BT.2020 10 bit</summary>
		BT2020_10 = 14,
		/// <summary>ITU-R BT.2020 12 bit</summary>
		BT2020_12 = 15,
		/// <summary>ITU-R BT.2100 Perceptual Quantization</summary>
		BT2100Perceptual = 16,
		/// <summary>SMPTE ST 428-1</summary>
		SMPTE428 = 17,
		/// <summary>ARIB STD-B67 (HLG)</summary>
		AribB67 = 18,
	}
}