using Diligent.Graphics;
using System;
using System.Numerics;
using static Vrmac.Utils.Cursor.Render.Utils;

namespace Vrmac.Utils.Cursor.Render
{
	struct MonoCursor: iCursorRender
	{
		readonly IBuffer constantBuffer;

		readonly IPipelineState psoInvert;
		readonly IShaderResourceBinding bindingsInvert;
		readonly IShaderResourceVariable textureVariableInvert;

		readonly IPipelineState psoRgb;
		readonly IShaderResourceBinding bindingsRgb;
		readonly IShaderResourceVariable textureVariableRgb;

		Vector4 position;

		void IDisposable.Dispose()
		{
			textureVariableRgb?.Dispose();
			bindingsRgb?.Dispose();
			psoRgb?.Dispose();

			textureVariableInvert?.Dispose();
			bindingsInvert?.Dispose();
			psoInvert?.Dispose();

			constantBuffer?.Dispose();
		}

		public MonoCursor( Context context, IRenderDevice device, iPipelineStateFactory stateFactory, iShaderFactory shaderFactory, iStorageFolder assets, IShader vs )
		{
			constantBuffer = device.CreateDynamicUniformBuffer<Vector4>( "Cursor CB" );

			PipelineStateDesc desc = createStateDesc( context );

			// === First pass, setup that weird blending to invert colors ===
			RenderTargetBlendDesc blendDesc = new RenderTargetBlendDesc( false )
			{
				BlendEnable = true,
				SrcBlend = BlendFactor.InvDestColor,
				DestBlend = BlendFactor.Zero,
			};
			desc.GraphicsPipeline.BlendDesc.setRenderTarget( blendDesc );
			initState( stateFactory, shaderFactory, assets, vs, "CursorMaskPS.hlsl", "Invert bits" );

			stateFactory.apply( ref desc );
			psoInvert = device.CreatePipelineState( ref desc );
			psoInvert.GetStaticVariableByName( ShaderType.Vertex, "CursorCB" ).Set( constantBuffer );

			bindingsInvert = psoInvert.CreateShaderResourceBinding( true );
			textureVariableInvert = bindingsInvert.GetVariableByName( ShaderType.Pixel, "g_Texture" );

			// === Second pass, normal alpha blending ===
			desc.premultipliedAlphaBlending();
			initState( stateFactory, shaderFactory, assets, vs, "CursorColorPS.hlsl", "Monochrome cursor" );

			stateFactory.apply( ref desc );
			psoRgb = device.CreatePipelineState( ref desc );
			psoRgb.GetStaticVariableByName( ShaderType.Vertex, "CursorCB" ).Set( constantBuffer );

			bindingsRgb = psoRgb.CreateShaderResourceBinding( true );
			textureVariableRgb = bindingsRgb.GetVariableByName( ShaderType.Pixel, "g_Texture" );

			position = default;
		}

		public void updateTexture( MonochromeCursorTexture texture )
		{
			textureVariableRgb.Set( texture.texture );
			textureVariableInvert.Set( texture.texture );
		}

		void iCursorRender.updatePosition( ref Vector4 positionAndSize )
		{
			position = positionAndSize;
		}

		void iCursorRender.render( IDeviceContext context, IBuffer vertexBuffer )
		{
			context.writeBuffer( constantBuffer, ref position );
			context.SetVertexBuffer( 0, vertexBuffer, 0 );
			DrawAttribs draw = new DrawAttribs( false )
			{
				NumVertices = 4,
				Flags = DrawFlags.VerifyAll
			};

			context.SetPipelineState( psoInvert );
			context.CommitShaderResources( bindingsInvert );
			context.Draw( ref draw );

			context.SetPipelineState( psoRgb );
			context.CommitShaderResources( bindingsRgb );
			context.Draw( ref draw );
		}
	}
}