// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using Diligent.Graphics;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>2D vector of integers</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct CPoint
	{
		public int x, y;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public CPoint( int x, int y )
		{
			this.x = x;
			this.y = y;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			return $"[ {x}, {y} ]";
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool operator ==( CPoint a, CPoint b )
		{
			return a.x == b.x && a.y == b.y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool operator !=( CPoint a, CPoint b )
		{
			return a.x != b.x || a.y != b.y;
		}
		public override bool Equals( object obj ) => ( obj is CPoint that ) && this == that;

		public override int GetHashCode()
		{
			return HashCode.Combine( x, y );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static CPoint operator +( CPoint a, CPoint b )
		{
			return new CPoint( a.x + b.x, a.y + b.y );
		}
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static CPoint operator -( CPoint a, CPoint b )
		{
			return new CPoint( a.x - b.x, a.y - b.y );
		}

		public Vector2 asFloat => new Vector2( x, y );
		public CSize asSize => new CSize( x, y );
	}
}