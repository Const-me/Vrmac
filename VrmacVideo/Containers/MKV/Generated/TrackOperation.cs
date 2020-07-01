using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Operation that needs to be applied on tracks to create this virtual track. For more details <a href="https://www.matroska.org/technical/notes.html#track-operation">look at the Specification Notes</a> on the subject.</summary>
	public sealed partial class TrackOperation
	{
		/// <summary>Contains the list of all video plane tracks that need to be combined to create this 3D track</summary>
		public readonly TrackCombinePlanes trackCombinePlanes;
		/// <summary>Contains the list of all tracks whose Blocks need to be combined to create this virtual track</summary>
		public readonly TrackJoinBlocks trackJoinBlocks;

		internal TrackOperation( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TrackCombinePlanes:
						trackCombinePlanes = new TrackCombinePlanes( stream );
						break;
					case eElement.TrackJoinBlocks:
						trackJoinBlocks = new TrackJoinBlocks( stream );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}