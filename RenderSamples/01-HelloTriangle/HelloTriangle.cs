using Diligent.Graphics;
using Vrmac;

namespace RenderSamples
{
	class HelloTriangle: SampleBase
	{
		IPipelineState pipelineState;

		protected override void createResources( IRenderDevice device )
		{
			// Diligent Engine can use HLSL source on all supported platforms.
			// It will convert HLSL to GLSL in OpenGL mode, while Vulkan backend will compile it directly to SPIRV.
			string VSSource = @"
struct PSInput 
{ 
    float4 Pos   : SV_POSITION; 
    float3 Color : COLOR; 
};

void main( in uint VertId : SV_VertexID, out PSInput PSIn ) 
{
    float4 Pos[3];
    Pos[0] = float4(-0.5, -0.5, 0.0, 1.0);
    Pos[1] = float4( 0.0, +0.5, 0.0, 1.0);
    Pos[2] = float4(+0.5, -0.5, 0.0, 1.0);

    float3 Col[3];
    Col[0] = float3(1.0, 0.0, 0.0); // red
    Col[1] = float3(0.0, 1.0, 0.0); // green
    Col[2] = float3(0.0, 0.0, 1.0); // blue

    PSIn.Pos   = Pos[VertId];
    PSIn.Color = Col[VertId];
}";

			string PSSource = @"
struct PSInput 
{ 
    float4 Pos   : SV_POSITION; 
    float3 Color : COLOR; 
};

struct PSOutput
{ 
    float4 Color : SV_TARGET; 
};

void main( in PSInput PSIn, out PSOutput PSOut )
{
    PSOut.Color = float4(PSIn.Color.rgb, 1.0);
}
";
			PipelineStateDesc PSODesc = new PipelineStateDesc( false );

			PSODesc.setBufferFormats( context );

			// Primitive topology defines what kind of primitives will be rendered by this pipeline state
			PSODesc.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
			// No back face culling for this tutorial
			PSODesc.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.None;
			// Disable depth testing
			PSODesc.GraphicsPipeline.DepthStencilDesc.DepthEnable = false;

			iShaderFactory shaderFactory = device.GetShaderFactory();

			// We won't be using the device object after this, `using` is to release the COM interface once finished
			using( iPipelineStateFactory stateFactory = device.CreatePipelineStateFactory() )
			{
				// Compile the two shaders
				ShaderSourceInfo sourceInfo = new ShaderSourceInfo( ShaderType.Vertex, ShaderSourceLanguage.Hlsl );
				sourceInfo.combinedTextureSamplers = true;  // This appears to be the requirement of OpenGL backend.

				using( var vs = shaderFactory.compileFromSource( VSSource, sourceInfo ) )
					stateFactory.graphicsVertexShader( vs );

				sourceInfo.shaderType = ShaderType.Pixel;
				using( var ps = shaderFactory.compileFromSource( PSSource, sourceInfo ) )
					stateFactory.graphicsPixelShader( ps );

				stateFactory.apply( ref PSODesc );

				pipelineState = device.CreatePipelineState( ref PSODesc );
			}
		}

		static readonly Vector4 clearColor = Color.parse( "#ccc" );

		protected override void render( ITextureView swapChainRgb, ITextureView swapChainDepthStencil )
		{
			var ic = context.context;

			ic.SetRenderTarget( swapChainRgb, swapChainDepthStencil );

			// Clear the back buffer
			float[] ClearColor = new float[ 4 ] { 0.350f, 0.350f, 0.350f, 1.0f };
			// Let the engine perform required state transitions
			ic.ClearRenderTarget( swapChainRgb, clearColor );
			ic.ClearDepthStencil( swapChainDepthStencil, ClearDepthStencilFlags.DepthFlag, 1.0f, 0 );

			// Set the pipeline state in the immediate context
			ic.SetPipelineState( pipelineState );
			ic.CommitShaderResources( null );

			DrawAttribs drawAttrs = new DrawAttribs( true );
			drawAttrs.NumVertices = 3; // We will render 3 vertices
			ic.Draw( ref drawAttrs );
		}
	}
}