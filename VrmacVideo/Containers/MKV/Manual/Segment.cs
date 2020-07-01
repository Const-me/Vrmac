using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VrmacVideo.Containers.MKV
{
	public sealed partial class Segment
	{
		/// <summary>Parse an MKV file</summary>
		public static Segment read( Stream stream )
		{
			var elt = stream.readElementId();
			if( elt != eElement.EBML )
				throw new ArgumentException( "The input stream ain’t EBML" );
			EBML root = new EBML( stream );
			if( root.docType != "matroska" )
				throw new ArgumentException( "The input stream ain’t an MKV" );

			elt = stream.readElementId();
			if( elt != eElement.Segment )
				throw new ArgumentException();
			return new Segment( stream );
		}

		public readonly long position;
		public eVideoCodec videoCodec { get; private set; }

		public TrackEntry findVideoTrack()
		{
			if( null == this.tracks )
				throw new ArgumentException( "No tracks found in that file" );

			TrackEntry[] tracks = this.tracks.SelectMany( t => t.trackEntry )
				.Where( e => e.trackType == eTrackType.Video )
				.ToArray();

			if( tracks.Length < 1 )
				throw new ArgumentException( "No video tracks found in that file" );

			TrackEntry videoTrack;
			videoTrack = tracks[ 0 ];
			if( tracks.Length > 1 )
				Logger.logWarning( "Multiple video tracks, using the first one" );

			switch( videoTrack.codecID )
			{
				case "V_MPEG4/ISO/AVC":
					videoCodec = eVideoCodec.h264;
					break;
				case "V_MPEGH/ISO/HEVC":
					videoCodec = eVideoCodec.h265;
					throw new NotSupportedException( $"h265 video codec is not supported" );
					// break;
				default:
					throw new ArgumentException( $"The video codec, \"{ videoTrack.codecID }\", is not supported by the library." );
			}

			if( null == videoTrack.codecPrivate )
				throw new NotSupportedException( $"The MKV is lacking private data of the h264 codec" );

			return videoTrack;
		}

		public TrackEntry findAudioTrack()
		{
			TrackEntry[] tracks = this.tracks.SelectMany( t => t.trackEntry )
				.Where( e => e.trackType == eTrackType.Audio )
				.ToArray();
			if( tracks.Length < 1 )
				throw new ArgumentException( "No audio tracks found in that file" );

			TrackEntry track;
			track = tracks[ 0 ];
			if( tracks.Length > 1 )
				Logger.logWarning( "Multiple audio tracks, using the first one" );

			return track;
		}

		internal SeekPoint[] buildSeekIndex( ulong trackId )
		{
			if( null == cues.cuePoint )
				return null;
			List<SeekPoint> list = new List<SeekPoint>( cues.cuePoint.Length );
			var result = SeekIndex.convertSeekIndex( cues.cuePoint.SelectMany( cp => cp.cueTrackPositions ), cluster, trackId );
			list.AddRange( result );
			return list.ToArray();
		}
	}
}