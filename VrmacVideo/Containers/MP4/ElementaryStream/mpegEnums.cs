﻿// This C# source file has been generated by a T4 template.
namespace VrmacVideo.Containers.MP4
{
	/// <summary>ISO/IEC 14496-1 Table 1</summary>
	public enum eDescriptorTag: byte
	{
		/// <summary>Forbidden</summary>
		None = 0x00,
		/// <summary>ObjectDescrTag</summary>
		Object = 0x01,
		/// <summary>InitialObjectDescrTag</summary>
		InitialObject = 0x02,
		/// <summary>ES_DescrTag</summary>
		ElementaryStream = 0x03,
		/// <summary>DecoderConfigDescrTag</summary>
		DecoderConfiguration = 0x04,
		/// <summary>DecSpecificInfoTag</summary>
		DecoderSpecificInfo = 0x05,
		/// <summary>SLConfigDescrTag</summary>
		SyncLayerConfiguration = 0x06,
		/// <summary>ContentIdentDescrTag</summary>
		ContentIdentification = 0x07,
		/// <summary>SupplContentIdentDescrTag</summary>
		SupplementaryContentIdentification = 0x08,
		/// <summary>IPI_DescrPointerTag</summary>
		IPIdentificationPointer = 0x09,
		/// <summary>IPMP_DescrPointerTag, Intellectual Property Management and Protection</summary>
		IPMPPointer = 0x0A,
		/// <summary>IPMP_DescrTag, Intellectual Property Management and Protection</summary>
		IPMP = 0x0B,
		/// <summary>QoS_DescrTag, Quality of Service</summary>
		QoS = 0x0C,
		/// <summary>RegistrationDescrTag</summary>
		Registration = 0x0D,
		/// <summary>ES_ID_IncTag</summary>
		ElementaryStreamIdInc = 0x0E,
		/// <summary>ES_ID_RefTag</summary>
		ElementaryStreamIdRef = 0x0F,
		/// <summary>MP4_IOD_Tag</summary>
		Mp4InitialObjectDescriptor = 0x10,
		/// <summary>MP4_OD_Tag</summary>
		Mp4ObjectDescriptor = 0x11,
		/// <summary>IPL_DescrPointerRefTag, no idea what’s IPL</summary>
		IPL_DescrPointerRef = 0x12,
		/// <summary>ExtensionProfileLevelDescrTag</summary>
		ExtensionProfileLevel = 0x13,
		/// <summary>profileLevelIndicationIndexDescrTag</summary>
		ProfileLevelIndicationIndex = 0x14,
		/// <summary>ContentClassificationDescrTag</summary>
		ContentClassification = 0x40,
		/// <summary>KeyWordDescrTag</summary>
		KeyWord = 0x41,
		/// <summary>RatingDescrTag</summary>
		Rating = 0x42,
		/// <summary>LanguageDescrTag</summary>
		Language = 0x43,
		/// <summary>ShortTextualDescrTag</summary>
		ShortTextual = 0x44,
		/// <summary>ExpandedTextualDescrTag</summary>
		ExpandedTextual = 0x45,
		/// <summary>ContentCreatorNameDescrTag</summary>
		ContentCreatorName = 0x46,
		/// <summary>ContentCreationDateDescrTag</summary>
		ContentCreationDate = 0x47,
		/// <summary>OCICreatorNameDescrTag</summary>
		ContentInformationAuthor = 0x48,
		/// <summary>OCICreationDateDescrTag</summary>
		ContentInformationDate = 0x49,
		/// <summary>SmpteCameraPositionDescrTag, Society of Motion Picture and Television Engineers, probably only relevant for media production</summary>
		SmpteCameraPosition = 0x4A,
		/// <summary>SegmentDescrTag</summary>
		Segment = 0x4B,
		/// <summary>MediaTimeDescrTag</summary>
		MediaTime = 0x4C,
		/// <summary>IPMP_ToolsListDescrTag, Intellectual Property Management and Protection</summary>
		IPMP_ToolsList = 0x60,
		/// <summary>IPMP_ToolTag, Intellectual Property Management and Protection</summary>
		IPMP_Tool = 0x61,
		/// <summary>M4MuxTimingDescrTag</summary>
		M4MuxTiming = 0x62,
		/// <summary>M4MuxCodeTableDescrTag</summary>
		M4MuxCodeTable = 0x63,
		/// <summary>ExtSLConfigDescrTag</summary>
		ExtSLConfig = 0x64,
		/// <summary>M4MuxBufferSizeDescrTag</summary>
		M4MuxBufferSize = 0x65,
		/// <summary>M4MuxIdentDescrTag</summary>
		M4MuxIdent = 0x66,
		/// <summary>DependencyPointerTag</summary>
		DependencyPointer = 0x67,
		/// <summary>DependencyMarkerTag</summary>
		DependencyMarker = 0x68,
		/// <summary>M4MuxChannelDescrTag</summary>
		M4MuxChannel = 0x69,
	}

