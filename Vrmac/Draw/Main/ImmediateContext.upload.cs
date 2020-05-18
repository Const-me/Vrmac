using Diligent.Graphics;
using System;
using System.Diagnostics;
using Vrmac.Draw.Shaders;

namespace Vrmac.Draw.Main
{
	abstract partial class ImmediateContext
	{
		static void dbgPrintGlyphVertices( ReadOnlySpan<sVertexWithId> span )
		{
			foreach( var v in span )
			{
				int pos = BitConverter.SingleToInt32Bits( v.position.X );
				int uv = BitConverter.SingleToInt32Bits( v.position.Y );
				ConsoleLogger.logDebug( "xy [ {0}, {1} ]	uv [ {2:x}, {3:x} ]	id {4:x}", pos & 0xFFFF, pos >> 16, uv & 0xFFFF, uv >> 16, v.id );
			}
		}

		void uploadVertices( IDeviceContext ic, Span<sDrawCall> drawCallsSpan )
		{
			ReadOnlySpan<int> baseVertices = buffersLayout.baseVertices;

			using( var mapped = resources.getVertexBuffer().map<sVertexWithId>( ic, buffersLayout.vertexBufferSize ) )
			{
				Span<sVertexWithId> span = mapped.span;

				for( int i = 0; i < drawCallsSpan.Length; i++ )
				{
					int bv = baseVertices[ i ];
					if( bv < 0 )
						continue;   // Empty draw call, i.e. completely clipped out

					Span<sVertexWithId> dest = span.Slice( bv );

					ref var dc = ref drawCallsSpan[ i ];
					uint id = VertexID.vertex( i );
					int sn = dc.order.sn;
					if( sn >= 0 )
					{
						drawMeshes.meshes[ sn ].mesh.mesh.copyVertices( dest, id );
						continue;
					}
					sn = -sn - 1;

					if( dc.drawCall.mesh == eMesh.SpriteRectangle )
					{
						Rect rc = drawMeshes.spriteCommands[ sn ].rect;
						SpriteMesh.writeVertices( dest, ref rc, id );
						continue;
					}

					if( dc.drawCall.isText )
					{
						var text = drawMeshes.textCommands[ sn ];
						sMeshDataSize mds;
						if( text.consoleWidth.HasValue )
							mds = text.font.renderConsole( dest, id, text.text, text.rectangle.topLeft, text.textRendering, text.consoleWidth.Value );
						else
							mds = text.font.renderBlock( dest, id, text.text, ref text.rectangle, text.textRendering );

						Debug.Assert( mds.vertices <= text.meshDataSize.vertices && mds.triangles <= text.meshDataSize.triangles );
						// Store the actual count of triangles written.
						// uploadIndices function needs it later, to fill unused portion of the index buffer with UINT_MAX (32 bit IB) or 0xFFFF (16 bit IB) discarding the data.
						// Technically that's wasted GPU bandwidth, however:
						// (a) Not much of it, as normal text only contains a few of these soft hyphens and ligatures.
						// (b) Saves substantial amount of resources allowing to generate the complete mesh with a single pass over the glyphs.
						// With exact-sized buffers, we would have to build the complete text mesh in managed memory, then copy to GPU.
						// Current implementation only buffers 1 word for left aligned blocks, or 1 line for middle or right aligned blocks, or nothing at all for single-line text. The vertices go straight to mapped GPU verftex buffer.
						text.actualTriangles = mds.triangles;
						drawMeshes.textCommands[ sn ] = text;

						// dbgPrintGlyphVertices( dest );
						continue;
					}

					sDrawRectCommand cmd = drawMeshes.rectCommands[ sn ];
					if( cmd.strokeWidth.HasValue )
						RectangleMesh.strokedVertices( dest, id, ref cmd.rect, cmd.strokeWidth.Value );
					else
						RectangleMesh.filledVertices( dest, id, ref cmd.rect );
				}
			}
		}

		interface iUploadIndices<E>
		{
			E invalidIndex { get; }
			void opaque( iTriangleMesh mesh, Span<E> span, int baseVertex );
			void transparent( iTriangleMesh mesh, Span<E> span, int baseVertex );
			void strokedRect( Span<E> destSpan, int baseVertex );
			void filledRect( Span<E> destSpan, int baseVertex );
			void spriteRect( Span<E> destSpan, int baseVertex );
			void glyphRun( Span<E> destSpan, int baseVertex, int countTriangles );
		}

		struct Traits16: iUploadIndices<ushort>
		{
			public ushort invalidIndex => ushort.MaxValue;

			public void opaque( iTriangleMesh mesh, Span<ushort> span, int baseVertex ) =>
				mesh.copyOpaqueTriangles( span, baseVertex );
			public void transparent( iTriangleMesh mesh, Span<ushort> span, int baseVertex ) =>
				mesh.copyTransparentTriangles( span, baseVertex );
			public void strokedRect( Span<ushort> destSpan, int baseVertex ) =>
				RectangleMesh.strokedIndices( destSpan, baseVertex );
			public void filledRect( Span<ushort> destSpan, int baseVertex ) =>
				RectangleMesh.filledIndices( destSpan, baseVertex );
			public void spriteRect( Span<ushort> destSpan, int baseVertex ) =>
				SpriteMesh.writeIndices( destSpan, baseVertex );
			public void glyphRun( Span<ushort> destSpan, int baseVertex, int countTriangles ) =>
				Text.Utils.writeIndices( destSpan, baseVertex, countTriangles );
		}

