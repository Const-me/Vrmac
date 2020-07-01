using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>The Root Element that contains all other Top-Level Elements (Elements defined only at Level 1). A Matroska file is composed of 1 Segment.</summary>
	public sealed partial class Segment
	{
		/// <summary>Contains the Segment Position of other Top-Level Elements.</summary>
		public readonly SeekHead[] seekHead;
		/// <summary>Contains general information about the Segment.</summary>
		public readonly Info[] info;
		/// <summary>The Top-Level Element containing the (monolithic) Block structure.</summary>
		public readonly ClusterPlaceholder[] cluster;
		/// <summary>A Top-Level Element of information with many tracks described.</summary>
		public readonly Tracks[] tracks;
		/// <summary>A Top-Level Element to speed seeking access. All entries are local to the Segment.</summary>
		public readonly Cues cues;
		/// <summary>Contain attached files.</summary>
		public readonly Attachments attachments;
		/// <summary>A system to define basic menus and partition data. For more detailed information, look at the <a href="https://www.matroska.org/technical/chapters.html">Chapters Explanation</a>.</summary>
		public readonly Chapters chapters;
		/// <summary>Element containing metadata describing Tracks, Editions, Chapters, Attachments, or the Segment as a whole. A list of valid tags can be found <a href="https://www.matroska.org/technical/tagging.html">here.</a></summary>
		public readonly Tags[] tags;

		internal Segment( Stream stream )
		{
			List<SeekHead> seekHeadlist = null;
			List<Info> infolist = null;
			List<ClusterPlaceholder> clusterlist = null;
			List<Tracks> trackslist = null;
			List<Tags> tagslist = null;
			ElementReader reader = new ElementReader( stream );
			position = stream.Position;

			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.SeekHead:
						if( null == seekHeadlist ) seekHeadlist = new List<SeekHead>();
						seekHeadlist.Add( new SeekHead( stream ) );
						break;
					case eElement.Info:
						if( null == infolist ) infolist = new List<Info>();
						infolist.Add( new Info( stream ) );
						break;
					case eElement.Cluster:
						if( null == clusterlist ) clusterlist = new List<ClusterPlaceholder>();
						clusterlist.Add( new ClusterPlaceholder( stream, position ) );
						break;
					case eElement.Tracks:
						if( null == trackslist ) trackslist = new List<Tracks>();
						trackslist.Add( new Tracks( stream ) );
						break;
					case eElement.Cues:
						cues = new Cues( stream );
						break;
					case eElement.Attachments:
						attachments = new Attachments( stream );
						break;
					case eElement.Chapters:
						chapters = new Chapters( stream );
						break;
					case eElement.Tags:
						if( null == tagslist ) tagslist = new List<Tags>();
						tagslist.Add( new Tags( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( seekHeadlist != null ) seekHead = seekHeadlist.ToArray();
			if( infolist != null ) info = infolist.ToArray();
			if( clusterlist != null ) cluster = clusterlist.ToArray();
			if( trackslist != null ) tracks = trackslist.ToArray();
			if( tagslist != null ) tags = tagslist.ToArray();
		}
	}
}