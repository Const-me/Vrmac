using Diligent.Graphics;
using System;
using System.Diagnostics;
using Vrmac.Animation;
using Vrmac.Utils;
using Vrmac.Utils.Cursor;

namespace Vrmac
{
	/// <summary>3D rendering context of Vrmac graphics engine.</summary>
	public sealed class Context: iContent
	{
		/// <summary>The scene being rendered.</summary>
		public iScene scene { get; }
		/// <summary>COM pointer to the C++ object that actually implements all that stuff.</summary>
		public iRenderingContext renderContext { get; private set; }
		/// <summary>Diligent GPU device context, to send draw calls or other commands</summary>
		public IDeviceContext context { get; private set; }

		/// <summary>Cache of the compiled shaders</summary>
		public ShaderCache shaderCache { get; private set; }

		/// <summary>True if the renderer uses OpenGL or OpenGL ES. You probably need this value because perspective projection matrices use slightly different math.</summary>
		/// <seealso cref="Matrix.CreatePerspectiveFieldOfView(float, float, float, float, bool)" />
		public bool isOpenGlDevice { get; private set; }

		/// <summary>Current DPI scale setting</summary>
		/// <remarks>Currently, it’s only set on Windows.</remarks>
		public double dpiScalingFactor { get; private set; } = 1;

		/// <summary>Size of the swap chain in pixels</summary>
		public CSize swapChainSize { get; private set; }

		/// <summary>Width / height ratio of the swap chain</summary>
		public float aspectRatio => swapChainSize.isEmpty ? 0 : (float)swapChainSize.cx / (float)swapChainSize.cy;

		/// <summary>Format of the swap chain’s buffers</summary>
		public SwapChainFormats swapChainFormats { get; private set; }

		internal int swapChainBuffersCount { get; private set; }
		internal int getCurrentBackBufferIndex() => swapChain.GetCurrentBackBufferIndex();

		/// <summary>Current state of the window</summary>
		/// <remarks>For full-screen rendering always eShowWindow.Fullscreen and never changes.</remarks>
		public eShowWindow windowState { get; private set; } = eShowWindow.Fullscreen;

		/// <summary>Construct the object</summary>
		/// <param name="scene">The scene to render.</param>
		/// <param name="flags">Miscellaneous flags</param>
		public Context( iScene scene, eCreateContextFlags flags = eCreateContextFlags.CacheCompiledShaders )
		{
			this.scene = scene;
			creationFlags = flags;
		}

		readonly eCreateContextFlags creationFlags;
		ISwapChain swapChain;

		void iContent.sourceInitialized( out iShaderCache cache )
		{
			if( creationFlags.HasFlag( eCreateContextFlags.CacheCompiledShaders ) )
			{
				shaderCache = new ShaderCache( true );
				cache = shaderCache;
			}
			else
				cache = null;
		}

		static byte getSampleCount( ISwapChain swapChain )
		{
			using( var rtv = swapChain.GetCurrentBackBufferRTV() )
			using( var texture = rtv.GetTexture() )
			{
				var desc = texture.GetDesc();
				return (byte)desc.SampleCount;
			}
		}

		/// <summary>A dispatcher that’s running this context</summary>
		public Dispatcher dispatcher { get; private set; }

		/// <summary>Initialize the object.</summary>
		public void initialize( iRenderingContext rc )
		{
			if( null != renderContext )
				throw new ApplicationException( "Already initialized" );
			dispatcher = Dispatcher.currentDispatcher;
			if( null == dispatcher )
				throw new ApplicationException( "Context.initialize requires the current thread to have a dispatcher" );

			renderContext = rc;
			context = rc.context;
			swapChain = rc.swapChain;
			if( rc is iDiligentWindow window )
				dpiScalingFactor = window.dpiScaling;

			var device = rc.device;
			// This is required because runtime catches exceptions from async void methods, and redirects them to dispatcher.
			// Not what we want, therefore this method can't be async.
			try
			{
				isOpenGlDevice = device.isGlDevice();

				var scDesc = swapChain.GetDesc();
				swapChainSize = new CSize( scDesc.Width, scDesc.Height );
				swapChainFormats = new SwapChainFormats( scDesc.ColorBufferFormat, scDesc.DepthBufferFormat, getSampleCount( swapChain ) );
				swapChainBuffersCount = scDesc.BufferCount;

				animation = new Animations( this );
				scene.createResources( this, device );
			}
			catch( Exception )
			{
				device.Dispose();
				throw;
			}
			initializeAsync( device );
		}

