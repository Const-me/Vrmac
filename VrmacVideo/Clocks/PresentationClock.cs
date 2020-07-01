using System;

namespace VrmacVideo
{
	/// <summary>Interface for the audio thread to call presentation clock object</summary>
	interface iAudioPresentationClock
	{
		void updateTimestamp( TimeSpan timestamp );
		void drainComplete();
		bool waitForVideoFrame();
	}

	abstract class PresentationClock: IDisposable
	{
		public PresentationClock( StatefulVideoDecoder videoDecoder )
		{
			this.videoDecoder = videoDecoder;
		}

		protected readonly StatefulVideoDecoder videoDecoder;

		public bool onVideoStreamTick( out ulong presentationTime )
		{
			videoDecoder.thread.marshalException();

			TimeSpan? tsPending = videoDecoder.nextFramePresentationTime;
			if( !tsPending.HasValue )
			{
				// Don't have pending video frames at the moment
				presentationTime = default;
				return false;
			}

			if( !onVideoStreamTick( tsPending.Value ) )
			{
				presentationTime = default;
				return false;
			}

			presentationTime = (ulong)tsPending.Value.Ticks;
			return true;
		}

		protected abstract bool onVideoStreamTick( TimeSpan pendingVideoFrame );

		public abstract TimeSpan getCurrentTime();

		public virtual void Dispose() { }

		public abstract bool isPaused { get; }
		public abstract void pause();
		public abstract void resume();

		public abstract void seek( TimeSpan where );

		/// <summary>Called by decoder thread before it starts posting encoded audio frames which should come after the seek.
		public virtual void waitForAudioDrain() { }

		/// <summary>Called by decoder thread when it decoded the target frame of the seek, and it's time to restart the clock.</summary>
		/// <remarks>While the decoder thread is doing that, audio thread sleeps on an event.</remarks>
		public virtual void signalVideoReady() { }
	}
}