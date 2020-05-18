using ComLight;
using System;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>Setup data for the mesh indexer</summary>
	public struct sMeshIndexerSetup
	{
		/// <summary>Maximum count of vertices you can commit in a single batch.</summary>
		/// <remarks>You can call <see cref="iMeshIndexer.commitBatch(uint)" /> multiple times, building a mesh that has many more vertices than this number.</remarks>
		public uint verticesPerBatch;

		/// <summary>Size of a single vertex, in bytes.</summary>
		public ushort bytesPerVertex;

		/// <summary>These 4*componentsInPosition first bytes of each vertex will be used as a hash map key to unduplicate vertices.</summary>
		/// <remarks>The indexing algorithm doesn’t use floating point math, it treats these values as opaque chunks of 32-bit items.
		/// This means 0.0f and -0.0f are treated as 2 different numbers.</remarks>
		public byte componentsInPosition;
	}

	/// <summary>Mesh optimizer passes</summary>
	/// <seealso href="https://github.com/zeux/meshoptimizer" />
	[Flags]
	public enum eMeshOptimizerFlags: byte
	{
		/// <summary>Vertex cache optimization</summary>
		VertexCache = 1,
		/// <summary>Overdraw optimization</summary>
		Overdraw = 2,
		/// <summary>Vertex fetch optimization</summary>
		VertexFetch = 4,
		/// <summary>All of the above</summary>
		All = 7,
	};

	/// <summary>Structure describing an indexed triangle mesh</summary>
	public struct sIndexedMesh
	{
		/// <summary>Count of indices in the mesh, pass this value to <see cref="DrawIndexedAttribs.NumIndices" /> field.</summary>
		public int countIndices;
		/// <summary>Data type of the index buffer, pass this value to <see cref="DrawIndexedAttribs.IndexType" /> field.</summary>
		/// <remarks>Mesh indexer may generate Uint8, Uint16 or Uint32 indices, the most compact one sufficient for the mesh.</remarks>
		public GpuValueType indexType;
	}

	/// <summary>Utility object to index meshes.</summary>
	[ComInterface( "060ae52a-cb3d-446a-b711-143f3e84a280", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface iMeshIndexer: IDisposable
	{
		/// <summary>Native pointer you can write vertices to</summary>
		IntPtr getPointer();

		/// <summary>Get the details of the configured buffer</summary>
		[RetValIndex] sMeshIndexerSetup getSetup();

		/// <summary>Commit a batch of vertices to the indexer.</summary>
		/// <remarks>This method doesn’t access GPU and can be called from any thread.</remarks>
		/// <param name="vertices">Count of vertices to index. If the value is less than <see cref="sMeshIndexerSetup.verticesPerBatch" />, the indexer will process the initial portion of the buffer.</param>
		void commitBatch( uint vertices );

		/// <summary>Generate smooth normals and append extra float3 field to each vertex.</summary>
		/// <param name="minCosAngle">Minimum cosine of angle between normals of adjacent triangles to smooth the normals. Pass a value greater than 1.0 to de-index the mesh completely.</param>
		/// <remarks>
		/// <para>This method doesn’t access GPU and can be called from any thread.</para>
		/// <para>This method assumes the mesh will have indexed triangles topology, and the first component of vertices is float3 position.</para>
		/// </remarks>
		void generateNormals( float minCosAngle );

		/// <summary>Optimize the mesh using all vertices committed so far.</summary>
		/// <remarks>
		/// <para>This method doesn’t access GPU and can be called from any thread.</para>
		/// <para>When <see cref="eMeshOptimizerFlags.Overdraw" /> bit is set, this method assumes the first component of vertices is float3 position.</para>
		/// </remarks>
		void optimizeMesh( eMeshOptimizerFlags flags, float overdrawThreshold = 1.05f );

		/// <summary>Build indexed mesh in VRAM using all vertices committed so far, and optionally optimize the mesh for vertex cache, overdraw, and vertex fetch.</summary>
		/// <remarks>This method does access GPU, and should only be called on the GUI thread who owns the context.</remarks>
		void createMesh( out IBuffer vertexBuffer, out IBuffer indexBuffer, out sIndexedMesh result,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string vbName = null,
			[MarshalAs( UnmanagedType.LPUTF8Str )] string ibName = null );

		/// <summary>Clear all the data in this object. This way it may be reused to build multiple meshes of the same vertex format.</summary>
		/// <remarks>Reusing the mesh indexer saves non-trivial amount of time wasted on malloc &amp; free.
		/// However, you shouldn’t call indexer methods from different thread in parallel, it will likely crash.
		/// If you have many meshes to index and want to parallelize, create an indexer per background thread.</remarks>
		void clear();
	}

	/// <summary>Extension methods for iMeshIndexer</summary>
	public static class MeshIndexerExt
	{
		/// <summary>Unmanaged buffer of iMeshIndexer wrapped into a Span you can write your vertices</summary>
		public static Span<T> getBuffer<T>( this iMeshIndexer indexer ) where T: unmanaged
		{
			sMeshIndexerSetup setup = indexer.getSetup();
			int expected = setup.bytesPerVertex;
			int actual = Marshal.SizeOf<T>();
			if( expected != actual )
				throw new ArgumentException( $"Vertex size mismatch; indexer expects { expected } bytes / vertex, yet sizeof( { typeof( T ).FullName } ) is { actual }" );

			unsafe
			{
				return new Span<T>( (void*)indexer.getPointer(), (int)setup.verticesPerBatch );
			}
		}
	}
}