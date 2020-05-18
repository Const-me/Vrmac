using System;

namespace Diligent.Graphics
{
	/// <summary>A pair of vertex + index buffers</summary>
	public class IndexedMesh: IDisposable
	{
		/// <summary>Release GPU resources</summary>
		public void Dispose()
		{
			vertexBuffer?.Dispose();
			indexBuffer?.Dispose();
			GC.SuppressFinalize( this );
		}

		readonly IBuffer vertexBuffer, indexBuffer;
		DrawIndexedAttribs drawCall;
		/// <summary>Bounding box of the mesh. Availability of this data depends on how you have created the mesh.</summary>
		public readonly BoundingBox? boundingBox;

		internal IndexedMesh( IBuffer vb, IBuffer ib, int countIndices, GpuValueType indexType, BoundingBox? bbox = null )
		{
			vertexBuffer = vb;
			indexBuffer = ib;
			drawCall = new DrawIndexedAttribs( true );
			drawCall.NumIndices = countIndices;
			drawCall.IndexType = indexType;
			drawCall.Flags = DrawFlags.VerifyAll;
			boundingBox = bbox;
		}

		/// <summary>Render the mesh</summary>
		public void draw( IDeviceContext context )
		{
			context.SetVertexBuffer( 0, vertexBuffer, 0 );
			context.SetIndexBuffer( indexBuffer, 0 );
			context.DrawIndexed( ref drawCall );
		}
	}
}