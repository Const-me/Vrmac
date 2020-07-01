using System;
using VrmacVideo.Audio;

namespace VrmacVideo.Containers.MKV
{
	sealed class AudioTrack: iAudioTrack
	{
		readonly MkvMediaFile file;
		public readonly TrackEntry track;
		readonly TrackInfo info;

		public AudioTrack( MkvMediaFile file, TrackEntry track )
		{
			this.file = file;
			this.track = track;
			info = new TrackInfo( track );
		}

		TrackInfo iAudioTrack.info => info;

		iAudioTrackReader iAudioTrack.createReader() => new AudioReader( file, track );
	}
}