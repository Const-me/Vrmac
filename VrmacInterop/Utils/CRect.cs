// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using System;
using System.Runtime.InteropServices;
using Vrmac;

/// <summary>Integer rectangle</summary>
[StructLayout( LayoutKind.Sequential )]
public struct CRect: IEquatable<CRect>
{
	public int left, top, right, bottom;

	public CRect( int left, int top, int right, int bottom )
	{
		this.left = left;
		this.top = top;
		this.right = right;
		this.bottom = bottom;
	}

	public CRect( CPoint topLeft, CSize size )
	{
		left = topLeft.x;
		top = topLeft.y;
		right = topLeft.x + size.cx;
		bottom = topLeft.y + size.cy;
	}

	public CSize size => new CSize( right - left, bottom - top );
	public CPoint topLeft => new CPoint( left, top );
	public CPoint bottomRight => new CPoint( right, bottom );
	public CPoint center => new CPoint( ( left + right ) / 2, ( top + bottom ) / 2 );
	public bool isEmpty => right <= left || bottom <= top;

	/// <summary>Returns a string that represents the current object.</summary>
	public override string ToString() =>
		$"{ topLeft } - { bottomRight }, size { size }";

	public bool Equals( CRect that )
	{
		return left == that.left && top == that.top && right == that.right && bottom == that.bottom;
	}

	public override bool Equals( object obj )
	{
		if( obj is CRect rc )
			return Equals( rc );
		return false;
	}

	public override int GetHashCode() => HashCode.Combine( typeof( CRect ), left, top, right, bottom );

	public static bool operator ==( CRect a, CRect b )
	{
		return a.Equals( b );
	}
	public static bool operator !=( CRect a, CRect b )
	{
		return !a.Equals( b );
	}

	/// <summary>An empty rectangle at [ 0, 0 ]</summary>
	public static CRect empty => default;
}