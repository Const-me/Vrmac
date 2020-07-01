namespace VrmacVideo.Containers.MP4
{
	public enum eAudioObjectType: byte
	{
		/// <summary>Raw PCM data</summary>
		Null = 0,
		/// <summary>Very similar to the AAC Main Profile</summary>
		AacMain = 1,
		/// <summary>The counterpart to the MPEG-2 AAC Low Complexity Profile</summary>
		AacLC = 2,
		/// <summary>Scalable Sampling Rate</summary>
		AacSSR = 3,
		/// <summary>Long Term Predictor</summary>
		AacLTP = 4,
		/// <summary>Uses a different bitstream syntax to support bitrate- and bandwidth- scalability</summary>
		AacScalable = 6,
		/// <summary>Quantizes the MDCT coefficients. This coding scheme is based on fixed rate vector quantization instead of Huffman coding in AAC.</summary>
		TwinVQ = 7,
		/// <summary>Supported by the CELP speech coding tools</summary>
		CELP = 8,
		/// <summary>Some parametric speech coding</summary>
		HVXC = 9,
		/// <summary>allows very-low-bitrate phonemic descriptions of speech to be transmitted in the bitstream</summary>
		TTSI = 12,
		/// <summary>allows the use of all MPEG-4 Structured Audio tools</summary>
		MainSynthetic = 13,
		/// <summary>simple „sampling synthesis“ in presentations where the quality and flexibility of the full synthesis toolset is not required</summary>
		WavetableSynthesis = 14,
		/// <summary>General Midi</summary>
		MIDI = 15,
		/// <summary>Algorithmic Synthesis and Audio FX</summary>
		Algorithmic = 16,
		/// <summary>Error Resilient (ER) AAC Low Complexity (LC)</summary>
		ErAacLC = 17,
		/// <summary>Error Resilient (ER) AAC Long Term Predictor (LTP)</summary>
		ErAacLTP = 19,
		/// <summary>Error Resilient (ER) AAC scalable</summary>
		ErAacScalable = 20,
		/// <summary>Error Resilient (ER) TwinVQ</summary>
		ErTwinVQ = 21,
		/// <summary>Error Resilient (ER) BSAC</summary>
		ErBSAC = 22,
		/// <summary>Error Resilient (ER) AAC LD</summary>
		ErAacLD = 23,
		/// <summary>Error Resilient (ER) CELP</summary>
		ErCELP = 24,
		/// <summary>Error Resilient (ER) HVXC</summary>
		ErHVXC = 25,
		/// <summary>Error Resilient (ER) HILN</summary>
		ErHILN = 26,
		/// <summary>Error Resilient (ER) Parametric</summary>
		ErParametric = 27,

		ParametricStereo = 29,
		MpegSurround = 30,
		/// <summary>Signal AOT uses more than 5 bits</summary>
		Escape = 31,

		/// <summary>MPEG-Layer1 in mp4</summary>
		MP3ONMP4_L1 = 32,
		/// <summary>MPEG-Layer2 in mp4</summary>
		MP3ONMP4_L2 = 33,
		/// <summary>MPEG-Layer3 in mp4</summary>
		MP3ONMP4_L3 = 34,
		/// <summary>AAC + SLS</summary>
		AAC_SLS = 37,
		/// <summary></summary>
		SLS = 38,
		/// <summary>AAC Enhanced Low Delay</summary>
		ER_AAC_ELD = 39,

		/// <summary>USAC</summary>
		USAC = 42,
		/// <summary>SAOC</summary>
		SAOC = 43,
		/// <summary>Low Delay MPEG Surround</summary>
		LD_MPEGS = 44,

		// Pseudo AOTs
		/// <summary>Virtual AOT for DRM (ER-AAC-SCAL without SBR)</summary>
		DRM_AAC = 143,
		/// <summary>Virtual AOT for DRM (ER-AAC-SCAL with SBR)</summary>
		DRM_SBR = 144,
		/// <summary>Virtual AOT for DRM (ER-AAC-SCAL with SBR and MPEG-PS)</summary>
		DRM_MPEG_PS = 145,

		None = 0xFF,
	}
}