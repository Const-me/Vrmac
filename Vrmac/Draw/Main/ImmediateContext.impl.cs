using System;
using Vrmac.Draw.Shaders;

namespace Vrmac.Draw.Main
{
	abstract partial class ImmediateContext
	{
		void flushIfNeeded( byte newDrawCalls )
		{
			drawCallsUpperBound += newDrawCalls;
			if( drawCallsUpperBound > MoreDrawCallsState.maxDrawCalls )
			{
				flush();
				drawCallsUpperBound = newDrawCalls;
			}
		}

		Order order()
		{
			Order res = new Order( drawMeshes.meshes.length, currentZ );
			currentZ++;
			return res;
		}

		readonly GpuResources resources;
		readonly StatesCache states;
		readonly iDepthValues depthValues;
		int currentZ = 0;
		int drawCallsUpperBound = 0;
		internal readonly iTesselator tesselatorThread;

		/// <summary>Draw calls sent by user</summary>
		readonly Buffer<sDrawCall> calls = new Buffer<sDrawCall>();

		DrawMeshes drawMeshes = new DrawMeshes( true );

		readonly BuffersLayout buffersLayout = new BuffersLayout();

		void layoutBuffers( Span<sDrawCall> drawCallsSpan )
		{
			buffersLayout.clear();

			for( int i = 0; i < drawCallsSpan.Length; i++ )
			{
				ref var dc = ref drawCallsSpan[ i ];
				int sn = dc.order.sn;

				if( sn >= 0 )
				{
					buffersLayout.addMesh( i, drawMeshes.meshes[ sn ].mesh.meshInfo );
					continue;
				}
				sn = -sn - 1;

				if( dc.drawCall.mesh == eMesh.SpriteRectangle )
				{
					sMeshDataSize mds = new sMeshDataSize( SpriteMesh.countVertices, SpriteMesh.countTriangles );
					buffersLayout.addTransparent( i, ref mds );
					continue;
				}

				if( dc.drawCall.isText )
				{
					// Glyph runs can be either opaque or transparent, depending on text background
					sMeshDataSize mds = drawMeshes.textCommands[ sn ].meshDataSize;
					buffersLayout.addTransparent( i, ref mds );
					continue;
				}

				sDrawRectCommand cmd = drawMeshes.rectCommands[ sn ];
				sMeshDataSize size;
				if( cmd.strokeWidth.HasValue )
					size = RectangleMesh.sizeStroked;
				else
					size = RectangleMesh.sizeFilled;
				buffersLayout.addOpaque( i, ref size );
			}

			buffersLayout.layout();
		}
	}
}