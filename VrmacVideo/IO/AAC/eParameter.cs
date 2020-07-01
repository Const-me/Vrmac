namespace VrmacVideo.IO.AAC
{
	/// <summary>AAC decoder setting parameters</summary>
	public enum eParameter: int
	{
		/// <summary>Defines how the decoder processes two channel signals.
		/// 0: Leave both signals as they are (default).
		/// 1: Create a dual mono output signal from channel 1.
		/// 2: Create a dual mono output signal from channel 2.
		/// 3: Create a dual mono output signal by mixing both channels: (L' = R' = 0.5*Ch1 + 0.5*Ch2)</summary>
		PcmDualChannelOutputMode = 0x0002,

		/// <summary>Output buffer channel ordering. 0: MPEG PCE style order, 1: WAV file channel order (default).</summary>
		PcmOutputChannelMapping = 0x0003,

		/// <summary>Enable signal level limiting.
		/// -1: Auto-config. Enable limiter for all non-lowdelay configurations by default.
		/// 0: Disable limiter in general.
		/// 1: Enable limiter always.
		/// It is recommended to call the decoder with a AACDEC_CLRHIST flag to reset all states when the limiter switch is changed explicitly.</summary>
		PcmLimiterEnable = 0x0004,

		/// <summary>Signal level limiting attack time in ms.
		/// Default configuration is 15 ms. Adjustable range from 1 ms to 15 ms.</summary>
		PcmLimiterAttackTime = 0x0005,

		/// <summary>Signal level limiting release time in ms.
		/// Default configuration is 50 ms. Adjustable time must be larger than 0 ms.</summary>
		PcmLimiterReleasTime = 0x0006,

		/// <summary>Minimum number of PCM output channels.</summary>
		/// <remarks>
		/// If higher than the number of encoded audio channels, a simple channel extension is applied (see note 4 for exceptions).
		/// -1, 0: Disable channel extension feature. The decoder output contains the same number of channels as the encoded bitstream.
		/// 1: This value is currently needed only together with the mix-down feature. <seealso cref="PcmMaxOutputChannels" /> and note 2 below.
		/// 2: Encoded mono signals will be duplicated to achieve a 2/0/0.0 channel output configuration.
		/// 6: The decoder tries to reorder encoded signals with less than six channels to achieve a 3/0/2.1 channel output signal.
		/// Missing channels will be filled with a zero signal. If reordering is not possible, the empty channels will simply be appended.
		/// Only available if instance is configured to support multichannel output.
		/// 8: The decoder tries to reorder encoded signals with less than eight channels to achieve a 3/0/4.1 channel output signal.
		/// Missing channels will be filled with a zero signal. If reordering is not possible the empty channels will simply be appended.
		/// Only available if instance is configured to support multichannel output.
		/// NOTES
		/// 1. The channel signaling (CStreamInfo::pChannelType and CStreamInfo::pChannelIndices) will not be modified.
		/// Added empty channels will be signaled with channel type AUDIO_CHANNEL_TYPE::ACT_NONE.
		///  2. If the parameter value is greater than that of ::AAC_PCM_MAX_OUTPUT_CHANNELS both will be set to the same value.
		///  3. This parameter does not affect MPEG Surround processing.
		///  4. This parameter will be ignored if the number of encoded audio channels is greater than 8.
		///  </remarks>
		PcmMinOutputChannels = 0x0011,

		/// <summary>Maximum number of PCM output channels.</summary>
		/// <remarks>
		/// If lower than the number of encoded audio channels, downmixing is applied accordingly (see note 5 for exceptions).
		/// If dedicated metadata is available in the stream it will be used to achieve better mixing results.
		/// -1, 0: Disable downmixing feature. The decoder output contains the same number of channels as the encoded bitstream.
		/// 1: All encoded audio configurations with more than one channel will be mixed down to one mono output signal.
		/// 2: The decoder performs a stereo mix-down if the number encoded audio channels is greater than two.
		/// 6: If the number of encoded audio channels is greater than six the decoder performs a mix-down to meet the target output configuration of 3/0/2.1 channels.
		/// Only available if instance is configured to support multichannel output.
		/// 8: This value is currently needed only together with the channel extension feature. See ::AAC_PCM_MIN_OUTPUT_CHANNELS and note 2 below.
		/// Only available if instance is configured to support multichannel output.
		/// NOTES:
		/// 1. Down-mixing of any seven or eight channel configuration not defined in ISO/IEC 14496-3 PDAM 4 is not supported by this software version.
		/// 2. If the parameter value is greater than zero but smaller than ::AAC_PCM_MIN_OUTPUT_CHANNELS both will be set to same value.
		/// 3. The operating mode of the MPEG Surround module will be set accordingly.
		/// 4. Setting this parameter with any value will disable the binaural processing of the MPEG Surround module
		/// 5. This parameter will be ignored if the number of encoded audio channels is greater than 8.
		/// </remarks>
		PcmMaxOutputChannels = 0x0012,

		/// <summary>See <see cref="eMetadataProfile" /> for available values.</summary>
		MetadataProfile = 0x0020,

		/// <summary>Defines the time in ms after which all the bitstream associated meta-data (DRC, downmix coefficients, ...) will be reset to default if no update has been received.
		/// Negative values disable the feature.</summary>
		MetadataExpiryTime = 0x0021,

		/// <summary>Error concealment processing method</summary>
		/// <remarks>
		/// 0: Spectral muting.
		/// 1: Noise substitution.
		/// 2: Energy interpolation (adds additional signal delay of one frame, see ::CONCEAL_INTER. only some AOTs are supported).
		/// </remarks>
		ConcealMethod = 0x0100,

		/// <summary>MPEG-4 / MPEG-D Dynamic Range Control (DRC): Scaling factor for boosting gain values.
		/// Defines how the boosting DRC factors (conveyed in the bitstream) will be applied to the decoded signal.
		/// The valid values range from 0 (don't apply boost factors) to 127 (fully apply boost factors). Default value is 0 for MPEG-4 DRC and 127 for MPEG-D DRC.</summary>
		DrcBoostFactor = 0x0200,

		/// <summary>MPEG-4 / MPEG-D DRC: Scaling factor for attenuating gain values.
		/// Same as <see cref="DrcBoostFactor" /> but for attenuating DRC factors.</summary>
		DrcAttenuationFactor = 0x0201,

		/// <summary>MPEG-4 / MPEG-D DRC: Target reference level / decoder target loudness.</summary>
		/// <remarks>
		/// Defines the level below full-scale (quantized in steps of 0.25dB) to which the output audio signal will be normalized to by the DRC module.
		/// The parameter controls loudness normalization for both MPEG-4 DRC and MPEG-D DRC.
		/// The valid values range from 40 (-10 dBFS) to 127 (-31.75 dBFS).
		/// Example values:
		/// 124 (-31 dBFS) for audio/video receivers (AVR) or other devices allowing audio playback with high dynamic range,
		/// 96 (-24 dBFS) for TV sets or equivalent devices (default),
		/// 64 (-16 dBFS) for mobile devices where the dynamic range of audio playback is restricted.
		/// Any value smaller than 0 switches off loudness normalization and MPEG-4 DRC.</remarks>
		DrcReferenceLevel = 0x0202,

		/// <summary>MPEG-4 DRC: En-/Disable DVB specific heavy compression (aka RF mode).
		/// If set to 1, the decoder will apply the compression values from the DVB specific ancillary data field.
		/// At the same time the MPEG-4 Dynamic Range Control tool will be disabled. By default, heavy compression is disabled.</summary>
		DrcHeavyCompression = 0x0203,

		/// <summary>MPEG-4 DRC: Default presentation mode (DRC parameter handling).</summary>
		/// <remarks>Defines the handling of the DRC parameters boost factor, attenuation factor and heavy compression, if no presentation mode is indicated in the bitstream.
		/// Default: <see cref="eDrcDefaultPresentationModeOptions.ParameterHandlingDisabled" /></summary>
		/// <seealso cref="eDrcDefaultPresentationModeOptions" />
		DrcDefaultPresentationMode = 0x0204,

		/// <summary>MPEG-4 DRC: Encoder target level for light (i.e. not heavy) compression.</summary>
		/// <remarks>
		/// If known, this declares the target reference level that was assumed at the encoder for calculation of limiting gains.
		/// The valid values range from 0 (full-scale) to 127 (31.75 dB below full-scale).
		/// This parameter is used only with ::AAC_DRC_PARAMETER_HANDLING_ENABLED and ignored otherwise.
		/// Default: 127 (worst-case assumption).</summary>
		DrcEncTargetLevel = 0x0205,

		/// <summary>MPEG-D DRC: Request a DRC effect type for selection of a DRC set.</summary>
		/// <remarks>
		/// Supported indices are:
		/// -1: DRC off. Completely disables MPEG-D DRC.
		/// 0: None (default). Disables MPEG-D DRC, but automatically enables DRC if necessary to prevent clipping.
		/// 1: Late night
		/// 2: Noisy environment
		/// 3: Limited playback range
		/// 4: Low playback level
		/// 5: Dialog enhancement
		/// 6: General compression. Used for generally enabling MPEG-D DRC without particular request.</remarks>
		UnidrcSetEffect = 0x0206,

		/// <summary>MPEG-D DRC: Enable album mode.</summary>
		/// <remarks>
		/// 0: Disabled (default),
		/// 1: Enabled.
		/// n Disabled album mode leads to application of gain sequences for fading in and out, if provided in the bitstream.
		/// Enabled album mode makes use of dedicated album loudness information, if provided in the bitstream.
		/// </remarks>
		UnidrcAlbumMode = 0x0207,

		/// <summary>Quadrature Mirror Filter (QMF) Bank processing mode.</summary>
		/// <remarks>
		/// -1: Use internal default. Implies MPEG Surround partially complex accordingly.
		/// 0: Use complex QMF data mode.
		/// 1: Use real (low power) QMF data mode.
		/// </remarks>
		QmfLowpower = 0x0300,

		/// <summary>Clear internal bit stream buffer of transport layers.
		/// The decoder will start decoding at new data passed after this event and any previous data is discarded.</summary>
		TpdecClearBuffer = 0x0603,
	}

	/// <summary>Options for handling of DRC parameters, if presentation mode is not indicated in bitstream</summary>
	public enum eDrcDefaultPresentationModeOptions: int
	{
		/// <summary>DRC parameter handling disabled, all parameters are applied as requested.</summary>
		ParameterHandlingDisabled = -1,
		/// <summary>Apply changes to requested DRC parameters to prevent clipping.</summary>
		ParameterHandlingEnabled = 0,
		/// <summary>Use DRC presentation mode 1 as default (e.g. for Nordig)</summary>
		PresentationMode1Default = 1,
		/// <summary>Use DRC presentation mode 2 as default (e.g. for DTG DBook)</summary>
		PresentationMode2Default = 2,
	}

	/// <summary>The available metadata profiles which are mostly related to downmixing.
	/// The values define the arguments for the use with <see cref="eParameter.MetadataProfile" />.</summary>
	public enum eMetadataProfile: int
	{
		/// <summary>he standard profile creates a mixdown signal based on the advanced downmix metadata (from a DSE).
		/// The equations and default values are defined in ISO/IEC 14496:3 Amendment 4.
		/// Any other (legacy) downmix metadata will be ignored. No other parameter will be modified.</summary>
		MpegStandard = 0,

		/// <summary>This profile behaves identical to the standard profile if advanced downmix metadata (from a DSE) is available.
		/// If not, the matrix_mixdown information embedded in the program configuration element (PCE) will be applied.
		/// If neither is the case, the module creates a mixdown using the default coefficients as defined in ISO/IEC 14496:3 AMD 4.
		/// The profile can be used to support legacy digital TV( e.g.DVB) streams.</summary>
		MpegLegacy = 1,

		/// <summary>Similar to the <see cref="MpegLegacy"/> profile,
		/// but if both the advanced (ISO/IEC 14496:3 AMD 4) and the legacy (PCE) MPEG downmix metadata are available the latter will be applied.</summary>
		MpegLegacyPrio = 2,

		/// <summary>Downmix creation as described in ABNT NBR 15602-2.
		/// But if advanced downmix metadata (ISO/IEC 14496:3 AMD 4) is available it will be preferred because of the higher resolutions.
		/// In addition the metadata expiry time will be set to the value defined in the ARIB standard, <see cref="eParameter.MetadataExpiryTime" /></summary>
		AribJapan
	}
}