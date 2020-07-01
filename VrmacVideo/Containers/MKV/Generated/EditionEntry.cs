using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains all information about a Segment edition.</summary>
	public sealed partial class EditionEntry
	{
		/// <summary>A unique ID to identify the edition. It's useful for tagging an edition.</summary>
		public readonly ulong editionUID;
		/// <summary>If an edition is hidden (1), it SHOULD NOT be available to the user interface (but still to Control Tracks; see <a href="https://www.matroska.org/technical/chapters.html#flags">flag notes</a>). (1 bit)</summary>
		public readonly byte editionFlagHidden = 0;
		/// <summary>If a flag is set (1) the edition SHOULD be used as the default one. (1 bit)</summary>
		public readonly byte editionFlagDefault = 0;
		/// <summary>Specify if the chapters can be defined multiple times and the order to play them is enforced. (1 bit)</summary>
		public readonly byte editionFlagOrdered = 0;
		/// <summary>Contains the atom information to use as the chapter atom (apply to all tracks).</summary>
		public readonly ChapterAtom[] chapterAtom;

		internal EditionEntry( Stream stream )
		{
			List<ChapterAtom> chapterAtomlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.EditionUID:
						editionUID = reader.readUlong();
						break;
					case eElement.EditionFlagHidden:
						editionFlagHidden = (byte)reader.readUint( 0 );
						break;
					case eElement.EditionFlagDefault:
						editionFlagDefault = (byte)reader.readUint( 0 );
						break;
					case eElement.EditionFlagOrdered:
						editionFlagOrdered = (byte)reader.readUint( 0 );
						break;
					case eElement.ChapterAtom:
						if( null == chapterAtomlist ) chapterAtomlist = new List<ChapterAtom>();
						chapterAtomlist.Add( new ChapterAtom( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( chapterAtomlist != null ) chapterAtom = chapterAtomlist.ToArray();
		}
	}
}