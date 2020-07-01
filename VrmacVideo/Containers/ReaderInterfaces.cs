using System;
using VrmacVideo.Audio;
using VrmacVideo.IO;

namespace VrmacVideo
{
	abstract class StreamPosition
	{
		public readonly TimeSpan time;

		public StreamPosition( TimeSpan time )
		{
			this.time = time;
		}
	}

	interface iTrackReader
	{
		StreamPosition findStreamPosition( TimeSpan ts );
		StreamPosition findKeyFrame( StreamPosition seekFrame );
		void seekToSample( StreamPosition where );
	}

	interface iVideoTrackReader: iTrackReader
	{
		eNaluAction writeNextNalu( EncodedBuffer dest );
	}

	interface iAudioTrackReader: iTrackReader
	{
		bool read( iDecoderQueues queues );
	}
}