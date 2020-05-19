using System;
using System.Numerics;
using Vrmac.Draw;

namespace Vrmac.Direct2D
{
	sealed class DrawContext: iImmediateDrawContext
	{
		readonly iDrawDevice device;
		readonly Draw.iDrawDevice vrmacDevice;
		Draw.iDrawDevice Draw.iDrawContext.device => vrmacDevice;
		public DrawContext( Draw.iDrawDevice vrmacDevice, iDrawDevice d2dDevice )
		{
			this.vrmacDevice = vrmacDevice;
			device = d2dDevice;
		}
		iDrawContext context = null;

		public iImmediateDrawContext begin( iDrawContext dc, Vector4 clearColor )
		{
			if( null != context )
				throw new ApplicationException( "You can't call iVrmacDrawDevice.begin() several times" );
			dc.beginDraw();
			float clearAlpha = clearColor.W;
			if( clearAlpha >= 1.0f / 255.0f )
				dc.clear( ref clearColor );
			context = dc;
			transform.clear();
			return this;
		}

		void iImmediateDrawContext.flush() { }

		void IDisposable.Dispose()
		{
			context?.endDraw();
			context = null;
		}

		/* internal iSolidColorBrush getSolidBrush( Vector4 solidColor )
		{
			iSolidColorBrush brush = brushesCache.lookup( solidColor );
			if( null != brush )
				return brush;
			brush = device.createSolidColorBrush( ref solidColor );
			brushesCache.add( solidColor, brush );
			return brush;
		} */

		void applyTransform()
		{
			if( !transform.changed )
				return;
			Matrix3x2 m = transform.current;
			context.setTransform( ref m );
			transform.clearChanged();
		}

		void Draw.iDrawContext.fillGeometry( Draw.iGeometry path, Draw.iBrush brush )
		{
			applyTransform();
			context.fillGeometry( (iGeometry)path, (iBrush)brush );
		}
		void Draw.iDrawContext.drawGeometry( Draw.iGeometry geometry, Draw.iBrush brush, float strokeWidth, Draw.iStrokeStyle strokeStyle )
		{
			applyTransform();
			context.drawGeometry( (iGeometry)geometry, (iBrush)brush, strokeWidth, (iStrokeStyle)strokeStyle );
		}

		readonly MatrixStack transform = new MatrixStack();
		MatrixStack Draw.iDrawContext.transform => transform;

		void Draw.iDrawContext.drawRectangle( Rect rect, Draw.iBrush brush, float width )
		{
			applyTransform();

			rect.deflate( width * 0.5f );
			context.drawRectangle( ref rect, width, (iBrush)brush );
		}

		void Draw.iDrawContext.fillAndStroke( Draw.iGeometry geometry, Draw.iBrush fill, Draw.iBrush stroke, float strokeWidth, Draw.iStrokeStyle strokeStyle )
		{
			applyTransform();
			context.fillGeometry( (iGeometry)geometry, (iBrush)fill );
			context.drawGeometry( (iGeometry)geometry, (iBrush)stroke, strokeWidth, (iStrokeStyle)strokeStyle );
		}

		void Draw.iDrawContext.drawSprite( Rect rect, int spriteIndex ) =>
			throw new NotSupportedException();

		void Draw.iDrawContext.drawText( string text, iFont font, Rect layoutRect, Draw.iBrush foreground, Draw.iBrush background ) =>
			throw new NotImplementedException();

		void Draw.iDrawContext.drawConsoleText( string text, int width, float fontSize, Vector2 position, Draw.iBrush foreground, Draw.iBrush background ) =>
			throw new NotImplementedException();

		void Draw.iDrawContext.fillRectangle( Rect rect, Draw.iBrush brush ) =>
			throw new NotImplementedException();

		CSize Draw.iDrawContext.measureText( string text, float width, iFont font ) =>
			throw new NotImplementedException();

		CSize Draw.iDrawContext.measureConsoleText( string text, int width, float fontSize ) =>
			throw new NotImplementedException();
	}
}