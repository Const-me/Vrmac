using Diligent.Graphics;
using VrmacVideo;
using VrmacVideo.Containers.MP4;

namespace Vrmac.MediaEngine
{
	/// <summary>3D GPU pipeline state that renders NV12 frames into RGB texture exposed by the media engine.</summary>
	sealed class Nv12State
	{
		static readonly string resourceFolder = typeof( Nv12State ).Namespace;
		const string varTexture = "nv12";

		readonly IPipelineState pipelineState;
		readonly IShaderResourceBinding binding;
		readonly IShaderResourceVariable textureVariable;
		readonly ITextureView[] sourceTextures;
		readonly Viewport viewport;

		static SamplerDesc samplerDesc()
		{
			var s = new SamplerDesc( false );
			s.MinFilter = s.MagFilter = s.MipFilter = FilterType.Point;
			return s;
		}

		public Nv12State( IRenderDevice device, TextureFormat format, Nv12Texture[] textures, ref sDecodedVideoSize videoSize )
		{
			// Create the pipeline state
			var pso = new PipelineStateDesc( false );
			pso.GraphicsPipeline.DepthStencilDesc.DepthEnable = false;
			pso.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.None;
			pso.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
			pso.GraphicsPipeline.NumRenderTargets = 1;
			pso.GraphicsPipeline.setRTVFormat( 0, format );
			pso.ResourceLayout.DefaultVariableType = ShaderResourceVariableType.Static;

			var compiler = device.GetShaderFactory();
			iStorageFolder assets = StorageFolder.embeddedResources( System.Reflection.Assembly.GetExecutingAssembly(), resourceFolder );
			using( var psf = device.CreatePipelineStateFactory() )
			{
				psf.setName( "Video PSO" );

				using( var vs = compiler.compileHlslFile( assets, "VideoVS.hlsl", ShaderType.Vertex ) )
					psf.graphicsVertexShader( vs );

				using( var ps = compilePixelShader( compiler, assets ) )
					psf.graphicsPixelShader( ps );

				psf.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Dynamic, varTexture );

				var sampler = samplerDesc();
				psf.layoutStaticSampler( ShaderType.Pixel, ref sampler, varTexture );

				psf.apply( ref pso );
				pipelineState = device.CreatePipelineState( ref pso );
			}

			// Create resource binding and cache the variable
			binding = pipelineState.CreateShaderResourceBinding( true );
			textureVariable = binding.GetVariableByName( ShaderType.Pixel, varTexture );

			// Copy views of the textures into array
			sourceTextures = new ITextureView[ textures.Length ];
			for( int i = 0; i < textures.Length; i++ )
				sourceTextures[ i ] = textures[ i ].view;

			// Create GL viewport structure with weird values for cropping the video
			viewport = new Viewport( false )
			{
				TopLeftX = -videoSize.cropRect.left,
				// TopLeftY = videoSize.cropRect.top,
				// OpenGL uses opposite Y direction there.
				TopLeftY = videoSize.cropRect.bottom - videoSize.size.cy,
				Width = videoSize.size.cx,
				Height = videoSize.size.cy,
			};
		}

		static IShader compilePixelShader( iShaderFactory compiler, iStorageFolder assets )
		{
			// return compiler.compileHlslFile( assets, "Nv12PS.hlsl", ShaderType.Pixel );
			ShaderSourceInfo sourceInfo = new ShaderSourceInfo( ShaderType.Pixel, ShaderSourceLanguage.Glsl );
			sourceInfo.combinedTextureSamplers = true;
			return compiler.compileFromFile( assets, "Nv12PS.glsl", sourceInfo );
		}

		public void render( IDeviceContext context, ITextureView rtv, CSize outputSize, int index )
		{
			// Setup stuff
			context.SetRenderTarget( rtv, null );
			context.setViewport( outputSize, viewport );
			context.SetPipelineState( pipelineState );
			textureVariable.Set( sourceTextures[ index ] );
			context.CommitShaderResources( binding );

			// Render a full-screen triangle, no vertex buffer needed
			DrawAttribs draw = new DrawAttribs( false )
			{
				NumVertices = 3,
				Flags = DrawFlags.VerifyAll
			};
			context.Draw( ref draw );
		}
	}
}