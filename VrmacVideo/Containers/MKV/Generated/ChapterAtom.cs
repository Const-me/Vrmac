using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains the atom information to use as the chapter atom (apply to all tracks).</summary>
	public sealed partial class ChapterAtom
	{
		/// <summary>A unique ID to identify the Chapter.</summary>
		public readonly ulong chapterUID;
		/// <summary>A unique string ID to identify the Chapter. Use for <a href="https://w3c.github.io/webvtt/#webvtt-cue-identifier">WebVTT cue identifier storage</a>.</summary>
		public readonly string chapterStringUID;
		/// <summary>Timestamp of the start of Chapter (not scaled).</summary>
		public readonly ulong chapterTimeStart;
		/// <summary>Timestamp of the end of Chapter (timestamp excluded, not scaled).</summary>
		public readonly ulong chapterTimeEnd;
		/// <summary>If a chapter is hidden (1), it SHOULD NOT be available to the user interface (but still to Control Tracks; see <a href="https://www.matroska.org/technical/chapters.html#flags">flag notes</a>). (1 bit)</summary>
		public readonly byte chapterFlagHidden = 0;
		/// <summary>Specify whether the chapter is enabled. It can be enabled/disabled by a Control Track. When disabled, the movie SHOULD skip all the content between the TimeStart and TimeEnd of this chapter (see <a
		/// href="https://www.matroska.org/technical/chapters.html#flags">flag notes</a>). (1 bit)</summary>
		public readonly byte chapterFlagEnabled = 1;
		/// <summary>The SegmentUID of another Segment to play during this chapter.</summary>
		public readonly Guid? chapterSegmentUID;
		/// <summary>The EditionUID to play from the Segment linked in ChapterSegmentUID. If ChapterSegmentEditionUID is undeclared then no Edition of the linked Segment is used.</summary>
		public readonly ulong chapterSegmentEditionUID;
		/// <summary>Specify the physical equivalent of this ChapterAtom like "DVD" (60) or "SIDE" (50), see <a href="https://www.matroska.org/technical/basics.html#physical-types">complete list of values</a>.</summary>
		public readonly ulong chapterPhysicalEquiv;
		/// <summary>List of tracks on which the chapter applies. If this Element is not present, all tracks apply</summary>
		public readonly ChapterTrack chapterTrack;
		/// <summary>Contains all possible strings to use for the chapter display.</summary>
		public readonly ChapterDisplay[] chapterDisplay;
		/// <summary>Contains all the commands associated to the Atom.</summary>
		public readonly ChapProcess[] chapProcess;

		internal ChapterAtom( Stream stream )
		{
			List<ChapterDisplay> chapterDisplaylist = null;
			List<ChapProcess> chapProcesslist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ChapterUID:
						chapterUID = reader.readUlong();
						break;
					case eElement.ChapterStringUID:
						chapterStringUID = reader.readUtf8();
						break;
					case eElement.ChapterTimeStart:
						chapterTimeStart = reader.readUlong();
						break;
					case eElement.ChapterTimeEnd:
						chapterTimeEnd = reader.readUlong();
						break;
					case eElement.ChapterFlagHidden:
						chapterFlagHidden = (byte)reader.readUint( 0 );
						break;
					case eElement.ChapterFlagEnabled:
						chapterFlagEnabled = (byte)reader.readUint( 1 );
						break;
					case eElement.ChapterSegmentUID:
						chapterSegmentUID = reader.readGuid();
						break;
					case eElement.ChapterSegmentEditionUID:
						chapterSegmentEditionUID = reader.readUlong();
						break;
					case eElement.ChapterPhysicalEquiv:
						chapterPhysicalEquiv = reader.readUlong();
						break;
					case eElement.ChapterTrack:
						chapterTrack = new ChapterTrack( stream );
						break;
					case eElement.ChapterDisplay:
						if( null == chapterDisplaylist ) chapterDisplaylist = new List<ChapterDisplay>();
						chapterDisplaylist.Add( new ChapterDisplay( stream ) );
						break;
					case eElement.ChapProcess:
						if( null == chapProcesslist ) chapProcesslist = new List<ChapProcess>();
						chapProcesslist.Add( new ChapProcess( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( chapterDisplaylist != null ) chapterDisplay = chapterDisplaylist.ToArray();
			if( chapProcesslist != null ) chapProcess = chapProcesslist.ToArray();
		}
	}
}