using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.NativeSetup
{
	static class SetupWindowsDllPath
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		static extern bool SetDllDirectoryW( [In, MarshalAs( UnmanagedType.LPWStr )] string lpPathName );

		public static void setup( string rootDir )
		{
			string dir = Path.Combine( rootDir, "win10-x64" );
			if( SetDllDirectoryW( dir ) )
				return;
			throw new Win32Exception( Marshal.GetLastWin32Error(), "SetDllDirectory WinAPI failed" );
		}
	}
}