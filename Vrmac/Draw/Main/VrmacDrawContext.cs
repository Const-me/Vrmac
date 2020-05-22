using Diligent.Graphics;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vrmac.Draw.Text;

namespace Vrmac.Draw.Main
{
	sealed class VrmacDrawContext: ImmediateContext, iImmediateDrawContext
	{
		internal readonly Context context;
		readonly DrawDevice device;
		iDrawDevice iDrawContext.device => device;

		public VrmacDrawContext( Context context, DrawDevice device, iVrmacDraw factory ) :
			base( context, factory, device.paletteTexture )
		{
			this.context = context;
			this.device = device;
		}

		public static sStrokeStyle defaultStrokeStyle()
		{
			return new sStrokeStyle()
			{
				startCap = eLineCap.Flat,
				endCap = eLineCap.Flat,
				lineJoin = eLineJoin.Bevel,
			};
		}

		Matrix3x2 rootTransform;

		ITextureView rgbTarget;
		bool cleared;

		public iImmediateDrawContext begin( ref Matrix3x2 rootTransform, SwapChainFormats swapFormat, ITextureView rgbTarget, bool opaqueColor )
		{
			this.rgbTarget = rgbTarget;
			this.rootTransform = rootTransform;
			transform.clear();
			cleared = opaqueColor;
			return this;
		}

		public void Dispose()
		{
			flush();
			device.present( cleared );
			rgbTarget = null;
		}
		void IDisposable.Dispose() => Dispose();

		void iImmediateDrawContext.flush() => base.flush();

		void iDrawContext.fillGeometry( iGeometry geometry, iBrush brush )
		{
			switch( brush )
			{
				case SolidColorBrush solidColor:
					var cd = solidColor.data;
					fillGeometry( (iPathGeometry)geometry, cd, true );
					return;
			}
			throw new NotImplementedException();
		}

		void iDrawContext.drawGeometry( iGeometry geometry, iBrush brush, float width, iStrokeStyle strokeStyle )
		{
			switch( brush )
			{
				case SolidColorBrush solidColor:
					var cd = solidColor.data;
					sStrokeStyle ss = strokeStyle?.strokeStyle ?? defaultStrokeStyle();
					strokeGeometry( (iPathGeometry)geometry, cd, null, width, ref ss );
					return;
			}
			throw new NotImplementedException();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		float computePixelSize( ref Matrix3x2 curr )
		{
			float scaling = curr.getScaling().maxCoordinate();
			return device.dpiScaling.mulUnits / scaling;
		}

		protected override void getCurrentTransform( out Matrix3x2 matrix, out float pixel )
		{
			Matrix3x2 curr = transform.current;
			matrix = curr * rootTransform;
			pixel = computePixelSize( ref curr );
		}

		protected override CPoint transformToPhysicalPixels( Vector2 point, out IntMatrix? intMatrix )
		{
			Matrix3x2 tform = transform.current;
			point = Vector2.Transform( point, tform );
			point *= device.dpiScaling.mulPixels;
			intMatrix = tform.snapMatrixToInt();
			return point.roundToInt();
		}

		protected override Vector2 getUserScaling()
		{
			return transform.current.getScaling();
		}

		void iDrawContext.drawRectangle( Rect rect, iBrush brush, float width ) =>
			drawRectangle( ref rect, width, brush.data().paletteIndex );

		void iDrawContext.fillRectangle( Rect rect, iBrush brush ) =>
			fillRectangle( ref rect, brush.data() );

		void iDrawContext.fillAndStroke( iGeometry geometry, iBrush fill, iBrush stroke, float strokeWidth, iStrokeStyle strokeStyle )
		{
			sStrokeStyle ss = strokeStyle?.strokeStyle ?? defaultStrokeStyle();
			iPathGeometry path = (iPathGeometry)geometry;
			var palette = device.paletteTexture;
			// Disabling VAA of filled + stroked meshes for the filled shape. These 2 layers of transparency, one from the mesh below another from the stroke above, aren't doing much good.
			// This also saves non-trivial count of triangles, as VAA filled meshes have 3-4 times more triangles than non-VAA meshes of the same geometry.
			fillGeometry( path, fill.data(), false );
			strokeGeometry( path, stroke.data(), fill.data(), strokeWidth, ref ss, 1 );
		}

		void iDrawContext.drawSprite( Rect rect, int spriteIndex )
		{
			TextureAtlas atlas = device.textureAtlas;
			if( null == atlas )
				throw new ApplicationException( "Load sprites into the atlas first" );
			atlas.update();
			var uv = atlas[ spriteIndex ];
			drawSprite( ref rect, ref uv );
		}

		CSize iDrawContext.measureText( string text, float width, iFont fontInterface )
		{
			Matrix3x2 curr = transform.current;
			float pixel = computePixelSize( ref curr );
			int widthPIxels = (int)MathF.Round( width / pixel );

			Matrix3x2 tform = transform.current;
			eTextRendering renderMode = textRenderingStyle( tform.snapMatrixToInt() );

			var font = (Font)fontInterface;
			return font.measureText( text, widthPIxels, renderMode );
		}

		void iDrawContext.drawText( string text, iFont font, Rect layoutRect, iBrush foreground, iBrush background )
		{
			drawText( text, (Font)font, ref layoutRect, foreground.data(), background.data() );
		}

		protected override void updatePalette()
		{
			device.paletteTexture.update();
		}

		protected override void updateFontTextures()
		{
			device.fontTextures.update();
		}

		Text.Font getMonospaceFont( float fontSize )
		{
			iFontCollection fonts = device.fontCollection;
			var fontFace = fonts.defaultMono( eFontStyleFlags.Normal );
			return (Text.Font)fontFace.createFont( fontSize, device.dpiScaling.mulPixels );
		}

		CSize iDrawContext.measureConsoleText( string text, int widthChars, float fontSize )
		{
			var font = getMonospaceFont( fontSize );
			return font.measureConsoleText( text, widthChars );
		}

		void iDrawContext.drawConsoleText( string text, int width, float fontSize, Vector2 position, iBrush foreground, iBrush background )
		{
			var font = getMonospaceFont( fontSize );
			drawConsoleText( text, width, font, position, foreground.data(), background.data() );
		}

		readonly MatrixStack transform = new MatrixStack();
		MatrixStack iDrawContext.transform => transform;
	}
}