using Diligent.Graphics;
using System.Numerics;
using VrmacVideo;
using static Vrmac.MediaEngine.Render.VertexBuffer;

namespace Vrmac.MediaEngine.Render
{
	abstract class RenderBase: iVideoRenderState
	{
		const string resourceFolder = "Vrmac.MediaEngine.Render";
		const string varTexture = "videoTexture";

		sDecodedVideoSize videoSize;
		IBuffer vertexBuffer;

		readonly IPipelineState pipelineState;
		readonly IShaderResourceBinding binding;
		readonly IShaderResourceVariable textureVariable;

		public RenderBase( IRenderDevice device, CSize renderTargetSize, SwapChainFormats formats, Vector4 borderColor, sDecodedVideoSize videoSize )
		{
			this.videoSize = videoSize;
			// Create vertex buffer
			vertexBuffer = createVideoVertexBuffer( device, renderTargetSize, ref videoSize );

			// Create pipeline state
			var pso = new PipelineStateDesc( false );
			pso.GraphicsPipeline.DepthStencilDesc.DepthEnable = false;
			pso.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
			pso.GraphicsPipeline.NumRenderTargets = 1;
			pso.GraphicsPipeline.setRTVFormat( 0, formats.color );
			pso.GraphicsPipeline.DSVFormat = formats.depth;
			pso.ResourceLayout.DefaultVariableType = ShaderResourceVariableType.Static;

			var compiler = device.GetShaderFactory();
			iStorageFolder assets = StorageFolder.embeddedResources( System.Reflection.Assembly.GetExecutingAssembly(), resourceFolder );
			using( var psf = device.CreatePipelineStateFactory() )
			{
				psf.setName( "Video PSO" );
				setupVideoInputLayout( psf );

				using( var vs = compiler.compileHlslFile( assets, "VideoVS.hlsl", ShaderType.Vertex ) )
					psf.graphicsVertexShader( vs );

				(string uvMin, string uvMax) = videoUvCroppedRect( ref videoSize );
				string colorString = Utils.printFloat4( borderColor );
				using( var ps = compilePixelShader( compiler, assets, uvMin, uvMax, colorString ) )
					psf.graphicsPixelShader( ps );

				psf.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Dynamic, varTexture );

				var sampler = new SamplerDesc( false )
				{
					MipFilter = FilterType.Point,
				};
				psf.layoutStaticSampler( ShaderType.Pixel, ref sampler, varTexture );

				psf.apply( ref pso );
				pipelineState = device.CreatePipelineState( ref pso );
			}

			// Create resource binding and cache the variable, we gonna need both on every frame rendered
			binding = pipelineState.CreateShaderResourceBinding( true );
			textureVariable = binding.GetVariableByName( ShaderType.Pixel, varTexture );
		}

		protected void drawTriangle( IDeviceContext ic, ITextureView textureView )
		{
			ic.SetPipelineState( pipelineState );
			textureVariable.Set( textureView );
			ic.CommitShaderResources( binding );
			ic.setVertexBuffer( vertexBuffer );
			DrawAttribs draw = new DrawAttribs( false )
			{
				NumVertices = 3,
				Flags = DrawFlags.VerifyAll
			};
			ic.Draw( ref draw );
		}

		public abstract void render( IDeviceContext ic );
		public abstract void renderLastFrame( IDeviceContext ic );

		void iVideoRenderState.resize( IRenderDevice device, CSize newSize )
		{
			ComUtils.clear( ref vertexBuffer );
			vertexBuffer = createVideoVertexBuffer( device, newSize, ref videoSize );
		}

		public void Dispose()
		{
			textureVariable?.Dispose();
			binding?.Dispose();
			pipelineState?.Dispose();
			ComUtils.clear( ref vertexBuffer );
		}

		protected abstract IShader compilePixelShader( iShaderFactory compiler, iStorageFolder assets,
			string uvMin, string uvMax, string colorString );
	}
}