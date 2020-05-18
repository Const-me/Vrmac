using ComLight;
using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Specifies how the intersecting areas of geometries or figures are combined to form the area of the composite geometry.</summary>
	public enum eFillMode: byte
	{
		/// <summary>Determines whether a point is in the fill region by drawing a ray from that point to infinity in any direction, and then counting the number of path segments within the given shape that the ray crosses.
		/// If this number is odd, the point is in the fill region; if even, the point is outside the fill region.</summary>
		Alternate = 0,
		/// <summary>Determines whether a point is in the fill region of the path by drawing a ray from that point to infinity in any direction, and then examining the places where a segment of the shape crosses the ray.
		/// Starting with a count of zero, add one each time a segment crosses the ray from left to right and subtract one each time a path segment crosses the ray from right to left,
		/// as long as left and right are seen from the perspective of the ray.
		/// After counting the crossings, if the result is zero, then the point is outside the path. Otherwise, it is inside the path.</summary>
		Winding = 1,
	}

	/// <summary>Flags for circular arc path segments</summary>
	[Flags]
	public enum eArcFlags: byte
	{
		/// <summary>Arcs are drawn in a counterclockwise (negative-angle) direction</summary>
		DirCounterClockwise = 0,
		/// <summary>Arcs are drawn in a clockwise (positive-angle) direction.</summary>
		DirClockwise = 1,

		/// <summary>An arc's sweep should be 180 degrees or less.</summary>
		ArcSmall = 0,
		/// <summary>An arc's sweep should be 180 degrees or greater.</summary>
		ArcLarge = 2,
	}

	/// <summary>Expose or implements a subset of ID2D1GeometrySink interface</summary>
	[ComInterface( "2cd9069f-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface iGeometrySink
	{
		/// <summary>Creates a line segment between the current point and the specified end point and adds it to the geometry sink.</summary>
		/// <param name="point">The end point of the line to draw.</param>
		void addLine( Vector2 point );

		/// <summary>Adds a single arc to the path geometry.</summary>
		/// <param name="point">The end point of the arc</param>
		/// <param name="size">The x-radius and y-radius of the arc</param>
		/// <param name="angle">A value that specifies angle in radiens, in the clockwise direction the ellipse is rotated relative to the current coordinate system</param>
		/// <param name="flags">Misc. flags</param>
		void addArc( Vector2 point, Vector2 size, float angle, eArcFlags flags );

		/// <summary>Create a cubic Bezier curve between the current point and the specified endpoint.</summary>
		void addBezier( Vector2 point1, Vector2 point2, Vector2 point3 );

		/// <summary>Creates a quadratic Bezier curve between the current point and the specified end point.</summary>
		void addQuadraticBezier( Vector2 point1, Vector2 point2 );

		/// <summary>Starts a new figure at the specified point</summary>
		void beginFigure( Vector2 startPoint, [MarshalAs( UnmanagedType.U1 )] bool filled = true );

		/// <summary>Ends the current figure; optionally, closes it.</summary>
		void endFigure( [MarshalAs( UnmanagedType.U1 )] bool closed = true );

		/// <summary>Gets the method used to determine which points are inside the geometry described by this geometry sink and which points are outside.</summary>
		void getFillMode( out eFillMode fillMode );
		/// <summary>Specifies the method used to determine which points are inside the geometry described by this geometry sink and which points are outside.</summary>
		void setFillMode( eFillMode fillMode );
		/// <summary>The method used to determine which points are inside the geometry described by this geometry sink and which points are outside.</summary>
		eFillMode fillMode { get; set; }
	}
}