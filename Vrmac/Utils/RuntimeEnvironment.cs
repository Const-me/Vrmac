using Diligent.Graphics;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Operating system</summary>
	public enum eOperatingSystem: byte
	{
		/// <summary>Win32</summary>
		Windows,
		/// <summary>Linux</summary>
		Linux,
	}

	/// <summary>Exposes some basic information about the current runtime environment</summary>
	public static class RuntimeEnvironment
	{
		/// <summary>Operating system</summary>
		public static readonly eOperatingSystem operatingSystem;

		/// <summary>Default render device type; it depends on OS and CPU architecture.</summary>
		public static readonly RenderDeviceType defaultDevice;

		static RuntimeEnvironment()
		{
			operatingSystem = detectOs();
			defaultDevice = pickRenderer();
		}

		static eOperatingSystem detectOs()
		{
			// https://stackoverflow.com/a/38795621/126995
			string windir = Environment.GetEnvironmentVariable( "windir" );
			if( !string.IsNullOrEmpty( windir ) && windir.Contains( @"\" ) && Directory.Exists( windir ) )
				return eOperatingSystem.Windows;

			if( File.Exists( @"/proc/sys/kernel/ostype" ) )
			{
				string osType = File.ReadAllText( @"/proc/sys/kernel/ostype" );
				// Console.WriteLine( "osType = {0}", osType );
				if( osType.StartsWith( "Linux", StringComparison.OrdinalIgnoreCase ) )
					return eOperatingSystem.Linux;
			}

			throw new NotImplementedException( "The platform is not supported." );
		}

		static RenderDeviceType pickRenderer()
		{
			switch( operatingSystem )
			{
				case eOperatingSystem.Windows:
					return RenderDeviceType.D3D12;

				case eOperatingSystem.Linux:
					switch( RuntimeInformation.ProcessArchitecture )
					{
						case Architecture.X86:
						case Architecture.X64:
							return RenderDeviceType.GL;
						case Architecture.Arm:
							return RenderDeviceType.GLES;
						default:
							throw new NotImplementedException( "The CPU architecture is not supported." );
					}
			}
			throw new NotImplementedException( "The platform is not supported." );
		}

		/// <summary>true when running on Windows</summary>
		public static bool runningWindows => operatingSystem == eOperatingSystem.Windows;
		/// <summary>true when running on Linux</summary>
		public static bool runningLinux => operatingSystem == eOperatingSystem.Linux;
	}
}