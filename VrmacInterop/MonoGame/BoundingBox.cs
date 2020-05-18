// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member

namespace Diligent.Graphics
{
	[StructLayout( LayoutKind.Sequential ), DataContract, DebuggerDisplay( "{DebugDisplayString,nq}" )]
	public struct BoundingBox: IEquatable<BoundingBox>
	{
		[DataMember]
		public Vector3 Min;

		[DataMember]
		public Vector3 Max;

		public const int CornerCount = 8;

		public BoundingBox( Vector3 min, Vector3 max )
		{
			Min = min;
			Max = max;
		}

		public ContainmentType Contains( BoundingBox box )
		{
			//test if all corner is in the same side of a face by just checking min and max
			if( box.Max.X < Min.X
				|| box.Min.X > Max.X
				|| box.Max.Y < Min.Y
				|| box.Min.Y > Max.Y
				|| box.Max.Z < Min.Z
				|| box.Min.Z > Max.Z )
				return ContainmentType.Disjoint;

			if( box.Min.X >= Min.X
				&& box.Max.X <= Max.X
				&& box.Min.Y >= Min.Y
				&& box.Max.Y <= Max.Y
				&& box.Min.Z >= Min.Z
				&& box.Max.Z <= Max.Z )
				return ContainmentType.Contains;

			return ContainmentType.Intersects;
		}

		public void Contains( ref BoundingBox box, out ContainmentType result )
		{
			result = Contains( box );
		}

		public ContainmentType Contains( BoundingFrustum frustum )
		{
			//TODO: bad done here need a fix. 
			//Because question is not frustum contain box but reverse and this is not the same
			int i;
			ContainmentType contained;
			Vector3[] corners = frustum.GetCorners();

			// First we check if frustum is in box
			for( i = 0; i < corners.Length; i++ )
			{
				Contains( ref corners[ i ], out contained );
				if( contained == ContainmentType.Disjoint )
					break;
			}

			if( i == corners.Length ) // This means we checked all the corners and they were all contain or instersect
				return ContainmentType.Contains;

			if( i != 0 )             // if i is not equal to zero, we can fastpath and say that this box intersects
				return ContainmentType.Intersects;

			// If we get here, it means the first (and only) point we checked was actually contained in the frustum.
			// So we assume that all other points will also be contained. If one of the points is disjoint, we can
			// exit immediately saying that the result is Intersects
			i++;
			for( ; i < corners.Length; i++ )
			{
				Contains( ref corners[ i ], out contained );
				if( contained != ContainmentType.Contains )
					return ContainmentType.Intersects;

			}

			// If we get here, then we know all the points were actually contained, therefore result is Contains
			return ContainmentType.Contains;
		}

		public ContainmentType Contains( BoundingSphere sphere )
		{
			if( sphere.Center.X - Min.X >= sphere.Radius
				&& sphere.Center.Y - Min.Y >= sphere.Radius
				&& sphere.Center.Z - Min.Z >= sphere.Radius
				&& Max.X - sphere.Center.X >= sphere.Radius
				&& Max.Y - sphere.Center.Y >= sphere.Radius
				&& Max.Z - sphere.Center.Z >= sphere.Radius )
				return ContainmentType.Contains;

			double dmin = 0;

			double e = sphere.Center.X - Min.X;
			if( e < 0 )
			{
				if( e < -sphere.Radius )
				{
					return ContainmentType.Disjoint;
				}
				dmin += e * e;
			}
			else
			{
				e = sphere.Center.X - Max.X;
				if( e > 0 )
				{
					if( e > sphere.Radius )
					{
						return ContainmentType.Disjoint;
					}
					dmin += e * e;
				}
			}

			e = sphere.Center.Y - Min.Y;
			if( e < 0 )
			{
				if( e < -sphere.Radius )
				{
					return ContainmentType.Disjoint;
				}
				dmin += e * e;
			}
			else
			{
				e = sphere.Center.Y - Max.Y;
				if( e > 0 )
				{
					if( e > sphere.Radius )
					{
						return ContainmentType.Disjoint;
					}
					dmin += e * e;
				}
			}

			e = sphere.Center.Z - Min.Z;
			if( e < 0 )
			{
				if( e < -sphere.Radius )
				{
					return ContainmentType.Disjoint;
				}
				dmin += e * e;
			}
			else
			{
				e = sphere.Center.Z - Max.Z;
				if( e > 0 )
				{
					if( e > sphere.Radius )
					{
						return ContainmentType.Disjoint;
					}
					dmin += e * e;
				}
			}

			if( dmin <= sphere.Radius * sphere.Radius )
				return ContainmentType.Intersects;

			return ContainmentType.Disjoint;
		}

