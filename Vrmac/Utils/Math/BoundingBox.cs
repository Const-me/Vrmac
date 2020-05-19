using System;
using System.Numerics;

namespace Vrmac
{
	/// <summary>An axis-aligned bounding box in 3D</summary>
	public struct BoundingBox: IEquatable<BoundingBox>
	{
		/// <summary>Minimum vertex of the box</summary>
		public Vector3 Min;
		/// <summary>The maximum vertex of the box</summary>
		public Vector3 Max;

		/// <summary>Compare for equality</summary>
		public static bool operator ==( BoundingBox a, BoundingBox b )
		{
			return a.Equals( b );
		}
		/// <summary>Compare for inequality</summary>
		public static bool operator !=( BoundingBox a, BoundingBox b )
		{
			return !a.Equals( b );
		}

		/// <summary>Compare for equality</summary>
		public bool Equals( BoundingBox other )
		{
			return ( Min == other.Min ) && ( Max == other.Max );
		}
		/// <summary>Compare for equality</summary>
		public override bool Equals( object obj )
		{
			return ( obj is BoundingBox that ) && Equals( that );
		}

		/// <summary>Compute a hash code</summary>
		public override int GetHashCode()
		{
			return HashCode.Combine( Min, Max );
		}

		/// <summary>Center of the box</summary>
		public Vector3 center => ( Min + Max ) * 0.5f;

		/// <summary>Size of the box</summary>
		public Vector3 size => Max - Min;

		/// <summary>Create union of 2 boxes</summary>
		public static BoundingBox union( BoundingBox a, BoundingBox b )
		{
			return new BoundingBox()
			{
				Min = Vector3.Min( a.Min, b.Min ),
				Max = Vector3.Max( a.Max, b.Max )
			};
		}

		/// <summary>Compute bounding box of a non-empty span of 3D points.</summary>
		public static BoundingBox compute( ReadOnlySpan<Vector3> points )
		{
			if( points.IsEmpty )
				throw new ArgumentException();

			Vector3 i = points[ 0 ];
			Vector3 ax = i;
			foreach( Vector3 pt in points )
			{
				i = Vector3.Min( i, pt );
				ax = Vector3.Max( ax, pt );
			}

			return new BoundingBox()
			{
				Min = i,
				Max = ax
			};
		}

		/// <summary>A string for debugger</summary>
		public override string ToString()
		{
			Vector3 size = this.size;
			return $"[ { Min.X }, { Min.Y }, { Min.Z } ] - [ { Max.X }, { Max.Y }, { Max.Z } ], size [ { size.X }, { size.Y }, { size.Z } ]";
		}
	}
}