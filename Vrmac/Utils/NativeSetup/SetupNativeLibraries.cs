using System;
using System.IO;
using Vrmac.Utils.NativeSetup;

namespace Vrmac
{
	/// <summary>Setup unmanaged DLL search path based on the platform.</summary>
	public static class SetupNativeLibraries
	{
		/// <summary>The method does nothing, intentionally so. It’s job is to run the static constructor of this class, exactly once.</summary>
		public static void setup()
		{ }

		static SetupNativeLibraries()
		{
			var ass = System.Reflection.Assembly.GetEntryAssembly();
			string rootDir = Path.GetDirectoryName( ass.Location );

			var os = RuntimeEnvironment.operatingSystem;
			switch( os )
			{
				case eOperatingSystem.Windows:
					if( !Environment.Is64BitProcess )
						throw new ApplicationException( "When running on Windows, Vrmac Graphics requires a 64-bit process." );
					SetupWindowsDllPath.setup( rootDir );
					return;
				case eOperatingSystem.Linux:
					SetupLinuxDllPath.setup( rootDir );
					return;
			}

			throw new NotImplementedException( $"SetupNativeLibraries vailed due to unexpected operating system { (int)os }" );
		}
	}
}