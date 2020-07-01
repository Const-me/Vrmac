using System;
using Vrmac;
using Vrmac.Animation;
using Vrmac.Input;
using Vrmac.MediaEngine;

namespace RenderSamples
{
	sealed class VideoPlayerController: iMouseWheelHandler, iButtonHandler
	{
		static readonly TimeSpan veryLargeSeek = TimeSpan.FromMinutes( 10 );
		static readonly TimeSpan largeSeek = TimeSpan.FromMinutes( 1 );
		static readonly TimeSpan smallSeek = TimeSpan.FromSeconds( 5 );

		Animations animations;
		/// <summary>Fake animation to force game-style main loop, as opposed to environment friendly one.</summary>
		IDisposable fakeAnimation = null;

		iMediaEngine mediaEngine;

		public void initialize( Animations anim, iMediaEngine media )
		{
			animations = anim;
			mediaEngine = media;
			if( null == fakeAnimation )
				fakeAnimation = animations.startFake();
		}

		public bool paused { get; private set; }
		void playPause()
		{
			if( null == mediaEngine )
				return;
			if( paused )
			{
				mediaEngine.play();
				ConsoleLogger.logInfo( "Resumed the playback" );
				fakeAnimation = animations.startFake();
			}
			else
			{
				mediaEngine.pause();
				ConsoleLogger.logInfo( "Paused the playback" );
				fakeAnimation?.Dispose();
				fakeAnimation = null;
			}
			paused = !paused;
		}

		void seek( TimeSpan delta )
		{
			TimeSpan where = mediaEngine.currentTime + delta;
			if( where.Ticks < 0 )
				where = TimeSpan.Zero;
			else if( where > mediaEngine.duration )
				where = mediaEngine.duration;
			mediaEngine.currentTime = where;
		}

		public void keyPressed( eKey key, eKeyboardState keyboardState )
		{
			switch( key )
			{
				case eKey.Space:
					playPause();
					return;
				case eKey.Home:
					mediaEngine.currentTime = TimeSpan.Zero;
					return;
				case eKey.PageUp:
					seek( keyboardState.HasFlag( eKeyboardState.ControlDown ) ? -veryLargeSeek : -largeSeek );
					return;
				case eKey.PageDown:
					seek( keyboardState.HasFlag( eKeyboardState.ControlDown ) ? veryLargeSeek : largeSeek );
					return;
				case eKey.Left:
					seek( -smallSeek );
					return;
				case eKey.Right:
					seek( smallSeek );
					return;
			}
		}

		void iMouseWheelHandler.wheel( CPoint point, int delta, eMouseButtonsState bs )
		{

		}

		void iButtonHandler.buttonDown( CPoint point, eMouseButton button, eMouseButtonsState bs )
		{
			switch( button )
			{
				case eMouseButton.Left:
					playPause();
					return;
				case eMouseButton.Middle:
					mediaEngine.muted = !mediaEngine.muted;
					return;
			}
		}

		void iButtonHandler.buttonUp( CPoint point, eMouseButton button, eMouseButtonsState bs )
		{ }
	}
}