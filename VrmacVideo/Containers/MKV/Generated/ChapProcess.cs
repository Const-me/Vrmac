using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains all the commands associated to the Atom.</summary>
	public sealed partial class ChapProcess
	{
		/// <summary>Contains the type of the codec used for the processing. A value of 0 means native Matroska processing (to be defined), a value of 1 means the <a href="https://www.matroska.org/technical/chapters.html#dvd">DVD</a>
		/// command set is used. More codec IDs can be added later.</summary>
		public readonly ulong chapProcessCodecID = 0;
		/// <summary>Some optional data attached to the ChapProcessCodecID information. <a href="https://www.matroska.org/technical/chapters.html#dvd">For ChapProcessCodecID = 1</a>, it is the "DVD level" equivalent.</summary>
		public readonly Blob chapProcessPrivate;
		/// <summary>Contains all the commands associated to the Atom.</summary>
		public readonly ChapProcessCommand[] chapProcessCommand;

		internal ChapProcess( Stream stream )
		{
			List<ChapProcessCommand> chapProcessCommandlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ChapProcessCodecID:
						chapProcessCodecID = reader.readUlong( 0 );
						break;
					case eElement.ChapProcessPrivate:
						chapProcessPrivate = Blob.read( reader );
						break;
					case eElement.ChapProcessCommand:
						if( null == chapProcessCommandlist ) chapProcessCommandlist = new List<ChapProcessCommand>();
						chapProcessCommandlist.Add( new ChapProcessCommand( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( chapProcessCommandlist != null ) chapProcessCommand = chapProcessCommandlist.ToArray();
		}
	}
}