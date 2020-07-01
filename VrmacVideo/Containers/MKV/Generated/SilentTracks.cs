using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking or to decide what track to use.</summary>
	public sealed partial class SilentTracks
	{
		/// <summary>One of the track number that are not used from now on in the stream. It could change later if not specified as silent in a further Cluster.</summary>
		public readonly ulong[] silentTrackNumber;

		internal SilentTracks( Stream stream )
		{
			List<ulong> silentTrackNumberlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.SilentTrackNumber:
						if( null == silentTrackNumberlist ) silentTrackNumberlist = new List<ulong>();
						silentTrackNumberlist.Add( reader.readUlong() );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( silentTrackNumberlist != null ) silentTrackNumber = silentTrackNumberlist.ToArray();
		}
	}
}