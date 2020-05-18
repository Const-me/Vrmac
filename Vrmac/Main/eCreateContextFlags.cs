using System;

namespace Vrmac
{
	/// <summary>Flags for the constructor of <see cref="Context" /> class.</summary>
	[Flags]
	public enum eCreateContextFlags: byte
	{
		/// <summary>None of the below</summary>
		None = 0,
		/// <summary>Cache compiler shaders in a ZIP file under %TEMP% on Windows or ~/.cache on Linux.</summary>
		/// <remarks>Raspberry Pi4 GPU driver doesn't implement the required functionality for shaders cache to work. The GL_NUM_PROGRAM_BINARY_FORMATS value is zero.
		/// Very unfortunate because with the 2 compilers, HLSL-to-GLSL and then GLSL to byte code, caching would help a lot with startup times.</remarks>
		CacheCompiledShaders = 1,

		/// <summary>Use the built-in 2D engine even on Windows. Without this flag, the library will instead use Direct2D on Windows. The flag has no effect on Linux.</summary>
		PreferVrmac2D = 2,
	}
}