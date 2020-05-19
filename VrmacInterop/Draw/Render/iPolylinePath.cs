using ComLight;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Polyline path metadata</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sPolylinePath
	{
		/// <summary>Count of figures in the path.</summary>
		public int figuresCount;
		/// <summary>Combined count of points in all polylines. If no polylines are available, will be 0.</summary>
		public int totalVertices;
		/// <summary>Precision that was used to generate the polylines from paths. If the source path didn’t have any splines, this field will be zero.</summary>
		public float precision;
		/// <summary>Specifies how the intersecting areas of geometries or figures are combined to form the area of the composite geometry.</summary>
		public eFillMode fillMode;
	}

	/// <summary>Metadata for a figure stored in iPolylinePath</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sPolylineFigure
	{
		/// <summary>Count of vertices in the polyline</summary>
		public int vertexCount;
		byte m_filled, m_closed;
		/// <summary>True if the figure is filled</summary>
		public bool isFilled => m_filled != 0;
		/// <summary>True if the figure is closed. Also true if it’s not, but the first vertex is very close to the last one.</summary>
		public bool isClosed => m_closed != 0;
		/// <summary>Pointer to the first vertex, they are of type Vector2; IntPtr.Zero if the figure has no points.</summary>
		public IntPtr firstVertexPointer;
	}

	/// <summary>Filled mesh generation option</summary>
	[Flags]
	public enum eBuildFilledMesh: byte
	{
		/// <summary>Skip filled mesh</summary>
		None = 0,
		/// <summary>Build a filled mesh. You won't get the mesh without this bit set.</summary>
		FilledMesh = 1,
		/// <summary>Do the VAA magic</summary>
		VAA = 2,
		/// <summary>If set, the mesh will have no opaque triangles, the entire mesh will be transparent.</summary>
		BrushHasTransparency = 4,
	}

	/// <summary>Polyline path in native memory</summary>
	[ComInterface( "73376248-4aca-4445-94b0-37256b6e09b9", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iPolylinePath: IDisposable
	{
		/// <summary>Get metadata about this polyline path</summary>
		void getInfo( out sPolylinePath info );
		/// <summary>Metadata about this polyline path</summary>
		sPolylinePath info { get; }

		/// <summary>Get MD4 hash of the polyline points.</summary>
		void getHash( out Guid guid );
		/// <summary>MD4 hash of the payload, use it to compare them. If two polyline paths have same hash, there's extremely high chance they are equal.</summary>
		Guid hash { get; }

		/// <summary>Get the figures data. The input must be a pinned array/span with the length info.figuresCount</summary>
		void getFigures( ref sPolylineFigure figures );

		/// <summary>Build the triangle meshes</summary>
		/// <remarks>The method uses SIMD a lot, on PC it requires SSE3, SSE4.1 and FMA3 support.</remarks>
		void createMesh( iTriangleMesh result, eBuildFilledMesh filledMesh, float pixelSize, IntPtr strokeInfoOrNull );

		/// <summary>Swap two paths</summary>
		void swap( iPolylinePath that );

		/// <summary>Clips the path to viewport, write results into another path.</summary>
		void clip( float strokeWidth, [In] ref Matrix3x2 transform, IntPtr clipRectOrNull, iPolylinePath destPath, out eClipResult clipResult );
	}
}