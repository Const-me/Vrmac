using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Creates shapes geometry in managed memory.</summary>
	public static class Shapes
	{
		/// <summary>Polygon shape</summary>
		public static iPathData polygon( ReadOnlySpan<Vector2> vertices )
		{
			if( vertices.Length < 3 )
				throw new ArgumentException( "Polygons must have at least 3 vertices" );

			float[] data = new float[ ( vertices.Length - 1 ) * 2 ];
			vertices.Slice( 1 ).CopyTo( MemoryMarshal.Cast<float, Vector2>( data.AsSpan() ) );
			return new SimpleShape( eSegmentKind.Line, vertices[ 0 ], data );
		}

		/// <summary>Axis-aligned rectangle shape</summary>
		public static iPathData rectangle( Rect rect )
		{
			Span<Vector2> vertices = stackalloc Vector2[ 4 ];
			vertices[ 0 ] = rect.topLeft;
			vertices[ 1 ] = new Vector2( rect.bottom, rect.left );
			vertices[ 2 ] = rect.bottomRight;
			vertices[ 3 ] = new Vector2( rect.top, rect.right );
			return polygon( vertices );
		}

		/// <summary>Ellipse shape</summary>
		public static iPathData ellipse( Vector2 center, Vector2 radius )
		{
			if( radius.X <= 0 || radius.Y <= 0 )
				throw new ArgumentException( "Ellipse radius must be positive" );

			// Theoretically, just 1 segment is enough, with all these weird flags for large arcs.
			// Practically, I expect this thing to be much more numerically stable with 4 edges, 90 degrees / each.
			Vector2 startingPoint = new Vector2( center.X, center.Y - radius.Y );
			float[] data = new float[ 4 * 5 ];
			var span = MemoryMarshal.Cast<float, sArcData>( data.AsSpan() );

			span[ 0 ].endPoint = new Vector2( center.X - radius.X, center.Y );
			span[ 1 ].endPoint = new Vector2( center.X, center.Y + radius.Y );
			span[ 2 ].endPoint = new Vector2( center.X + radius.X, center.Y );
			span[ 3 ].endPoint = startingPoint;

			for( int i = 0; i < 4; i++ )
			{
				span[ i ].size = radius;
				span[ i ].angleDegrees = 90;
			}
			return new SimpleShape( eSegmentKind.Arc, startingPoint, data );
		}

		/// <summary>Create circle shape</summary>
		public static iPathData circle( Vector2 center, float radius )
		{
			return ellipse( center, new Vector2( radius, radius ) );
		}

		static Vector2 v2( float x, float y )
		{
			return new Vector2( x, y );
		}

		/// <summary>Rounded axis-aligned rectangle shape.</summary>
		/// <remarks>If the radius is 0, will create a normal rectangle instead. If the radius is too large compared to the rectangle, will create an ellipse instead.
		/// Depending on the arguments, may also create oval shape, 2 half circles joined by a rectangle.</remarks>
		public static iPathData roundedRectangle( Rect rect, Vector2 radius )
		{
			if( radius.X < 0 || radius.Y < 0 )
				throw new ArgumentException( "Rounded rectangle radius must be non-negative" );
			if( radius.X < 1E-4f || radius.Y < 1E-4f )
				return rectangle( rect );

			int collapsedFlags = 0;
			if( radius.X * 2 >= rect.width )
			{
				radius.X = rect.width * 0.5f;
				collapsedFlags |= 1;
			}

			if( radius.Y * 2 >= rect.width )
			{
				radius.Y = rect.height * 0.5f;
				collapsedFlags |= 2;
			}

			if( collapsedFlags == 3 )
			{
				// Both horizontal and vertical sides are collapsed. This turns rounded rectangle into an ellipse.
				return ellipse( rect.center, radius );
			}

			eArcFlags af = eArcFlags.ArcSmall | eArcFlags.DirCounterClockwise;
			if( collapsedFlags == 0 )
			{
				// Rounded rectangle
				PathBuilder builder = new PathBuilder( 4 * 5 + 3 * 2, 4 + 3 );
				using( var fb = builder.newFigure() )
				{
					// Starting
					fb.move( v2( rect.left + radius.X, rect.top ) );
					// Top left
					fb.arc( v2( rect.left, rect.top + radius.Y ), radius, 90, af );
					// Left
					fb.line( v2( rect.left, rect.bottom - radius.Y ) );
					// Bottom left
					fb.arc( v2( rect.left + radius.X, rect.bottom ), radius, 90, af );
					// Bottom
					fb.line( v2( rect.right - radius.X, rect.bottom ) );
					// Bottom right
					fb.arc( v2( rect.right, rect.bottom - radius.Y ), radius, 90, af );
					// Right
					fb.line( v2( rect.right, rect.top + radius.Y ) );
					// Top right
					fb.arc( v2( rect.right - radius.X, rect.top ), radius, 90, af );
					// No top line, close the figure instead
					fb.closeFigure();
				}
				return builder.build();
			}

			if( collapsedFlags == 1 )
			{
				// Oval with vertical lines
				float xCenter = ( rect.left + rect.right ) * 0.5f;
				PathBuilder builder = new PathBuilder( 4 * 5 + 2, 4 + 1 );
				using( var fb = builder.newFigure() )
				{
					// Starting
					fb.move( v2( rect.right, rect.top + radius.Y ) );
					// Top right
					fb.arc( v2( xCenter, rect.top ), radius, 90, af );
					// Top left
					fb.arc( v2( rect.left, rect.top + radius.Y ), radius, 90, af );
					// Left
					fb.line( v2( rect.left, rect.bottom - radius.Y ) );
					// Bottom left
					fb.arc( v2( xCenter, rect.bottom ), radius, 90, af );
					// Bottom right
					fb.arc( v2( rect.right, rect.bottom - radius.Y ), radius, 90, af );
					// No right line, close the figure instead
					fb.closeFigure();
				}
				return builder.build();
			}

			if( collapsedFlags == 2 )
			{
				// Oval with horizontal lines
				float yc = ( rect.top + rect.bottom ) * 0.5f;
				PathBuilder builder = new PathBuilder( 4 * 5 + 2, 4 + 1 );
				using( var fb = builder.newFigure() )
				{
					// Starting
					fb.move( v2( rect.left + radius.X, rect.top ) );
					// Top left
					fb.arc( v2( rect.left, yc ), radius, 90, af );
					// Bottom left
					fb.arc( v2( rect.left + radius.X, rect.bottom ), radius, 90, af );
					// Bottom
					fb.line( v2( rect.right - radius.X, rect.bottom ) );
					// Bottom right
					fb.arc( v2( rect.right, yc ), radius, 90, af );
					// Top right
					fb.arc( v2( rect.right - radius.X, rect.top ), radius, 90, af );
					// No top line, close the figure instead
					fb.closeFigure();
				}
				return builder.build();
			}

			throw new ApplicationException( "Unexpected" );
		}

		/// <summary>Rounded axis-aligned rectangle shape with the same corner radius for X and Y.</summary>
		/// <remarks>If the radius is 0, will create a normal rectangle instead. If the radius is too large compared to the rectangle, will create an ellipse instead.
		/// Depending on the arguments, may also create oval shape, 2 half circles joined by a rectangle.</remarks>
		public static iPathData roundedRectangle( Rect rect, float radius ) =>
			roundedRectangle( rect, new Vector2( radius, radius ) );
	}
}