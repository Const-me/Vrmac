using ComLight;
using Diligent.Graphics;
using System.Runtime.InteropServices;
using Vrmac.FreeType;

namespace Vrmac.Draw
{
	/// <summary>This class serves as a factory for Vrmac-implemented 2D drawing stuff.</summary>
	[ComInterface( "d9569be0-bc0e-4562-b8b1-d2a62eaa3a7a", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iVrmacDraw
	{
		/// <summary>Creates path geometry object and initialize it with the data.</summary>
		[RetValIndex]
		iPathGeometry createPathGeometry( [In] ref float points, [In] ref sPathSegment segments, [In] ref sPathFigure figures, sPathData pathData );

		/// <summary>Create empty polyline path object</summary>
		/// <remarks>Unlike paths, polyline paths are mutable, you should reuse them to save the cost of malloc/free.</remarks>
		[RetValIndex]
		iPolylinePath createPolylinePath();

		/// <summary>Create empty triangle mesh object</summary>
		/// <remarks>Unlike paths, triangle meshes are mutable, you should reuse them to save the cost of malloc/free.</remarks>
		[RetValIndex]
		iTriangleMesh createTriangleMesh();

		/// <summary>Create an empty texture atlas.</summary>
		/// <remarks>The initial size is small, and it will only have a single layer. Will grow by powers of 2 as needed.</remarks>
		[RetValIndex]
		iTextureAtlas createTextureAtlas( IRenderDevice device, eTextureAtlasFormat pixelFormat );

		/// <summary>Load FreeType 2 native library.</summary>
		/// <remarks>On Windows this will load the included VrmacFT.dll. On Linux however this will load FreeType installed in the OS, or fail is the required package is missing.</remarks>
		/// <seealso href="https://packages.debian.org/buster/libfreetype6" />
		[RetValIndex]
		iFreeType loadFreeType();
	}
}