// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using System.Runtime.InteropServices;
using Vrmac;

/// <summary>Integer rectangle</summary>
[StructLayout( LayoutKind.Sequential )]
public struct CRect
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
	public override string ToString()
	{
		return $"{ topLeft } - { bottomRight }, size { size }";
	}

	/// <summary>An empty rectangle at [ 0, 0 ]</summary>
	public static CRect empty => new CRect( 0, 0, 0, 0 );
}