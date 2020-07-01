using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>A system to define basic menus and partition data. For more detailed information, look at the <a href="https://www.matroska.org/technical/chapters.html">Chapters Explanation</a>.</summary>
	public sealed partial class Chapters
	{
		/// <summary>Contains all information about a Segment edition.</summary>
		public readonly EditionEntry[] editionEntry;

		internal Chapters( Stream stream )
		{
			List<EditionEntry> editionEntrylist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.EditionEntry:
						if( null == editionEntrylist ) editionEntrylist = new List<EditionEntry>();
						editionEntrylist.Add( new EditionEntry( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( editionEntrylist != null ) editionEntry = editionEntrylist.ToArray();
		}
	}
}