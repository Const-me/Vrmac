using Diligent.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Vrmac.Utils
{
	static class MeshLoader
	{
		const bool logTime = true;

		static string nameVb( string name )
		{
			if( null == name )
				return null;
			return name + " vertex buffer";
		}
		static string nameIb( string name )
		{
			if( null == name )
				return null;
			return name + " index buffer";
		}

		public static IndexedMesh createIndexed<TVertex, TIndex>( IRenderDevice device, TVertex[] vertices, TIndex[] indices, string name )
			where TVertex : unmanaged
			where TIndex : unmanaged
		{
			// Find index type
			GpuValueType indexType;
			switch( Type.GetTypeCode( typeof( TIndex ) ) )
			{
				// https://stackoverflow.com/a/4902207/126995
				case TypeCode.Int32:
				case TypeCode.UInt32:
					indexType = GpuValueType.Uint32;
					break;
				case TypeCode.Int16:
				case TypeCode.UInt16:
					indexType = GpuValueType.Uint16;
					break;
				case TypeCode.Byte:
				case TypeCode.SByte:
					indexType = GpuValueType.Uint8;
					break;
				default:
					throw new ArgumentException( $"IndexedMesh.create: index type must be an integer, not { typeof( TIndex ).GetType().Name }" );
			}

			// Create vertex buffer
			BufferDesc VertBuffDesc = new BufferDesc( false )
			{
				Usage = Usage.Static,
				BindFlags = BindFlags.VertexBuffer,
			};
			string vbName = nameVb( name );
			var vb = device.CreateBuffer( VertBuffDesc, vertices, vbName );

			// Create index buffer
			BufferDesc IndBuffDesc = new BufferDesc( false )
			{
				Usage = Usage.Static,
				BindFlags = BindFlags.IndexBuffer
			};
			string ibName = nameIb( name );
			var ib = device.CreateBuffer( IndBuffDesc, indices, ibName );

			// Wrap both into the object
			return new IndexedMesh( vb, ib, indices.Length, indexType );
		}

		static int readStlHeader( Stream stream )
		{
			byte[] header = new byte[ 84 ];
			if( 84 != stream.Read( header, 0, 84 ) )
				throw new EndOfStreamException();

			string first5 = null;
			try
			{
				first5 = Encoding.ASCII.GetString( header, 0, 5 );
			}
			catch( Exception ) { }  // Paradoxically, an exception means we're good, the header was not ASCII.

			if( null != first5 )
			{
				if( first5.ToLowerInvariant() == "solid" )
					throw new ArgumentException( "STL loader only supports binary STL files, and the input data is a text one" );
			}

			return BitConverter.ToInt32( header, 80 );
		}

		[StructLayout( LayoutKind.Sequential, Pack = 2 )]
		struct StlTriangle
		{
			public Vector3 normal, a, b, c;
			// Wikipedia thinks it's attribute byte count.
			// Real world STL files contain all sort of garbage there, software just ignores these values.
			// Ignoring, too.
			ushort bs;
		}

		static void copyBatch( Span<Vector3> destSpan, Span<StlTriangle> sourceSpan )
		{
			var src = sourceSpan.GetEnumerator();
			var dest = destSpan.GetEnumerator();
			while( src.MoveNext() )
			{
				dest.MoveNext();
				dest.Current = src.Current.a;

				dest.MoveNext();
				dest.Current = src.Current.b;

				dest.MoveNext();
				dest.Current = src.Current.c;
			}
		}

		static BoundingBox loadStlTriangles( iMeshIndexer indexer, Stream stream, int trianglesCount, int trianglesPerBatch )
		{
			Memory<StlTriangle> buffer = new Memory<StlTriangle>( new StlTriangle[ trianglesPerBatch ] );
			BoundingBox? box = null;

			Span<Vector3> indexerBuffer = indexer.getBuffer<Vector3>();

			while( trianglesCount > 0 )
			{
				int batch = Math.Min( trianglesCount, trianglesPerBatch );

				// Read into the buffer, it's over an array in managed memory
				Memory<StlTriangle> slice = buffer.Slice( 0, batch );
				stream.Read( MemoryMarshal.AsBytes( slice.Span ) );

				// Copy the batch to native memory
				copyBatch( indexerBuffer, slice.Span );

				// Update the box. Using the data from native memory because it has better locality, we only consume 72% of the source data, the rest is normals and BS.
				BoundingBox bb = BoundingBox.compute( indexerBuffer.Slice( 0, batch * 3 ) );
				if( box.HasValue )
					box = BoundingBox.union( box.Value, bb );
				else
					box = bb;

				// Index these vertices
				indexer.commitBatch( (uint)batch * 3 );

				trianglesCount -= batch;
			}
			return box.Value;
		}

		static BoundingBox loadStlImpl( iMeshIndexer indexer, Stream stream, int trianglesCount, int trianglesPerBatch, float? minCosAngle )
		{
			Stopwatch sw;
			if( logTime )
				sw = Stopwatch.StartNew();
			var bbox = loadStlTriangles( indexer, stream, trianglesCount, trianglesPerBatch );
			if( logTime )
				ConsoleLogger.logDebug( "Loading STL and indexing vertices: {0}", sw.Elapsed.print() );
			if( minCosAngle.HasValue )
			{
				if( logTime )
					sw.Restart();
				indexer.generateNormals( minCosAngle.Value );
				if( logTime )
					ConsoleLogger.logDebug( "Generating normals: {0}", sw.Elapsed.print() );
			}
			if( logTime )
				sw.Restart();
			indexer.optimizeMesh( eMeshOptimizerFlags.All );
			if( logTime )
				ConsoleLogger.logDebug( "Optimizing the mesh: {0}", sw.Elapsed.print() );
			return bbox;
		}

		static sMeshIndexerSetup stlIndexerSetup( int trianglesPerBatch )
		{
			sMeshIndexerSetup setup = new sMeshIndexerSetup();
			setup.bytesPerVertex = 12;
			setup.componentsInPosition = 3;
			setup.verticesPerBatch = (uint)trianglesPerBatch * 3;
			return setup;
		}

		public static IndexedMesh loadStl( IRenderDevice device, Stream stream, float? minCosAngle, string name )
		{
			int triangles = readStlHeader( stream );
			if( triangles <= 0 )
				throw new ArgumentException( "Malformed STL file" );

			// BTW, the default threshold for LOH is about 83kb, or 6.9k Vector3 vertices:
			// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap

			int trianglesPerBatch = Math.Min( triangles, 1024 );
			sMeshIndexerSetup setup = stlIndexerSetup( trianglesPerBatch );

			using( var indexer = device.CreateMeshIndexer( ref setup ) )
			{
				BoundingBox bbox = loadStlImpl( indexer, stream, triangles, trianglesPerBatch, minCosAngle );
				indexer.createMesh( out var vb, out var ib, out var metadata, nameVb( name ), nameIb( name ) );
				return new IndexedMesh( vb, ib, metadata.countIndices, metadata.indexType, bbox );
			}
		}

		public static async Task<IndexedMesh> loadStlAsync( IRenderDevice device, Stream stream, float? minCosAngle, string name )
		{
			int triangles = readStlHeader( stream );
			if( triangles <= 0 )
				throw new ArgumentException( "Malformed STL file" );

			int trianglesPerBatch = Math.Min( triangles, 1024 );
			sMeshIndexerSetup setup = stlIndexerSetup( trianglesPerBatch );

			using( var indexer = device.CreateMeshIndexer( ref setup ) )
			{
				Func<BoundingBox> fn = () => loadStlImpl( indexer, stream, triangles, trianglesPerBatch, minCosAngle );
				BoundingBox bbox = await Task.Run( fn );
				indexer.createMesh( out var vb, out var ib, out var metadata, nameVb( name ), nameIb( name ) );
				return new IndexedMesh( vb, ib, metadata.countIndices, metadata.indexType, bbox );
			}
		}
	}
}