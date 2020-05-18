using ComLight;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Allows C++ code to open files by name. Used by iShaderFactory to read shaders source code.</summary>
	/// <remarks>You can return any .NET streams, not just files. If you feel brave, you can even pass TCP streams to C++ code.</remarks>
	[ComInterface( "7c468805-75b1-4ef9-bf8d-a64e12e03e84", eMarshalDirection.ToNative )]
	public interface iStorageFolder
	{
		/// <summary>Open a file for read access.</summary>
		void openRead( [MarshalAs( UnmanagedType.LPUTF8Str )] string name, [ReadStream] out Stream stm );
	}
}