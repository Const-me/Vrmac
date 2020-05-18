using System;

namespace Vrmac.Draw
{
	/// <summary>Combines sStrokeStyle with line width</summary>
	struct StrokeStyleW: IEquatable<StrokeStyleW>
	{
		public readonly sStrokeStyle strokeStyle;
		public readonly float strokeWidth;

		public StrokeStyleW( sStrokeStyle strokeStyle, float strokeWidth )
		{
			this.strokeStyle = strokeStyle;
			this.strokeWidth = strokeWidth;
		}

		/// <summary>Determines whether two object instances are equal</summary>
		public override bool Equals( object obj )
		{
			if( obj is sStrokeStyle ss )
				return Equals( ss );
			return false;
		}
		/// <summary>Determines whether two instances are equal</summary>
		public bool Equals( StrokeStyleW p )
		{
			return strokeStyle == p.strokeStyle && strokeWidth == p.strokeWidth;
		}
		/// <summary>Compute hash code</summary>
		public override int GetHashCode()
		{
			return HashCode.Combine( strokeStyle, strokeWidth );
		}
		/// <summary>Compare for equality</summary>
		public static bool operator ==( StrokeStyleW lhs, StrokeStyleW rhs )
		{
			return lhs.Equals( rhs );
		}
		/// <summary>Compare for inequality</summary>
		public static bool operator !=( StrokeStyleW lhs, StrokeStyleW rhs )
		{
			return !( lhs.Equals( rhs ) );
		}
	}
}