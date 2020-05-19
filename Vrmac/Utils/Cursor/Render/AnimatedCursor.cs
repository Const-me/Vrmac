using Diligent.Graphics;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac.Animation;
using static Vrmac.Utils.Cursor.Render.Utils;

namespace Vrmac.Utils.Cursor.Render
{
	struct AnimatedCursor: iCursorRender
	{
		const eAnimationTimer animationTimer = eAnimationTimer.RelativeTime;

		readonly IBuffer constantBuffer;
		readonly IPipelineState pipelineState;
		readonly IShaderResourceBinding bindings;
		readonly IShaderResourceVariable textureVariable;

		[StructLayout( LayoutKind.Sequential )]
		struct AnimatedConstantBuffer
		{
			public Vector4 position;
			public int frame;
			uint padding1;
			ulong padding2;
		}
		AnimatedConstantBuffer animatedConstants;

		readonly Timers timers;

		TimeSpan lastFrameSwitch, lastRendered, frameDuration;
		int animationFrames;

		public AnimatedCursor( Context context, IRenderDevice device, iPipelineStateFactory stateFactory, iShaderFactory shaderFactory, iStorageFolder assets )
		{
			constantBuffer = device.CreateDynamicUniformBuffer<AnimatedConstantBuffer>( "Animated cursor CB" );

			PipelineStateDesc desc = createStateDesc( context );
			desc.premultipliedBlendingMaxAlpha();

			initState( stateFactory, shaderFactory, assets, "AniCursorVS.hlsl", "AniCursorPS.hlsl", "Animated cursor" );

			stateFactory.apply( ref desc );
			pipelineState = device.CreatePipelineState( ref desc );

			pipelineState.GetStaticVariableByName( ShaderType.Vertex, "CursorCB" ).Set( constantBuffer );
			pipelineState.GetStaticVariableByName( ShaderType.Pixel, "CursorCB" ).Set( constantBuffer );

			bindings = pipelineState.CreateShaderResourceBinding( true );
			textureVariable = bindings.GetVariableByName( ShaderType.Pixel, "g_Texture" );

			animatedConstants = default;
			timers = context.animation.timers;
			lastFrameSwitch = timers[ animationTimer ];
			lastRendered = lastFrameSwitch;
			frameDuration = default;
			animationFrames = 0;
		}

		void IDisposable.Dispose()
		{
			textureVariable?.Dispose();
			bindings?.Dispose();
			pipelineState?.Dispose();
			constantBuffer?.Dispose();
		}

		void iCursorRender.updatePosition( ref Vector4 positionAndSize )
		{
			animatedConstants.position = positionAndSize;
		}

		public void updateTexture( AnimatedCursorTexture texture )
		{
			textureVariable.Set( texture.texture );

			TimeSpan now = timers[ animationTimer ];
			lastFrameSwitch = lastRendered = now;
			animatedConstants.frame = 0;
			frameDuration = texture.frameDuration;
			animationFrames = texture.animationFrames;
		}

		void iCursorRender.render( IDeviceContext context, IBuffer vertexBuffer )
		{
			advanceAnimation();

			context.writeBuffer( constantBuffer, ref animatedConstants );
			context.SetPipelineState( pipelineState );
			context.CommitShaderResources( bindings );
			renderQuad( context, vertexBuffer );
		}

		void advanceAnimation()
		{
			TimeSpan now = timers[ animationTimer ];
			TimeSpan frameVisible = now - lastFrameSwitch;
			TimeSpan halfFrame = TimeSpan.FromTicks( ( now.Ticks - lastRendered.Ticks ) / 2 );
			if( frameVisible + halfFrame >= frameDuration )
			{
				animatedConstants.frame++;
				animatedConstants.frame = animatedConstants.frame % animationFrames;
				lastFrameSwitch = now;
			}
			lastRendered = now;
		}
	}
}