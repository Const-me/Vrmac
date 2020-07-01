namespace VrmacVideo.Containers.MKV
{
	/// <summary>The colour primaries of the video. For clarity, the value and meanings for Primaries are adopted from Table 2 of ISO/IEC 23091-4 or ITU-T H.273.</summary>
	public enum ePrimaries: byte
	{
		/// <summary>ITU-R BT.709</summary>
		BT709 = 1,
		/// <summary>unspecified</summary>
		Unspecified = 2,
		/// <summary>ITU-R BT.470M</summary>
		BT470M = 4,
		/// <summary>ITU-R BT.470BG - BT.601 625</summary>
		BT601 = 5,
		/// <summary>ITU-R BT.601 525 - SMPTE 170M</summary>
		SMPTE170M = 6,
		/// <summary>SMPTE 240M</summary>
		Smpte240m = 7,
		/// <summary>FILM</summary>
		Film = 8,
		/// <summary>ITU-R BT.2020</summary>
		BT2020 = 9,
		/// <summary>SMPTE ST 428-1</summary>
		SMPTE428 = 10,
		/// <summary>SMPTE RP 432-2</summary>
		SmpteRp432 = 11,
		/// <summary>SMPTE EG 432-2</summary>
		SmpteEg432 = 12,
		/// <summary>EBU Tech. 3213-E - JEDEC P22 phosphors</summary>
		Ebu3213 = 22,
	}
}