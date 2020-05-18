using Diligent.Graphics;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw.Shaders
{
	sealed class StatesCache: IDisposable
	{
		static readonly string resourceFolder = typeof( StatesCache ).Namespace;

		readonly GpuResources resources;
		// Arrays are much faster to lookup than hash maps. The bits of the index are enum values of eShaderMacros enum.
		readonly VrmacStateBase[] cache = new VrmacStateBase[ 16 ];

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static int cacheIndex( eShaderMacros macros )
		{
			return (int)macros;
		}

		public StatesCache( GpuResources resources )
		{
			this.resources = resources;
		}

		public void Dispose()
		{
			for( int i = 0; i < cache.Length; i++ )
			{
				var s = cache[ i ];
				s?.dispose();
				cache[ i ] = null;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public VrmacStateBase getState( int drawCallsCount, bool opaquePass, eShaderMacros drawFeatures )
		{
			eShaderMacros macros = drawFeatures;
			if( drawCallsCount <= FewDrawCallsState.smallCount )
				macros |= eShaderMacros.FewDrawCalls;
			if( opaquePass )
				macros |= eShaderMacros.OpaquePass;

			int idx = cacheIndex( macros );
			VrmacStateBase state = cache[ idx ];
			if( null != state )
				return state;
			state = compileState( macros );
			cache[ idx ] = state;
			return state;
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		VrmacStateBase compileState( eShaderMacros macros )
		{
			using( var dev = resources.context.renderContext.device )
			using( var stateFactory = dev.CreatePipelineStateFactory() )
			{
				PipelineStateDesc desc = createStateDesc();
				setupIdLayout( stateFactory );
				compileShaders( dev, stateFactory, "draw", macros );

				if( macros.HasFlag( eShaderMacros.FewDrawCalls ) )
					return new FewDrawCallsState( resources, dev, ref desc, stateFactory, macros );

				return new MoreDrawCallsState( resources, dev, ref desc, stateFactory, macros );
			}
		}

		// Create PipelineStateDesc with some essential setup
		PipelineStateDesc createStateDesc()
		{
			PipelineStateDesc desc = new PipelineStateDesc( false );
			desc.GraphicsPipeline.NumRenderTargets = 1;
			desc.setBufferFormats( resources.context );

			desc.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
			desc.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.None;
			desc.GraphicsPipeline.DepthStencilDesc.DepthEnable = true;

			desc.ResourceLayout.DefaultVariableType = ShaderResourceVariableType.Static;
			return desc;
		}

		// Setup VS input layout for sVertexWithId structure: float2 position, uint id
		static void setupIdLayout( iPipelineStateFactory stateFactory )
		{
			LayoutElement elt = new LayoutElement( false )
			{
				InputIndex = 0,
				BufferSlot = 0,
				NumComponents = 2,
				ValueType = GpuValueType.Float32,
				IsNormalized = false
			};
			stateFactory.graphicsLayoutElement( elt );

			elt.InputIndex = 1;
			elt.NumComponents = 1;
			elt.ValueType = GpuValueType.Uint32;
			stateFactory.graphicsLayoutElement( elt );
		}

		static void compileShaders( IRenderDevice device, iPipelineStateFactory stateFactory, string name, eShaderMacros macros )
		{
			var compiler = device.GetShaderFactory();
			iStorageFolder src = StorageFolder.embeddedResources( System.Reflection.Assembly.GetExecutingAssembly(), resourceFolder );

			var defines = macros.defines().ToArray();

			stateFactory.graphicsVertexShader( compiler.compileShader( src, name, ShaderType.Vertex, defines ) );
			stateFactory.graphicsPixelShader( compiler.compileShader( src, name, ShaderType.Pixel, defines ) );
		}
	}
}