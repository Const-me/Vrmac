using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>A Top-Level Element to speed seeking access. All entries are local to the Segment.</summary>
	public struct Cues
	{
		/// <summary>Contains all information relative to a seek point in the Segment.</summary>
		public readonly CuePoint[] cuePoint;

		internal Cues( Stream stream )
		{
			cuePoint = default;
			List<CuePoint> cuePointlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.CuePoint:
						if( null == cuePointlist ) cuePointlist = new List<CuePoint>();
						cuePointlist.Add( new CuePoint( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( cuePointlist != null ) cuePoint = cuePointlist.ToArray();
		}
	}
}