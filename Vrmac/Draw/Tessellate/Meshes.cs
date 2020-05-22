using System;
using Vrmac.Draw.Shaders;

namespace Vrmac.Draw.Tessellate
{
	enum eState: byte
	{
		Idle,
		Pending,
		Running,
		Ready,
	}

	sealed class Meshes: iTessellatedMeshes
	{
		readonly iTriangleMesh frontBuffer, backBuffer;
		iTriangleMesh iTessellatedMeshes.mesh => frontBuffer;

		public eClipResult clipResultFront { get; private set; } = eClipResult.ClippedOut;
		eClipResult clipResultBack = eClipResult.ClippedOut;
		eClipResult iTessellatedMeshes.clipResult => clipResultFront;
		iPathGeometry iTessellatedMeshes.sourcePath => path;

		public readonly iPathGeometry path;

		/// <summary>Hash sum of the polyline source for the current front buffer triangle mesh</summary>
		public Guid hashFront { get; private set; }
		/// <summary>Hash sum of the polyline source for the current back buffer triangle mesh</summary>
		Guid hashBack;

		public Meshes( iPathGeometry path, iVrmacDraw factory )
		{
			this.path = path;
			frontBuffer = factory.createTriangleMesh();
			backBuffer = factory.createTriangleMesh();
		}

		public ExtraStrokeMesh extraStroke { get; private set; }

		public Options options;
		public eState state = eState.Idle;

		bool clearBackBuffer()
		{
			backBuffer.clear();
			extraStroke?.clearBackBuffer();
			return true;
		}

		public bool tessellate( ref Options options, ref Rect clip, iPolylinePath poly, ref Guid hashFront )
		{
			eClipResult result = path.buildClippedPolylines( poly, clip, options.transform, options.precision, options.stroke.width );
			clipResultBack = result;

			if( result == eClipResult.ClippedOut )
				return clearBackBuffer();

			Guid hash = poly.hash;
			if( hash == hashFront )
			{
				// Same polyline was used to generate the current front buffer.
				// Return false to skip buffers swap.
				return false;
			}
			if( hash == hashBack )
				return true;
			hashBack = hash;

			if( result == eClipResult.FillsEntireViewport )
			{
				var fillOptions = options.fill;
				if( !fillOptions.HasFlag( eBuildFilledMesh.FilledMesh ) )
					return clearBackBuffer();

				// When there're no visible edges of the path, no point in doing VAA, will be clipped out
				fillOptions &= ~eBuildFilledMesh.VAA;
				poly.createMesh( backBuffer, fillOptions, 0 );
				return true;
			}

			if( options.stroke.width <= 0 )
				poly.createMesh( backBuffer, options.fill, options.pixel );
			else if( options.fill == eBuildFilledMesh.None )
				poly.createMesh( backBuffer, options.stroke );
			else if( options.separateStrokeMesh )
			{
				poly.createMesh( backBuffer, options.fill, options.pixel );
				poly.createMesh( extraStroke.backBuffer, options.stroke );
			}
			else
				poly.createMesh( backBuffer, options.fill, options.pixel, options.stroke );

			return true;
		}

		public void flipBuffers()
		{
			frontBuffer.swap( backBuffer );
			hashFront = hashBack;
			hashBack = default;
			clipResultFront = clipResultBack;
			clipResultBack = eClipResult.ClippedOut;
			extraStroke?.flipBuffers();
			cacheMeshInfo();
		}

		public static sDrawCallInfo cacheMeshInfo( sTriangleMesh i )
		{
			eRenderPassFlags rpf = eRenderPassFlags.None;
			byte dc = 0;
			if( i.opaqueTriangles > 0 )
			{
				dc++;
				rpf |= eRenderPassFlags.Opaque;
			}
			if( i.transparentTriangles > 0 )
			{
				dc++;
				rpf |= eRenderPassFlags.Transparent;
			}
			return new sDrawCallInfo( rpf, dc );
		}

		void cacheMeshInfo()
		{
			var i = frontBuffer.info;
			meshInfo = i;
			drawInfo = cacheMeshInfo( i );
			extraStroke?.cacheMeshInfo();
		}

		public sTriangleMesh meshInfo { get; private set; }
		public sDrawCallInfo drawInfo { get; private set; }

		public void dispose()
		{
			frontBuffer.Dispose();
			backBuffer.Dispose();
			extraStroke?.Dispose();
		}

		public void flushCached()
		{
			hashFront = hashBack = default;
			drawInfo = default;
			frontBuffer.clear();
			backBuffer.clear();
			extraStroke?.flushCached();
		}

		public bool hasPolylines => hashFront != default;

		public void ensureExtraStroke( iVrmacDraw factory )
		{
			if( null != extraStroke )
				return;
			extraStroke = new ExtraStrokeMesh( this, factory );
		}
	}
}