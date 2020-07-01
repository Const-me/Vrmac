using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains general information about the Segment.</summary>
	public sealed partial class Info
	{
		/// <summary>A randomly generated unique ID to identify the Segment amongst many others (128 bits).</summary>
		public readonly Guid? segmentUID;
		/// <summary>A filename corresponding to this Segment.</summary>
		public readonly string segmentFilename;
		/// <summary>A unique ID to identify the previous Segment of a Linked Segment (128 bits).</summary>
		public readonly Guid? prevUID;
		/// <summary>A filename corresponding to the file of the previous Linked Segment.</summary>
		public readonly string prevFilename;
		/// <summary>A unique ID to identify the next Segment of a Linked Segment (128 bits).</summary>
		public readonly Guid? nextUID;
		/// <summary>A filename corresponding to the file of the next Linked Segment.</summary>
		public readonly string nextFilename;
		/// <summary>A randomly generated unique ID that all Segments of a Linked Segment MUST share (128 bits).</summary>
		public readonly Guid[] segmentFamily;
		/// <summary>A tuple of corresponding ID used by chapter codecs to represent this Segment.</summary>
		public readonly ChapterTranslate[] chapterTranslate;
		/// <summary>Timestamp scale in nanoseconds (1.000.000 means all timestamps in the Segment are expressed in milliseconds).</summary>
		public readonly ulong timestampScale = 1000000;
		/// <summary>Duration of the Segment in nanoseconds based on TimestampScale.</summary>
		public readonly double? duration;
		/// <summary>The date and time that the Segment was created by the muxing application or library.</summary>
		public readonly DateTime dateUTC;
		/// <summary>General name of the Segment.</summary>
		public readonly string title;
		/// <summary>Muxing application or library (example: "libmatroska-0.4.3").</summary>
		public readonly string muxingApp;
		/// <summary>Writing application (example: "mkvmerge-0.3.3").</summary>
		public readonly string writingApp;

		internal Info( Stream stream )
		{
			List<Guid> segmentFamilylist = null;
			List<ChapterTranslate> chapterTranslatelist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.SegmentUID:
						segmentUID = reader.readGuid();
						break;
					case eElement.SegmentFilename:
						segmentFilename = reader.readUtf8();
						break;
					case eElement.PrevUID:
						prevUID = reader.readGuid();
						break;
					case eElement.PrevFilename:
						prevFilename = reader.readUtf8();
						break;
					case eElement.NextUID:
						nextUID = reader.readGuid();
						break;
					case eElement.NextFilename:
						nextFilename = reader.readUtf8();
						break;
					case eElement.SegmentFamily:
						if( null == segmentFamilylist ) segmentFamilylist = new List<Guid>();
						segmentFamilylist.Add( reader.readGuid() );
						break;
					case eElement.ChapterTranslate:
						if( null == chapterTranslatelist ) chapterTranslatelist = new List<ChapterTranslate>();
						chapterTranslatelist.Add( new ChapterTranslate( stream ) );
						break;
					case eElement.TimestampScale:
						timestampScale = reader.readUlong( 1000000 );
						break;
					case eElement.Duration:
						duration = reader.readFloat();
						break;
					case eElement.DateUTC:
						dateUTC = reader.readDate();
						break;
					case eElement.Title:
						title = reader.readUtf8();
						break;
					case eElement.MuxingApp:
						muxingApp = reader.readUtf8();
						break;
					case eElement.WritingApp:
						writingApp = reader.readUtf8();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( segmentFamilylist != null ) segmentFamily = segmentFamilylist.ToArray();
			if( chapterTranslatelist != null ) chapterTranslate = chapterTranslatelist.ToArray();
		}
	}
}