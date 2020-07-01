using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>List of tracks on which the chapter applies. If this Element is not present, all tracks apply</summary>
	public sealed partial class ChapterTrack
	{
		/// <summary>UID of the Track to apply this chapter to. In the absence of a control track, choosing this chapter will select the listed Tracks and deselect unlisted tracks. Absence of this Element indicates that the Chapter SHOULD
		/// be applied to any currently used Tracks.</summary>
		public readonly ulong[] chapterTrackUID;

		internal ChapterTrack( Stream stream )
		{
			List<ulong> chapterTrackUIDlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ChapterTrackUID:
						if( null == chapterTrackUIDlist ) chapterTrackUIDlist = new List<ulong>();
						chapterTrackUIDlist.Add( reader.readUlong() );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( chapterTrackUIDlist != null ) chapterTrackUID = chapterTrackUIDlist.ToArray();
		}
	}
}