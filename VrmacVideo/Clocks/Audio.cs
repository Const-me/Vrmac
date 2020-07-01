using System;

namespace VrmacVideo.Clocks
{
	/// <summary>Audio-driven presentation clock which behaves just like IMFMediaEngine on Windows.
	/// It returns false from onVideoStreamTick method when it’s too early to deliver the next frame of the video.</summary>
	/// <remarks>Used on Linux for windowed-mode rendering when we don’t know the refresh rate of the display.</remarks>
	sealed class Audio: AudioBase
	{
		public Audio( StatefulVideoDecoder videoDecoder, iAudioPlayer audioPlayer ) :
			base( videoDecoder, audioPlayer )
		{
			Logger.logVerbose( "Presentation clock: using latency-optimized audio" );
		}

		protected override bool onVideoStreamTick( TimeSpan pendingVideoFrame )
		{
			audioPlayer.marshalException();
			if( isPaused )
				return false;

			long clock = audioClock;

			if( pendingVideoFrame.Ticks > clock )
			{
				// The first of the pending video frames should be rendered in the future, in relation to the audio clock
				// Logger.logVerbose( "AudioPresentationClock.onVideoStreamTick: skipping, delta {0}, audio clock {1}, video clock {2}", msBetween( clock, pendingVideoFrame ), TimeSpan.FromTicks( clock ), pendingVideoFrame );
				return false;
			}
			// Logger.logVerbose( "AudioPresentationClock.onVideoStreamTick: rendering, delta {0}, audio clock {1}, video clock {2}", msBetween( clock, pendingVideoFrame ), TimeSpan.FromTicks( clock ), pendingVideoFrame );
			return true;
		}
	}
}