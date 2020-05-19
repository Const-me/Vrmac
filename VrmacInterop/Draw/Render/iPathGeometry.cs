using ComLight;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Basic information about the path</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sPathInfo
	{
		/// <summary>Count of figures in the path</summary>
		public int figuresCount;
		/// <summary>Combined count of input data points in all figures.</summary>
		/// <remarks>The polylines length is almost unrelated. Polylines can be much larger for splines built with small precision, or much smaller for filled self-intersecting figures.</remarks>
		public int totalPointsCount;
		/// <summary>Combined count of points in all polylines. If no polylines are available, will be 0.</summary>
		public int totalPolylineVertices;
		/// <summary>Specifies how the intersecting areas of geometries or figures are combined to form the area of the composite geometry.</summary>
		public eFillMode fillMode;
	}

	/// <summary>Combines stroke style and mesh width</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sStrokeInfo: IEquatable<sStrokeInfo>
	{
		/// <summary>Describes the stroke that outlines a shape.</summary>
		public sStrokeStyle style;
		/// <summary>Width of the stroke, in path's units.</summary>
		public float width;

		/// <summary>Construct the structure</summary>
		public sStrokeInfo( sStrokeStyle style, float width )
		{
			this.style = style;
			this.width = width;
		}

		/// <summary>Determines whether two object instances are equal</summary>
		public override bool Equals( object obj )
		{
			if( obj is sStrokeInfo ss )
				return Equals( ss );
			return false;
		}
		/// <summary>Determines whether two instances are equal</summary>
		public bool Equals( sStrokeInfo p )
		{
			return style == p.style && width == p.width;
		}
		/// <summary>Compute hash code</summary>
		public override int GetHashCode()
		{
			return HashCode.Combine( style, width );
		}
		/// <summary>Compare for equality</summary>
		public static bool operator ==( sStrokeInfo lhs, sStrokeInfo rhs )
		{
			return lhs.Equals( rhs );
		}
		/// <summary>Compare for inequality</summary>
		public static bool operator !=( sStrokeInfo lhs, sStrokeInfo rhs )
		{
			return !( lhs.Equals( rhs ) );
		}
	}

	/// <summary>Result of path clipping with viewport</summary>
	public enum eClipResult: byte
	{
		/// <summary>The whole path is outside viewport</summary>
		ClippedOut = 0,
		/// <summary>The path fills entire viewport, no edges are visible</summary>
		FillsEntireViewport = 1,
		/// <summary>Some edges of the path intersected the viewport</summary>
		HasVisibleEdges = 2,
	}

	/// <summary>Path geometry in unmanaged memory.</summary>
	/// <remarks>This interface doesn't access GPU whatsoever, and may be called from any thread. Not concurrently, though.</remarks>
	[ComInterface( "d9569be0-bc0e-4562-b8b1-d2a62eaa3a7a", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iPathGeometry: iGeometry
	{
		/// <summary>Get count of figures and fill mode.</summary>
		void getInfo( out sPathInfo data );
		/// <summary>Get count of figures and fill mode.</summary>
		[Property( "Info" )]
		sPathInfo pathInfo { get; }

		/// <summary>Get various flags about this path</summary>
		void getFlags( out ePathFlags flags );
		/// <summary>Get various flags about this path</summary>
		ePathFlags flags { get; }

		/// <summary>Compute bounding box after the transform is applied to this path</summary>
		/// <param name="transform">Transformation matrix</param>
		/// <remarks>Stroke radius is not included.</remarks>
		[RetValIndex] new Rect getBounds( [In] ref Matrix3x2 transform );

		/// <summary>Same as above but mush faster, should be well under 1µs.</summary>
		/// <remarks>The implementation only uses 8 float values, min/max along XY and 2 diagonals, effectively a bounding octagon.
		/// Provides a conservative estimate, guaranteed to contain the complete geometry. Stroke radius is not included.</remarks>
		/// <param name="transform">Transformation matrix</param>
		/// <param name="strokeWidth">Width of the stroke</param>
		[RetValIndex] Rect getApproximateBounds( [In] ref Matrix3x2 transform, float strokeWidth = 0 );

		/// <summary>True if the transformed approximate bounds intersects with a rectangle.</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		bool ioTestApproximateBounds( [In] ref Matrix3x2 transform, float strokeWidth, IntPtr clipRectOrNull );

		/// <summary>Convert splines into polylines, with specified precision.</summary>
		/// <remarks>For paths made of polygons (without splines), this just calls memcpy couple times.</remarks>
		void buildPolylines( float precision, iPolylinePath polyPath );

		/// <summary>Convert splines into polylines, with specified precision, also clip everything to viewport.</summary>
		/// <remarks>The method uses SIMD a lot. On PC, it requires a CPU supporting SSE4.1 and FMA3 instructions.</remarks>
		void buildPolylines( float precision, float strokeWidth, [In] ref Matrix3x2 transform, IntPtr clipRectOrNull, iPolylinePath polyPath, out eClipResult clipResult );
	}
}