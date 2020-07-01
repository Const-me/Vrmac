using System;
using System.IO;
using System.Threading;
using VrmacVideo.Containers.MP4;

namespace VrmacVideo.Audio
{
	sealed class Reader: iAudioTrackReader
	{
		readonly Stream stream;
		readonly SampleReader sampleReader;

		public Reader( Mp4File mp4 )
		{
			stream = mp4.reader.stream;
			sampleReader = new SampleReader( mp4.audioTrack );
		}

		bool iAudioTrackReader.read( iDecoderQueues queues )
		{
			if( sampleReader.EOF )
			{
				Thread.Sleep( 1 );
				return false;
			}

			int cbSample = sampleReader.seek( stream );
			Span<byte> span = queues.dequeueEmpty( out int idx, cbSample );
			stream.read( span );
			queues.enqueueEncoded( idx, cbSample, sampleReader.timestamp );
			sampleReader.advance();
			return true;
		}

		StreamPosition iTrackReader.findStreamPosition( TimeSpan ts ) => sampleReader.findStreamPosition( ts );
		StreamPosition iTrackReader.findKeyFrame( StreamPosition seekFrame ) => sampleReader.findKeyFrame( (Mp4StreamPosition)seekFrame );
		void iTrackReader.seekToSample( StreamPosition index ) => sampleReader.seekToSample( ( (Mp4StreamPosition)index ).sample );
	}
}