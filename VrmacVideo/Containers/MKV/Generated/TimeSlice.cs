using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains extra time information about the data contained in the Block. Being able to interpret this Element is not REQUIRED for playback.</summary>
	public struct TimeSlice
	{
		/// <summary>The reverse number of the frame in the lace (0 is the last frame, 1 is the next to last, etc). Being able to interpret this Element is not REQUIRED for playback.</summary>
		public readonly ulong laceNumber;
		/// <summary>The number of the frame to generate from this lace with this delay (allow you to generate many frames from the same Block/Frame).</summary>
		public readonly ulong frameNumber;
		/// <summary>The ID of the BlockAdditional Element (0 is the main Block).</summary>
		public readonly ulong blockAdditionID;
		/// <summary>The (scaled) delay to apply to the Element.</summary>
		public readonly ulong delay;
		/// <summary>The (scaled) duration to apply to the Element.</summary>
		public readonly ulong sliceDuration;

		internal TimeSlice( Stream stream )
		{
			laceNumber = 0;
			frameNumber = 0;
			blockAdditionID = 0;
			delay = 0;
			sliceDuration = 0;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.LaceNumber:
						laceNumber = reader.readUlong( 0 );
						break;
					case eElement.FrameNumber:
						frameNumber = reader.readUlong( 0 );
						break;
					case eElement.BlockAdditionID:
						blockAdditionID = reader.readUlong( 0 );
						break;
					case eElement.Delay:
						delay = reader.readUlong( 0 );
						break;
					case eElement.SliceDuration:
						sliceDuration = reader.readUlong( 0 );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}