using Diligent.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Vrmac.Draw
{
	sealed class BuffersLayout
	{
		struct BufferSlice
		{
			public readonly int dc;
			public readonly int elements;
			public BufferSlice( int dc, int elements )
			{
				this.dc = dc;
				this.elements = elements;
			}
			public override string ToString() => $"draw call #{ dc } - { elements } elements";
		}

		readonly List<BufferSlice> opaqueVertices = new List<BufferSlice>();
		readonly List<BufferSlice> transparentVertices = new List<BufferSlice>();
		readonly List<int> emptyCalls = new List<int>();

		readonly List<BufferSlice> opaqueIndices = new List<BufferSlice>();
		readonly List<BufferSlice> transparentIndices = new List<BufferSlice>();

		public void clear()
		{
			opaqueVertices.Clear();
			transparentVertices.Clear();
			opaqueIndices.Clear();
			transparentIndices.Clear();
			emptyCalls.Clear();
			drawCallsCount = 0;
			vertexBufferSize = 0;
			indexBufferSize = 0;
		}

		public void addOpaque( int dc, ref sMeshDataSize mds )
		{
			Debug.Assert( dc == opaqueVertices.Count + transparentVertices.Count + emptyCalls.Count );

			int indices = mds.triangles * 3;
			opaqueVertices.Add( new BufferSlice( dc, mds.vertices ) );
			opaqueIndices.Add( new BufferSlice( dc, indices ) );

			vertexBufferSize += mds.vertices;
			indexBufferSize += indices;
		}

		public void addTransparent( int dc, ref sMeshDataSize mds )
		{
			int indices = mds.triangles * 3;
			transparentVertices.Add( new BufferSlice( dc, mds.vertices ) );
			transparentIndices.Add( new BufferSlice( dc, indices ) );

			vertexBufferSize += mds.vertices;
			indexBufferSize += indices;
		}

		public void addMesh( int dc, sTriangleMesh meshInfo )
		{
			Debug.Assert( dc == opaqueVertices.Count + transparentVertices.Count + emptyCalls.Count );

			if( meshInfo.opaqueTriangles > 0 )
			{
				opaqueVertices.Add( new BufferSlice( dc, meshInfo.vertices ) );
				opaqueIndices.Add( new BufferSlice( dc, meshInfo.opaqueTriangles * 3 ) );

				if( meshInfo.transparentTriangles > 0 )
					transparentIndices.Add( new BufferSlice( dc, meshInfo.transparentTriangles * 3 ) );
			}
			else if( meshInfo.transparentTriangles > 0 )
			{
				transparentVertices.Add( new BufferSlice( dc, meshInfo.vertices ) );
				transparentIndices.Add( new BufferSlice( dc, meshInfo.transparentTriangles * 3 ) );
			}
			else
			{
				Debug.Assert( 0 == meshInfo.vertices );
				emptyCalls.Add( dc );
			}

			vertexBufferSize += meshInfo.vertices;
			indexBufferSize += ( meshInfo.opaqueTriangles + meshInfo.transparentTriangles ) * 3;
		}

		int[] vertexOffsets = new int[ 64 ];

		public struct IndexSlice
		{
			public readonly int baseVertex;
			public readonly int baseIndex;
			public IndexSlice( int v, int i )
			{
				baseVertex = v;
				baseIndex = i;
			}
			public override string ToString() => $"Base vertex { baseVertex }, base index { baseIndex }";
		}

		IndexSlice[] opaqueIdxOffsets = new IndexSlice[ 64 ];
		IndexSlice[] transpIdxOffsets = new IndexSlice[ 64 ];

		void layoutVertices()
		{
			drawCallsCount = opaqueVertices.Count + transparentVertices.Count + emptyCalls.Count;
			if( vertexOffsets.Length < drawCallsCount )
			{
				int capacity = drawCallsCount.nextPowerOf2();
				vertexOffsets = new int[ capacity ];
				opaqueIdxOffsets = new IndexSlice[ capacity ];
				transpIdxOffsets = new IndexSlice[ capacity ];
			}

			int off = 0;
			for( int i = opaqueVertices.Count - 1; i >= 0; i-- )
			{
				BufferSlice bs = opaqueVertices[ i ];
				vertexOffsets[ bs.dc ] = off;
				off += bs.elements;
			}
			for( int i = 0; i < transparentVertices.Count; i++ )
			{
				BufferSlice bs = transparentVertices[ i ];
				vertexOffsets[ bs.dc ] = off;
				off += bs.elements;
			}
			foreach( int ec in emptyCalls )
				vertexOffsets[ ec ] = -1;
			vertexBufferSize = off;
		}

		// Returns count of opaque indices
		int layoutIndices()
		{
			IndexSlice empty = new IndexSlice( -1, -1 );
			opaqueIdxOffsets.AsSpan().Slice( 0, drawCallsCount ).Fill( empty );
			transpIdxOffsets.AsSpan().Slice( 0, drawCallsCount ).Fill( empty );

			int off = 0;
			for( int i = opaqueIndices.Count - 1; i >= 0; i-- )
			{
				BufferSlice bs = opaqueIndices[ i ];
				int dc = bs.dc;
				opaqueIdxOffsets[ dc ] = new IndexSlice( vertexOffsets[ dc ], off );
				off += bs.elements;
			}

			int opaqueIndicesCount = off;

			for( int i = 0; i < transparentIndices.Count; i++ )
			{
				BufferSlice bs = transparentIndices[ i ];
				int dc = bs.dc;
				transpIdxOffsets[ dc ] = new IndexSlice( vertexOffsets[ dc ], off );
				off += bs.elements;
			}

			indexBufferSize = off;
			return opaqueIndicesCount;
		}

		public void layout()
		{
			layoutVertices();
			int opaqueIndicesCount = layoutIndices();

			drawInfo = new DrawInfo( opaqueIndicesCount, indexBufferSize - opaqueIndicesCount );
		}

		public int drawCallsCount { get; private set; }
		public int vertexBufferSize { get; private set; }
		public int indexBufferSize { get; private set; }

		public ReadOnlySpan<int> baseVertices => vertexOffsets.AsSpan().Slice( 0, drawCallsCount );
		public ReadOnlySpan<IndexSlice> opaqueIndexOffsets => opaqueIdxOffsets.AsSpan().Slice( 0, drawCallsCount );
		public ReadOnlySpan<IndexSlice> transparentIndexOffsets => transpIdxOffsets.AsSpan().Slice( 0, drawCallsCount );

		public struct DrawInfo
		{
			public readonly int firstOpaqueIndex, opaqueIndices;
			public readonly int firstTransparentIndex, transparentIndices;
			public DrawInfo( int opaque, int transparent )
			{
				firstOpaqueIndex = 0;
				opaqueIndices = opaque;
				firstTransparentIndex = opaque;
				transparentIndices = transparent;
			}
			public override string ToString() =>
				$"Opaque indices [ { firstOpaqueIndex } - { firstOpaqueIndex + opaqueIndices } ], transparent indices [ { firstTransparentIndex } - { firstTransparentIndex + transparentIndices } ]";
		}

		public DrawInfo drawInfo { get; private set; }

		public DrawIndexedAttribs opaqueDrawAttribs( GpuValueType indexType )
		{
			return new DrawIndexedAttribs( false )
			{
				NumIndices = drawInfo.opaqueIndices,
				IndexType = indexType,
				Flags = DrawFlags.VerifyAll,
				FirstIndexLocation = drawInfo.firstOpaqueIndex
			};
		}

		public DrawIndexedAttribs transparentDrawAttribs( GpuValueType indexType )
		{
			return new DrawIndexedAttribs( false )
			{
				NumIndices = drawInfo.transparentIndices,
				IndexType = indexType,
				Flags = DrawFlags.VerifyAll,
				FirstIndexLocation = drawInfo.firstTransparentIndex
			};
		}
	}
}