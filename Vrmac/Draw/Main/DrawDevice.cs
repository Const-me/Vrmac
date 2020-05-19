using Diligent.Graphics;
using System.Numerics;
using Vrmac.Draw.SwapChain;
using Vrmac.Draw.Text;

namespace Vrmac.Draw.Main
{
	sealed class DrawDevice: iDrawDevice, iVrmacDrawDevice
	{
		/// <summary>Utility object to scale stuff with dpiScalingFactor, from pixels to units and vice versa.</summary>
		public DpiScaling dpiScaling { get; private set; }

		/// <summary>Size of the viewport, in DPI scaled units</summary>
		public Vector2 viewportSize { get; private set; }

		bool iDrawDevice.premultipliedAlphaBrushes => true;

		public readonly iVrmacDraw factory = Render.graphicsEngine.createDrawFactory().createVrmacDraw();
		readonly Context context3d;
		iSwapChain swapChain;

		internal DrawDevice( Context context3d )
		{
			this.context3d = context3d;
			paletteTexture = new PaletteTexture( context3d );
			context = new VrmacDrawContext( context3d, this, factory );
			rootTransform = new Matrix3x2();
			rootTransform.M31 = -1;
			rootTransform.M32 = 1;

			onResized( context3d.swapChainSize, context3d.dpiScalingFactor );
			context3d.swapChainResized.add( this, onResized );

			fontTextures = new Textures( context3d, factory );
		}

		Matrix3x2 rootTransform;

		void onResized( CSize newSize, double dpi )
		{
			dpiScaling = new DpiScaling( dpi );
			viewportSize = new Vector2( (float)( newSize.cx / dpi ), (float)( newSize.cy / dpi ) );
			rootTransform.M11 = (float)( 2.0 / viewportSize.X );
			rootTransform.M22 = (float)( -2.0 / viewportSize.Y );

			foreach( var sub in resized )
				sub( viewportSize, dpi );
			swapChain?.destroyTargets();
		}

		public WeakEvent<ResizedDelegate> resized { get; } = new WeakEvent<ResizedDelegate>();

		DpiScaling iDrawDevice.dpiScaling => dpiScaling;
		Vector2 iDrawDevice.viewportSize => viewportSize;
		WeakEvent<ResizedDelegate> iDrawDevice.resized => resized;

		Rect? iVrmacDrawDevice.clippingRectangleOverride
		{
			get => context.tesselatorThread.customClippingRect;
			set => context.tesselatorThread.customClippingRect = value;
		}

		public void Dispose()
		{
			context?.context?.drawDeviceDisposed();
			context?.Dispose();
		}

		iGeometry iDrawDevice.createPathGeometry( iPathData data )
		{
			return data.createPathGeometry( factory );
		}

		iGeometry iDrawDevice.createRectangleGeometry( Rect rect )
		{
			iPathData data = Shapes.rectangle( rect );
			return data.createPathGeometry( factory );
		}

		readonly VrmacDrawContext context;

		iImmediateDrawContext iDrawDevice.begin( ITextureView rtv, ITextureView dsv, Vector4 clearColor )
		{
			if( null == swapChain )
				swapChain = SwapChains.create( context.context );
			// ConsoleLogger.logDebug( "iDrawDevice.begin 1" );
			ITextureView rgb = swapChain.begin( rtv, dsv, clearColor );
			// ConsoleLogger.logDebug( "iDrawDevice.begin 2" );
			iImmediateDrawContext dc = context.begin( ref rootTransform, swapChain.swapChainFormats, rgb, clearColor.isNotTransparent() );
			// ConsoleLogger.logDebug( "iDrawDevice.begin 3" );
			return dc;
		}

		internal void present( bool replace )
		{
			swapChain.end( replace );
		}

		// SolidColorBrush objects are very small, very fast to construct, and unlike D2D they don't own any unmanaged resources.
		// The purpose of this cache - reduce load on the GC: we have better things to occupy CPU with.
		const int brushesCacheCapacity = 0x100;
		LruCache<Vector4, iBrush> brushesCache = new LruCache<Vector4, iBrush>( brushesCacheCapacity );

		iBrush iDrawDevice.createSolidColorBrush( Vector4 color )
		{
			iBrush brush = brushesCache.lookup( color );
			if( null != brush )
				return brush;

			brush = new SolidColorBrush( paletteTexture, ref color );
			brushesCache.add( color, brush );
			return brush;
		}

		iStrokeStyle iDrawDevice.createStrokeStyle( sStrokeStyle style )
		{
			return new StrokeStyle( style );
		}

		internal static readonly Rect clipSpaceRectangle = new Rect( -1, -1, 1, 1 );

		public TextureAtlas textureAtlas { get; private set; }

		TextureAtlas iDrawDevice.textureAtlas
		{
			get
			{
				if( null != textureAtlas )
					return textureAtlas;
				textureAtlas = new TextureAtlas( context3d, factory, eTextureAtlasFormat.RGBA8 );
				return textureAtlas;
			}
		}

		public FontCollectionBase fontCollection { get; private set; }

		iFontCollection iDrawDevice.fontCollection
		{
			get
			{
				if( null != fontCollection )
					return fontCollection;
				if( RuntimeEnvironment.runningWindows )
					fontCollection = new WindowsFonts( this );
				else
					fontCollection = new LinuxFonts( this );
				return fontCollection;
			}
		}

		public PaletteTexture paletteTexture { get; }
		public Textures fontTextures { get; }
	}
}