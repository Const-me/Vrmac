using System;
using System.Collections.Generic;

namespace VrmacVideo.Containers.MKV
{
	struct SeekPoint
	{
		public readonly int cluster;
		/// <summary>The relative position inside the Cluster of the referenced SimpleBlock or BlockGroup with 0 being the first possible position for an Element inside that Cluster.</summary>
		public readonly uint relativePosition;
		/// <summary>The duration of the block according to the Segment time base. If missing the track's DefaultDuration does not apply and no duration information is available in terms of the cues.</summary>
		public readonly ulong duration;
		/// <summary>Number of the Block in the specified Cluster.</summary>
		public readonly ulong blockNumber;
		/// <summary>The Segment Position of the Codec State corresponding to this Cue Element. 0 means that the data is taken from the initial Track Entry.</summary>
		public readonly ulong codecState;
		/// <summary>The Clusters containing the referenced Blocks.</summary>
		public readonly CueReference[] reference;

		public SeekPoint( int cluster, CueTrackPositions ctp )
		{
			this.cluster = cluster;
			relativePosition = checked((uint)ctp.cueRelativePosition);
			duration = ctp.cueDuration;
			blockNumber = ctp.cueBlockNumber;
			codecState = ctp.cueCodecState;
			reference = ctp.cueReference;
		}

		public override string ToString() => $"Cluster { cluster }, relativePosition { relativePosition }, duration { duration }, blockNumber { blockNumber }";
	}

	static class SeekIndex
	{
		public static IEnumerable<SeekPoint> convertSeekIndex( IEnumerable<CueTrackPositions> positions, ClusterPlaceholder[] clusters, ulong track )
		{
			Dictionary<long, int> clusterIndices = new Dictionary<long, int>();
			for( int i = 0; i < clusters.Length; i++ )
			{
				long clusterPos = clusters[ i ].segmentPosition;
				clusterIndices[ clusterPos ] = i;
			}

			foreach( var c in positions )
			{
				if( c.cueTrack != track )
					continue;

				long key = (long)c.cueClusterPosition;
				if( clusterIndices.TryGetValue( key, out int idx ) )
				{
					yield return new SeekPoint( idx, c );
					continue;
				}

				throw new ApplicationException( "MKV cluster wasn't found" );
			}
		}
	}
}