using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Describes a track with all Elements.</summary>
	public sealed partial class TrackEntry
	{
		/// <summary>The track number as used in the Block Header (using more than 127 tracks is not encouraged, though the design allows an unlimited number).</summary>
		public readonly ulong trackNumber;
		/// <summary>A unique ID to identify the Track. This SHOULD be kept the same when making a direct stream copy of the Track to another file.</summary>
		public readonly ulong trackUID;
		/// <summary>A set of track types coded on 8 bits.</summary>
		public readonly eTrackType trackType;
		/// <summary>Set if the track is usable. (1 bit)</summary>
		public readonly byte flagEnabled = 1;
		/// <summary>Set if that track (audio, video or subs) SHOULD be active if no language found matches the user preference. (1 bit)</summary>
		public readonly byte flagDefault = 1;
		/// <summary>Set if that track MUST be active during playback. There can be many forced track for a kind (audio, video or subs), the player SHOULD select the one which language matches the user preference or the default + forced
		/// track. Overlay MAY happen between a forced and non-forced track of the same kind. (1 bit)</summary>
		public readonly byte flagForced = 0;
		/// <summary>Set if the track MAY contain blocks using lacing. (1 bit)</summary>
		public readonly byte flagLacing = 1;
		/// <summary>The minimum number of frames a player SHOULD be able to cache during playback. If set to 0, the reference pseudo-cache system is not used.</summary>
		public readonly ulong minCache = 0;
		/// <summary>The maximum cache size necessary to store referenced frames in and the current frame. 0 means no cache is needed.</summary>
		public readonly ulong maxCache;
		/// <summary>Number of nanoseconds (not scaled via TimestampScale) per frame ('frame' in the Matroska sense -- one Element put into a (Simple)Block).</summary>
		public readonly ulong defaultDuration;
		/// <summary>The period in nanoseconds (not scaled by TimestampScale) between two successive fields at the output of the decoding process (see <a href="https://www.matroska.org/technical/notes.html#defaultdecodedfieldduration">the
		/// notes</a>)</summary>
		public readonly ulong defaultDecodedFieldDuration;
		/// <summary>A value to add to the Block's Timestamp. This can be used to adjust the playback offset of a track.</summary>
		public readonly int trackOffset = 0;
		/// <summary>The maximum value of <a href="https://www.matroska.org/technical/elements.html#BlockAddID">BlockAddID</a>. A value 0 means there is no <a
		/// href="https://www.matroska.org/technical/elements.html#BlockAdditions">BlockAdditions</a> for this track.</summary>
		public readonly ulong maxBlockAdditionID = 0;
		/// <summary>Contains elements that describe each value of <a href="https://www.matroska.org/technical/elements.html#BlockAddID">BlockAddID</a> found in the Track.</summary>
		public readonly BlockAdditionMapping[] blockAdditionMapping;
		/// <summary>A human-readable track name.</summary>
		public readonly string name;
		/// <summary>Specifies the language of the track in the <a href="https://www.matroska.org/technical/basics.html#language-codes">Matroska languages form</a>. This Element MUST be ignored if the LanguageIETF Element is used in the
		/// same TrackEntry.</summary>
		public readonly string language = "eng";
		/// <summary>Specifies the language of the track according to <a href="https://tools.ietf.org/html/bcp47">BCP 47</a> and using the <a href="https://www.iana.com/assignments/language-subtag-registry/language-subtag-registry">IANA
		/// Language Subtag Registry</a>. If this Element is used, then any Language Elements used in the same TrackEntry MUST be ignored.</summary>
		public readonly string languageIETF;
		/// <summary>An ID corresponding to the codec, see the <a href="https://www.matroska.org/technical/codec_specs.html">codec page</a> for more info.</summary>
		public readonly string codecID;
		/// <summary>Private data only known to the codec.</summary>
		public readonly byte[] codecPrivate;
		/// <summary>A human-readable string specifying the codec.</summary>
		public readonly string codecName;
		/// <summary>The UID of an attachment that is used by this codec.</summary>
		public readonly ulong attachmentLink;
		/// <summary>A string describing the encoding setting used.</summary>
		public readonly string codecSettings;
		/// <summary>A URL to find information about the codec used.</summary>
		public readonly string[] codecInfoURL;
		/// <summary>A URL to download about the codec used.</summary>
		public readonly string[] codecDownloadURL;
		/// <summary>The codec can decode potentially damaged data (1 bit).</summary>
		public readonly byte codecDecodeAll = 1;
		/// <summary>Specify that this track is an overlay track for the Track specified (in the u-integer). That means when this track has a gap (see <a href="https://www.matroska.org/technical/elements.html#SilentTracks">SilentTracks</a>)
		/// the overlay track SHOULD be used instead. The order of multiple TrackOverlay matters, the first one is the one that SHOULD be used. If not found it SHOULD be the second, etc.</summary>
		public readonly ulong[] trackOverlay;
		/// <summary>CodecDelay is The codec-built-in delay in nanoseconds. This value MUST be subtracted from each block timestamp in order to get the actual timestamp. The value SHOULD be small so the muxing of tracks with the same actual
		/// timestamp are in the same Cluster.</summary>
		public readonly ulong codecDelay = 0;
		/// <summary>After a discontinuity, SeekPreRoll is the duration in nanoseconds of the data the decoder MUST decode before the decoded data is valid.</summary>
		public readonly ulong seekPreRoll = 0;
		/// <summary>The track identification for the given Chapter Codec.</summary>
		public readonly TrackTranslate[] trackTranslate;
		/// <summary>Video settings.</summary>
		public readonly Video video;
		/// <summary>Audio settings.</summary>
		public readonly Audio audio;
		/// <summary>Operation that needs to be applied on tracks to create this virtual track. For more details <a href="https://www.matroska.org/technical/notes.html#track-operation">look at the Specification Notes</a> on the subject.</summary>
		public readonly TrackOperation trackOperation;
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly ulong trickTrackUID;
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly Guid? trickTrackSegmentUID;
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly ulong trickTrackFlag = 0;
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly ulong trickMasterTrackUID;
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly Guid? trickMasterTrackSegmentUID;
		/// <summary>Settings for several content encoding mechanisms like compression or encryption.</summary>
		public readonly ContentEncodings contentEncodings;

		internal TrackEntry( Stream stream )
		{
			List<BlockAdditionMapping> blockAdditionMappinglist = null;
			List<string> codecInfoURLlist = null;
			List<string> codecDownloadURLlist = null;
			List<ulong> trackOverlaylist = null;
			List<TrackTranslate> trackTranslatelist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TrackNumber:
						trackNumber = reader.readUlong();
						break;
					case eElement.TrackUID:
						trackUID = reader.readUlong();
						break;
					case eElement.TrackType:
						trackType = (eTrackType)reader.readByte();
						break;
					case eElement.FlagEnabled:
						flagEnabled = (byte)reader.readUint( 1 );
						break;
					case eElement.FlagDefault:
						flagDefault = (byte)reader.readUint( 1 );
						break;
					case eElement.FlagForced:
						flagForced = (byte)reader.readUint( 0 );
						break;
					case eElement.FlagLacing:
						flagLacing = (byte)reader.readUint( 1 );
						break;
					case eElement.MinCache:
						minCache = reader.readUlong( 0 );
						break;
					case eElement.MaxCache:
						maxCache = reader.readUlong();
						break;
					case eElement.DefaultDuration:
						defaultDuration = reader.readUlong();
						break;
					case eElement.DefaultDecodedFieldDuration:
						defaultDecodedFieldDuration = reader.readUlong();
						break;
					case eElement.TrackOffset:
						trackOffset = reader.readInt( 0 );
						break;
					case eElement.MaxBlockAdditionID:
						maxBlockAdditionID = reader.readUlong( 0 );
						break;
					case eElement.BlockAdditionMapping:
						if( null == blockAdditionMappinglist ) blockAdditionMappinglist = new List<BlockAdditionMapping>();
						blockAdditionMappinglist.Add( new BlockAdditionMapping( stream ) );
						break;
					case eElement.Name:
						name = reader.readUtf8();
						break;
					case eElement.Language:
						language = reader.readAscii();
						break;
					case eElement.LanguageIETF:
						languageIETF = reader.readAscii();
						break;
					case eElement.CodecID:
						codecID = reader.readAscii();
						break;
					case eElement.CodecPrivate:
						codecPrivate = reader.readByteArray();
						break;
					case eElement.CodecName:
						codecName = reader.readUtf8();
						break;
					case eElement.AttachmentLink:
						attachmentLink = reader.readUlong();
						break;
					case eElement.CodecSettings:
						codecSettings = reader.readUtf8();
						break;
					case eElement.CodecInfoURL:
						if( null == codecInfoURLlist ) codecInfoURLlist = new List<string>();
						codecInfoURLlist.Add( reader.readAscii() );
						break;
					case eElement.CodecDownloadURL:
						if( null == codecDownloadURLlist ) codecDownloadURLlist = new List<string>();
						codecDownloadURLlist.Add( reader.readAscii() );
						break;
					case eElement.CodecDecodeAll:
						codecDecodeAll = (byte)reader.readUint( 1 );
						break;
					case eElement.TrackOverlay:
						if( null == trackOverlaylist ) trackOverlaylist = new List<ulong>();
						trackOverlaylist.Add( reader.readUlong() );
						break;
					case eElement.CodecDelay:
						codecDelay = reader.readUlong( 0 );
						break;
					case eElement.SeekPreRoll:
						seekPreRoll = reader.readUlong( 0 );
						break;
					case eElement.TrackTranslate:
						if( null == trackTranslatelist ) trackTranslatelist = new List<TrackTranslate>();
						trackTranslatelist.Add( new TrackTranslate( stream ) );
						break;
					case eElement.Video:
						video = new Video( stream );
						break;
					case eElement.Audio:
						audio = new Audio( stream );
						break;
					case eElement.TrackOperation:
						trackOperation = new TrackOperation( stream );
						break;
					case eElement.TrickTrackUID:
						trickTrackUID = reader.readUlong();
						break;
					case eElement.TrickTrackSegmentUID:
						trickTrackSegmentUID = reader.readGuid();
						break;
					case eElement.TrickTrackFlag:
						trickTrackFlag = reader.readUlong( 0 );
						break;
					case eElement.TrickMasterTrackUID:
						trickMasterTrackUID = reader.readUlong();
						break;
					case eElement.TrickMasterTrackSegmentUID:
						trickMasterTrackSegmentUID = reader.readGuid();
						break;
					case eElement.ContentEncodings:
						contentEncodings = new ContentEncodings( stream );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( blockAdditionMappinglist != null ) blockAdditionMapping = blockAdditionMappinglist.ToArray();
			if( codecInfoURLlist != null ) codecInfoURL = codecInfoURLlist.ToArray();
			if( codecDownloadURLlist != null ) codecDownloadURL = codecDownloadURLlist.ToArray();
			if( trackOverlaylist != null ) trackOverlay = trackOverlaylist.ToArray();
			if( trackTranslatelist != null ) trackTranslate = trackTranslatelist.ToArray();
		}
	}
}