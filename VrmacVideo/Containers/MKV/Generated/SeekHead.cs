using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains the Segment Position of other Top-Level Elements.</summary>
	public sealed partial class SeekHead
	{
		/// <summary>Contains a single seek entry to an EBML Element.</summary>
		public readonly Seek[] seek;

		internal SeekHead( Stream stream )
		{
			List<Seek> seeklist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.Seek:
						if( null == seeklist ) seeklist = new List<Seek>();
						seeklist.Add( new Seek( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( seeklist != null ) seek = seeklist.ToArray();
		}
	}
}