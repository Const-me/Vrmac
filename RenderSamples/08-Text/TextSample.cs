using Diligent.Graphics;
using System;
using System.Numerics;
using Vrmac;
using Vrmac.Animation;
using Vrmac.Draw;

namespace RenderSamples
{
	class TextSample: SampleBase, iDeltaTimeUpdate
	{
		struct sFont
		{
			readonly iFontFace face;
			readonly float sizePointe;
			public iFont font { get; private set; }
			public sFont( iFontFace face, float size )
			{
				this.face = face;
				sizePointe = size;
				font = null;
			}
			public void create( double dpi )
			{
				font = face.createFont( sizePointe, (float)dpi );
			}
		}
		sFont comicSans, defaultSerif;
		iBrush black, white, green, background;

		protected override void createResources( IRenderDevice device )
		{
			var fonts = context.drawDevice.fontCollection;
			comicSans = new sFont( fonts.comicSans( eFontStyleFlags.Normal ), 32 );
			defaultSerif = new sFont( fonts.defaultSerif( eFontStyleFlags.Normal ), 18 );

			onDpiChanged( context.dpiScalingFactor );
			context.dpiChanged.add( this, onDpiChanged );

			black = context.drawDevice.createSolidBrush( ConsoleColor.Black );
			white = context.drawDevice.createSolidBrush( ConsoleColor.White );
			green = context.drawDevice.createSolidBrush( ConsoleColor.Green );
			background = context.drawDevice.createSolidColorBrush( backgroundColor );
		}

		void onDpiChanged( double newDpi )
		{
			defaultSerif.create( newDpi );
			comicSans.create( newDpi );
		}

		static readonly Vector4 backgroundColor = Color.parse( "#CCC" );

		const string lipsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

		Rect pixelsRectangle( iDrawDevice dev, Vector2 topLeft, CSize sizePx, Vector2 padding = default)
		{
			Vector2 size = sizePx.asFloat * dev.dpiScaling.mulUnits;
			return new Rect( topLeft, topLeft + size + padding );
		}

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			iDrawDevice dev = context.drawDevice;
			Rect rect = new Rect( Vector2.Zero, dev.viewportSize );
			rect = rect.deflate( 32, 32 );

			using( var dc = dev.begin( swapChainRgb, swapChainDepthStencil, backgroundColor ) )
			{
				Matrix3x2 imageRotation = rotationMatrix( dev.viewportSize, -0.11f );
				Matrix3x2 trans = Matrix3x2.CreateTranslation( 0, -70 );
				dc.transform.push( imageRotation * trans );
				dc.drawText( "Hello World", comicSans.font, rect, black, background );
				dc.transform.pop();

				// rect.top += dev.dpiScaling.mulUnits * comicSans.font.lineHeightPixels + 12;
				rect.top += dev.dpiScaling.mulUnits * comicSans.font.lineHeightPixels + 24;

				CSize lipsumSize = dc.measureText( lipsum, rect.width, defaultSerif.font );
				dc.fillRectangle( pixelsRectangle( dev, rect.topLeft, lipsumSize ), white );
				dc.drawText( lipsum, defaultSerif.font, rect, black, white ); return;

				CSize consoleSize = dc.measureConsoleText( lipsum, 80, 14 );

				// Apparently, when FreeType measures fonts it allocates height on the top for diacritic combining characters.
				Vector2 paddingTopLeft = new Vector2( 12, 2 );
				Vector2 paddingBottomRight = new Vector2( 12, 12 );
				Vector2 size = consoleSize.asFloat * dev.dpiScaling.mulUnits;
				Rect consoleRect = new Rect( rect.topLeft, rect.topLeft + size + paddingTopLeft + paddingBottomRight );

				dc.fillRectangle( consoleRect, black );

				dc.drawConsoleText( lipsum, 80, 14, rect.topLeft + paddingTopLeft, green, black );
			}
		}

		static Matrix3x2 rotationMatrix( Vector2 vps, float angle )
		{
			return Matrix3x2.CreateRotation( angle, vps * 0.5f );
		}

		void iDeltaTimeUpdate.tick( float elapsedSeconds )
		{

		}
	}
}