		public void Contains( ref BoundingSphere sphere, out ContainmentType result )
		{
			result = Contains( sphere );
		}

		public ContainmentType Contains( Vector3 point )
		{
			ContainmentType result;
			Contains( ref point, out result );
			return result;
		}

		public void Contains( ref Vector3 point, out ContainmentType result )
		{
			//first we get if point is out of box
			if( point.X < Min.X
				|| point.X > Max.X
				|| point.Y < Min.Y
				|| point.Y > Max.Y
				|| point.Z < Min.Z
				|| point.Z > Max.Z )
			{
				result = ContainmentType.Disjoint;
			}
			else
			{
				result = ContainmentType.Contains;
			}
		}

		static readonly Vector3 MaxVector3 = new Vector3( float.MaxValue );
		static readonly Vector3 MinVector3 = new Vector3( float.MinValue );

		/// <summary>Create a bounding box from the given list of points.</summary>
		/// <param name="points">The array of Vector3 instances defining the point cloud to bound</param>
		/// <param name="index">The base index to start iterating from</param>
		/// <param name="count">The number of points to iterate</param>
		/// <returns>A bounding box that encapsulates the given point cloud.</returns>
		/// <exception cref="ArgumentException">Thrown if the given array is null or has no points.</exception>
		public static BoundingBox CreateFromPoints( Vector3[] points, int index = 0, int count = -1 )
		{
			if( points == null || points.Length == 0 )
				throw new ArgumentException();

			if( count == -1 )
				count = points.Length;

			var minVec = MaxVector3;
			var maxVec = MinVector3;
			for( int i = index; i < count; i++ )
			{
				minVec.X = ( minVec.X < points[ i ].X ) ? minVec.X : points[ i ].X;
				minVec.Y = ( minVec.Y < points[ i ].Y ) ? minVec.Y : points[ i ].Y;
				minVec.Z = ( minVec.Z < points[ i ].Z ) ? minVec.Z : points[ i ].Z;

				maxVec.X = ( maxVec.X > points[ i ].X ) ? maxVec.X : points[ i ].X;
				maxVec.Y = ( maxVec.Y > points[ i ].Y ) ? maxVec.Y : points[ i ].Y;
				maxVec.Z = ( maxVec.Z > points[ i ].Z ) ? maxVec.Z : points[ i ].Z;
			}

			return new BoundingBox( minVec, maxVec );
		}

