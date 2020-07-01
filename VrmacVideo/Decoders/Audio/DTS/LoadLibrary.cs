using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VrmacVideo.Decoders.Audio.DTS
{
	static class LoadLibrary
	{
		const int RTLD_NOW = 0x002;
		[DllImport( "libdl" )]
		static extern IntPtr dlopen( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string fileName, int flags );

		static LoadLibrary()
		{
			var ass = System.Reflection.Assembly.GetEntryAssembly();
			string rootDir = Path.GetDirectoryName( ass.Location );
			string path = Path.Combine( rootDir, "linux-arm", "libDtsDecoder.so" );

			IntPtr handle = dlopen( path, RTLD_NOW );
			if( handle != IntPtr.Zero )
				return;
			throw new ApplicationException( $"dlopen failed to load libDtsDecoder.so" );
		}

		public static void load()
		{
			// Does nothing, it needs to be there to call static constructor, exactly once.
		}
	}
}