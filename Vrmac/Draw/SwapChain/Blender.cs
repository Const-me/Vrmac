using Diligent.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Vrmac.Draw.SwapChain
{
	/// <summary>Utility class that resolves MSAA 2D render target texture into single-sample 3D render target, while alpha blending 2D content over the 3D target.</summary>
	/// <remarks>
	/// <para>It renders a full-screen triangle, and runs a custom pixel shader.</para>
	/// <para>Not used anymore, the 2D content is now rendered directly to 3D target.</para>
	/// </remarks>
	sealed class Blender: IDisposable
	{
		const string resourceSubfolder = "Draw.SwapChain";
		const string textureVarName = "g_Texture";

		readonly IPipelineState pipelineState;
		public readonly byte samplesCount;

		public Blender( Context context, IRenderDevice device, byte samplesCount )
		{
			PipelineStateDesc desc = new PipelineStateDesc( false );
			desc.setBufferFormats( context );
			desc.premultipliedAlphaBlending();
			desc.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
			desc.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.None;
			desc.GraphicsPipeline.DepthStencilDesc.DepthEnable = false;

			iShaderFactory shaderFactory = device.GetShaderFactory();
			iStorageFolder assets = StorageFolder.embeddedResources( Assembly.GetExecutingAssembly(), resourceSubfolder );
			using( iPipelineStateFactory stateFactory = device.CreatePipelineStateFactory() )
			{
				stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, textureVarName );

				stateFactory.graphicsVertexShader( shaderFactory.compileShader( assets, "Blend", ShaderType.Vertex ) );

				ShaderSourceInfo ssi;
				string src;
				if( RuntimeEnvironment.operatingSystem == eOperatingSystem.Windows )
				{
					ssi = new ShaderSourceInfo( ShaderType.Pixel, ShaderSourceLanguage.Hlsl );
					src = "BlendPS.hlsl";
				}
				else
				{
					ssi = new ShaderSourceInfo( ShaderType.Pixel, ShaderSourceLanguage.Glsl );
					ssi.combinedTextureSamplers = true;
					src = "BlendPS.glsl";
				}

				string name = $"BlendPS { samplesCount }x";
				var ps = shaderFactory.compileFromFile( assets, src, ssi, name, shaderMacros( samplesCount ) );
				stateFactory.graphicsPixelShader( ps );

				stateFactory.apply( ref desc );
				pipelineState = device.CreatePipelineState( ref desc );
			}
			this.samplesCount = samplesCount;
		}

		/// <summary>For performance reasons, count of samples is specified at shader's compile time, by defining a macro.</summary>
		static IEnumerable<(string, string)> shaderMacros( byte samplesCount )
		{
			yield return ("samplesCount", samplesCount.ToString());
		}

		public void Dispose()
		{
			pipelineState?.Dispose();
		}

		public IShaderResourceBinding bindSource( ITextureView source )
		{
			var bindings = pipelineState.CreateShaderResourceBinding( true );
			using( var textureVariable = bindings.GetVariableByName( ShaderType.Pixel, textureVarName ) )
				textureVariable.Set( source );
			return bindings;
		}

		public void blend( IDeviceContext ic, IShaderResourceBinding source )
		{
			ic.SetPipelineState( pipelineState );
			ic.CommitShaderResources( source );
			ic.SetVertexBuffer( 0, null, 0 );
			DrawAttribs draw = new DrawAttribs( false )
			{
				NumVertices = 3,
				Flags = DrawFlags.VerifyAll
			};
			// ConsoleLogger.logDebug( "BlendTextures.blend, rendering" );
			ic.Draw( ref draw );
			// ConsoleLogger.logDebug( "BlendTextures.blend, rendered" );
		}
	}
}