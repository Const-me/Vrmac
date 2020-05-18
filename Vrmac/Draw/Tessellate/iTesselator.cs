using System;
using Vrmac.Draw.Shaders;

namespace Vrmac.Draw
{
	interface iTessellatedMeshes
	{
		/// <summary>The result of the tessellation</summary>
		iTriangleMesh mesh { get; }
		/// <summary>Size of the buffers in the mesh</summary>
		sTriangleMesh meshInfo { get; }
		/// <summary>Render passes, and draw calls - precise value after all the clipping. E.g. if clipped to nothing at all, will be empty.</summary>
		sDrawCallInfo drawInfo { get; }
		/// <summary>Result of clipping of the path against viewport rectangle. The value is useful to optimize rendering.</summary>
		eClipResult clipResult { get; }
		/// <summary>Source geometry for the mesh</summary>
		iPathGeometry sourcePath { get; }
	}

	struct sDrawCallInfo
	{
		public readonly eRenderPassFlags renderPassFlags;
		public readonly byte drawCallsCount;

		public sDrawCallInfo( eRenderPassFlags rpf, byte dcc )
		{
			renderPassFlags = rpf;
			drawCallsCount = dcc;
		}

		public override string ToString()
		{
			return $"Pass flags { renderPassFlags }";
		}
	}

	struct sPendingDrawCall
	{
		/// <summary>Render passes, and draw calls - upper bounds estimate before the accurate data is produced by background threads</summary>
		public readonly sDrawCallInfo drawInfo;

		/// <summary>Initially empty, it only receives data after iTesselator.syncThreads() call.</summary>
		public readonly iTessellatedMeshes mesh;

		public sPendingDrawCall( iTessellatedMeshes mesh, eRenderPassFlags rpf, byte dcc )
		{
			drawInfo = new sDrawCallInfo( rpf, dcc );
			this.mesh = mesh;
		}
	}

	interface iTesselator: IDisposable
	{
		void begin();

		// The methods below return conservative estimates, i.e. max. count of draw calls and render passes.
		// The instance argument refers to the instance number within frame. Any integer will work; for optimal performance however, they better be 0 or 1.

		sPendingDrawCall fill( iPathGeometry path, ref Matrix tform, float pixel, eBuildFilledMesh fillOptions, int instance = 0 );

		sPendingDrawCall stroke( iPathGeometry path, ref Matrix tform, float pixel, sStrokeInfo stroke, int instance = 0 );

		sPendingDrawCall fillAndStroke( iPathGeometry path, ref Matrix tform, float pixel, eBuildFilledMesh fillOptions, sStrokeInfo stroke, int instance = 0 );

		void syncThreads();

		Rect? customClippingRect { get; set; }
	}
}