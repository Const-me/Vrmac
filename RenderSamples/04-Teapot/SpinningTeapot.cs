using Diligent.Graphics;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Vrmac;
using Vrmac.Input;
using Vrmac.Utils;

namespace RenderSamples
{
	class SpinningTeapot: SampleBase, iKeyPressedHandler, iMouseInput, iSceneAsyncInit
	{
		TeapotResources resources;
		TeapotMotion motion;
		Matrix4x4 teapotWorld = Matrix4x4.Identity;
		Matrix4x4 worldView;
		readonly MouseHandler mouseHandler;

		public SpinningTeapot()
		{
			mouseHandler = new MouseHandler( context );
		}

		protected override void createResources( IRenderDevice device )
		{
			resources = new TeapotResources( context, device );

			// context.mouseCursor = eCursor.Arrow;
			context.mouseCursor = eCursor.Working;

			mouseHandler.subscribe( this );
		}

		async Task iSceneAsyncInit.createResourcesAsync( Context context )
		{
			using( var device = context.device )
				await resources.createAsync( device );
			motion = new TeapotMotion( context, mouseHandler );
			teapotWorld = resources.meshBoundingBox.transformToUnitCube();
			mouseHandler.subscribe( motion );
		}

		static readonly Vector4 clearColor = Color.parse( "#0b0030" );

		void renderScene( IDeviceContext ic, ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			// ConsoleLogger.logDebug( "SpinningTeapot.render" );

			ic.SetRenderTarget( swapChainRgb, swapChainDepthStencil, ResourceStateTransitionMode.Transition );

			// Clear the back buffer
			ic.ClearRenderTarget( swapChainRgb, clearColor );
			ic.ClearDepthStencil( swapChainDepthStencil, ClearDepthStencilFlags.DepthFlag, 1.0f, 0, ResourceStateTransitionMode.Transition );

			if( resources.haveMesh )
			{
				Matrix4x4 world = teapotWorld * Matrix4x4.CreateFromQuaternion( extraRotation * motion.rotation );
				Matrix4x4 view = Matrix4x4.CreateTranslation( 0, 0, 5 );
				Vector3 cameraPos = new Vector3( 0, -3, 0 );
				view = DiligentMatrices.createLookAt( cameraPos, Vector3.Zero, Vector3.UnitZ );
				worldView = world * view;

				float NearPlane = 0.1f;
				float FarPlane = 100;
				// Projection matrix differs between DX and OpenGL
				Matrix4x4 projection = DiligentMatrices.createPerspectiveFieldOfView( 0.25f * MathF.PI * motion.zoomFactor, context.aspectRatio, NearPlane, FarPlane, isOpenGlDevice );

				resources.draw( ic, ref worldView, ref projection );
			}
		}

		readonly Quaternion extraRotation = Quaternion.CreateFromYawPitchRoll( 0, MathF.PI * -0.1f, 0 );

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			printFps();
			renderScene( context.context, swapChainRgb, swapChainDepthStencil );
		}

		void iKeyPressedHandler.keyPressed( eKey key, eKeyboardState keyboardState )
		{
			switch( key )
			{
				case eKey.Space:
					motion?.toggleAnimation();
					break;
				case eKey.Enter:
					motion?.toggleForce();
					break;
				case eKey.PageUp:
					motion?.increaseForce();
					break;
				case eKey.PageDown:
					motion?.decreaseForce();
					break;
			}
		}

		Vrmac.Input.Linux.RawDevice iMouseInput.getMouseDevice() => null;
		iMouseHandler iMouseInput.mouseHandler => mouseHandler;
	}
}