using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>The Clusters containing the referenced Blocks.</summary>
	public struct CueReference
	{
		/// <summary>Timestamp of the referenced Block.</summary>
		public readonly ulong cueRefTime;
		/// <summary>The Segment Position of the Cluster containing the referenced Block.</summary>
		public readonly ulong cueRefCluster;
		/// <summary>Number of the referenced Block of Track X in the specified Cluster.</summary>
		public readonly ulong cueRefNumber;
		/// <summary>The Segment Position of the Codec State corresponding to this referenced Element. 0 means that the data is taken from the initial Track Entry.</summary>
		public readonly ulong cueRefCodecState;

		internal CueReference( Stream stream )
		{
			cueRefTime = default;
			cueRefCluster = default;
			cueRefNumber = 1;
			cueRefCodecState = 0;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.CueRefTime:
						cueRefTime = reader.readUlong();
						break;
					case eElement.CueRefCluster:
						cueRefCluster = reader.readUlong();
						break;
					case eElement.CueRefNumber:
						cueRefNumber = reader.readUlong( 1 );
						break;
					case eElement.CueRefCodecState:
						cueRefCodecState = reader.readUlong( 0 );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}