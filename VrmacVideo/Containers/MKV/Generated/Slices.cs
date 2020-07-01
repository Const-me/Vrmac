using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains slices description.</summary>
	public struct Slices
	{
		/// <summary>Contains extra time information about the data contained in the Block. Being able to interpret this Element is not REQUIRED for playback.</summary>
		public readonly TimeSlice[] timeSlice;

		internal Slices( Stream stream )
		{
			timeSlice = default;
			List<TimeSlice> timeSlicelist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TimeSlice:
						if( null == timeSlicelist ) timeSlicelist = new List<TimeSlice>();
						timeSlicelist.Add( new TimeSlice( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( timeSlicelist != null ) timeSlice = timeSlicelist.ToArray();
		}
	}
}