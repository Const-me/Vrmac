using Diligent.Graphics;
using System;
using Vrmac;
using Vrmac.Direct2D;
using Vrmac.Draw;

namespace RenderSamples
{
	/* class DrawResources: IDisposable
	{
		readonly Context context3D;
		readonly DrawContext d2d;
		iRectangleGeometry rcBackground;
		iSolidColorBrush backgroundBrush;

		public DrawResources( Context vc )
		{
			context3D = vc;
			d2d = vc.createDrawContext();
			backgroundBrush = d2d.device.createSolidBrush( "#6ccc" );
			rcBackground = default;

			onResized( d2d.size );
			d2d.resized.add( this, onResized );
		}

		void IDisposable.Dispose()
		{
			rcBackground?.Dispose();
			rcBackground?.Dispose();
			d2d?.Dispose();
		}

		void onResized( Vector2 size )
		{
			rcBackground?.Dispose();
			rcBackground = null;

			Rect rc = new Rect( Vector2.Zero, size );
			rc.top = d2d.dpiScaling.roundToPixels( rc.bottom - 20 );
			rcBackground = d2d.device.createRectangleGeometry( ref rc );
		}

		public void draw()
		{
			d2d.context.beginDraw();
			d2d.context.fillGeometry( rcBackground, backgroundBrush );
			d2d.context.endDraw();
		}
	} */
}