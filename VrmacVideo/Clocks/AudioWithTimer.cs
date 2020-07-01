using System;
using Vrmac;
using VrmacVideo.IO;

namespace VrmacVideo.Clocks
{
	/// <summary>A version of audio-driven presentation clock which uses timerfd kernel object to sleep until it's time to render a frame.</summary>
	/// <remarks>
	/// Saves a small fraction of CPU time this way.
	/// Used on Linux for bare metal rendering when we know for sure the refresh rate of the display, because we initialized that video mode with DRM/KMS when the application started up.
	/// Doesn’t work particularly well when you have other dynamic stuff besides the video, like a mouse pointer: the pointer will render at video rate, e.g. 24 FPS.
	/// </remarks>
	sealed class AudioWithTimer: AudioBase
	{
		readonly TimeSpan frameDuration;

		public AudioWithTimer( StatefulVideoDecoder videoDecoder, iAudioPlayer audioPlayer, Rational refreshRate ) :
			base( videoDecoder, audioPlayer )
		{
			frameDuration = TimeSpan.FromTicks( TimeSpan.TicksPerSecond * refreshRate.denominator / refreshRate.numerator );
			Logger.logVerbose( "Presentation clock: using resource-optimized audio" );
		}

		protected override bool onVideoStreamTick( TimeSpan pendingVideoFrame )
		{
			audioPlayer.marshalException();
			if( isPaused )
				return false;

			long clock = audioClock;
			if( clock < 0 )
				return false;

			if( pendingVideoFrame.Ticks > clock )
			{
				// The first of the pending video frames should be rendered in the future, in relation to the audio clock
				TimeSpan remainingTime = TimeSpan.FromTicks( pendingVideoFrame.Ticks - clock );
				if( remainingTime <= frameDuration )
				{
					// The remaining time to wait is smaller than duration of the frame. Render now, it will take some time to do, and then it'll wait for vsync.
					return true;
				}

				// The remaining time to wait is longer than frame on the display.
				// Sleep for the correct amount of time, and then render.
				LibC.sleep( remainingTime - frameDuration );
				return true;
			}
			// We're too late already, this video frame should have been rendered in the past.
			return true;
		}
	}
}