		async void initializeAsync( IRenderDevice device )
		{
			using( device )
			{
				if( scene is iSceneAsyncInit sai )
					await sai.createResourcesAsync( this, device );
				animation.timers.start();
			}
		}

		ITextureView cachedRtv, cachedDsv;

		/// <summary>While rendering a frame, this set to the currently used back buffer texture view. All the other time it’s null.</summary>
		public ITextureView backBufferTexture { get; private set; }

		void iContent.resized( ref sWindowPosition position )
		{
			// ConsoleLogger.logDebug( "{0}", position );
			try
			{
				// No need to waste resources rendering frames while the window is minimized.
				if( position.show == eShowWindow.Minimized )
				{
					if( null == timersHardPause )
						animation.timers.pause();
					Dispatcher.currentDispatcher.nativeDispatcher.runPolicy = eDispatcherRunPolicy.EnvironmentFriendly;
				}
				else
				{
					if( null == timersHardPause && animation.any )
					{
						animation.timers.resume();
						Dispatcher.currentDispatcher.nativeDispatcher.runPolicy = eDispatcherRunPolicy.GameStyle;
					}
				}

				ComUtils.clear( ref cachedRtv );
				ComUtils.clear( ref cachedDsv );
				context?.SetRenderTargets( 0, null, null, ResourceStateTransitionMode.None );

				windowState = position.show;

				if( dpiScalingFactor != position.dpiScaling )
				{
					dpiScalingFactor = position.dpiScaling;
					var evt = m_dpiChanged;
					if( null != evt )
						foreach( var sub in evt )
							sub( dpiScalingFactor );
				}

				if( position.show != eShowWindow.Minimized )
				{
					foreach( var rr in releaseResources )
						rr( eReleaseResources.Buffers );

					swapChain?.Resize( position.size.cx, position.size.cy );
					swapChainSize = position.size;
					foreach( var sub in swapChainResized )
						sub( position.size, position.dpiScaling );
				}
			}
			catch( Exception ex )
			{
				// This is to marshal that exception across C++ code on the stack. C++ can't deliver .NET exceptions, it only knows about HRESULT codes.
				// cacheException call makes it so the complete .NET exception is delivered to the caller, not just the HRESULT code of it.
				NativeContext.cacheException( ex );
				throw;
			}
		}

		void render()
		{
			// ConsoleLogger.logDebug( "ContentBase.render" );
			var rtv = swapChain?.GetCurrentBackBufferRTV();
			Debug.Assert( null != rtv );
			ComUtils.assign( ref cachedRtv, rtv );
			var dsv = swapChain?.GetDepthBufferDSV();
			Debug.Assert( null != dsv );
			ComUtils.assign( ref cachedDsv, dsv );
			if( null == rtv || null == dsv )
				return;

			animation?.update();
			// ConsoleLogger.logDebug( "ContentBase.render 2" );
			backBufferTexture = rtv;
			try
			{
				// MicroProfiler.start();
				scene.render( this, rtv, dsv );
				m_cursor?.render();

				string screenshot = System.Threading.Interlocked.Exchange( ref screenshotLocation, null );
				if( null != screenshot )
				{
					using( var dev = device )
					using( var tx = rtv.GetTexture() )
						ScreenGrabber.saveTexture( dev, context, tx, screenshot );
				}
				// MicroProfiler.key( "done rendering" );
			}
			finally
			{
				backBufferTexture = null;
			}
			swapChain.Present( 1 );
			// MicroProfiler.finish();
		}

		void iContent.render()
		{
			try
			{
				render();
			}
			catch( Exception ex )
			{
				NativeContext.cacheException( ex );
				throw;
			}
		}
		bool iContent.shouldClose() => ( scene as iWindowEvents )?.shouldClose() ?? true;
		bool iContent.shouldExit() => ( scene as iWindowEvents )?.shouldExit() ?? true;

		IDisposable timersHardPause = null;

		void iContent.releaseCachedResources( eReleaseResources what )
		{
			try
			{
				if( null == timersHardPause )
					timersHardPause = animation.timers.hardPause();

				foreach( var rr in releaseResources )
					rr( what );

				ComUtils.clear( ref cachedRtv );
				ComUtils.clear( ref cachedDsv );
				context?.SetRenderTargets( 0, null, null, ResourceStateTransitionMode.None );

				if( what == eReleaseResources.Buffers )
					return;

				context?.Dispose();
				context = null;
				if( what == eReleaseResources.Context )
					return;

				swapChain?.Dispose();
				swapChain = null;
			}
			catch( Exception ex )
			{
				NativeContext.cacheException( ex );
				throw;
			}
		}

