using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>A Top-Level Element of information with many tracks described.</summary>
	public sealed partial class Tracks
	{
		/// <summary>Describes a track with all Elements.</summary>
		public readonly TrackEntry[] trackEntry;

		internal Tracks( Stream stream )
		{
			List<TrackEntry> trackEntrylist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TrackEntry:
						if( null == trackEntrylist ) trackEntrylist = new List<TrackEntry>();
						trackEntrylist.Add( new TrackEntry( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( trackEntrylist != null ) trackEntry = trackEntrylist.ToArray();
		}
	}
}