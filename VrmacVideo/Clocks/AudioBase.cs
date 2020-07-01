using System;
using System.Threading;

namespace VrmacVideo.Clocks
{
	/// <summary>Base class for presentation clock which uses presentation time from the audio stream being played.</summary>
	/// <remarks>It’s very hard to speed up or slow down audio without introducing artifacts. Need FFT which would use non-trivial amount of CPU time, also latency.
	/// OTOH, it’s very simple to delay video frames.</remarks>
	abstract class AudioBase: PresentationClock, iAudioPresentationClock
	{
		static readonly TimeSpan videoSeekTimeout = TimeSpan.FromSeconds( 15 );

		public AudioBase( StatefulVideoDecoder videoDecoder, iAudioPlayer audioPlayer ) :
			base( videoDecoder )
		{
			this.audioPlayer = audioPlayer;
			audioPlayer.setPresentationClock( this );
		}

		protected readonly iAudioPlayer audioPlayer;

		long m_audioClock = -1;

		void iAudioPresentationClock.updateTimestamp( TimeSpan timestamp )
		{
			Interlocked.Exchange( ref m_audioClock, timestamp.Ticks );
		}

		public override TimeSpan getCurrentTime()
		{
			long clock = Interlocked.Read( ref m_audioClock );
			return TimeSpan.FromTicks( clock );
		}

		protected long audioClock => Interlocked.Read( ref m_audioClock );

		bool m_paused = false;
		public override bool isPaused => m_paused;

		public override void pause()
		{
			m_paused = true;
			audioPlayer.pause();
		}
		public override void resume()
		{
			m_paused = false;
			audioPlayer.play();
		}

		void iAudioPresentationClock.drainComplete()
		{
			lock( syncRoot )
			{
				drainComplete = true;
				Monitor.Pulse( syncRoot );
			}
		}

		readonly ManualResetEventSlim videoReady = new ManualResetEventSlim( false );

		bool iAudioPresentationClock.waitForVideoFrame()
		{
			if( !videoReady.Wait( videoSeekTimeout ) )
				throw new TimeoutException( "Video seek took too long to decode from previous keyframe to the destination; the audio thread gave up waiting." );
			Logger.logVerbose( "iAudioPresentationClock.waitForVideoFrame: the video stream is ready" );
			return true;
		}

		readonly object syncRoot = new object();
		bool drainComplete = false;
		TimeSpan? seekDest;

		public override void seek( TimeSpan where )
		{
			m_paused = true;
			var pos = videoDecoder.thread.findStreamsPosition( where );

			videoReady.Reset();
			lock( syncRoot )
			{
				drainComplete = false;
				seekDest = pos.audio.time;
			}
			audioPlayer.seek( pos.audio.time );
			videoDecoder.thread.seek( ref pos );
		}

		public override void waitForAudioDrain()
		{
			lock( syncRoot )
			{
				while( true )
				{
					if( drainComplete )
					{
						m_audioClock = -1;
						return;
					}
					if( !Monitor.Wait( syncRoot, 250 ) )
						throw new TimeoutException( "The audio thread failed to drain the playback queue within reasonable time" );
				}
			}
		}

		public override void signalVideoReady()
		{
			videoReady.Set();
			// Logger.logVerbose( "AudioBase.signalVideoReady" );
			m_paused = false;
		}

		public override void Dispose()
		{
			videoReady?.Dispose();
			base.Dispose();
		}
	}
}