using ComLight;
using System;
using Vrmac.Input;
using Vrmac.Input.Linux;
using Vrmac.ModeSet;

namespace Vrmac
{
	/// <summary>This static class integrates everything together: mode setting, graphics setup, rendering, input, and more.</summary>
	public static class Render
	{
		static void setupWindowedInput( iDiligentWindow window, Context content )
		{
			iKeyboardInput keyboardInput = content.scene as iKeyboardInput;
			iMouseInput mouseInput = content.scene as iMouseInput;
			if( null == keyboardInput && null == mouseInput )
				return;
			if( null != keyboardInput )
			{
				var handler = keyboardInput.keyboardHandler;
				if( handler is iInputEventTime eventTime )
					eventTime.sourceInitialized( window.timeSource() );
				window.input.keyboardInputHandler( handler );
			}
			if( null != mouseInput )
			{
				var handler = mouseInput.mouseHandler;
				if( handler is iInputEventTime eventTime )
					eventTime.sourceInitialized( window.timeSource() );
				window.input.mouseInputHandler( handler );
			}
		}

		static void flushShadersCache( this Context content )
		{
			if( null != content.shaderCache )
			{
				int wrote = content.shaderCache.flushToDisk();
				if( wrote > 0 )
					ConsoleLogger.logDebug( "Flushed shaders cache, wrote {0} new entries there", wrote );
				else
					ConsoleLogger.logDebug( "No new shader cache entries" );
			}
			else
				ConsoleLogger.logDebug( "Shaders cache was not set up" );
		}

		/// <summary>Create a window on desktop, and run the application rendering into that window.</summary>
		/// <remarks>This works on both Windows and Linux. On Linux you need a desktop manager for this, "Raspbian Buster Lite" distro doesn’t have it.</remarks>
		public static void renderWindowed( this iGraphicsEngine engine, Context content, string windowTitle = null, iWindowSetup windowSetup = null )
		{
			if( null == currentEngine )
				currentEngine = new WeakReference<iGraphicsEngine>( engine );

			var deviceType = RuntimeEnvironment.defaultDevice;

			engine.loadKeySyms();
			using( var dispatcher = engine.dispatcher() )
			using( var window = dispatcher.nativeDispatcher.createWindow( content, windowSetup, deviceType ) )
			{
				content.initialize( window );
				// Wire up the input
				setupWindowedInput( window, content );
				// Set the title
				window.windowTitle = windowTitle ?? $"{ content.ToString() } ( { deviceType } )";

				// Run the main loop
				dispatcher.run( content );
			}
			content.flushShadersCache();
		}

		static void setupRawInput( Dispatcher dispatcher, Context content )
		{
			if( content.scene is iKeyboardInput keyboardInput )
			{
				var handler = keyboardInput.keyboardHandler;
				var dev = dispatcher.openRawKeyboard( handler, keyboardInput.getKeyboardDevice() );
				( handler as iInputEventTime )?.sourceInitialized( dev );
			}
			if( content.scene is iMouseInput mouseInput )
			{
				var handler = mouseInput.mouseHandler;
				var dev = dispatcher.openRawMouse( mouseInput.mouseHandler, mouseInput.getMouseDevice() );
				( handler as iInputEventTime )?.sourceInitialized( dev );
			}
		}

		static iGpuConnector openConnector( iGpu gpu, Context content )
		{
			return gpu.openConnector( 0 );
		}

		/// <summary>Render on physical display.</summary>
		/// <remarks>This only works on Linux. If a desktop manager is running, will probably fail because it won’t get access to the GPU.</remarks>
		public static void renderFullScreen( this iGraphicsEngine engine, Context content, CSize resolution, iVideoSetup videoSetup = null )
		{
			if( !RuntimeEnvironment.runningLinux )
				throw new NotSupportedException( "Render.renderFullScreen method only supported on Linux" );

			if( null == currentEngine )
				currentEngine = new WeakReference<iGraphicsEngine>( engine );

			using( var dispatcher = engine.dispatcher() )
			using( var gpuEnum = engine.createGpuEnumerator() )
			using( var gpu = gpuEnum.openFirstAdapter() )
			using( var connector = openConnector( gpu, content ) )
			{
				sVideoMode mode;
				if( !connector.findVideoMode( out mode, ref resolution ) )
					throw new ApplicationException( "The requested resolution is not supported by the combination of GPU and display" );

				using( var modesetContext = connector.createContext( mode.index, videoSetup ) )
				{
					using( var gl = ComLightCast.cast<iGlRenderContext>( modesetContext ) )
						Console.WriteLine( "Initialized video mode: {0}, {1}", modesetContext.getMode(), gl.getEglConfig() );

					using( var renderContext = engine.createFullScreenContext( content, modesetContext ) )
					{
						content.initialize( renderContext );
						setupRawInput( dispatcher, content );

						// Console.WriteLine( "Initialized rendering" );

						dispatcher.nativeDispatcher.renderFrame( renderContext, true );
						// Run the main loop
						dispatcher.run( content );
					}
				}
			}
			content.flushShadersCache();
		}

		static WeakReference<iGraphicsEngine> currentEngine;

		/// <summary>Get the graphics engine object</summary>
		public static iGraphicsEngine graphicsEngine =>
			currentEngine.TryGetTarget( out var res ) ? res : null;

		internal static Direct2D.iDrawDevice createDirect2dDevice( Context content )
		{
			if( content.renderContext is iDiligentWindow window && RuntimeEnvironment.operatingSystem == eOperatingSystem.Windows )
			{
				using( var df = graphicsEngine.createDrawFactory() )
					return df.createD2dDevice( window );
			}
			throw new NotSupportedException( "Direct2D is Windows only" );
		}
	}
}