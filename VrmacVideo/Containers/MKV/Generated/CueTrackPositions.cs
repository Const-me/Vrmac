using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contain positions for different tracks corresponding to the timestamp.</summary>
	public struct CueTrackPositions
	{
		/// <summary>The track for which a position is given.</summary>
		public readonly ulong cueTrack;
		/// <summary>The Segment Position of the Cluster containing the associated Block.</summary>
		public readonly ulong cueClusterPosition;
		/// <summary>The relative position inside the Cluster of the referenced SimpleBlock or BlockGroup with 0 being the first possible position for an Element inside that Cluster.</summary>
		public readonly ulong cueRelativePosition;
		/// <summary>The duration of the block according to the Segment time base. If missing the track's DefaultDuration does not apply and no duration information is available in terms of the cues.</summary>
		public readonly ulong cueDuration;
		/// <summary>Number of the Block in the specified Cluster.</summary>
		public readonly ulong cueBlockNumber;
		/// <summary>The Segment Position of the Codec State corresponding to this Cue Element. 0 means that the data is taken from the initial Track Entry.</summary>
		public readonly ulong cueCodecState;
		/// <summary>The Clusters containing the referenced Blocks.</summary>
		public readonly CueReference[] cueReference;

		internal CueTrackPositions( Stream stream )
		{
			cueTrack = default;
			cueClusterPosition = default;
			cueRelativePosition = default;
			cueDuration = default;
			cueBlockNumber = 1;
			cueCodecState = 0;
			cueReference = default;
			List<CueReference> cueReferencelist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.CueTrack:
						cueTrack = reader.readUlong();
						break;
					case eElement.CueClusterPosition:
						cueClusterPosition = reader.readUlong();
						break;
					case eElement.CueRelativePosition:
						cueRelativePosition = reader.readUlong();
						break;
					case eElement.CueDuration:
						cueDuration = reader.readUlong();
						break;
					case eElement.CueBlockNumber:
						cueBlockNumber = reader.readUlong( 1 );
						break;
					case eElement.CueCodecState:
						cueCodecState = reader.readUlong( 0 );
						break;
					case eElement.CueReference:
						if( null == cueReferencelist ) cueReferencelist = new List<CueReference>();
						cueReferencelist.Add( new CueReference( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( cueReferencelist != null ) cueReference = cueReferencelist.ToArray();
		}
	}
}