		void iContent.swapChainRecreated( iDiligentWindow window )
		{
			try
			{
				swapChain = window.swapChain;
				context = window.context;

				timersHardPause?.Dispose();
				timersHardPause = null;

				var scDesc = swapChain.GetDesc();
				swapChainSize = new CSize( scDesc.Width, scDesc.Height );
				byte samples = swapChainFormats.sampleCount;
				swapChainFormats = new SwapChainFormats( scDesc.ColorBufferFormat, scDesc.DepthBufferFormat, samples );
				swapChainBuffersCount = scDesc.BufferCount;
			}
			catch( Exception ex )
			{
				NativeContext.cacheException( ex );
				throw;
			}
		}

		/// <summary>Use this object to start or cancel animations</summary>
		public Animations animation { get; private set; }

		/// <summary>Delegate to receive swap chain resized event</summary>
		public delegate void SwapChainResizedDelegate( CSize newSize, double dpiScaling );

		/// <summary>Subscribe to this event to react on user resizing the window.</summary>
		/// <remarks>Only called in windowed mode. Ain’t called when user minimizes the window.</remarks>
		public WeakEvent<SwapChainResizedDelegate> swapChainResized { get; } = new WeakEvent<SwapChainResizedDelegate>();

		internal delegate void ReleaseResourcesDelegate( eReleaseResources what );
		internal WeakEvent<ReleaseResourcesDelegate> releaseResources { get; } = new WeakEvent<ReleaseResourcesDelegate>();

		WeakEvent<Action<double>> m_dpiChanged = null;
		/// <summary>Subscribe to this event to get notified when DPI multiplier changes</summary>
		public WeakEvent<Action<double>> dpiChanged => MiscUtils.createOnFirstUse( ref m_dpiChanged );

		Draw.iDrawDevice m_2d = null;

		/// <summary>Get or create 2D graphics context.</summary>
		/// <remarks>On Windows in D2D mode this will load quite a few Windows DLLs: DiligentNative.dll uses delay loading for 2D graphics OS dependencies.</remarks>
		public Draw.iDrawDevice drawDevice
		{
			get
			{
				if( null != m_2d )
					return m_2d;
				bool vrmac = creationFlags.HasFlag( eCreateContextFlags.PreferVrmac2D );
				if( RuntimeEnvironment.operatingSystem == eOperatingSystem.Windows && !vrmac )
					m_2d = new Draw.Direct2dDevice( this );
				else
					m_2d = new Draw.Main.DrawDevice( this );
				return m_2d;
			}
		}

		internal void drawDeviceDisposed()
		{
			m_2d = null;
		}

		/// <summary>If the dispatcher is running a videogame-style message loop because animations, do nothing and return false.
		/// If it’s running environment friendly loop, schedule rendering of another frame and return true.</summary>
		/// <remarks>May be called from any thread</remarks>
		public bool queueRenderFrame()
		{
			if( dispatcher.nativeDispatcher.runPolicy == eDispatcherRunPolicy.GameStyle )
				return false;
			dispatcher.nativeDispatcher.renderFrame( renderContext, true );
			return true;
		}

		/// <summary>3D device</summary>
		public IRenderDevice device => renderContext.device;

		internal string screenshotLocation;

		/// <summary>Set this property to render a mouse cursor</summary>
		public eCursor mouseCursor
		{
			get => m_cursor?.cursor ?? eCursor.None;
			set => setMouseCursor( value );
		}

		MouseCursor m_cursor;

		MouseCursor createCursor()
		{
			if( null != m_cursor )
				return m_cursor;
			m_cursor = new MouseCursor( this );
			return m_cursor;
		}
		void setMouseCursor( eCursor cursor )
		{
			if( mouseCursor == cursor )
				return;

			if( cursor == eCursor.None )
			{
				m_cursor?.hide();
				return;
			}

			createCursor().cursor = cursor;
		}

		/// <summary>Update the position of mouse cursor</summary>
		internal void setMouseCursorPosition( CPoint position )
		{
			createCursor().position = position;
		}
	}
}