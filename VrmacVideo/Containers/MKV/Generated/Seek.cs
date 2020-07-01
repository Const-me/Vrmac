using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains a single seek entry to an EBML Element.</summary>
	public sealed partial class Seek
	{
		/// <summary>The binary ID corresponding to the Element name.</summary>
		public readonly eElement seekID;
		/// <summary>The Segment Position of the Element.</summary>
		public readonly ulong seekPosition;

		internal Seek( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.SeekID:
						seekID = (eElement)reader.readUint();
						break;
					case eElement.SeekPosition:
						seekPosition = reader.readUlong();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}