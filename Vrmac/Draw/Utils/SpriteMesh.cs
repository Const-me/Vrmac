using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	// A geometry shader would work better for that, but they are not available yet on Pi4.
	// Not a big deal, the 2D vertices are just 12 bytes / each, sending 4 of them per quad.
	static class SpriteMesh
	{
		public const int countVertices = 4;
		public const int countTriangles = 2;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void writeVertices( Span<sVertexWithId> span, ref Rect rectangle, uint id )
		{
			Span<Vector2> rectVerts = stackalloc Vector2[ 4 ];
			rectangle.listVertices( rectVerts );

			if( 0 != ( id & 0xff ) )
				throw new ArgumentException();
			// Rect.listVertices outputs them starting from top-left in counter clockwise order.
			// Vertex shader uses the lowest bit for U, bit 0x2 for V.
			// That's why the remapping.

			// Minor optimization - unrolling the loop manually. 4 is too short, branch prediction will mispredict 25% of the times.
			span[ 0 ].position = rectVerts[ 0 ];
			span[ 0 ].id = id;
			span[ 1 ].position = rectVerts[ 1 ];
			span[ 1 ].id = id | 2u;
			span[ 2 ].position = rectVerts[ 2 ];
			span[ 2 ].id = id | 3u;
			span[ 3 ].position = rectVerts[ 3 ];
			span[ 3 ].id = id | 1u;

			/* ushort magicBytesRemap = 0x1320;

			for( int i = 0; i < 4; i++ )
			{
				span[ i ].position = rectVerts[ i ];
				int magicByte = ( magicBytesRemap >> ( i * 4 ) ) & 3;
				span[ i ].id = id | (uint)( magicByte );
			} */
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void writeIndices( Span<ushort> span, int baseVertex ) =>
			RectangleMesh.filledIndices( span, baseVertex );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void writeIndices( Span<uint> span, int baseVertex ) =>
			RectangleMesh.filledIndices( span, baseVertex );
	}
}