using System;
using System.Numerics;
using Diligent.Graphics;

namespace Vrmac.Draw
{
	/// <summary>Context class for 2D graphics.</summary>
	sealed class Direct2dDevice: iDrawDevice
	{
		/// <summary>Utility object to scale stuff with dpiScalingFactor, from pixels to units and vice versa.</summary>
		public DpiScaling dpiScaling { get; private set; }
		DpiScaling iDrawDevice.dpiScaling => dpiScaling;

		/// <summary>2D device</summary>
		internal Direct2D.iDrawDevice d2dDevice { get; private set; }
		/// <summary>2D context</summary>
		internal Direct2D.iDrawContext d2dContext { get; private set; }
		/// <summary>Size of the viewport, in DPI scaled units</summary>
		public Vector2 viewportSize { get; private set; }
		Vector2 iDrawDevice.viewportSize => viewportSize;

		readonly Direct2D.DrawContext context;
		readonly Context context3D;

		internal Direct2dDevice( Context context3D )
		{
			this.context3D = context3D;
			d2dDevice = Vrmac.Render.createDirect2dDevice( context3D );
			d2dContext = d2dDevice.createContext();
			onResized( context3D.swapChainSize, context3D.dpiScalingFactor );
			context3D.swapChainResized.add( this, onResized );
			context3D.releaseResources.add( this, onReleaseResources );

			context = new Direct2D.DrawContext( this, d2dDevice );
		}

		bool iDrawDevice.premultipliedAlphaBrushes => false;

		void onResized( CSize newSize, double dpi )
		{
			dpiScaling = new DpiScaling( dpi );
			d2dContext.dpiScaling = dpi;
			viewportSize = new Vector2( (float)( newSize.cx / dpi ), (float)( newSize.cy / dpi ) );
			foreach( var sub in resized )
				sub( viewportSize, dpi );
		}

		/// <summary>Destroy device and context</summary>
		public void Dispose()
		{
			viewportSize = Vector2.Zero;
			foreach( var sub in resized )
				sub( viewportSize, 1 );
			resized.clear();

			d2dContext?.Dispose();
			d2dContext = null;
			d2dDevice?.Dispose();
			d2dDevice = null;
			context3D?.drawDeviceDisposed();
		}
		void IDisposable.Dispose() => Dispose();

		/// <summary>Subscribe to this event to react on user resizing the window, or changes in DPI.</summary>
		public WeakEvent<ResizedDelegate> resized { get; } = new WeakEvent<ResizedDelegate>();
		WeakEvent<ResizedDelegate> iDrawDevice.resized => resized;

		void onReleaseResources( eReleaseResources what )
		{
			d2dDevice?.releaseResources( what );
		}

		iGeometry iDrawDevice.createPathGeometry( iPathData data )
		{
			return data.createPathGeometry( d2dDevice );
		}

		iGeometry iDrawDevice.createRectangleGeometry( Rect rect )
		{
			return d2dDevice.createRectangleGeometry( ref rect );
		}

		const int brushesCacheSize = 32;
		readonly LruCache<Vector4, iBrush> brushesCache = new LruCache<Vector4, iBrush>( brushesCacheSize );

		iBrush iDrawDevice.createSolidColorBrush( Vector4 color )
		{
			iBrush brush = brushesCache.lookup( color );
			if( null != brush )
				return brush;
			brush = d2dDevice.createSolidColorBrush( ref color );
			brushesCache.add( color, brush );
			return brush;
		}

		iStrokeStyle iDrawDevice.createStrokeStyle( sStrokeStyle style )
		{
			return d2dDevice.createStrokeStyle( ref style );
		}

		iImmediateDrawContext iDrawDevice.begin( ITextureView rtv, ITextureView dsv, Vector4 clearColor )
		{
			context3D.context.SetRenderTarget( rtv, dsv );
			return context.begin( d2dContext, clearColor );
		}

		// internal Direct2D.iSolidColorBrush getSolidBrush( Vector4 color ) => context.getSolidBrush( color );

		TextureAtlas iDrawDevice.textureAtlas => throw new NotSupportedException();
		iFontCollection iDrawDevice.fontCollection => throw new NotImplementedException();
	}
}