	/// <summary>ISO/IEC 14496-1 Table 6</summary>
	public enum eObjectType: byte
	{
		/// <summary>Forbidden</summary>
		Empty = 0x00,
		/// <summary>Systems ISO/IEC 14496-1, BIFSConfig specified in ISO/IEC 14496-11</summary>
		Mpeg4ScenesV1 = 0x01,
		/// <summary>Systems ISO/IEC 14496-1, BIFSv2Config specified in ISO/IEC 14496-11.</summary>
		Mpeg4ScenesV2 = 0x02,
		/// <summary>Interaction Stream</summary>
		Interaction = 0x03,
		/// <summary>Systems ISO/IEC 14496-1 Extended BIFS Configuration</summary>
		Mpeg4ScenesExtended = 0x04,
		/// <summary>Systems ISO/IEC 14496-1 AFX; AFX (Animation Framework eXtension) is a set of tools for efficiently coding the shape, texture and animation of interactive synthetic 3D objects. </summary>
		AFX = 0x05,
		/// <summary>Font Data Stream</summary>
		Font = 0x06,
		/// <summary>Synthesized Texture Stream</summary>
		SynthesizedTexture = 0x07,
		/// <summary>Streaming Text Stream</summary>
		StreamingText = 0x08,
		/// <summary>Visual ISO/IEC 14496-2</summary>
		Mpeg4Video = 0x20,
		/// <summary>Visual ITU-T Recommendation H.264 | ISO/IEC 14496-10</summary>
		h264 = 0x21,
		/// <summary>Parameter Sets for ITU-T Recommendation H.264 | ISO/IEC 14496-10</summary>
		h264ParameterSets = 0x22,
		/// <summary>Audio ISO/IEC 14496-3</summary>
		Mpeg4Audio = 0x40,
		/// <summary>Visual ISO/IEC 13818-2 Simple Profile</summary>
		h262Simple = 0x60,
		/// <summary>Visual ISO/IEC 13818-2 Main Profile</summary>
		h262Main = 0x61,
		/// <summary>Visual ISO/IEC 13818-2 SNR Profile, Signal-to-Noise Ratio (SNR) scalable</summary>
		h262SNR = 0x62,
		/// <summary>Visual ISO/IEC 13818-2 Spatial Profile</summary>
		h262Spatial = 0x63,
		/// <summary>Visual ISO/IEC 13818-2 High Profile</summary>
		h262High = 0x64,
		/// <summary>Visual ISO/IEC 13818-2 422 Profile</summary>
		h262_422 = 0x65,
		/// <summary>Audio ISO/IEC 13818-7 Main Profile</summary>
		Aac = 0x66,
		/// <summary>Audio ISO/IEC 13818-7 LowComplexity Profile</summary>
		AacLC = 0x67,
		/// <summary>Audio ISO/IEC 13818-7 Scaleable Sampling Rate Profile</summary>
		AacSSR = 0x68,
		/// <summary>Audio ISO/IEC 13818-3</summary>
		Mpeg2Audio = 0x69,
		/// <summary>Visual ISO/IEC 11172-2</summary>
		Mpeg1Video = 0x6A,
		/// <summary>Audio ISO/IEC 11172-3</summary>
		MP3 = 0x6B,
		/// <summary>Visual ISO/IEC 10918-1</summary>
		JPEG = 0x6C,
		/// <summary>No object type specified</summary>
		None = 0xFF,
	}

	/// <summary>ISO/IEC 14496-1 Table 5</summary>
	public enum eStreamType: byte
	{
		/// <summary>Forbidden</summary>
		None = 0x00,
		/// <summary>ObjectDescriptorStream, to dynamically update object descriptors or their components</summary>
		ObjectDescriptor = 0x01,
		/// <summary>ClockReferenceStream, for the sole purpose of conveying clock reference time stamps</summary>
		ClockReference = 0x02,
		/// <summary>SceneDescriptionStream, MPEG-4 Part 11 “Scene description and application engine”</summary>
		Scene = 0x03,
		/// <summary>VisualStream</summary>
		Video = 0x04,
		/// <summary>AudioStream</summary>
		Audio = 0x05,
		/// <summary>MPEG7Stream, Multimedia content description</summary>
		MPEG7 = 0x06,
		/// <summary>IPMPStream, Intellectual Property Management and Protection</summary>
		IPMP = 0x07,
		/// <summary>ObjectContentInfoStream; OCI stream is an elementary stream that conveys time-varying object content information, termed OCI events.</summary>
		ObjectContentInfo = 0x08,
		/// <summary>MPEGJStream, some embedded Java stuff from 2005</summary>
		MPEGJ = 0x09,
		/// <summary>Interaction Stream</summary>
		Interaction = 0x0A,
		/// <summary>IPMPToolStream, Intellectual Property Management and Protection</summary>
		IPMPTool = 0x0B,
	}

