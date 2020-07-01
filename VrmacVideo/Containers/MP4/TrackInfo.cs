using System;
using VrmacVideo.Containers.MP4;

namespace VrmacVideo.Audio
{
	partial struct TrackInfo
	{
		internal TrackInfo( Mp4File mp4 )
		{
			var sam = mp4.audioTrackSample;
			MediaInfo in4 = mp4.audioTrack.info;
			maxBytesInFrame = in4.mediaInformation.sampleTable.maxBytesInFrame;
			sampleRate = (int)( sam.sampleRateInt >> 16 );
			bitsPerSample = sam.bitsPerSample;
			channelsCount = sam.channelCount;

			if( sam is MP4AudioSampleEntry )
			{
				audioCodec = eAudioCodec.AAC;
				uint sampleDelta = in4.mediaInformation.sampleTable.timeToSample[ 0 ].sampleDelta;
				uint timeScale = in4.timeScale;
				long samples = ( (long)sampleDelta * sampleRate ) / timeScale;
				samplesPerFrame = checked((int)samples);
				if( 16 != bitsPerSample )
					throw new NotSupportedException( "Currently, the library only supports 16-bit samples" );

				decoderConfigBlob = mp4.audioTrackSample.audioSpecificConfig;
				strippedHeader = null;
				return;
			}
			if( sam is Ac3AudioSampleEntry )
			{
				audioCodec = eAudioCodec.AC3;
				decoderConfigBlob = null;
				strippedHeader = null;
				if( maxBytesInFrame > IO.Dolby.liba52.maxEncodedBuffer )
					throw new ArgumentException( $"Sample entry specified { maxBytesInFrame } bytes / frame, this exceeds the codec limit of { IO.Dolby.liba52.maxEncodedBuffer }" );
				samplesPerFrame = IO.Dolby.liba52.samplesPerFrame;
				return;
			}

			throw new ApplicationException( "Unrecognized audio codec" );
		}
	}
}