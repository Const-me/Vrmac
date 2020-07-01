using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Specifies which other elements the metadata represented by the Tag applies to. If empty or not present, then the Tag describes everything in the Segment.</summary>
	public sealed partial class Targets
	{
		/// <summary>A number to indicate the logical level of the target.</summary>
		public readonly eTargetTypeValue targetTypeValue = eTargetTypeValue.Album;
		/// <summary>An informational string that can be used to display the logical level of the target like "ALBUM", "TRACK", "MOVIE", "CHAPTER", etc (see <a
		/// href="https://www.matroska.org/technical/tagging.html#targettypes">TargetType</a>).</summary>
		public readonly eTargetType? targetType;
		/// <summary>A unique ID to identify the Track(s) the tags belong to. If the value is 0 at this level, the tags apply to all tracks in the Segment.</summary>
		public readonly ulong[] tagTrackUID;
		/// <summary>A unique ID to identify the EditionEntry(s) the tags belong to. If the value is 0 at this level, the tags apply to all editions in the Segment.</summary>
		public readonly ulong[] tagEditionUID;
		/// <summary>A unique ID to identify the Chapter(s) the tags belong to. If the value is 0 at this level, the tags apply to all chapters in the Segment.</summary>
		public readonly ulong[] tagChapterUID;
		/// <summary>A unique ID to identify the Attachment(s) the tags belong to. If the value is 0 at this level, the tags apply to all the attachments in the Segment.</summary>
		public readonly ulong[] tagAttachmentUID;

		internal Targets( Stream stream )
		{
			List<ulong> tagTrackUIDlist = null;
			List<ulong> tagEditionUIDlist = null;
			List<ulong> tagChapterUIDlist = null;
			List<ulong> tagAttachmentUIDlist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.TargetTypeValue:
						targetTypeValue = (eTargetTypeValue)reader.readByte( 50 );
						break;
					case eElement.TargetType:
						targetType = Enum.Parse<eTargetType>( reader.readAscii() );
						break;
					case eElement.TagTrackUID:
						if( null == tagTrackUIDlist ) tagTrackUIDlist = new List<ulong>();
						tagTrackUIDlist.Add( reader.readUlong( 0 ) );
						break;
					case eElement.TagEditionUID:
						if( null == tagEditionUIDlist ) tagEditionUIDlist = new List<ulong>();
						tagEditionUIDlist.Add( reader.readUlong( 0 ) );
						break;
					case eElement.TagChapterUID:
						if( null == tagChapterUIDlist ) tagChapterUIDlist = new List<ulong>();
						tagChapterUIDlist.Add( reader.readUlong( 0 ) );
						break;
					case eElement.TagAttachmentUID:
						if( null == tagAttachmentUIDlist ) tagAttachmentUIDlist = new List<ulong>();
						tagAttachmentUIDlist.Add( reader.readUlong( 0 ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( tagTrackUIDlist != null ) tagTrackUID = tagTrackUIDlist.ToArray();
			if( tagEditionUIDlist != null ) tagEditionUID = tagEditionUIDlist.ToArray();
			if( tagChapterUIDlist != null ) tagChapterUID = tagChapterUIDlist.ToArray();
			if( tagAttachmentUIDlist != null ) tagAttachmentUID = tagAttachmentUIDlist.ToArray();
		}
	}
}