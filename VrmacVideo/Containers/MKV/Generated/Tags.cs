using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Element containing metadata describing Tracks, Editions, Chapters, Attachments, or the Segment as a whole. A list of valid tags can be found <a href="https://www.matroska.org/technical/tagging.html">here.</a></summary>
	public sealed partial class Tags
	{
		/// <summary>A single metadata descriptor.</summary>
		public readonly Tag[] tag;

		internal Tags( Stream stream )
		{
			List<Tag> taglist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.Tag:
						if( null == taglist ) taglist = new List<Tag>();
						taglist.Add( new Tag( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( taglist != null ) tag = taglist.ToArray();
		}
	}
}