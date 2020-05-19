using Diligent.Graphics;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vrmac;
using Vrmac.Animation;
using Vrmac.Draw;
using Vrmac.Input;
using Matrix = Vrmac.Draw.Matrix;

namespace RenderSamples
{
	class TigerSvgSample: SampleBase, iMouseWheelHandler, iMouseInput, iDeltaTimeUpdate, iKeyPressedHandler
	{
		static readonly Vector4 background = Color.parse( "#777" );
		Angle rotationAngle = new Angle();
		const float rotationSpeed = MathF.PI / 11;
		SvgImage image;
		readonly ViewboxController viewboxController;

		float boxesOpacity = 0;
		float radiansPerSecond = 0;
		// float radiansPerSecond = rotationSpeed;
		bool customClipping = false;

		const string resourceFolder = "RenderSamples/06-TigerSvg";

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			if( null == image )
				return;

			var dev = context.drawDevice;
			Rect rcImage = viewboxController.getImageBox();
			using( var dc = context.drawDevice.begin( swapChainRgb, swapChainDepthStencil, background ) )
			{
				Matrix imageRotation = rotationMatrix( dev.viewportSize, rotationAngle );
				dc.transform.push( imageRotation );

				image.render( dc, rcImage, boxesOpacity );

				if( customClipping )
				{
					dc.transform.pushIdentity();
					var brush = dev.createSolidBrush( "#F0F" );
					dc.drawRectangle( customClippingRect.viewportFromClipSpace( dev.viewportSize ), brush, 2 );
				}
			}
			printFps();
		}

		static Matrix rotationMatrix( Vector2 vps, float angle )
		{
			return Matrix.createRotation( angle, vps / 2 );
		}

		void iDeltaTimeUpdate.tick( float elapsedSeconds )
		{
			rotationAngle.rotate( radiansPerSecond, elapsedSeconds );
		}

		readonly MouseHandler mouseHandler;
		public TigerSvgSample()
		{
			mouseHandler = new MouseHandler( context );
			viewboxController = new ViewboxController( context );
		}

		readonly MouseDragEvent rightDragEvent = new MouseDragEvent( eMouseButton.Right );

		protected override void createResources( IRenderDevice rd )
		{
			var dev = context.drawDevice;

			// Parse the tiger
			iStorageFolder resources = StorageFolder.embeddedResources( Assembly.GetExecutingAssembly(), resourceFolder );
			resources.openRead( "Tiger.gz", out Stream stream );
			SvgSink builder = new SvgSink();
			using( stream )
				SvgParser.parse( stream, builder );

			image = builder.build( dev );

			mouseHandler.subscribe( this );

			mouseHandler.subscribe( rightDragEvent );
			rightDragEvent.dpiSpeedMultiplier = dev.dpiScaling.mulUnits;
			rightDragEvent.onDragDelta += onRightMouseDrag;

			if( radiansPerSecond != 0 )
				context.animation.startDelta( this );
		}

		void onRightMouseDrag( Vector2 diff )
		{
			viewboxController.pan( diff );
			context.queueRenderFrame();
		}

		void iMouseWheelHandler.wheel( CPoint point, int delta, eMouseButtonsState bs )
		{
			viewboxController.startZoom( point, delta );
		}

		Vrmac.Input.Linux.RawDevice iMouseInput.getMouseDevice() => null;

		static bool toggleFloat( ref float v, float c )
		{
			if( 0 == v )
			{
				v = c;
				return true;
			}
			v = 0;
			return false;
		}
		void iKeyPressedHandler.keyPressed( eKey key )
		{
			switch( key )
			{
				case eKey.R:
					if( toggleFloat( ref radiansPerSecond, rotationSpeed ) )
						context.animation.startDelta( this );
					else
						context.animation.cancelDelta( this );
					break;

				case eKey.B:
					toggleFloat( ref boxesOpacity, 0.75f );
					context.queueRenderFrame();
					break;

				case eKey.C:
					if( context.drawDevice is iVrmacDrawDevice vdd )
					{
						customClipping = !customClipping;
						if( customClipping )
							vdd.clippingRectangleOverride = customClippingRect;
						else
							vdd.clippingRectangleOverride = null;
						context.queueRenderFrame();
					}
					break;

				case eKey.Home:
					viewboxController.reset();
					context.queueRenderFrame();
					break;
			}
		}

		iMouseHandler iMouseInput.mouseHandler => mouseHandler;

		const float customClippingSize = 0.444f;
		static readonly Rect customClippingRect = new Rect( -customClippingSize, -customClippingSize, customClippingSize, customClippingSize );
	}
}