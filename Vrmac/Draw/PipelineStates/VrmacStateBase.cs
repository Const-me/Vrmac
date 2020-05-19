using Diligent.Graphics;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vrmac.Draw.Main;

namespace Vrmac.Draw.Shaders
{
	abstract class VrmacStateBase
	{
		public abstract void uploadDrawCalls( IDeviceContext ic, Span<sDrawCall> drawCalls, iDepthValues depthValues, ref DrawMeshes meshes );

		public static readonly int drawCallSize = Marshal.SizeOf<sDrawCallData>();

		const string varStaticConstantsBuffer = "StaticConstantsBuffer";

		const string varSpriteAtlas = "textureAtlas";
		const string varGrayscaleFontAtlas = "grayscaleFontAtlas";
		const string varCleartypeFontAtlas = "cleartypeFontAtlas";
		const string varPaletteTexture = "paletteTexture";

		readonly GpuResources resources;
		protected readonly eShaderMacros shaderMacros;

		protected readonly IPipelineState pipelineState;
		IShaderResourceBinding resourceBinding;

		public VrmacStateBase( GpuResources resources, IRenderDevice device, ref PipelineStateDesc desc, iPipelineStateFactory stateFactory, eShaderMacros macros )
		{
			this.resources = resources;
			shaderMacros = macros;

			layoutUniforms( stateFactory );
			stateFactory.setName( $"2D state { macros }" );

			stateFactory.layoutVariable( ShaderType.Vertex, ShaderResourceVariableType.Mutable, varPaletteTexture );
			stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, varPaletteTexture );

			if( macros.HasFlag( eShaderMacros.TextureAtlas ) )
			{
				stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, varSpriteAtlas );
				SamplerDesc samplerDesc = new SamplerDesc( false );
				stateFactory.layoutStaticSampler( ShaderType.Pixel, ref samplerDesc, varSpriteAtlas );

				resources.context.drawDevice.textureAtlas.resized.add( this, dropResourceBindings );
			}

			if( macros.HasFlag( eShaderMacros.TextRendering ) )
			{
				stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, varGrayscaleFontAtlas );
				SamplerDesc samplerDesc = new SamplerDesc( false );
				stateFactory.layoutStaticSampler( ShaderType.Pixel, ref samplerDesc, varGrayscaleFontAtlas );

				stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, varCleartypeFontAtlas );

				resources.initTextCBuffer();
			}

			declareResources( stateFactory );

			stateFactory.apply( ref desc );
			setupDepthState( ref desc, macros );

			pipelineState = device.CreatePipelineState( ref desc );

			resources.context.swapChainResized.add( this, ( CSize s, double d ) => dropResourceBindings() );
			resources.paletteTexture.textureResized.add( this, dropResourceBindings );
		}

		static void setupDepthState( ref PipelineStateDesc desc, eShaderMacros macros )
		{
			// Enable depth for both opaque and transparent passes..
			desc.GraphicsPipeline.DepthStencilDesc.DepthEnable = true;
			desc.GraphicsPipeline.DepthStencilDesc.DepthFunc = ComparisonFunction.Less;

			// ..and set it so the depth is written on opaque pass, but readonly on transparent one.
			// With some luck, this should allow early Z rejection on both passes, saving tremendous amount of pixel shader invocations and fillrate bandwidth.
			if( macros.HasFlag( eShaderMacros.OpaquePass ) )
			{
				desc.GraphicsPipeline.DepthStencilDesc.DepthWriteEnable = true;
			}
			else
			{
				desc.GraphicsPipeline.DepthStencilDesc.DepthWriteEnable = false;
				// desc.premultipliedAlphaBlending();

				RenderTargetBlendDesc blendDesc = new RenderTargetBlendDesc( false )
				{
					BlendEnable = true,
					SrcBlend = BlendFactor.One,
					DestBlend = BlendFactor.InvSrcAlpha,

					SrcBlendAlpha = BlendFactor.One,
					DestBlendAlpha = BlendFactor.One,
					BlendOpAlpha = BlendOperation.Max,
				};
				desc.GraphicsPipeline.BlendDesc.setRenderTarget( blendDesc );
			}
		}

		void layoutUniforms( iPipelineStateFactory stateFactory )
		{
			// See #if preprocessor expression in utils.hlsli around that buffer
			bool hasConstants = !shaderMacros.HasFlag( eShaderMacros.OpaquePass ) || shaderMacros.HasFlag( eShaderMacros.TextRendering );
			if( hasConstants )
			{
				stateFactory.layoutVariable( ShaderType.Vertex, ShaderResourceVariableType.Mutable, varStaticConstantsBuffer );
				stateFactory.layoutVariable( ShaderType.Pixel, ShaderResourceVariableType.Mutable, varStaticConstantsBuffer );
			}
		}

		void bindStaticCBuffer( IShaderResourceBinding binding )
		{
			if( shaderMacros.HasFlag( eShaderMacros.OpaquePass ) )
				return;

			using( var vv = binding.GetVariableByName( ShaderType.Vertex, varStaticConstantsBuffer ) )
				vv?.Set( resources.staticCBuffer );
			using( var vv = binding.GetVariableByName( ShaderType.Pixel, varStaticConstantsBuffer ) )
				vv?.Set( resources.staticCBuffer );
		}

		protected virtual void declareResources( iPipelineStateFactory stateFactory ) { }

		protected virtual void bindMutableResources( IShaderResourceBinding bindings ) { }

		/// <summary>Destroy the resource binding object</summary>
		/// <remarks>Called when buffers are recreated due to expansion.
		/// Also called when user resizes window, or drags the window to a monitor with different DPI, invalidating the data in StaticConstantsBuffer.</remarks>
		protected void dropResourceBindings()
		{
			ComUtils.clear( ref resourceBinding );
		}

		void bindStaticCBufferForText( IShaderResourceBinding binding )
		{
			IBuffer buffer = resources.staticCBufferForText;
			using( var vv = binding.GetVariableByName( ShaderType.Vertex, varStaticConstantsBuffer ) )
				vv?.Set( buffer );
			using( var vv = binding.GetVariableByName( ShaderType.Pixel, varStaticConstantsBuffer ) )
				vv?.Set( buffer );
		}

		void ensureResourceBindings()
		{
			if( null != resourceBinding )
				return;

			var binding = pipelineState.CreateShaderResourceBinding( true );
			// Bind mutable resources which don't depend on the draw calls count
			if( shaderMacros.HasFlag( eShaderMacros.TextRendering ) )
				bindStaticCBufferForText( binding );
			else
				bindStaticCBuffer( binding );

			var paletteVal = binding.GetVariableByName( ShaderType.Vertex, varPaletteTexture );
			paletteVal?.Set( resources.paletteTexture.textureView );
			paletteVal = binding.GetVariableByName( ShaderType.Pixel, varPaletteTexture );
			paletteVal?.Set( resources.paletteTexture.textureView );

			if( shaderMacros.HasFlag( eShaderMacros.TextureAtlas ) )
			{
				var spriteVar = binding.GetVariableByName( ShaderType.Pixel, varSpriteAtlas );
				using( var view = resources.context.drawDevice.textureAtlas.nativeTexture )
					spriteVar.Set( view );
			}

			if( shaderMacros.HasFlag( eShaderMacros.TextRendering ) )
			{
				var atlasVar = binding.GetVariableByName( ShaderType.Pixel, varGrayscaleFontAtlas );
				using( var view = resources.grayscaleFontAtlas() )
					atlasVar?.Set( view );

				atlasVar = binding.GetVariableByName( ShaderType.Pixel, varCleartypeFontAtlas );
				using( var view = resources.clearTypeFontAtlas() )
					atlasVar?.Set( view );
			}

			// Bind extra mutable resources which depend on specific derived class
			bindMutableResources( binding );

			// Finally, cache the new resource bindings.
			// Hopefully we won't need to re-bind these things too often, buffers and texture atlases all grow exponentially.
			resourceBinding = binding;
		}

		public void bind( IDeviceContext ic )
		{
			ensureResourceBindings();
			ic.SetPipelineState( pipelineState );
			ic.CommitShaderResources( resourceBinding );
		}

		public virtual void dispose()
		{
			dropResourceBindings();
			pipelineState?.Dispose();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		protected void produceDrawCalls( Span<sDrawCallData> result, Span<sDrawCall> drawCalls, iDepthValues depthValues, ref DrawMeshes meshes )
		{
			int count = drawCalls.Length;
			for( int i = 0; i < count; i++ )
			{
				ref sDrawCall src = ref drawCalls[ i ];
				ref sDrawCallData dest = ref result[ i ];
				ref Matrix3x2 m = ref src.transform;
				// Rotation portion of the matrix
				dest.rotation = m.rotationMatrix();
				// Translation portion of the matrix
				dest.translationAndVaa.X = m.M31;
				dest.translationAndVaa.Y = m.M32;
				// Z value
				dest.translationAndVaa.Z = depthValues.value( src.order.z );
				// VAA value, only used by thick lines, BTW
				dest.translationAndVaa.W = src.vaa;

				dest.miscIntegers.x = src.drawCall.packToInt;

				if( src.drawCall.brush == eBrush.Sprite )
				{
					// Sprites use 3 higher misc.integers for texture coordinates
					int spriteIndex = -1 - src.order.sn;
					Sprite uv = meshes.spriteCommands[ spriteIndex ].sprite;
					int miscIntY = uv.layer;
					miscIntY |= src.foreground << 8;
					dest.miscIntegers.y = miscIntY;
					unchecked
					{
						dest.miscIntegers.z = (int)( uv.uv & 0xFFFFFFFFu );
						dest.miscIntegers.w = (int)( uv.uv >> 32 );
					}
				}
				else
				{
					if( src.drawCall.vaa != eVaaKind.StrokedThin )
					{
						dest.miscIntegers.y = src.foreground;
						dest.miscIntegers.z = src.background;
					}
					else
					{
						// Thin lines need these colors interpolated, based on how thin.
						// We interpolate here on CPU, to save a few shader instructions per vertex.
						// Then pack to FP16 in miscIntegers.yz.
						ulong stroke = resources.paletteTexture.colorValue( src.foreground );
						ulong bg = resources.paletteTexture.colorValue( src.background );
						ulong color = GraphicsUtils.fp16Lerp( bg, stroke, src.mixFactor );
						unchecked
						{
							dest.miscIntegers.y = (int)( color & uint.MaxValue );
							dest.miscIntegers.z = (int)( color >> 32 );
						}
					}
					dest.miscIntegers.w = 0;
				}
			}
		}
	}
}