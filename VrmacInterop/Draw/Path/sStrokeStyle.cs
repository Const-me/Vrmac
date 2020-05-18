using System;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Describes the shape at the end of a line or segment.</summary>
	/// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/d2d1/ne-d2d1-d2d1_cap_style" />
	public enum eLineCap: byte
	{
		/// <summary>A cap that does not extend past the last point of the line. Comparable to cap used for objects other than lines.</summary>
		Flat,
		/// <summary>Half of a square that has a length equal to the line thickness.</summary>
		Square,
		/// <summary>A semicircle that has a diameter equal to the line thickness.</summary>
		Round,
		/// <summary>An isosceles right triangle whose hypotenuse is equal in length to the thickness of the line.</summary>
		Triangle,
	}

	/// <summary>Describes the shape that joins two lines or segments.</summary>
	/// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/d2d1/ne-d2d1-d2d1_line_join" />
	public enum eLineJoin: byte
	{
		/// <summary>Regular angular vertices.</summary>
		Miter,
		/// <summary>Beveled vertices.</summary>
		Bevel,
		/// <summary>Rounded vertices.</summary>
		Round,
		/// <summary>Regular angular vertices unless the join would extend beyond the miter limit; otherwise, beveled vertices.</summary>
		MiterOrBevel,
	}

	/// <summary>Describes the stroke that outlines a shape.</summary>
	/// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/d2d1/ns-d2d1-d2d1_stroke_style_properties" />
	[StructLayout( LayoutKind.Sequential )]
	public struct sStrokeStyle: IEquatable<sStrokeStyle>
	{
		/// <summary>The cap applied to the start of all the open figures in a stroked geometry.</summary>
		public eLineCap startCap;
		/// <summary>The cap applied to the end of all the open figures in a stroked geometry.</summary>
		public eLineCap endCap;
		/// <summary>A value that describes how segments are joined. This value is ignored for a vertex if the segment flags specify that the segment should have a smooth join.</summary>
		public eLineJoin lineJoin;
		/// <summary>The limit of the thickness of the join on a mitered corner. This value is always treated as though it is greater than or equal to 1.0f.</summary>
		public float miterLimit;

		/// <summary>Determines whether two object instances are equal</summary>
		public override bool Equals( object obj )
		{
			if( obj is sStrokeStyle ss )
				return Equals( ss );
			return false;
		}
		/// <summary>Determines whether two instances are equal</summary>
		public bool Equals( sStrokeStyle p )
		{
			return ( startCap == p.startCap ) && ( endCap == p.endCap ) && ( lineJoin == p.lineJoin ) && ( miterLimit == p.miterLimit );
		}
		/// <summary>Compute hash code</summary>
		public override int GetHashCode()
		{
			return HashCode.Combine( startCap, endCap, lineJoin, miterLimit );
		}
		/// <summary>Compare for equality</summary>
		public static bool operator ==( sStrokeStyle lhs, sStrokeStyle rhs )
		{
			return lhs.Equals( rhs );
		}
		/// <summary>Compare for inequality</summary>
		public static bool operator !=( sStrokeStyle lhs, sStrokeStyle rhs )
		{
			return !( lhs.Equals( rhs ) );
		}
	}
}