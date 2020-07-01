using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>A tuple of corresponding ID used by chapter codecs to represent this Segment.</summary>
	public sealed partial class ChapterTranslate
	{
		/// <summary>Specify an edition UID on which this correspondence applies. When not specified, it means for all editions found in the Segment.</summary>
		public readonly ulong[] chapterTranslateEditionUID;
		/// <summary>The <a href="https://www.matroska.org/technical/elements.html#ChapProcessCodecID">chapter codec</a></summary>
		public readonly eChapterTranslateCodec chapterTranslateCodec;
		/// <summary>The binary value used to represent this Segment in the chapter codec data. The format depends on the <a href="https://www.matroska.org/technical/chapters.html#ChapProcessCodecID">ChapProcessCodecID</a> used.</summary>
		public readonly byte[] chapterTranslateID;

		internal ChapterTranslate( Stream stream )
		{
			List<ulong> chapterTranslateEditionUIDlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ChapterTranslateEditionUID:
						if( null == chapterTranslateEditionUIDlist ) chapterTranslateEditionUIDlist = new List<ulong>();
						chapterTranslateEditionUIDlist.Add( reader.readUlong() );
						break;
					case eElement.ChapterTranslateCodec:
						chapterTranslateCodec = (eChapterTranslateCodec)reader.readByte();
						break;
					case eElement.ChapterTranslateID:
						chapterTranslateID = reader.readByteArray();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( chapterTranslateEditionUIDlist != null ) chapterTranslateEditionUID = chapterTranslateEditionUIDlist.ToArray();
		}
	}
}