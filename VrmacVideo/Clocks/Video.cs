using System;
using VrmacVideo.IO;

namespace VrmacVideo.Clocks
{
	/// <summary>Presentation clock for media files without audio. Untested.</summary>
	sealed class Video: PresentationClock
	{
		public Video( StatefulVideoDecoder videoDecoder ) :
			base( videoDecoder )
		{
			Logger.logVerbose( "Presentation clock: using video" );
		}

		TimeSpan? presentationStart = null;

		protected override bool onVideoStreamTick( TimeSpan pendingVideoFrame )
		{
			if( m_paused )
				return false;

			if( presentationStart.HasValue )
			{
				TimeSpan now = LibC.gettime( eClock.Monotonic );
				if( presentationStart.Value + pendingVideoFrame > now )
				{
					// The first of the pending frames should be rendered in the future, as measured with eClock.Monotonic OS clock.
					return false;
				}
			}
			else
				presentationStart = LibC.gettime( eClock.Monotonic ) - pendingVideoFrame;
			return true;
		}

		public override TimeSpan getCurrentTime()
		{
			if( presentationStart.HasValue )
			{
				TimeSpan now = LibC.gettime( eClock.Monotonic );
				return now - presentationStart.Value;
			}
			return TimeSpan.Zero;
		}

		bool m_paused = false;
		public override bool isPaused => m_paused;

		public override void pause()
		{
			m_paused = true;

		}
		public override void resume()
		{
			m_paused = false;
			presentationStart = null;
		}

		public override void seek( TimeSpan where )
		{
			throw new NotImplementedException();
		}
	}
}