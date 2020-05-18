using Diligent;
using Diligent.Graphics;
using System;
using Vrmac.Draw.Main;

namespace Vrmac.Draw.Shaders
{
	sealed class FewDrawCallsState: VrmacStateBase
	{
		public const int smallCount = 64;
		readonly IBuffer constantBuffer;

		public FewDrawCallsState( GpuResources res, IRenderDevice device, ref PipelineStateDesc desc, iPipelineStateFactory stateFactory, eShaderMacros macros ) :
			base( res, device, ref desc, stateFactory, macros )
		{
			constantBuffer = res.fewDrawCalls;
			pipelineState.GetStaticVariableByName( ShaderType.Vertex, "DrawCallsCBuffer" ).Set( constantBuffer );
		}

		public override void uploadDrawCalls( IDeviceContext ic, Span<sDrawCall> drawCalls, iDepthValues depthValues, ref DrawMeshes meshes )
		{
			int count = drawCalls.Length;
			if( count > smallCount )
				throw new ArgumentException( "Too many draw calls for FewDrawCallsState class" );

			var span = Unsafe.writeSpan<sDrawCallData>( ic.MapBuffer( constantBuffer, MapType.Write, MapFlags.Discard ), count );
			try
			{
				produceDrawCalls( span, drawCalls, depthValues, ref meshes );
			}
			finally
			{
				ic.UnmapBuffer( constantBuffer, MapType.Write );
			}
		}
	}
}