	/// <summary>ISO/IEC 14496-10 Table 7-1</summary>
	public enum eNaluType: byte
	{
		/// <summary>Unspecified</summary>
		None = 0,
		/// <summary>Coded slice of a non-IDR picture</summary>
		NonIDR = 1,
		/// <summary>Coded slice data partition A</summary>
		PartitionA = 2,
		/// <summary>Coded slice data partition B</summary>
		PartitionB = 3,
		/// <summary>Coded slice data partition C</summary>
		PartitionC = 4,
		/// <summary>Coded slice of an IDR picture</summary>
		IDR = 5,
		/// <summary>Supplemental enhancement information</summary>
		SEI = 6,
		/// <summary>Sequence parameter set</summary>
		SPS = 7,
		/// <summary>Picture parameter set</summary>
		PPS = 8,
		/// <summary>Access unit delimiter</summary>
		Delimiter = 9,
		/// <summary>End of sequence</summary>
		SequenceEnd = 10,
		/// <summary>End of stream</summary>
		StreamEnd = 11,
		/// <summary>Filler data</summary>
		Filler = 12,
	}

	/// <summary>Recommendation ITU-T H.264 Table 7-6 "Name association to slice_type"</summary>
	public enum eSliceType: byte
	{
		/// <summary>Predicted picture</summary>
		P = 0,
		/// <summary>Bidirectional predicted picture</summary>
		B = 1,
		/// <summary>Intra-coded picture</summary>
		I = 2,
		/// <summary>Switching P slice</summary>
		SP = 3,
		/// <summary>Switching I slice</summary>
		SI = 4,
		/// <summary>Predicted picture, fixed slice type</summary>
		PF = 5,
		/// <summary>Bidirectional predicted picture, fixed slice type</summary>
		BF = 6,
		/// <summary>Intra-coded picture, fixed slice type</summary>
		IF = 7,
		/// <summary>Switching P slice, fixed slice type</summary>
		SPF = 8,
		/// <summary>Switching I slice, fixed slice type</summary>
		SIF = 9,
	}

	/// <summary>ISO/IEC 14496-10 table E-3 "Colour primaries"</summary>
	public enum eColorPrimaries: byte
	{
		/// <summary>ITU-R Recommendation BT.709</summary>
		BT709 = 1,
		/// <summary>Image characteristics are unknown or as determined by the application</summary>
		Unspecified = 2,
		/// <summary>ITU-R Recommendation BT.470-2 System M</summary>
		BT470M = 4,
		/// <summary>ITU-R Recommendation BT.470-2 System B, G</summary>
		BT470BG = 5,
		/// <summary>Society of Motion Picture and Television Engineers 170M</summary>
		SMPTE170 = 6,
		/// <summary>Society of Motion Picture and Television Engineers 240M (1987)</summary>
		SMPTE240 = 7,
		/// <summary>Generic film, color filters using Illuminant C</summary>
		Film = 8,
	}

	/// <summary>ISO/IEC 14496-10 table E-4 "Transfer characteristics"</summary>
	public enum eTransferFunc: byte
	{
		/// <summary>ITU-R Recommendation BT.709</summary>
		BT709 = 1,
		/// <summary>Image characteristics are unknown or are determined by the application</summary>
		Unspecified = 2,
		/// <summary>ITU-R Recommendation BT.470-2 System M</summary>
		BT470M = 4,
		/// <summary>ITU-R Recommendation BT.470-2 System B, G</summary>
		BT470BG = 5,
		/// <summary>Society of Motion Picture and Television Engineers 170M</summary>
		SMPTE170 = 6,
		/// <summary>Society of Motion Picture and Television Engineers 240M (1987)</summary>
		SMPTE240 = 7,
		/// <summary>Linear transfer characteristics</summary>
		Linear = 8,
		/// <summary>Logarithmic transfer characteristic, 100:1 range</summary>
		Log100 = 9,
		/// <summary>Logarithmic transfer characteristic, 316.22777:1 range</summary>
		Log316 = 10,
	}
}