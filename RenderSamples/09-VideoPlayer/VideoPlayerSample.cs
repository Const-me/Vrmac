using Diligent.Graphics;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Vrmac;
using Vrmac.Input;
using Vrmac.Input.Linux;
using Vrmac.MediaEngine;

namespace RenderSamples
{
	class VideoPlayerSample: SampleBase, iSceneAsyncInit, iMouseInput, iKeyPressedHandler
	{
		public string mediaFilePath { get; private set; } = null;
		iMediaEngine mediaEngine;

		public VideoPlayerSample( string[] args )
		{
			mediaFilePath = MediaFileName.getPath( args );
			mouseHandler = new MouseHandler( context );
		}

		protected override void createResources( IRenderDevice device )
		{
			mediaEngine = context.createMediaEngine( context.swapChainFormats.color );
		}

		/// <summary>Utility object to render frames of that video</summary>
		iVideoRenderState renderState;

		// Color of the pixels on the outer border of the video frame, visible if aspect ratio of the video doesn't exactly match the RT.
		// Paint them with dark blue color just for lulz.
		// static Vector4 videoBorderColor => new Vector4( 0, 0, 0.25f, 1 );
		static Vector4 videoBorderColor => Color.black;

		async Task iSceneAsyncInit.createResourcesAsync( Context context )
		{
			iDiligentWindow wnd = context.renderContext as iDiligentWindow;
			wnd?.setTitle( "Vrmac graphics video player sample" );

			mediaEngine.autoPlay = true;
			// string dir = RuntimeEnvironment.runningWindows ? windowsDirectory : linuxDirectory;
			// await mediaEngine.loadMedia( Path.Combine( dir, fileName ) );
			await mediaEngine.loadMedia( mediaFilePath );

			var streams = mediaEngine.mediaStreams;
			if( !streams.HasFlag( eMediaStreams.Video ) )
				throw new ApplicationException( "There's no video in that media file" );

			using( var dev = context.device )
				renderState = mediaEngine.createRenderer( context, dev, videoBorderColor );
			context.swapChainResized.add( this, onResized );

			ConsoleLogger.logInfo( "VideoPlayerSample started playing the media" );
			mouseHandler.subscribe( controller );
			controller.initialize( animation, mediaEngine );

			if( null != wnd )
			{
				string fileName = Path.GetFileName( mediaFilePath );
				wnd.windowTitle = "Vrmac graphics video player sample: " + fileName;
			}

			// Debug code below: wait 2 seconds, and seek
			// await Task.Delay( 2000 );
			// mediaEngine.setCurrentTime( TimeSpan.FromSeconds( 2278 ) );
		}

		void onResized( CSize newSize, double dpiScaling )
		{
			using( var dev = context.device )
				renderState.resize( dev, newSize );
		}

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			if( null == renderState )
				return;
			IDeviceContext ic = context.context;
			ic.SetRenderTarget( swapChainRgb, null );

			if( controller.paused )
			{
				renderState.renderLastFrame( ic );
				return;
			}

			while( !mediaEngine.onVideoStreamTick( out ulong presentationTime ) )
				context.renderContext.waitForVBlank();

			renderState.render( ic );
		}

		MouseHandler mouseHandler;
		iMouseHandler iMouseInput.mouseHandler => mouseHandler;
		RawDevice iMouseInput.getMouseDevice() => null;

		readonly VideoPlayerController controller = new VideoPlayerController();

		void iKeyPressedHandler.keyPressed( eKey key, eKeyboardState keyboardState ) =>
			controller.keyPressed( key, keyboardState );
	}
}