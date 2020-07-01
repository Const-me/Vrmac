using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains a video plane track that need to be combined to create this 3D track</summary>
	public sealed partial class TrackPlane
	{
		/// <summary>The trackUID number of the track representing the plane.</summary>
		public readonly ulong trackPlaneUID;
		/// <summary>The kind of plane this track corresponds to.</summary>
		public readonly eTrackPlaneType trackPlaneType;

		internal TrackPlane( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TrackPlaneUID:
						trackPlaneUID = reader.readUlong();
						break;
					case eElement.TrackPlaneType:
						trackPlaneType = (eTrackPlaneType)reader.readByte();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}