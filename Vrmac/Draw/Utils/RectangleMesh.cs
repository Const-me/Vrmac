using Diligent.Graphics;
using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	// Utility class that implementing a mesh of rectangle.
	static class RectangleMesh
	{
		public static readonly sMeshDataSize sizeStroked = new sMeshDataSize() { vertices = 8, triangles = 8 };
		static readonly byte[] indexBufferStroked = new byte[ 8 * 3 ]
		{
			0,1,3,  0,3,2,	// Left
			2,3,5,  2,5,4,	// Bottom
			4,5,7,  4,7,6,	// Right
			6,7,1,  6,1,0   // Top
		};

		public static readonly sMeshDataSize sizeFilled = new sMeshDataSize() { vertices = 4, triangles = 2 };

		// This array is reused for font glyphs, that's why public.
		public static readonly byte[] indexBufferFilled = new byte[ 6 ]
		{
			0, 1, 3,  3, 1, 2
		};

		public static void strokedVertices( Span<sVertexWithId> span, uint id, ref Rect rectangle, float width )
		{
			Span<Vector2> rectVerts = stackalloc Vector2[ 4 ];
			rectangle.listVertices( rectVerts );

			Span<Vector2> offsetVerts = stackalloc Vector2[ 4 ];
			offsetVerts[ 0 ] = new Vector2( width );
			offsetVerts[ 1 ] = new Vector2( width, -width );
			offsetVerts[ 2 ] = -offsetVerts[ 0 ];
			offsetVerts[ 3 ] = new Vector2( -width, width );

			for( int i = 0; i < 4; i++ )
			{
				span[ i * 2 ].position = rectVerts[ i ];
				span[ i * 2 ].id = id;
				span[ i * 2 + 1 ].position = rectVerts[ i ] + offsetVerts[ i ];
				span[ i * 2 + 1 ].id = id;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static sMeshDataSize strokedIndices( Span<ushort> span, int baseVertex )
		{
			copyIndices( span, baseVertex, indexBufferStroked );
			return sizeStroked;
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static sMeshDataSize strokedIndices( Span<uint> span, int baseVertex )
		{
			copyIndices( span, baseVertex, indexBufferStroked );
			return sizeStroked;
		}

		public static void filledVertices( Span<sVertexWithId> span, uint id, ref Rect rectangle )
		{
			Span<Vector2> rectVerts = stackalloc Vector2[ 4 ];
			rectangle.listVertices( rectVerts );
			for( int i = 0; i < 4; i++ )
			{
				span[ i ].position = rectVerts[ i ];
				span[ i ].id = id;
			}
		}
		// The 2 methods below are also used for sprites, and glyph meshes.
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static sMeshDataSize filledIndices( Span<ushort> span, int baseVertex )
		{
			copyIndices( span, baseVertex, indexBufferFilled );
			return sizeFilled;
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static sMeshDataSize filledIndices( Span<uint> span, int baseVertex )
		{
			copyIndices( span, baseVertex, indexBufferFilled );
			return sizeFilled;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static void copyIndices( Span<ushort> span, int baseVertex, byte[] indexBuffer )
		{
			for( int i = 0; i < indexBuffer.Length; i++ )
				span[ i ] = checked((ushort)( baseVertex + indexBuffer[ i ] ));
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static void copyIndices( Span<uint> span, int baseVertex, byte[] indexBuffer )
		{
			for( int i = 0; i < indexBuffer.Length; i++ )
				span[ i ] = checked((uint)( baseVertex + indexBuffer[ i ] ));
		}
	}
}