		/// <summary>Create a bounding box from the given list of points.</summary>
		/// <param name="points">The list of Vector3 instances defining the point cloud to bound</param>
		/// <param name="index">The base index to start iterating from</param>
		/// <param name="count">The number of points to iterate</param>
		/// <returns>A bounding box that encapsulates the given point cloud.</returns>
		/// <exception cref="ArgumentException">Thrown if the given list is null or has no points.</exception>
		public static BoundingBox CreateFromPoints( List<Vector3> points, int index = 0, int count = -1 )
		{
			if( points == null || points.Count == 0 )
				throw new ArgumentException();

			if( count == -1 )
				count = points.Count;

			var minVec = MaxVector3;
			var maxVec = MinVector3;
			for( int i = index; i < count; i++ )
			{
				minVec.X = ( minVec.X < points[ i ].X ) ? minVec.X : points[ i ].X;
				minVec.Y = ( minVec.Y < points[ i ].Y ) ? minVec.Y : points[ i ].Y;
				minVec.Z = ( minVec.Z < points[ i ].Z ) ? minVec.Z : points[ i ].Z;

				maxVec.X = ( maxVec.X > points[ i ].X ) ? maxVec.X : points[ i ].X;
				maxVec.Y = ( maxVec.Y > points[ i ].Y ) ? maxVec.Y : points[ i ].Y;
				maxVec.Z = ( maxVec.Z > points[ i ].Z ) ? maxVec.Z : points[ i ].Z;
			}

			return new BoundingBox( minVec, maxVec );
		}

		/// <summary>Create a bounding box from the given list of points.</summary>
		/// <param name="points">The list of Vector3 instances defining the point cloud to bound</param>
		/// <returns>A bounding box that encapsulates the given point cloud.</returns>
		/// <exception cref="ArgumentException">Thrown if the given list has no points.</exception>
		public static BoundingBox CreateFromPoints( IEnumerable<Vector3> points )
		{
			if( points == null )
				throw new ArgumentNullException();

			var empty = true;
			var minVec = MaxVector3;
			var maxVec = MinVector3;
			foreach( var ptVector in points )
			{
				minVec.X = ( minVec.X < ptVector.X ) ? minVec.X : ptVector.X;
				minVec.Y = ( minVec.Y < ptVector.Y ) ? minVec.Y : ptVector.Y;
				minVec.Z = ( minVec.Z < ptVector.Z ) ? minVec.Z : ptVector.Z;

				maxVec.X = ( maxVec.X > ptVector.X ) ? maxVec.X : ptVector.X;
				maxVec.Y = ( maxVec.Y > ptVector.Y ) ? maxVec.Y : ptVector.Y;
				maxVec.Z = ( maxVec.Z > ptVector.Z ) ? maxVec.Z : ptVector.Z;

				empty = false;
			}
			if( empty )
				throw new ArgumentException();

			return new BoundingBox( minVec, maxVec );
		}

		/// <summary>Create a bounding box from the given list of points.</summary>
		public static BoundingBox CreateFromPoints( Span<Vector3> points )
		{
			var empty = true;
			var minVec = MaxVector3;
			var maxVec = MinVector3;
			foreach( var ptVector in points )
			{
				minVec.X = ( minVec.X < ptVector.X ) ? minVec.X : ptVector.X;
				minVec.Y = ( minVec.Y < ptVector.Y ) ? minVec.Y : ptVector.Y;
				minVec.Z = ( minVec.Z < ptVector.Z ) ? minVec.Z : ptVector.Z;

				maxVec.X = ( maxVec.X > ptVector.X ) ? maxVec.X : ptVector.X;
				maxVec.Y = ( maxVec.Y > ptVector.Y ) ? maxVec.Y : ptVector.Y;
				maxVec.Z = ( maxVec.Z > ptVector.Z ) ? maxVec.Z : ptVector.Z;

				empty = false;
			}
			if( empty )
				throw new ArgumentException();

			return new BoundingBox( minVec, maxVec );
		}

		public static BoundingBox CreateFromSphere( BoundingSphere sphere )
		{
			BoundingBox result;
			CreateFromSphere( ref sphere, out result );
			return result;
		}

		public static void CreateFromSphere( ref BoundingSphere sphere, out BoundingBox result )
		{
			var corner = new Vector3( sphere.Radius );
			result.Min = sphere.Center - corner;
			result.Max = sphere.Center + corner;
		}

		public static BoundingBox CreateMerged( BoundingBox original, BoundingBox additional )
		{
			BoundingBox result;
			CreateMerged( ref original, ref additional, out result );
			return result;
		}

