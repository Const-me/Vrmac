using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains the list of all video plane tracks that need to be combined to create this 3D track</summary>
	public sealed partial class TrackCombinePlanes
	{
		/// <summary>Contains a video plane track that need to be combined to create this 3D track</summary>
		public readonly TrackPlane[] trackPlane;

		internal TrackCombinePlanes( Stream stream )
		{
			List<TrackPlane> trackPlanelist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TrackPlane:
						if( null == trackPlanelist ) trackPlanelist = new List<TrackPlane>();
						trackPlanelist.Add( new TrackPlane( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( trackPlanelist != null ) trackPlane = trackPlanelist.ToArray();
		}
	}
}