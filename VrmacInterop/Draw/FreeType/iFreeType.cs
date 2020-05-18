using ComLight;
using System;
using System.IO;

namespace Vrmac.FreeType
{
	/// <summary>COM wrapoper around FreeType.</summary>
	/// <remarks>At the time of writing, Windows build uses version 2.10.2.
	/// Linux version doesn't include FreeType, instead it uses whatever version is installed in the system.</remarks>
	[ComInterface( "c5abc568-a870-4c91-819b-d7330ff5baa7", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface iFreeType: IDisposable
	{
		/// <summary>Create font from .NET stream</summary>
		[RetValIndex] iFont createFont( [ReadStream] Stream stream, string name = null, int faceIndex = 0 );
	}
}