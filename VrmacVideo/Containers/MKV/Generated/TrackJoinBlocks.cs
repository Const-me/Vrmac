using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains the list of all tracks whose Blocks need to be combined to create this virtual track</summary>
	public sealed partial class TrackJoinBlocks
	{
		/// <summary>The trackUID number of a track whose blocks are used to create this virtual track.</summary>
		public readonly ulong[] trackJoinUID;

		internal TrackJoinBlocks( Stream stream )
		{
			List<ulong> trackJoinUIDlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TrackJoinUID:
						if( null == trackJoinUIDlist ) trackJoinUIDlist = new List<ulong>();
						trackJoinUIDlist.Add( reader.readUlong() );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( trackJoinUIDlist != null ) trackJoinUID = trackJoinUIDlist.ToArray();
		}
	}
}