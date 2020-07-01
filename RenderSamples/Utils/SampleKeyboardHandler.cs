using System;
using System.IO;
using Vrmac;
using Vrmac.Input;

namespace RenderSamples.Utils
{
	/// <summary>An example keyboard handler that implements a few hot keys to switch to fullscreen and back, and move the window.</summary>
	class SampleKeyboardHandler: iKeyboardHandler
	{
		readonly Context context;

		public SampleKeyboardHandler( Context context )
		{
			this.context = context;
		}

		bool handleWindowedModeHotkey( iDiligentWindow window, eKey key, eKeyboardState ks )
		{
			// if( key == eKey.Esc && content.windowState == eShowWindow.Fullscreen )
			if( key == eKey.Esc )
			{
				window.moveWindow( eShowWindow.Normal );
				return true;
			}
			if( key == eKey.Up && ks.HasFlag( eKeyboardState.ControlDown ) )
			{
				window.moveWindow( eShowWindow.Maximized );
				return true;
			}
			if( key == eKey.Down && ks.HasFlag( eKeyboardState.ControlDown ) )
			{
				window.moveWindow( eShowWindow.Minimized );
				return true;
			}
			if( key == eKey.Enter && ks.HasFlag( eKeyboardState.AltDown ) )
			{
				if( ks.HasFlag( eKeyboardState.ControlDown ) )
					switchToTrueFullScreen( window );
				else
					window.moveWindow( eShowWindow.Fullscreen );
				return true;
			}
			return false;
		}

		void iKeyboardHandler.keyEvent( eKey key, eKeyValue what, ushort unicodeChar, eKeyboardState ks )
		{
			// Console.WriteLine( "iKeyboardHandler.keyEvent: {0} {1} {2:x} {3}", key, what, unicodeChar, ks );

			if( what != eKeyValue.Pressed )
				return;
			if( context.renderContext is iDiligentWindow window )
				if( handleWindowedModeHotkey( window, key, ks ) )
					return;

			if( key == eKey.P )
			{
				screenshot();
				return;
			}

			if( context.scene is iKeyPressedHandler hh )
				hh.keyPressed( key, ks );
		}

		void switchToTrueFullScreen( iDiligentWindow window )
		{
			if( window.windowState == eShowWindow.TrueFullscreen )
			{
				// Already in true full screen mode. Switch to maximized borderless.
				window.moveWindow( eShowWindow.Fullscreen );
				return;
			}

			iGraphicsEngine graphicsEngine = Render.graphicsEngine;
			using( var gpuEnum = graphicsEngine.createGpuEnumerator() )
			using( var gpu = gpuEnum.openDefaultAdapter() )
			using( var connector = gpu.openDefaultConnector() )
			{
				var format = context.swapChainFormats.color;
				if( format == Diligent.Graphics.TextureFormat.Unknown )
					throw new ApplicationException( "The swap chain was not created" );
				connector.setSurfaceFormat( format );

				// Debug code below
				// Console.WriteLine( "Following is available:\n{0}", string.Join( "\n", connector.getAllModes() ) );
				// connector.enumModes();

				CSize trueFullScreenRez = new CSize( 1920, 1080 );
				if( !connector.findVideoMode( out var mode, ref trueFullScreenRez ) )
					throw new ApplicationException( "The default monitor doesn't support FullHD" );
				window.fullScreen( connector, ref mode );
			}
		}

		static int sn = 1;

		void screenshot()
		{
			string path = Path.GetTempPath();
			string name = string.Format( "screenshot-{0:D2}.png", sn++ );
			path = Path.Combine( path, "Vrmac", name );
			context.saveScreenshot( path );
		}
	}
}