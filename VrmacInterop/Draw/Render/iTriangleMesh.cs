using ComLight;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Metadata about the triangle mesh</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sTriangleMesh
	{
		/// <summary>Count of vertices in the buffers</summary>
		public int vertices;
		/// <summary>Count of opaque triangles in the mesh</summary>
		public int opaqueTriangles;
		/// <summary>Count of transparent triangles in the mesh</summary>
		public int transparentTriangles;
		byte m_vaa;
		/// <summary>true if vertices have VAA magic bytes</summary>
		public bool hasVaa => m_vaa != 0;

		/// <summary>For debugging</summary>
		public override string ToString()
		{
			return $"{ vertices } vertices, { opaqueTriangles } opaque triangles, { transparentTriangles } transparent triangles";
		}
	}

	/// <summary>2D triangle mesh in native memory</summary>
	[ComInterface( "8fe9f2b2-5a55-4c1b-aede-cb404f50292c", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iTriangleMesh: IDisposable
	{
		/// <summary>Metadata about the triangle mesh</summary>
		void getInfo( out sTriangleMesh info );
		/// <summary>Metadata about the triangle mesh</summary>
		sTriangleMesh info { get; }

		/// <summary>Pointer to the first vertex, they are of type Vector2; IntPtr.Zero if the mesh is empty.</summary>
		IntPtr getVertexPointer();
		/// <summary>Pointer to the first opaque triangle, they are of type uint; IntPtr.Zero if the mesh has no opaque triangles.</summary>
		IntPtr getOpaqueIndexPointer();
		/// <summary>Pointer to the first opaque triangle, they are of type uint; IntPtr.Zero if the mesh has no transparent triangles.</summary>
		IntPtr getTransparentIndexPointer();

		/// <summary>Copy vertices out of the mesh, and store them along with the specified ID, and VAA magic bytes if they present.</summary>
		void copyVertices( ref sVertexWithId vertices, uint id );

		/// <summary>Copy opaque index buffer out of the mesh, optionally applying offset to indices (pass 0 for no offset), and optionally pack uint indices into ushort (pass 4 to skip).</summary>
		void copyOpaqueTriangles( IntPtr destinationPointer, int baseVertex, byte bytesPerIndex );

		/// <summary>Copy opaque index buffer out of the mesh, optionally applying offset to indices (pass 0 for no offset), and optionally pack uint indices into ushort (pass 4 to skip).</summary>
		void copyTransparentTriangles( IntPtr destinationPointer, int baseVertex, byte bytesPerIndex );

		/// <summary>Swap two meshes</summary>
		void swap( iTriangleMesh that );

		/// <summary>Clear the path, optionally releasing the memory</summary>
		void clear( [MarshalAs( UnmanagedType.U1 )] bool freeMemory = false );
	}
}