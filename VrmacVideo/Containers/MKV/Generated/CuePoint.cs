using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains all information relative to a seek point in the Segment.</summary>
	public struct CuePoint
	{
		/// <summary>Absolute timestamp according to the Segment time base.</summary>
		public readonly ulong cueTime;
		/// <summary>Contain positions for different tracks corresponding to the timestamp.</summary>
		public readonly CueTrackPositions[] cueTrackPositions;

		internal CuePoint( Stream stream )
		{
			cueTime = default;
			cueTrackPositions = default;
			List<CueTrackPositions> cueTrackPositionslist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.CueTime:
						cueTime = reader.readUlong();
						break;
					case eElement.CueTrackPositions:
						if( null == cueTrackPositionslist ) cueTrackPositionslist = new List<CueTrackPositions>();
						cueTrackPositionslist.Add( new CueTrackPositions( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( cueTrackPositionslist != null ) cueTrackPositions = cueTrackPositionslist.ToArray();
		}
	}
}