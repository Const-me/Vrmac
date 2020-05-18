using Diligent.Graphics;

namespace Vrmac.Utils.Cursor.Render
{
	// Utility functions for cursors pipeline setup and rendering
	static class Utils
	{
		public static PipelineStateDesc createStateDesc( Context context )
		{
			PipelineStateDesc desc = new PipelineStateDesc( false );
			desc.setBufferFormats( context );

			desc.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
			desc.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.Back;
			desc.GraphicsPipeline.DepthStencilDesc.DepthEnable = false;
			return desc;
		}

		public static void initState( iPipelineStateFactory stateFactory, iShaderFactory shaderFactory, iStorageFolder assets, IShader vs, string ps, string name )
		{
			stateFactory.clear();
			stateFactory.setName( name + " PSO" );

			// VS input layout
			LayoutElement elt = new LayoutElement( false )
			{
				InputIndex = 0,
				BufferSlot = 0,
				NumComponents = 2,
				ValueType = GpuValueType.Float32,
				IsNormalized = false
			};
			stateFactory.graphicsLayoutElement( elt );

			// Shaders
			stateFactory.graphicsVertexShader( vs );
			using( var shader = shaderFactory.compileHlslFile( assets, ps, ShaderType.Pixel, name + " PS" ) )
				stateFactory.graphicsPixelShader( shader );

			// Mutable texture variable
			stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, "g_Texture" );
			// Static sampler
			SamplerDesc samplerDesc = new SamplerDesc( false )
			{
				MinFilter = FilterType.Point,
				MagFilter = FilterType.Point,
				MipFilter = FilterType.Point,
			};
			stateFactory.layoutStaticSampler( ShaderType.Pixel, ref samplerDesc, "g_Texture" );
		}

		public static void initState( iPipelineStateFactory stateFactory, iShaderFactory shaderFactory, iStorageFolder assets, string vs, string ps, string name )
		{
			using( var shader = shaderFactory.compileHlslFile( assets, vs, ShaderType.Vertex, name + " VS" ) )
				initState( stateFactory, shaderFactory, assets, shader, ps, name );
		}

		public static void renderQuad( IDeviceContext context, IBuffer vertexBuffer )
		{
			context.SetVertexBuffer( 0, vertexBuffer, 0 );
			DrawAttribs draw = new DrawAttribs( false )
			{
				NumVertices = 4,
				Flags = DrawFlags.VerifyAll
			};
			context.Draw( ref draw );
		}
	}
}