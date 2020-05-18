using ComLight;
using Diligent.Graphics;
using System;
using Vrmac.Draw;
using Matrix = Vrmac.Draw.Matrix;

namespace Vrmac.Direct2D
{
	/// <summary>Represents a geometry resource</summary>
	[ComInterface( "2cd906a1-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface iGeometry: Draw.iGeometry
	{
		/// <summary>ID2D1Geometry COM pointer</summary>
		IntPtr getNativePointer();

		/// <summary>Get geometry bounding box, after the specified transform is applied</summary>
		[RetValIndex] new Rect getBounds( ref Matrix transform );
	}

	/// <summary>Describes a two-dimensional rectangle</summary>
	[ComInterface( "2cd906a2-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface iRectangleGeometry: iGeometry
	{
		/// <summary>ID2D1RectangleGeometry COM pointer</summary>
		new IntPtr getNativePointer();

		/// <summary>Get geometry bounding box, after the specified transform is applied</summary>
		[RetValIndex] new Rect getBounds( ref Matrix transform );

		/// <summary>Retrieves the rectangle that describes the rectangle geometry's dimensions.</summary>
		void getRectangle( out Rect rect );
		/// <summary>Retrieves the rectangle that describes the rectangle geometry's dimensions.</summary>
		Rect rectangle { get; }
	}

	/// <summary>Represents a complex shape that may be composed of arcs, curves, and lines.</summary>
	[ComInterface( "2cd906a5-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface iPathGeometry: iGeometry
	{
		/// <summary>ID2D1PathGeometry COM pointer</summary>
		new IntPtr getNativePointer();

		/// <summary>Get geometry bounding box, after the specified transform is applied</summary>
		[RetValIndex] new Rect getBounds( ref Matrix transform );
	}

	/// <summary>Encapsulates a device- and transform-dependent representation of a filled or stroked geometry.</summary>
	[ComInterface( "a16907d7-bc02-4801-99e8-8cf7f485f774", eMarshalDirection.ToManaged )]
	public interface iGeometryRealization: Draw.iGeometryRealization
	{
		/// <summary>ID2D1GeometryRealization COM pointer</summary>
		IntPtr getNativePointer();
	}

	/// <summary>Represents a set of vertices that form a list of triangles.</summary>
	[ComInterface( "2cd906c2-12e2-11dc-9fed-001143a055f9", eMarshalDirection.ToManaged )]
	public interface id2Mesh: iTriangleMesh
	{
		/// <summary>ID2D1Mesh COM pointer</summary>
		IntPtr getNativePointer();
	}
}