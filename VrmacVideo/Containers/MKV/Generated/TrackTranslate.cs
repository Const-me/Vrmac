using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>The track identification for the given Chapter Codec.</summary>
	public sealed partial class TrackTranslate
	{
		/// <summary>Specify an edition UID on which this translation applies. When not specified, it means for all editions found in the Segment.</summary>
		public readonly ulong[] trackTranslateEditionUID;
		/// <summary>The <a href="https://www.matroska.org/technical/elements.html#ChapProcessCodecID">chapter codec</a>.</summary>
		public readonly eTrackTranslateCodec trackTranslateCodec;
		/// <summary>The binary value used to represent this track in the chapter codec data. The format depends on the <a href="https://www.matroska.org/technical/elements.html#ChapProcessCodecID">ChapProcessCodecID</a> used.</summary>
		public readonly Blob trackTranslateTrackID;

		internal TrackTranslate( Stream stream )
		{
			List<ulong> trackTranslateEditionUIDlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TrackTranslateEditionUID:
						if( null == trackTranslateEditionUIDlist ) trackTranslateEditionUIDlist = new List<ulong>();
						trackTranslateEditionUIDlist.Add( reader.readUlong() );
						break;
					case eElement.TrackTranslateCodec:
						trackTranslateCodec = (eTrackTranslateCodec)reader.readByte();
						break;
					case eElement.TrackTranslateTrackID:
						trackTranslateTrackID = Blob.read( reader );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( trackTranslateEditionUIDlist != null ) trackTranslateEditionUID = trackTranslateEditionUIDlist.ToArray();
		}
	}
}