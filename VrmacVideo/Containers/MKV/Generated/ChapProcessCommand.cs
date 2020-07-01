using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains all the commands associated to the Atom.</summary>
	public sealed partial class ChapProcessCommand
	{
		/// <summary>Defines when the process command SHOULD be handled</summary>
		public readonly eChapProcessTime chapProcessTime;
		/// <summary>Contains the command information. The data SHOULD be interpreted depending on the ChapProcessCodecID value. <a href="https://www.matroska.org/technical/chapters.html#dvd">For ChapProcessCodecID = 1</a>, the data
		/// correspond to the binary DVD cell pre/post commands.</summary>
		public readonly Blob chapProcessData;

		internal ChapProcessCommand( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ChapProcessTime:
						chapProcessTime = (eChapProcessTime)reader.readByte();
						break;
					case eElement.ChapProcessData:
						chapProcessData = Blob.read( reader );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}