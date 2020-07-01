namespace VrmacVideo.Containers.MKV
{
	/// <summary>MKV element types</summary>
	public enum eElement: uint
	{
		/// <summary>Set the EBML characteristics of the data to follow. Each EBML Document has to start with this.</summary>
		EBML = 0x1a45dfa3,
		/// <summary>The version of EBML specifications used to create the EBML Document. The version of EBML defined in this document is 1, so EBMLVersion SHOULD be 1.</summary>
		EBMLVersion = 0x4286,
		/// <summary>The minimum EBML version an EBML Reader has to support to read this EBML Document. The EBMLReadVersion Element MUST be less than or equal to EBMLVersion.</summary>
		EBMLReadVersion = 0x42f7,
		/// <summary>The EBMLMaxIDLength Element stores the maximum permitted length in octets of the Element IDs to be found within the EBML Body. An EBMLMaxIDLength Element value of four is RECOMMENDED, though larger values are allowed.</summary>
		EBMLMaxIDLength = 0x42f2,
		/// <summary>The EBMLMaxSizeLength Element stores the maximum permitted length in octets of the expressions of all Element Data Sizes to be found within the EBML Body. The EBMLMaxSizeLength Element documents an upper bound for the
		/// length of all Element Data Size expressions within the EBML Body and not an upper bound for the value of all Element Data Size expressions within the EBML Body. EBML Elements that have an Element Data Size expression
		/// which is larger in octets than what is expressed by EBMLMaxSizeLength Element are invalid.</summary>
		EBMLMaxSizeLength = 0x42f3,
		/// <summary>A string that describes and identifies the content of the EBML Body that follows this EBML Header.</summary>
		DocType = 0x4282,
		/// <summary>The version of DocType interpreter used to create the EBML Document.</summary>
		DocTypeVersion = 0x4287,
		/// <summary>The minimum DocType version an EBML Reader has to support to read this EBML Document. The value of the DocTypeReadVersion Element MUST be less than or equal to the value of the DocTypeVersion Element.</summary>
		DocTypeReadVersion = 0x4285,
		/// <summary>A DocTypeExtension adds extra Elements to the main DocType+DocTypeVersion tuple it's attached to. An EBML Reader MAY know these extra Elements and how to use them. A DocTypeExtension MAY be used to iterate between
		/// experimental Elements before they are integrated in a regular DocTypeVersion. Reading one DocTypeExtension version of a DocType+DocTypeVersion tuple doesn't imply one should be able to read upper versions of this
		/// DocTypeExtension.</summary>
		DocTypeExtension = 0x4281,
		/// <summary>The name of the DocTypeExtension to differentiate it from other DocTypeExtension of the same DocType+DocTypeVersion tuple. A DocTypeExtensionName value MUST be unique within the EBML Header.</summary>
		DocTypeExtensionName = 0x4283,
		/// <summary>The version of the DocTypeExtension. Different DocTypeExtensionVersion values of the same DocType+DocTypeVersion+DocTypeExtensionName tuple MAY contain completely different sets of extra Elements. An EBML Reader MAY
		/// support multiple versions of the same DocTypeExtension, only one or none.</summary>
		DocTypeExtensionVersion = 0x4284,
		/// <summary>The CRC-32 Element contains a 32-bit Cyclic Redundancy Check value of all the Element Data of the Parent Element as stored except for the CRC-32 Element itself. When the CRC-32 Element is present, the CRC-32 Element
		/// MUST be the first ordered EBML Element within its Parent Element for easier reading.</summary>
		CRC32 = 0xbf,
		/// <summary>Used to void data or to avoid unexpected behaviors when using damaged data. The content is discarded. Also used to reserve space in a sub-element for later use.</summary>
		Void = 0xec,
		/// <summary>The Root Element that contains all other Top-Level Elements (Elements defined only at Level 1). A Matroska file is composed of 1 Segment.</summary>
		Segment = 0x18538067,
		/// <summary>Contains the Segment Position of other Top-Level Elements.</summary>
		SeekHead = 0x114d9b74,
		/// <summary>Contains a single seek entry to an EBML Element.</summary>
		Seek = 0x4dbb,
		/// <summary>The binary ID corresponding to the Element name.</summary>
		SeekID = 0x53ab,
		/// <summary>The Segment Position of the Element.</summary>
		SeekPosition = 0x53ac,
		/// <summary>Contains general information about the Segment.</summary>
		Info = 0x1549a966,
		/// <summary>A randomly generated unique ID to identify the Segment amongst many others (128 bits).</summary>
		SegmentUID = 0x73a4,
		/// <summary>A filename corresponding to this Segment.</summary>
		SegmentFilename = 0x7384,
		/// <summary>A unique ID to identify the previous Segment of a Linked Segment (128 bits).</summary>
		PrevUID = 0x3cb923,
		/// <summary>A filename corresponding to the file of the previous Linked Segment.</summary>
		PrevFilename = 0x3c83ab,
		/// <summary>A unique ID to identify the next Segment of a Linked Segment (128 bits).</summary>
		NextUID = 0x3eb923,
		/// <summary>A filename corresponding to the file of the next Linked Segment.</summary>
		NextFilename = 0x3e83bb,
		/// <summary>A randomly generated unique ID that all Segments of a Linked Segment MUST share (128 bits).</summary>
		SegmentFamily = 0x4444,
		/// <summary>A tuple of corresponding ID used by chapter codecs to represent this Segment.</summary>
		ChapterTranslate = 0x6924,
		/// <summary>Specify an edition UID on which this correspondence applies. When not specified, it means for all editions found in the Segment.</summary>
		ChapterTranslateEditionUID = 0x69fc,
		/// <summary>The <a href="https://www.matroska.org/technical/elements.html#ChapProcessCodecID">chapter codec</a></summary>
		ChapterTranslateCodec = 0x69bf,
		/// <summary>The binary value used to represent this Segment in the chapter codec data. The format depends on the <a href="https://www.matroska.org/technical/chapters.html#ChapProcessCodecID">ChapProcessCodecID</a> used.</summary>
		ChapterTranslateID = 0x69a5,
		/// <summary>Timestamp scale in nanoseconds (1.000.000 means all timestamps in the Segment are expressed in milliseconds).</summary>
		TimestampScale = 0x2ad7b1,
		/// <summary>Duration of the Segment in nanoseconds based on TimestampScale.</summary>
		Duration = 0x4489,
		/// <summary>The date and time that the Segment was created by the muxing application or library.</summary>
		DateUTC = 0x4461,
		/// <summary>General name of the Segment.</summary>
		Title = 0x7ba9,
		/// <summary>Muxing application or library (example: "libmatroska-0.4.3").</summary>
		MuxingApp = 0x4d80,
		/// <summary>Writing application (example: "mkvmerge-0.3.3").</summary>
		WritingApp = 0x5741,
		/// <summary>The Top-Level Element containing the (monolithic) Block structure.</summary>
		Cluster = 0x1f43b675,
		/// <summary>Absolute timestamp of the cluster (based on TimestampScale).</summary>
		Timestamp = 0xe7,
		/// <summary>The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking or to decide what track to use.</summary>
		SilentTracks = 0x5854,
		/// <summary>One of the track number that are not used from now on in the stream. It could change later if not specified as silent in a further Cluster.</summary>
		SilentTrackNumber = 0x58d7,
		/// <summary>The Segment Position of the Cluster in the Segment (0 in live streams). It might help to resynchronise offset on damaged streams.</summary>
		Position = 0xa7,
		/// <summary>Size of the previous Cluster, in octets. Can be useful for backward playing.</summary>
		PrevSize = 0xab,
		/// <summary>Similar to <a href="https://www.matroska.org/technical/basics.html#block-structure">Block</a> but without all the extra information, mostly used to reduced overhead when no extra feature is needed. (see <a
		/// href="https://www.matroska.org/technical/basics.html#simpleblock-structure">SimpleBlock Structure</a>)</summary>
		SimpleBlock = 0xa3,
		/// <summary>Basic container of information containing a single Block and information specific to that Block.</summary>
		BlockGroup = 0xa0,
		/// <summary>Block containing the actual data to be rendered and a timestamp relative to the Cluster Timestamp. (see <a href="https://www.matroska.org/technical/basics.html#block-structure">Block Structure</a>)</summary>
		Block = 0xa1,
		/// <summary>A Block with no data. It MUST be stored in the stream at the place the real Block would be in display order. (see <a href="https://www.matroska.org/technical/elements.html#BlockVirtual">Block Virtual</a>)</summary>
		BlockVirtual = 0xa2,
		/// <summary>Contain additional blocks to complete the main one. An EBML parser that has no knowledge of the Block structure could still see and use/skip these data.</summary>
		BlockAdditions = 0x75a1,
		/// <summary>Contain the BlockAdditional and some parameters.</summary>
		BlockMore = 0xa6,
		/// <summary>An ID to identify the BlockAdditional level. A value of 1 means the BlockAdditional data is interpreted as additional data passed to the codec with the Block data.</summary>
		BlockAddID = 0xee,
		/// <summary>Interpreted by the codec as it wishes (using the BlockAddID).</summary>
		BlockAdditional = 0xa5,
		/// <summary>The duration of the Block (based on TimestampScale). The BlockDuration Element can be useful at the end of a Track to define the duration of the last frame (as there is no subsequent Block available), or when there is a
		/// break in a track like for subtitle tracks.</summary>
		BlockDuration = 0x9b,
		/// <summary>This frame is referenced and has the specified cache priority. In cache only a frame of the same or higher priority can replace this frame. A value of 0 means the frame is not referenced.</summary>
		ReferencePriority = 0xfa,
		/// <summary>Timestamp of another frame used as a reference (ie: B or P frame). The timestamp is relative to the block it's attached to.</summary>
		ReferenceBlock = 0xfb,
		/// <summary>The Segment Position of the data that would otherwise be in position of the virtual block.</summary>
		ReferenceVirtual = 0xfd,
		/// <summary>The new codec state to use. Data interpretation is private to the codec. This information SHOULD always be referenced by a seek entry.</summary>
		CodecState = 0xa4,
		/// <summary>Duration in nanoseconds of the silent data added to the Block (padding at the end of the Block for positive value, at the beginning of the Block for negative value). The duration of DiscardPadding is not calculated in
		/// the duration of the TrackEntry and SHOULD be discarded during playback.</summary>
		DiscardPadding = 0x75a2,
		/// <summary>Contains slices description.</summary>
		Slices = 0x8e,
		/// <summary>Contains extra time information about the data contained in the Block. Being able to interpret this Element is not REQUIRED for playback.</summary>
		TimeSlice = 0xe8,
		/// <summary>The reverse number of the frame in the lace (0 is the last frame, 1 is the next to last, etc). Being able to interpret this Element is not REQUIRED for playback.</summary>
		LaceNumber = 0xcc,
		/// <summary>The number of the frame to generate from this lace with this delay (allow you to generate many frames from the same Block/Frame).</summary>
		FrameNumber = 0xcd,
		/// <summary>The ID of the BlockAdditional Element (0 is the main Block).</summary>
		BlockAdditionID = 0xcb,
		/// <summary>The (scaled) delay to apply to the Element.</summary>
		Delay = 0xce,
		/// <summary>The (scaled) duration to apply to the Element.</summary>
		SliceDuration = 0xcf,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		ReferenceFrame = 0xc8,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		ReferenceOffset = 0xc9,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		ReferenceTimestamp = 0xca,
		/// <summary>Similar to <a href="https://www.matroska.org/technical/basics.html#simpleblock-structure">SimpleBlock</a> but the data inside the Block are Transformed (encrypt and/or signed).</summary>
		EncryptedBlock = 0xaf,
		/// <summary>A Top-Level Element of information with many tracks described.</summary>
		Tracks = 0x1654ae6b,
		/// <summary>Describes a track with all Elements.</summary>
		TrackEntry = 0xae,
		/// <summary>The track number as used in the Block Header (using more than 127 tracks is not encouraged, though the design allows an unlimited number).</summary>
		TrackNumber = 0xd7,
		/// <summary>A unique ID to identify the Track. This SHOULD be kept the same when making a direct stream copy of the Track to another file.</summary>
		TrackUID = 0x73c5,
		/// <summary>A set of track types coded on 8 bits.</summary>
		TrackType = 0x83,
		/// <summary>Set if the track is usable. (1 bit)</summary>
		FlagEnabled = 0xb9,
		/// <summary>Set if that track (audio, video or subs) SHOULD be active if no language found matches the user preference. (1 bit)</summary>
		FlagDefault = 0x88,
		/// <summary>Set if that track MUST be active during playback. There can be many forced track for a kind (audio, video or subs), the player SHOULD select the one which language matches the user preference or the default + forced
		/// track. Overlay MAY happen between a forced and non-forced track of the same kind. (1 bit)</summary>
		FlagForced = 0x55aa,
		/// <summary>Set if the track MAY contain blocks using lacing. (1 bit)</summary>
		FlagLacing = 0x9c,
		/// <summary>The minimum number of frames a player SHOULD be able to cache during playback. If set to 0, the reference pseudo-cache system is not used.</summary>
		MinCache = 0x6de7,
		/// <summary>The maximum cache size necessary to store referenced frames in and the current frame. 0 means no cache is needed.</summary>
		MaxCache = 0x6df8,
		/// <summary>Number of nanoseconds (not scaled via TimestampScale) per frame ('frame' in the Matroska sense -- one Element put into a (Simple)Block).</summary>
		DefaultDuration = 0x23e383,
		/// <summary>The period in nanoseconds (not scaled by TimestampScale) between two successive fields at the output of the decoding process (see <a href="https://www.matroska.org/technical/notes.html#defaultdecodedfieldduration">the
		/// notes</a>)</summary>
		DefaultDecodedFieldDuration = 0x234e7a,
		/// <summary>DEPRECATED, DO NOT USE. The scale to apply on this track to work at normal speed in relation with other tracks (mostly used to adjust video speed when the audio length differs).</summary>
		TrackTimestampScale = 0x23314f,
		/// <summary>A value to add to the Block's Timestamp. This can be used to adjust the playback offset of a track.</summary>
		TrackOffset = 0x537f,
		/// <summary>The maximum value of <a href="https://www.matroska.org/technical/elements.html#BlockAddID">BlockAddID</a>. A value 0 means there is no <a
		/// href="https://www.matroska.org/technical/elements.html#BlockAdditions">BlockAdditions</a> for this track.</summary>
		MaxBlockAdditionID = 0x55ee,
		/// <summary>Contains elements that describe each value of <a href="https://www.matroska.org/technical/elements.html#BlockAddID">BlockAddID</a> found in the Track.</summary>
		BlockAdditionMapping = 0x41e4,
		/// <summary>The <a href="https://www.matroska.org/technical/elements.html#BlockAddID">BlockAddID</a> value being described. To keep MaxBlockAdditionID as low as possible, small values SHOULD be used.</summary>
		BlockAddIDValue = 0x41f0,
		/// <summary>A human-friendly name describing the type of BlockAdditional data as defined by the associated Block Additional Mapping.</summary>
		BlockAddIDName = 0x41a4,
		/// <summary>Stores the registered identifer of the Block Additional Mapping to define how the BlockAdditional data should be handled.</summary>
		BlockAddIDType = 0x41e7,
		/// <summary>Extra binary data that the BlockAddIDType can use to interpret the BlockAdditional data. The intepretation of the binary data depends on the BlockAddIDType value and the corresponding Block Additional Mapping.</summary>
		BlockAddIDExtraData = 0x41ed,
		/// <summary>A human-readable track name.</summary>
		Name = 0x536e,
		/// <summary>Specifies the language of the track in the <a href="https://www.matroska.org/technical/basics.html#language-codes">Matroska languages form</a>. This Element MUST be ignored if the LanguageIETF Element is used in the
		/// same TrackEntry.</summary>
		Language = 0x22b59c,
		/// <summary>Specifies the language of the track according to <a href="https://tools.ietf.org/html/bcp47">BCP 47</a> and using the <a href="https://www.iana.com/assignments/language-subtag-registry/language-subtag-registry">IANA
		/// Language Subtag Registry</a>. If this Element is used, then any Language Elements used in the same TrackEntry MUST be ignored.</summary>
		LanguageIETF = 0x22b59d,
		/// <summary>An ID corresponding to the codec, see the <a href="https://www.matroska.org/technical/codec_specs.html">codec page</a> for more info.</summary>
		CodecID = 0x86,
		/// <summary>Private data only known to the codec.</summary>
		CodecPrivate = 0x63a2,
		/// <summary>A human-readable string specifying the codec.</summary>
		CodecName = 0x258688,
		/// <summary>The UID of an attachment that is used by this codec.</summary>
		AttachmentLink = 0x7446,
		/// <summary>A string describing the encoding setting used.</summary>
		CodecSettings = 0x3a9697,
		/// <summary>A URL to find information about the codec used.</summary>
		CodecInfoURL = 0x3b4040,
		/// <summary>A URL to download about the codec used.</summary>
		CodecDownloadURL = 0x26b240,
		/// <summary>The codec can decode potentially damaged data (1 bit).</summary>
		CodecDecodeAll = 0xaa,
		/// <summary>Specify that this track is an overlay track for the Track specified (in the u-integer). That means when this track has a gap (see <a href="https://www.matroska.org/technical/elements.html#SilentTracks">SilentTracks</a>)
		/// the overlay track SHOULD be used instead. The order of multiple TrackOverlay matters, the first one is the one that SHOULD be used. If not found it SHOULD be the second, etc.</summary>
		TrackOverlay = 0x6fab,
		/// <summary>CodecDelay is The codec-built-in delay in nanoseconds. This value MUST be subtracted from each block timestamp in order to get the actual timestamp. The value SHOULD be small so the muxing of tracks with the same actual
		/// timestamp are in the same Cluster.</summary>
		CodecDelay = 0x56aa,
		/// <summary>After a discontinuity, SeekPreRoll is the duration in nanoseconds of the data the decoder MUST decode before the decoded data is valid.</summary>
		SeekPreRoll = 0x56bb,
		/// <summary>The track identification for the given Chapter Codec.</summary>
		TrackTranslate = 0x6624,
		/// <summary>Specify an edition UID on which this translation applies. When not specified, it means for all editions found in the Segment.</summary>
		TrackTranslateEditionUID = 0x66fc,
		/// <summary>The <a href="https://www.matroska.org/technical/elements.html#ChapProcessCodecID">chapter codec</a>.</summary>
		TrackTranslateCodec = 0x66bf,
		/// <summary>The binary value used to represent this track in the chapter codec data. The format depends on the <a href="https://www.matroska.org/technical/elements.html#ChapProcessCodecID">ChapProcessCodecID</a> used.</summary>
		TrackTranslateTrackID = 0x66a5,
		/// <summary>Video settings.</summary>
		Video = 0xe0,
		/// <summary>A flag to declare if the video is known to be progressive or interlaced and if applicable to declare details about the interlacement.</summary>
		FlagInterlaced = 0x9a,
		/// <summary>Declare the field ordering of the video. If FlagInterlaced is not set to 1, this Element MUST be ignored.</summary>
		FieldOrder = 0x9d,
		/// <summary>Stereo-3D video mode. There are some more details on <a href="https://www.matroska.org/technical/notes.html#multi-planar-and-3d-videos">3D support in the Specification Notes</a>.</summary>
		StereoMode = 0x53b8,
		/// <summary>Alpha Video Mode. Presence of this Element indicates that the BlockAdditional Element could contain Alpha data.</summary>
		AlphaMode = 0x53c0,
		/// <summary>DEPRECATED, DO NOT USE. Bogus StereoMode value used in old versions of libmatroska.</summary>
		OldStereoMode = 0x53b9,
		/// <summary>Width of the encoded video frames in pixels.</summary>
		PixelWidth = 0xb0,
		/// <summary>Height of the encoded video frames in pixels.</summary>
		PixelHeight = 0xba,
		/// <summary>The number of video pixels to remove at the bottom of the image.</summary>
		PixelCropBottom = 0x54aa,
		/// <summary>The number of video pixels to remove at the top of the image.</summary>
		PixelCropTop = 0x54bb,
		/// <summary>The number of video pixels to remove on the left of the image.</summary>
		PixelCropLeft = 0x54cc,
		/// <summary>The number of video pixels to remove on the right of the image.</summary>
		PixelCropRight = 0x54dd,
		/// <summary>Width of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements).</summary>
		DisplayWidth = 0x54b0,
		/// <summary>Height of the video frames to display. Applies to the video frame after cropping (PixelCrop* Elements).</summary>
		DisplayHeight = 0x54ba,
		/// <summary>How DisplayWidth &amp; DisplayHeight are interpreted.</summary>
		DisplayUnit = 0x54b2,
		/// <summary>Specify the possible modifications to the aspect ratio.</summary>
		AspectRatioType = 0x54b3,
		/// <summary>Specify the pixel format used for the Track's data as a FourCC. This value is similar in scope to the biCompression value of AVI's BITMAPINFOHEADER.</summary>
		ColourSpace = 0x2eb524,
		/// <summary>Gamma Value.</summary>
		GammaValue = 0x2fb523,
		/// <summary>Number of frames per second. <strong>Informational</strong> only.</summary>
		FrameRate = 0x2383e3,
		/// <summary>Settings describing the colour format.</summary>
		Colour = 0x55b0,
		/// <summary>The Matrix Coefficients of the video used to derive luma and chroma values from red, green, and blue color primaries. For clarity, the value and meanings for MatrixCoefficients are adopted from Table 4 of ISO/IEC
		/// 23001-8:2016 or ITU-T H.273.</summary>
		MatrixCoefficients = 0x55b1,
		/// <summary>Number of decoded bits per channel. A value of 0 indicates that the BitsPerChannel is unspecified.</summary>
		BitsPerChannel = 0x55b2,
		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed horizontally. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingHorz SHOULD be set to 1.</summary>
		ChromaSubsamplingHorz = 0x55b3,
		/// <summary>The amount of pixels to remove in the Cr and Cb channels for every pixel not removed vertically. Example: For video with 4:2:0 chroma subsampling, the ChromaSubsamplingVert SHOULD be set to 1.</summary>
		ChromaSubsamplingVert = 0x55b4,
		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed horizontally. This is additive with ChromaSubsamplingHorz. Example: For video with 4:2:1 chroma subsampling, the ChromaSubsamplingHorz SHOULD
		/// be set to 1 and CbSubsamplingHorz SHOULD be set to 1.</summary>
		CbSubsamplingHorz = 0x55b5,
		/// <summary>The amount of pixels to remove in the Cb channel for every pixel not removed vertically. This is additive with ChromaSubsamplingVert.</summary>
		CbSubsamplingVert = 0x55b6,
		/// <summary>How chroma is subsampled horizontally.</summary>
		ChromaSitingHorz = 0x55b7,
		/// <summary>How chroma is subsampled vertically.</summary>
		ChromaSitingVert = 0x55b8,
		/// <summary>Clipping of the color ranges.</summary>
		Range = 0x55b9,
		/// <summary>The transfer characteristics of the video. For clarity, the value and meanings for TransferCharacteristics are adopted from Table 3 of ISO/IEC 23091-4 or ITU-T H.273.</summary>
		TransferCharacteristics = 0x55ba,
		/// <summary>The colour primaries of the video. For clarity, the value and meanings for Primaries are adopted from Table 2 of ISO/IEC 23091-4 or ITU-T H.273.</summary>
		Primaries = 0x55bb,
		/// <summary>Maximum brightness of a single pixel (Maximum Content Light Level) in candelas per square meter (cd/m²).</summary>
		MaxCLL = 0x55bc,
		/// <summary>Maximum brightness of a single full frame (Maximum Frame-Average Light Level) in candelas per square meter (cd/m²).</summary>
		MaxFALL = 0x55bd,
		/// <summary>SMPTE 2086 mastering data.</summary>
		MasteringMetadata = 0x55d0,
		/// <summary>Red X chromaticity coordinate as defined by CIE 1931.</summary>
		PrimaryRChromaticityX = 0x55d1,
		/// <summary>Red Y chromaticity coordinate as defined by CIE 1931.</summary>
		PrimaryRChromaticityY = 0x55d2,
		/// <summary>Green X chromaticity coordinate as defined by CIE 1931.</summary>
		PrimaryGChromaticityX = 0x55d3,
		/// <summary>Green Y chromaticity coordinate as defined by CIE 1931.</summary>
		PrimaryGChromaticityY = 0x55d4,
		/// <summary>Blue X chromaticity coordinate as defined by CIE 1931.</summary>
		PrimaryBChromaticityX = 0x55d5,
		/// <summary>Blue Y chromaticity coordinate as defined by CIE 1931.</summary>
		PrimaryBChromaticityY = 0x55d6,
		/// <summary>White X chromaticity coordinate as defined by CIE 1931.</summary>
		WhitePointChromaticityX = 0x55d7,
		/// <summary>White Y chromaticity coordinate as defined by CIE 1931.</summary>
		WhitePointChromaticityY = 0x55d8,
		/// <summary>Maximum luminance. Represented in candelas per square meter (cd/m²).</summary>
		LuminanceMax = 0x55d9,
		/// <summary>Minimum luminance. Represented in candelas per square meter (cd/m²).</summary>
		LuminanceMin = 0x55da,
		/// <summary>Describes the video projection details. Used to render spherical and VR videos.</summary>
		Projection = 0x7670,
		/// <summary>Describes the projection used for this video track.</summary>
		ProjectionType = 0x7671,
		/// <summary>Private data that only applies to a specific projection.<br/>Semantics<br/>If ProjectionType equals 0 (Rectangular), 			then this element must not be present.<br/>If ProjectionType equals 1 (Equirectangular), then this
		/// element must be present and contain the same binary data that would be stored inside 			an ISOBMFF Equirectangular Projection Box ('equi').<br/>If ProjectionType equals 2 (Cubemap), then this element must be present and
		/// contain the same binary data that would be stored 			inside an ISOBMFF Cubemap Projection Box ('cbmp').<br/>If ProjectionType equals 3 (Mesh), then this element must be present and contain the same binary data that
		/// would be stored inside 			an ISOBMFF Mesh Projection Box ('mshp').<br/>Note: ISOBMFF box size and fourcc fields are not included in the binary data, but the FullBox version and flag fields are. This is to avoid
		/// redundant framing information while preserving versioning and semantics between the two container formats.</summary>
		ProjectionPrivate = 0x7672,
		/// <summary>Specifies a yaw rotation to the projection.<br/>Semantics<br/>Value represents a clockwise rotation, in degrees, around the up vector. This rotation must be applied before any ProjectionPosePitch or ProjectionPoseRoll
		/// rotations. The value of this field should be in the -180 to 180 degree range.</summary>
		ProjectionPoseYaw = 0x7673,
		/// <summary>Specifies a pitch rotation to the projection.<br/>Semantics<br/>Value represents a counter-clockwise rotation, in degrees, around the right vector. This rotation must be applied after the ProjectionPoseYaw rotation and
		/// before the ProjectionPoseRoll rotation. The value of this field should be in the -90 to 90 degree range.</summary>
		ProjectionPosePitch = 0x7674,
		/// <summary>Specifies a roll rotation to the projection.<br/>Semantics<br/>Value represents a counter-clockwise rotation, in degrees, around the forward vector. This rotation must be applied after the ProjectionPoseYaw and
		/// ProjectionPosePitch rotations. The value of this field should be in the -180 to 180 degree range.</summary>
		ProjectionPoseRoll = 0x7675,
		/// <summary>Audio settings.</summary>
		Audio = 0xe1,
		/// <summary>Sampling frequency in Hz.</summary>
		SamplingFrequency = 0xb5,
		/// <summary>Real output sampling frequency in Hz (used for SBR techniques).</summary>
		OutputSamplingFrequency = 0x78b5,
		/// <summary>Numbers of channels in the track.</summary>
		Channels = 0x9f,
		/// <summary>Table of horizontal angles for each successive channel.</summary>
		ChannelPositions = 0x7d7b,
		/// <summary>Bits per sample, mostly used for PCM.</summary>
		BitDepth = 0x6264,
		/// <summary>Operation that needs to be applied on tracks to create this virtual track. For more details <a href="https://www.matroska.org/technical/notes.html#track-operation">look at the Specification Notes</a> on the subject.</summary>
		TrackOperation = 0xe2,
		/// <summary>Contains the list of all video plane tracks that need to be combined to create this 3D track</summary>
		TrackCombinePlanes = 0xe3,
		/// <summary>Contains a video plane track that need to be combined to create this 3D track</summary>
		TrackPlane = 0xe4,
		/// <summary>The trackUID number of the track representing the plane.</summary>
		TrackPlaneUID = 0xe5,
		/// <summary>The kind of plane this track corresponds to.</summary>
		TrackPlaneType = 0xe6,
		/// <summary>Contains the list of all tracks whose Blocks need to be combined to create this virtual track</summary>
		TrackJoinBlocks = 0xe9,
		/// <summary>The trackUID number of a track whose blocks are used to create this virtual track.</summary>
		TrackJoinUID = 0xed,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		TrickTrackUID = 0xc0,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		TrickTrackSegmentUID = 0xc1,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		TrickTrackFlag = 0xc6,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		TrickMasterTrackUID = 0xc7,
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		TrickMasterTrackSegmentUID = 0xc4,
		/// <summary>Settings for several content encoding mechanisms like compression or encryption.</summary>
		ContentEncodings = 0x6d80,
		/// <summary>Settings for one content encoding like compression or encryption.</summary>
		ContentEncoding = 0x6240,
		/// <summary>Tells when this modification was used during encoding/muxing starting with 0 and counting upwards. The decoder/demuxer has to start with the highest order number it finds and work its way down. This value has to be
		/// unique over all ContentEncodingOrder Elements in the TrackEntry that contains this ContentEncodingOrder element.</summary>
		ContentEncodingOrder = 0x5031,
		/// <summary>A bit field that describes which Elements have been modified in this way. Values (big endian) can be OR'ed.</summary>
		ContentEncodingScope = 0x5032,
		/// <summary>A value describing what kind of transformation is applied.</summary>
		ContentEncodingType = 0x5033,
		/// <summary>Settings describing the compression used. This Element MUST be present if the value of ContentEncodingType is 0 and absent otherwise. Each block MUST be decompressable even if no previous block is available in order not
		/// to prevent seeking.</summary>
		ContentCompression = 0x5034,
		/// <summary>The compression algorithm used.</summary>
		ContentCompAlgo = 0x4254,
		/// <summary>Settings that might be needed by the decompressor. For Header Stripping (`ContentCompAlgo`=3), the bytes that were removed from the beginning of each frames of the track.</summary>
		ContentCompSettings = 0x4255,
		/// <summary>Settings describing the encryption used. This Element MUST be present if the value of `ContentEncodingType` is 1 (encryption) and MUST be ignored otherwise.</summary>
		ContentEncryption = 0x5035,
		/// <summary>The encryption algorithm used. The value '0' means that the contents have not been encrypted but only signed.</summary>
		ContentEncAlgo = 0x47e1,
		/// <summary>For public key algorithms this is the ID of the public key the the data was encrypted with.</summary>
		ContentEncKeyID = 0x47e2,
		/// <summary>Settings describing the encryption algorithm used. If `ContentEncAlgo` != 5 this MUST be ignored.</summary>
		ContentEncAESSettings = 0x47e7,
		/// <summary>The AES cipher mode used in the encryption.</summary>
		AESSettingsCipherMode = 0x47e8,
		/// <summary>A cryptographic signature of the contents.</summary>
		ContentSignature = 0x47e3,
		/// <summary>This is the ID of the private key the data was signed with.</summary>
		ContentSigKeyID = 0x47e4,
		/// <summary>The algorithm used for the signature.</summary>
		ContentSigAlgo = 0x47e5,
		/// <summary>The hash algorithm used for the signature.</summary>
		ContentSigHashAlgo = 0x47e6,
		/// <summary>A Top-Level Element to speed seeking access. All entries are local to the Segment.</summary>
		Cues = 0x1c53bb6b,
		/// <summary>Contains all information relative to a seek point in the Segment.</summary>
		CuePoint = 0xbb,
		/// <summary>Absolute timestamp according to the Segment time base.</summary>
		CueTime = 0xb3,
		/// <summary>Contain positions for different tracks corresponding to the timestamp.</summary>
		CueTrackPositions = 0xb7,
		/// <summary>The track for which a position is given.</summary>
		CueTrack = 0xf7,
		/// <summary>The Segment Position of the Cluster containing the associated Block.</summary>
		CueClusterPosition = 0xf1,
		/// <summary>The relative position inside the Cluster of the referenced SimpleBlock or BlockGroup with 0 being the first possible position for an Element inside that Cluster.</summary>
		CueRelativePosition = 0xf0,
		/// <summary>The duration of the block according to the Segment time base. If missing the track's DefaultDuration does not apply and no duration information is available in terms of the cues.</summary>
		CueDuration = 0xb2,
		/// <summary>Number of the Block in the specified Cluster.</summary>
		CueBlockNumber = 0x5378,
		/// <summary>The Segment Position of the Codec State corresponding to this Cue Element. 0 means that the data is taken from the initial Track Entry.</summary>
		CueCodecState = 0xea,
		/// <summary>The Clusters containing the referenced Blocks.</summary>
		CueReference = 0xdb,
		/// <summary>Timestamp of the referenced Block.</summary>
		CueRefTime = 0x96,
		/// <summary>The Segment Position of the Cluster containing the referenced Block.</summary>
		CueRefCluster = 0x97,
		/// <summary>Number of the referenced Block of Track X in the specified Cluster.</summary>
		CueRefNumber = 0x535f,
		/// <summary>The Segment Position of the Codec State corresponding to this referenced Element. 0 means that the data is taken from the initial Track Entry.</summary>
		CueRefCodecState = 0xeb,
		/// <summary>Contain attached files.</summary>
		Attachments = 0x1941a469,
		/// <summary>An attached file.</summary>
		AttachedFile = 0x61a7,
		/// <summary>A human-friendly name for the attached file.</summary>
		FileDescription = 0x467e,
		/// <summary>Filename of the attached file.</summary>
		FileName = 0x466e,
		/// <summary>MIME type of the file.</summary>
		FileMimeType = 0x4660,
		/// <summary>The data of the file.</summary>
		FileData = 0x465c,
		/// <summary>Unique ID representing the file, as random as possible.</summary>
		FileUID = 0x46ae,
		/// <summary>A binary value that a track/codec can refer to when the attachment is needed.</summary>
		FileReferral = 0x4675,
		/// <summary><a href="http://developer.divx.com/docs/divx_plus_hd/format_features/World_Fonts">DivX font extension</a></summary>
		FileUsedStartTime = 0x4661,
		/// <summary><a href="http://developer.divx.com/docs/divx_plus_hd/format_features/World_Fonts">DivX font extension</a></summary>
		FileUsedEndTime = 0x4662,
		/// <summary>A system to define basic menus and partition data. For more detailed information, look at the <a href="https://www.matroska.org/technical/chapters.html">Chapters Explanation</a>.</summary>
		Chapters = 0x1043a770,
		/// <summary>Contains all information about a Segment edition.</summary>
		EditionEntry = 0x45b9,
		/// <summary>A unique ID to identify the edition. It's useful for tagging an edition.</summary>
		EditionUID = 0x45bc,
		/// <summary>If an edition is hidden (1), it SHOULD NOT be available to the user interface (but still to Control Tracks; see <a href="https://www.matroska.org/technical/chapters.html#flags">flag notes</a>). (1 bit)</summary>
		EditionFlagHidden = 0x45bd,
		/// <summary>If a flag is set (1) the edition SHOULD be used as the default one. (1 bit)</summary>
		EditionFlagDefault = 0x45db,
		/// <summary>Specify if the chapters can be defined multiple times and the order to play them is enforced. (1 bit)</summary>
		EditionFlagOrdered = 0x45dd,
		/// <summary>Contains the atom information to use as the chapter atom (apply to all tracks).</summary>
		ChapterAtom = 0xb6,
		/// <summary>A unique ID to identify the Chapter.</summary>
		ChapterUID = 0x73c4,
		/// <summary>A unique string ID to identify the Chapter. Use for <a href="https://w3c.github.io/webvtt/#webvtt-cue-identifier">WebVTT cue identifier storage</a>.</summary>
		ChapterStringUID = 0x5654,
		/// <summary>Timestamp of the start of Chapter (not scaled).</summary>
		ChapterTimeStart = 0x91,
		/// <summary>Timestamp of the end of Chapter (timestamp excluded, not scaled).</summary>
		ChapterTimeEnd = 0x92,
		/// <summary>If a chapter is hidden (1), it SHOULD NOT be available to the user interface (but still to Control Tracks; see <a href="https://www.matroska.org/technical/chapters.html#flags">flag notes</a>). (1 bit)</summary>
		ChapterFlagHidden = 0x98,
		/// <summary>Specify whether the chapter is enabled. It can be enabled/disabled by a Control Track. When disabled, the movie SHOULD skip all the content between the TimeStart and TimeEnd of this chapter (see <a
		/// href="https://www.matroska.org/technical/chapters.html#flags">flag notes</a>). (1 bit)</summary>
		ChapterFlagEnabled = 0x4598,
		/// <summary>The SegmentUID of another Segment to play during this chapter.</summary>
		ChapterSegmentUID = 0x6e67,
		/// <summary>The EditionUID to play from the Segment linked in ChapterSegmentUID. If ChapterSegmentEditionUID is undeclared then no Edition of the linked Segment is used.</summary>
		ChapterSegmentEditionUID = 0x6ebc,
		/// <summary>Specify the physical equivalent of this ChapterAtom like "DVD" (60) or "SIDE" (50), see <a href="https://www.matroska.org/technical/basics.html#physical-types">complete list of values</a>.</summary>
		ChapterPhysicalEquiv = 0x63c3,
		/// <summary>List of tracks on which the chapter applies. If this Element is not present, all tracks apply</summary>
		ChapterTrack = 0x8f,
		/// <summary>UID of the Track to apply this chapter to. In the absence of a control track, choosing this chapter will select the listed Tracks and deselect unlisted tracks. Absence of this Element indicates that the Chapter SHOULD
		/// be applied to any currently used Tracks.</summary>
		ChapterTrackUID = 0x89,
		/// <summary>Contains all possible strings to use for the chapter display.</summary>
		ChapterDisplay = 0x80,
		/// <summary>Contains the string to use as the chapter atom.</summary>
		ChapString = 0x85,
		/// <summary>The languages corresponding to the string, in the <a href="https://www.loc.gov/standards/iso639-2/php/English_list.php">bibliographic ISO-639-2 form</a>. This Element MUST be ignored if the ChapLanguageIETF Element is
		/// used within the same ChapterDisplay Element.</summary>
		ChapLanguage = 0x437c,
		/// <summary>Specifies the language used in the ChapString according to <a href="https://tools.ietf.org/html/bcp47">BCP 47</a> and using the <a
		/// href="https://www.iana.com/assignments/language-subtag-registry/language-subtag-registry">IANA Language Subtag Registry</a>. If this Element is used, then any ChapLanguage Elements used in the same ChapterDisplay MUST
		/// be ignored.</summary>
		ChapLanguageIETF = 0x437d,
		/// <summary>The countries corresponding to the string, same 2 octets as in <a href="https://www.iana.org/domains/root/db">Internet domains</a>. This Element MUST be ignored if the ChapLanguageIETF Element is used within the same
		/// ChapterDisplay Element.</summary>
		ChapCountry = 0x437e,
		/// <summary>Contains all the commands associated to the Atom.</summary>
		ChapProcess = 0x6944,
		/// <summary>Contains the type of the codec used for the processing. A value of 0 means native Matroska processing (to be defined), a value of 1 means the <a href="https://www.matroska.org/technical/chapters.html#dvd">DVD</a>
		/// command set is used. More codec IDs can be added later.</summary>
		ChapProcessCodecID = 0x6955,
		/// <summary>Some optional data attached to the ChapProcessCodecID information. <a href="https://www.matroska.org/technical/chapters.html#dvd">For ChapProcessCodecID = 1</a>, it is the "DVD level" equivalent.</summary>
		ChapProcessPrivate = 0x450d,
		/// <summary>Contains all the commands associated to the Atom.</summary>
		ChapProcessCommand = 0x6911,
		/// <summary>Defines when the process command SHOULD be handled</summary>
		ChapProcessTime = 0x6922,
		/// <summary>Contains the command information. The data SHOULD be interpreted depending on the ChapProcessCodecID value. <a href="https://www.matroska.org/technical/chapters.html#dvd">For ChapProcessCodecID = 1</a>, the data
		/// correspond to the binary DVD cell pre/post commands.</summary>
		ChapProcessData = 0x6933,
		/// <summary>Element containing metadata describing Tracks, Editions, Chapters, Attachments, or the Segment as a whole. A list of valid tags can be found <a href="https://www.matroska.org/technical/tagging.html">here.</a></summary>
		Tags = 0x1254c367,
		/// <summary>A single metadata descriptor.</summary>
		Tag = 0x7373,
		/// <summary>Specifies which other elements the metadata represented by the Tag applies to. If empty or not present, then the Tag describes everything in the Segment.</summary>
		Targets = 0x63c0,
		/// <summary>A number to indicate the logical level of the target.</summary>
		TargetTypeValue = 0x68ca,
		/// <summary>An informational string that can be used to display the logical level of the target like "ALBUM", "TRACK", "MOVIE", "CHAPTER", etc (see <a
		/// href="https://www.matroska.org/technical/tagging.html#targettypes">TargetType</a>).</summary>
		TargetType = 0x63ca,
		/// <summary>A unique ID to identify the Track(s) the tags belong to. If the value is 0 at this level, the tags apply to all tracks in the Segment.</summary>
		TagTrackUID = 0x63c5,
		/// <summary>A unique ID to identify the EditionEntry(s) the tags belong to. If the value is 0 at this level, the tags apply to all editions in the Segment.</summary>
		TagEditionUID = 0x63c9,
		/// <summary>A unique ID to identify the Chapter(s) the tags belong to. If the value is 0 at this level, the tags apply to all chapters in the Segment.</summary>
		TagChapterUID = 0x63c4,
		/// <summary>A unique ID to identify the Attachment(s) the tags belong to. If the value is 0 at this level, the tags apply to all the attachments in the Segment.</summary>
		TagAttachmentUID = 0x63c6,
		/// <summary>Contains general information about the target.</summary>
		SimpleTag = 0x67c8,
		/// <summary>The name of the Tag that is going to be stored.</summary>
		TagName = 0x45a3,
		/// <summary>Specifies the language of the tag specified, in the <a href="https://www.matroska.org/technical/basics.html#language-codes">Matroska languages form</a>. This Element MUST be ignored if the TagLanguageIETF Element is
		/// used within the same SimpleTag Element.</summary>
		TagLanguage = 0x447a,
		/// <summary>Specifies the language used in the TagString according to <a href="https://tools.ietf.org/html/bcp47">BCP 47</a> and using the <a
		/// href="https://www.iana.com/assignments/language-subtag-registry/language-subtag-registry">IANA Language Subtag Registry</a>. If this Element is used, then any TagLanguage Elements used in the same SimpleTag MUST be
		/// ignored.</summary>
		TagLanguageIETF = 0x447b,
		/// <summary>A boolean value to indicate if this is the default/original language to use for the given tag.</summary>
		TagDefault = 0x4484,
		/// <summary>The value of the Tag.</summary>
		TagString = 0x4487,
		/// <summary>The values of the Tag if it is binary. Note that this cannot be used in the same SimpleTag as TagString.</summary>
		TagBinary = 0x4485,
	}
}