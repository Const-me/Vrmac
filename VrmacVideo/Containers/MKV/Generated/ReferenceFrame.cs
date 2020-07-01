using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
	public sealed partial class ReferenceFrame
	{
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly ulong referenceOffset;
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly ulong referenceTimestamp;

		internal ReferenceFrame( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ReferenceOffset:
						referenceOffset = reader.readUlong();
						break;
					case eElement.ReferenceTimestamp:
						referenceTimestamp = reader.readUlong();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}