using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;
using Vrmac.Draw.Main;

namespace Vrmac.Draw.Shaders
{
	class MoreDrawCallsState: VrmacStateBase
	{
		static readonly string bufferVarName;
		public static readonly int maxDrawCalls;

		const int D3D11_REQ_BUFFER_RESOURCE_TEXEL_COUNT_2_TO_EXP = 27;
		const int linuxUniformLimitKb = 64;	// 2GB on Windows versus 64kb on Linux, that's 4.5 orders of magnitude difference :-(

		static MoreDrawCallsState()
		{
			int cbCall = Marshal.SizeOf<sDrawCallData>();

			if( RuntimeEnvironment.runningWindows )
			{
				bufferVarName = "drawCalls";
				long maxVectors = 1L << D3D11_REQ_BUFFER_RESOURCE_TEXEL_COUNT_2_TO_EXP;  // 128M
				long maxBytes = maxVectors * 16;    // 2GB
				maxDrawCalls = (int)( maxBytes / cbCall );
			}
			else
			{
				bufferVarName = "DrawCallsCBuffer";
				maxDrawCalls = ( linuxUniformLimitKb * 1024 ) / cbCall;
			}
		}

		readonly DynamicBuffer drawCallsBuffer;

		public MoreDrawCallsState( GpuResources res, IRenderDevice device, ref PipelineStateDesc desc, iPipelineStateFactory stateFactory, eShaderMacros macros ) :
			base( res, device, ref desc, stateFactory, macros )
		{
			drawCallsBuffer = res.moreDrawCalls;

			drawCallsBuffer.resized.add( this, ( DynamicBuffer buffer, int capacity ) => dropResourceBindings() );
		}

		protected override void declareResources( iPipelineStateFactory stateFactory )
		{
			base.declareResources( stateFactory );

			stateFactory.layoutVariable( ShaderType.Vertex, ShaderResourceVariableType.Mutable, bufferVarName );
		}

		protected override void bindMutableResources( IShaderResourceBinding bindings )
		{
			base.bindMutableResources( bindings );

			using( var vv = bindings.GetVariableByName( ShaderType.Vertex, bufferVarName ) )
			{
				if( RuntimeEnvironment.runningWindows )
				{
					// On Windows that's a normal buffer, theoretically can grow up to 2GB or whatever huge value the resource limit.
					vv.Set( drawCallsBuffer.buffer.GetDefaultView( BufferViewType.ShaderResource ) );
				}
				else
				{
					// Unfortunately, Linux lags behind by 13 years and counting: HLSL has Buffer<float4> since D3D 10.0, readable in all shader stages.
					// BTW, Pi4 GPU is capable of 32 GFLOPs: https://www.raspberrypi.org/forums/viewtopic.php?p=1492907
					// Faster than a mid-range desktop D3D 10.0 GPU from 2007, GeForce 8500 GT: https://en.wikipedia.org/wiki/GeForce_8_series#Technical_summary
					vv.Set( drawCallsBuffer.buffer );
				}
			}
		}

		public override void uploadDrawCalls( IDeviceContext ic, Span<sDrawCall> drawCalls, iDepthValues depthValues, ref DrawMeshes meshes )
		{
			using( var mapped = drawCallsBuffer.map<sDrawCallData>( ic, drawCalls.Length ) )
				produceDrawCalls( mapped.span, drawCalls, depthValues, ref meshes );
		}
	}
}