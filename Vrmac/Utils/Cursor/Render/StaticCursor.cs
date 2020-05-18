using Diligent.Graphics;
using System;
using static Vrmac.Utils.Cursor.Render.Utils;

namespace Vrmac.Utils.Cursor.Render
{
	struct StaticCursor: iCursorRender
	{
		readonly IBuffer constantBuffer;
		readonly IPipelineState pipelineState;
		readonly IShaderResourceBinding bindings;
		readonly IShaderResourceVariable textureVariable;
		Vector4 position;

		void IDisposable.Dispose()
		{
			textureVariable?.Dispose();
			bindings?.Dispose();
			pipelineState?.Dispose();
			constantBuffer?.Dispose();
		}

		public StaticCursor( Context context, IRenderDevice device, iPipelineStateFactory stateFactory, iShaderFactory shaderFactory, iStorageFolder assets, IShader vs )
		{
			constantBuffer = device.CreateDynamicUniformBuffer<Vector4>( "Cursor CB" );

			PipelineStateDesc desc = createStateDesc( context );
			desc.premultipliedAlphaBlending();

			initState( stateFactory, shaderFactory, assets, vs, "CursorPS.hlsl", "Cursor" );

			stateFactory.apply( ref desc );
			pipelineState = device.CreatePipelineState( ref desc );
			pipelineState.GetStaticVariableByName( ShaderType.Vertex, "CursorCB" ).Set( constantBuffer );

			bindings = pipelineState.CreateShaderResourceBinding( true );
			textureVariable = bindings.GetVariableByName( ShaderType.Pixel, "g_Texture" );

			position = default;
		}

		public void updateTexture( StaticCursorTexture texture )
		{
			textureVariable.Set( texture.texture );
		}

		void iCursorRender.updatePosition( ref Vector4 positionAndSize )
		{
			position = positionAndSize;
		}

		void iCursorRender.render( IDeviceContext context, IBuffer vertexBuffer )
		{
			context.writeBuffer( constantBuffer, ref position );
			context.SetPipelineState( pipelineState );
			context.CommitShaderResources( bindings );
			renderQuad( context, vertexBuffer );
		}
	}
}