		public static void CreateMerged( ref BoundingBox original, ref BoundingBox additional, out BoundingBox result )
		{
			result.Min.X = Math.Min( original.Min.X, additional.Min.X );
			result.Min.Y = Math.Min( original.Min.Y, additional.Min.Y );
			result.Min.Z = Math.Min( original.Min.Z, additional.Min.Z );
			result.Max.X = Math.Max( original.Max.X, additional.Max.X );
			result.Max.Y = Math.Max( original.Max.Y, additional.Max.Y );
			result.Max.Z = Math.Max( original.Max.Z, additional.Max.Z );
		}

		public bool Equals( BoundingBox other )
		{
			return ( Min == other.Min ) && ( Max == other.Max );
		}

		public override bool Equals( object obj )
		{
			return ( obj is BoundingBox ) ? Equals( (BoundingBox)obj ) : false;
		}

		public Vector3[] GetCorners()
		{
			return new Vector3[] {
				new Vector3(Min.X, Max.Y, Max.Z),
				new Vector3(Max.X, Max.Y, Max.Z),
				new Vector3(Max.X, Min.Y, Max.Z),
				new Vector3(Min.X, Min.Y, Max.Z),
				new Vector3(Min.X, Max.Y, Min.Z),
				new Vector3(Max.X, Max.Y, Min.Z),
				new Vector3(Max.X, Min.Y, Min.Z),
				new Vector3(Min.X, Min.Y, Min.Z)
			};
		}

		public void GetCorners( Vector3[] corners )
		{
			if( corners == null )
			{
				throw new ArgumentNullException( "corners" );
			}
			if( corners.Length < 8 )
			{
				throw new ArgumentOutOfRangeException( "corners", "Not Enought Corners" );
			}
			corners[ 0 ].X = Min.X;
			corners[ 0 ].Y = Max.Y;
			corners[ 0 ].Z = Max.Z;
			corners[ 1 ].X = Max.X;
			corners[ 1 ].Y = Max.Y;
			corners[ 1 ].Z = Max.Z;
			corners[ 2 ].X = Max.X;
			corners[ 2 ].Y = Min.Y;
			corners[ 2 ].Z = Max.Z;
			corners[ 3 ].X = Min.X;
			corners[ 3 ].Y = Min.Y;
			corners[ 3 ].Z = Max.Z;
			corners[ 4 ].X = Min.X;
			corners[ 4 ].Y = Max.Y;
			corners[ 4 ].Z = Min.Z;
			corners[ 5 ].X = Max.X;
			corners[ 5 ].Y = Max.Y;
			corners[ 5 ].Z = Min.Z;
			corners[ 6 ].X = Max.X;
			corners[ 6 ].Y = Min.Y;
			corners[ 6 ].Z = Min.Z;
			corners[ 7 ].X = Min.X;
			corners[ 7 ].Y = Min.Y;
			corners[ 7 ].Z = Min.Z;
		}

		public override int GetHashCode()
		{
			return Min.GetHashCode() + Max.GetHashCode();
		}

		public bool Intersects( BoundingBox box )
		{
			bool result;
			Intersects( ref box, out result );
			return result;
		}

		public void Intersects( ref BoundingBox box, out bool result )
		{
			if( ( Max.X >= box.Min.X ) && ( Min.X <= box.Max.X ) )
			{
				if( ( Max.Y < box.Min.Y ) || ( Min.Y > box.Max.Y ) )
				{
					result = false;
					return;
				}

				result = ( Max.Z >= box.Min.Z ) && ( Min.Z <= box.Max.Z );
				return;
			}

			result = false;
			return;
		}

		public bool Intersects( BoundingFrustum frustum )
		{
			return frustum.Intersects( this );
		}

		public bool Intersects( BoundingSphere sphere )
		{
			bool result;
			Intersects( ref sphere, out result );
			return result;
		}