		struct Traits32: iUploadIndices<uint>
		{
			public uint invalidIndex => uint.MaxValue;

			// Yep, exact copy-paste of the above. Sometimes C++ templates are just better.
			public void opaque( iTriangleMesh mesh, Span<uint> span, int baseVertex ) =>
				mesh.copyOpaqueTriangles( span, baseVertex );
			public void transparent( iTriangleMesh mesh, Span<uint> span, int baseVertex ) =>
				mesh.copyTransparentTriangles( span, baseVertex );
			public void strokedRect( Span<uint> destSpan, int baseVertex ) =>
				RectangleMesh.strokedIndices( destSpan, baseVertex );
			public void filledRect( Span<uint> destSpan, int baseVertex ) =>
				RectangleMesh.filledIndices( destSpan, baseVertex );
			public void spriteRect( Span<uint> destSpan, int baseVertex ) =>
				SpriteMesh.writeIndices( destSpan, baseVertex );
			public void glyphRun( Span<uint> destSpan, int baseVertex, int countTriangles ) =>
				Text.Utils.writeIndices( destSpan, baseVertex, countTriangles );
		}

		void uploadIndices<E, T>( IDeviceContext ic, Span<sDrawCall> drawCallsSpan, T t )
			where E : unmanaged
			where T : struct, iUploadIndices<E>
		{
			ReadOnlySpan<BuffersLayout.IndexSlice> opaqueOffsets = buffersLayout.opaqueIndexOffsets;
			ReadOnlySpan<BuffersLayout.IndexSlice> transparentOffsets = buffersLayout.transparentIndexOffsets;

			using( var mapped = resources.getIndexBuffer().map<E>( ic, buffersLayout.indexBufferSize ) )
			{
				Span<E> span = mapped.span;
				for( int i = 0; i < drawCallsSpan.Length; i++ )
				{
					ref var dc = ref drawCallsSpan[ i ];
					int sn = dc.order.sn;
					BuffersLayout.IndexSlice slice;

					if( sn >= 0 )
					{
						iTessellatedMeshes mesh = drawMeshes.meshes[ sn ].mesh;
						var passFlags = mesh.drawInfo.renderPassFlags;
						if( passFlags.HasFlag( eRenderPassFlags.Opaque ) )
						{
							slice = opaqueOffsets[ i ];
							t.opaque( mesh.mesh, span.Slice( slice.baseIndex ), slice.baseVertex );
						}
						if( passFlags.HasFlag( eRenderPassFlags.Transparent ) )
						{
							slice = transparentOffsets[ i ];
							t.transparent( mesh.mesh, span.Slice( slice.baseIndex ), slice.baseVertex );
						}
						continue;
					}
					sn = -sn - 1;

					if( dc.drawCall.brush == eBrush.Sprite )
					{
						// Sprite rectangle
						slice = transparentOffsets[ i ];
						t.spriteRect( span.Slice( slice.baseIndex ), slice.baseVertex );
						continue;
					}

					if( dc.drawCall.isText )
					{
						// Glyph runs can be either opaque or transparent, depending on text background.
						sDrawTextCommand text = drawMeshes.textCommands[ sn ];
						int countTriangles = text.meshDataSize.triangles;
						slice = transparentOffsets[ i ];
						t.glyphRun( span.Slice( slice.baseIndex ), slice.baseVertex, text.actualTriangles );
						if( text.actualTriangles == text.meshDataSize.triangles )
						{
							// The text renderer used the complete buffer, nothing else to do here.
							continue;
						}

						// The text renderer produced less quads than initially estimated.
						// Need to fill the rest of the index buffer slice with magic number `-1` to avoid rendering whatever garbage gonna be in uninitialized buffers.
						// For obvious reason, a triangle uses 3 indices.
						int firstInvalidIndex = slice.baseIndex + text.actualTriangles * 3;
						int countInvalidIndices = ( text.meshDataSize.triangles - text.actualTriangles ) * 3;
						span.Slice( firstInvalidIndex, countInvalidIndices ).Fill( t.invalidIndex );
						continue;
					}

					// Built-in rectangle made by iDrawContext.drawRectangle call
					slice = opaqueOffsets[ i ];
					var dest = span.Slice( slice.baseIndex );
					sDrawRectCommand cmd = drawMeshes.rectCommands[ sn ];
					if( cmd.strokeWidth.HasValue )
						t.strokedRect( dest, slice.baseVertex );
					else
						t.filledRect( dest, slice.baseVertex );
				}
			}
		}

		GpuValueType uploadIndices( IDeviceContext ic, Span<sDrawCall> drawCallsSpan )
		{
			if( buffersLayout.vertexBufferSize < 0x10000 )
			{
				uploadIndices<ushort, Traits16>( ic, drawCallsSpan, new Traits16() );
				return GpuValueType.Uint16;
			}

			uploadIndices<uint, Traits32>( ic, drawCallsSpan, new Traits32() );
			return GpuValueType.Uint32;
		}
	}
}