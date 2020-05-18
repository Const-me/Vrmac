// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Integer size</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct CSize: IEquatable<CSize>
	{
		public int cx, cy;

		public CSize( int width, int height )
		{
			cx = width;
			cy = height;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			return $"[ { cx } × { cy } ]";
		}

		public override int GetHashCode()
		{
			return HashCode.Combine( cx, cy );
		}
		public override bool Equals( object obj )
		{
			if( obj is CSize size )
				return Equals( size );
			return false;
		}
		public bool Equals( CSize p )
		{
			return cx == p.cx && cy == p.cy;
		}
		public static bool operator ==( CSize lhs, CSize rhs )
		{
			return lhs.Equals( rhs );
		}
		public static bool operator !=( CSize lhs, CSize rhs )
		{
			return !( lhs.Equals( rhs ) );
		}

		public bool isEmpty => cx <= 0 || cy <= 0;

		public Vector2 asFloat => new Vector2( cx, cy );
	}
}