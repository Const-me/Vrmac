
namespace Vrmac.Draw
{
	/* static class MeshSizeEstimate
	{
		public static sMeshDataSize estimateFilledSize( this iPathGeometry path )
		{
			ePathFlags flags = path.flags;
			if( flags.HasFlag( ePathFlags.HasFilledMesh ) )
				return path.getFilledMesh().dataSize;

			var info = path.pathInfo;

			return new sMeshDataSize()
			{
				vertices = info.totalPointsCount,
				triangles = info.totalPointsCount
			};
		}

		public static sMeshDataSize estimateStrokedSize( this iPathGeometry path )
		{
			var info = path.pathInfo;
			int segmentsEst = info.totalPolylineVertices;
			if( 0 == segmentsEst )
				segmentsEst = info.totalPointsCount;

			return new sMeshDataSize()
			{
				vertices = segmentsEst * 2,
				triangles = segmentsEst * 2
			};
		}
	} */
}