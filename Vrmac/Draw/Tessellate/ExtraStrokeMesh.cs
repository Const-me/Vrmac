using System;

namespace Vrmac.Draw.Tessellate
{
	sealed class ExtraStrokeMesh: iTessellatedMeshes, IDisposable
	{
		readonly Meshes owner;

		public ExtraStrokeMesh( Meshes owner, iVrmacDraw factory )
		{
			this.owner = owner;
			frontBuffer = factory.createTriangleMesh();
			backBuffer = factory.createTriangleMesh();
		}

		public readonly iTriangleMesh frontBuffer, backBuffer;

		iTriangleMesh iTessellatedMeshes.mesh => frontBuffer;

		public sTriangleMesh meshInfo { get; private set; }
		public sDrawCallInfo drawInfo { get; private set; }

		eClipResult iTessellatedMeshes.clipResult => owner.clipResultFront;
		iPathGeometry iTessellatedMeshes.sourcePath => owner.path;

		public void clearBackBuffer() =>
			backBuffer.clear();

		public void Dispose()
		{
			frontBuffer.Dispose();
			backBuffer.Dispose();
		}

		public void flipBuffers()
		{
			frontBuffer.swap( backBuffer );
		}

		public void cacheMeshInfo()
		{
			var i = frontBuffer.info;
			meshInfo = i;
			drawInfo = Meshes.cacheMeshInfo( i );
		}

		public void flushCached()
		{
			drawInfo = default;
			frontBuffer.clear();
			backBuffer.clear();
		}
	}
}