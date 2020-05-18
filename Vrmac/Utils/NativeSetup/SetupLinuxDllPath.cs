using System.IO;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.NativeSetup
{
	static class SetupLinuxDllPath
	{
		// On Linux there's no SetDllDirectoryW, and no equivalents either.
		// There's LD_LIBRARY_PATH environment variable mentioned in documentation, doesn't work at all.
		// Apparently, it was designed to be set by login or init scripts, as changes to the variable by the current process have no effect on the DLL loader.

		// To workaround, loading the native library by absolute path.
		// Fortunately, unlike Windows which needs the dependent VrmacFT.dll, Linux version of the DLL doesn't have any local dependencies, it only dependents on OS packages.

		const int RTLD_NOW = 0x002;
		[DllImport( "libdl" )]
		static extern IntPtr dlopen( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string fileName, int flags );

		public static void setup( string rootDir )
		{
			string path = Path.Combine( rootDir, "linux-arm", "libVrmac.so" );
			if( !File.Exists( path ) )
				throw new FileNotFoundException( $"Native DLL file \"{ path }\" is missing" );
			IntPtr handle = dlopen( path, RTLD_NOW );
			if( handle != IntPtr.Zero )
				return;
			throw new ApplicationException( $"dlopen failed to load libVrmac.so" );
		}
	}
}