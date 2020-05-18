using System;

namespace Vrmac.Draw.Tessellate
{
	struct DequedJob
	{
		public readonly Meshes meshes;
		Options options;
		Guid hash;

		public DequedJob( Meshes meshes )
		{
			this.meshes = meshes;
			options = meshes.options;
			hash = meshes.hashFront;
		}

		public static implicit operator bool( DequedJob x ) => x.meshes != null;

		public bool tessellate( ref Rect clip, iPolylinePath poly )
		{
			return meshes.tessellate( ref options, ref clip, poly, ref hash );
		}

		public bool isSameOptions()
		{
			Options newOptions = meshes.options;
			if( newOptions.equal( ref options ) )
				return true;
			options = newOptions;
			return false;
		}
	}
}