		public void Intersects( ref BoundingSphere sphere, out bool result )
		{
			var squareDistance = 0.0f;
			var point = sphere.Center;
			if( point.X < Min.X ) squareDistance += ( Min.X - point.X ) * ( Min.X - point.X );
			if( point.X > Max.X ) squareDistance += ( point.X - Max.X ) * ( point.X - Max.X );
			if( point.Y < Min.Y ) squareDistance += ( Min.Y - point.Y ) * ( Min.Y - point.Y );
			if( point.Y > Max.Y ) squareDistance += ( point.Y - Max.Y ) * ( point.Y - Max.Y );
			if( point.Z < Min.Z ) squareDistance += ( Min.Z - point.Z ) * ( Min.Z - point.Z );
			if( point.Z > Max.Z ) squareDistance += ( point.Z - Max.Z ) * ( point.Z - Max.Z );
			result = squareDistance <= sphere.Radius * sphere.Radius;
		}

		public PlaneIntersectionType Intersects( Plane plane )
		{
			PlaneIntersectionType result;
			Intersects( ref plane, out result );
			return result;
		}

		public void Intersects( ref Plane plane, out PlaneIntersectionType result )
		{
			// See http://zach.in.tu-clausthal.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html

			Vector3 positiveVertex;
			Vector3 negativeVertex;

			if( plane.Normal.X >= 0 )
			{
				positiveVertex.X = Max.X;
				negativeVertex.X = Min.X;
			}
			else
			{
				positiveVertex.X = Min.X;
				negativeVertex.X = Max.X;
			}

			if( plane.Normal.Y >= 0 )
			{
				positiveVertex.Y = Max.Y;
				negativeVertex.Y = Min.Y;
			}
			else
			{
				positiveVertex.Y = Min.Y;
				negativeVertex.Y = Max.Y;
			}

			if( plane.Normal.Z >= 0 )
			{
				positiveVertex.Z = Max.Z;
				negativeVertex.Z = Min.Z;
			}
			else
			{
				positiveVertex.Z = Min.Z;
				negativeVertex.Z = Max.Z;
			}

			// Inline Vector3.Dot(plane.Normal, negativeVertex) + plane.D;
			var distance = plane.Normal.X * negativeVertex.X + plane.Normal.Y * negativeVertex.Y + plane.Normal.Z * negativeVertex.Z + plane.D;
			if( distance > 0 )
			{
				result = PlaneIntersectionType.Front;
				return;
			}

			// Inline Vector3.Dot(plane.Normal, positiveVertex) + plane.D;
			distance = plane.Normal.X * positiveVertex.X + plane.Normal.Y * positiveVertex.Y + plane.Normal.Z * positiveVertex.Z + plane.D;
			if( distance < 0 )
			{
				result = PlaneIntersectionType.Back;
				return;
			}

			result = PlaneIntersectionType.Intersecting;
		}

		public float? Intersects( Ray ray )
		{
			return ray.Intersects( this );
		}

		public void Intersects( ref Ray ray, out float? result )
		{
			result = Intersects( ray );
		}

		public static bool operator ==( BoundingBox a, BoundingBox b )
		{
			return a.Equals( b );
		}

		public static bool operator !=( BoundingBox a, BoundingBox b )
		{
			return !a.Equals( b );
		}

		internal string DebugDisplayString
		{
			get
			{
				return string.Concat(
					"Min( ", Min.DebugDisplayString, " )  \r\n",
					"Max( ", Max.DebugDisplayString, " )"
					);
			}
		}

		public override string ToString()
		{
			return "{{Min:" + Min.ToString() + " Max:" + Max.ToString() + "}}";
		}

		/// <summary>Deconstruction method for <see cref="BoundingBox"/>.</summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public void Deconstruct( out Vector3 min, out Vector3 max )
		{
			min = Min;
			max = Max;
		}

		/// <summary>Center of the box</summary>
		public Vector3 center => ( Min + Max ) * 0.5f;

		/// <summary>Size of the box</summary>
		public Vector3 size => Max - Min;
	}
}