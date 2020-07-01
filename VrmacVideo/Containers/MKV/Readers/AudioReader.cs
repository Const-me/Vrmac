using System;
using System.Threading;
using VrmacVideo.Audio;

namespace VrmacVideo.Containers.MKV
{
	sealed class AudioReader: ReaderBase, iAudioTrackReader
	{
		bool iAudioTrackReader.read( iDecoderQueues queues )
		{
			if( EOF )
			{
				Thread.Sleep( 1 );
				return false;
			}
			int cb = getFrameSize();
			var span = queues.dequeueEmpty( out int idx, cb );
			lock( clusters.syncRoot )
				readCurrentFrame( span );
			queues.enqueueEncoded( idx, cb, timestamp );
			advance();
			return true;
		}

		StreamPosition iTrackReader.findStreamPosition( TimeSpan ts ) => findPosition( ts );

		// In DTS they are all key frames. AFAIK audio codecs don't generally have that stuff.
		StreamPosition iTrackReader.findKeyFrame( StreamPosition seekFrame ) => seekFrame;

		void iTrackReader.seekToSample( StreamPosition index ) => seekMedia( index );

		public AudioReader( MkvMediaFile file, TrackEntry track ) :
			base( file, track )
		{ }
	}
}