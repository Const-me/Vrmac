// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using Diligent.Graphics;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Rectangle with 4 single precision float values</summary>
	/// <remarks>Binary compatible with D2D_RECT_F from Direct2D.</remarks>
	/// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/dcommon/ns-dcommon-d2d_rect_f" />
	[StructLayout( LayoutKind.Sequential )]
	public struct Rect
	{
		public float left, top, right, bottom;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Rect( float left, float top, float right, float bottom )
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Rect( Vector2 topLeft, Vector2 bottomRight )
		{
			left = MathF.Min( topLeft.X, bottomRight.X );
			top = MathF.Min( topLeft.Y, bottomRight.Y );
			right = MathF.Max( topLeft.X, bottomRight.X );
			bottom = MathF.Max( topLeft.Y, bottomRight.Y );
		}

		/// <summary>Top left corner</summary>
		public Vector2 topLeft => new Vector2( left, top );

		/// <summary>Bottom right corner</summary>
		public Vector2 bottomRight => new Vector2( right, bottom );

		/// <summary>Scale the rectangle by a scalar</summary>
		public static Rect operator *( Rect value, float scaleFactor )
		{
			value.left *= scaleFactor;
			value.top *= scaleFactor;
			value.right *= scaleFactor;
			value.bottom *= scaleFactor;
			return value;
		}

		/// <summary>A string for debugger</summary>
		public override string ToString()
		{
			return $"[ { left }, { top } ] - [ { right }, { bottom } ], { width } × { height }";
		}

		/// <summary>Width of the rectangle</summary>
		public float width => right - left;
		/// <summary>Height of the rectangle</summary>
		public float height => bottom - top;
		/// <summary>Center of the rectangle</summary>
		public Vector2 center => new Vector2( ( left + right ) * 0.5f, ( top + bottom ) * 0.5f );
		/// <summary>Size of the rectangle</summary>
		public Vector2 size => new Vector2( width, height );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Rect deflate( float x, float y )
		{
			return new Rect( left + x, top + y, right - x, bottom - y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Rect deflate( float xy )
		{
			return deflate( xy, xy );
		}

		/// <summary>Get absolute position of a point relative to this rectangle. You’ll get <see cref="topLeft" /> if you pass [ 0, 0 ] vector to this method.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Vector2 getPoint( Vector2 rel )
		{
			return new Vector2(
				MathHelper.Lerp( left, right, rel.X ),
				MathHelper.Lerp( top, bottom, rel.Y )
			);
		}

		/// <summary>Make a vector with relative coordinates if a point relative to this rectangle. You’ll get [ 0, 0 ] if you pass the <see cref="topLeft" /> position to this method.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Vector2 makeRelative( Vector2 absolute )
		{
			return ( absolute - topLeft ) / size;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Rect scaleAroundCenter( float amount )
		{
			Vector2 c = center;
			Vector2 sz = size * ( amount * 0.5f );
			return new Rect( c - sz, c + sz );
		}

		/// <summary>Produce 4 vertices of the rectangle, and write them into the span.</summary>
		/// <remarks>
		/// <para>The order is counter-clockwise starting from the <see cref="topLeft" /> vertex.</para>
		/// <para>Protip: use stackalloc to create that span, it only needs 32 bytes of memory.</para>
		/// </remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void listVertices( Span<Vector2> vertices )
		{
			vertices[ 0 ] = topLeft;
			vertices[ 1 ] = new Vector2( left, bottom );
			vertices[ 2 ] = bottomRight;
			vertices[ 3 ] = new Vector2( right, top );
		}

		/// <summary>Compute bounding box from a set of points</summary>
		public static Rect createBoundingBox( ReadOnlySpan<Vector2> points )
		{
			Vector2 i = new Vector2( float.MaxValue );
			Vector2 ax = new Vector2( -float.MaxValue );
			foreach( Vector2 v in points )
			{
				i = Vector2.Min( i, v );
				ax = Vector2.Max( ax, v );
			}
			return new Rect( i, ax );
		}

		/// <summary>True if the two rectangles intersect</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool intersects( ref Rect b )
		{
			return left <= b.right && top <= b.bottom && right >= b.left && bottom >= b.top;
		}

		/// <summary>Convert clip space rectangle into viewport rectangle</summary>
		public Rect viewportFromClipSpace( Vector2 viewportSize )
		{
			Rect rc = this * 0.5f;

			return new Rect(
				viewportSize.X * ( rc.left + 0.5f ),
				viewportSize.Y * ( 0.5f - rc.bottom ),
				viewportSize.X * ( rc.right + 0.5f ),
				viewportSize.Y * ( 0.5f - rc.top )
				);
		}

		/// <summary>Convert viewport rectangle into clip space rectangle</summary>
		public Rect clipSpaceFromViewport( Vector2 viewportSize )
		{
			Vector2 mul = new Vector2( 2 ) / viewportSize;

			return new Rect(
				mul.X * left - 1,
				1 - mul.Y * bottom,
				mul.X * right - 1,
				1 - mul.Y * top
				);
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Rect offset( Vector2 amount )
		{
			return new Rect( topLeft + amount, bottomRight + amount );
		}
	}
}