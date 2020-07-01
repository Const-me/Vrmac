using System.Collections.Generic;
using System.Diagnostics;
using VrmacVideo.Containers.MP4.EditList;

namespace VrmacVideo.Containers.MP4
{
	public sealed class TrackMetadata
	{
		public readonly TrackHeader header;
		public readonly MediaInfo info;
		public readonly iEditList editList;

		internal TrackMetadata( TrackHeader header, MediaInfo info, iEditList editList )
		{
			this.header = header;
			this.info = info;
			this.editList = editList;
		}
	}

	public struct Metadata
	{
		public readonly MovieHeader movieHeader;
		public readonly TrackMetadata[] tracks;

		public Metadata( Mp4Reader reader )
		{
			reader.moveToBox( eBoxType.moov );
			Debug.Assert( 1 == reader.level );

			movieHeader = default;
			List<TrackMetadata> list = new List<TrackMetadata>();
			foreach( eBoxType boxType in reader.readChildren() )
			{
				switch( boxType )
				{
					case eBoxType.mvhd:
						movieHeader = new MovieHeader( reader );
						break;
					case eBoxType.trak:
						list.Add( parseTrack( reader, movieHeader.timescale ) );
						break;
					default:	// e.g. udta, for optional user data.
						reader.skipCurrentBox();
						break;
				}
			}

			tracks = list.ToArray();
		}

		static TrackMetadata parseTrack( Mp4Reader reader, uint timescale )
		{
			TrackHeader header = default;
			MediaInfo info = default;
			EditListBox editList = null;
			foreach( eBoxType boxType in reader.readChildren() )
			{
				switch( boxType )
				{
					case eBoxType.tkhd:
						header = new TrackHeader( reader, timescale );
						break;
					case eBoxType.mdia:
						info = new MediaInfo( reader );
						break;
					case eBoxType.edts:
						editList = EditListBox.load( reader );
						break;
					default:
						reader.skipCurrentBox();
						break;
				}
			}
			iEditList el = Mpeg4EditList.create( editList, timescale, info.timeScale );
			return new TrackMetadata( header, info, el );
		}

		IEnumerable<string> details()
		{
			yield return "--- Movie Header ---";
			yield return movieHeader.ToString();
			yield return "--- Tracks ---";
			foreach( var t in tracks )
				yield return t.ToString();
		}

		public override string ToString() => details().makeLines();
	}
}