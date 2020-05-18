using System;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>A native equivalent of <see cref="Version" /></summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sVersionInfo
	{
		/// <summary>Gets the value of the major component of the version number for the current sVersionInfo object.</summary>
		public int major;
		/// <summary>Gets the value of the minor component of the version number for the current sVersionInfo object.</summary>
		public int minor;
		/// <summary>Gets the value of the build component of the version number for the current sVersionInfo object.</summary>
		public int build;
		/// <summary>Gets the value of the revision component of the version number for the current sVersionInfo object.</summary>
		public int revision;
		/// <summary>Convert to version class from .NET</summary>
		public Version asVersion() => new Version( major, minor, build, revision );
	}

	/// <summary>Capability flags of the native library</summary>
	[Flags]
	public enum eCapabilityFlags: uint
	{
		/// <summary>The native library can create windows with 3D rendered content</summary>
		GraphicsWindowed = 1,
		/// <summary>The native library can render stuff to physical monitors.</summary>
		GraphicsFullscreen = 2,
		/// <summary>The library implements iGraphicsEngine.getCurrentDispatcher API</summary>
		ImplementsDispatcher = 4,
	}

	/// <summary>CPU architecture</summary>
	[Flags]
	public enum eArchitecture: uint
	{
		/// <summary>Unknown one, something is probably wrong</summary>
		Unknown = 0,
		/// <summary>32-bit PC</summary>
		x86,
		/// <summary>64-bit PC</summary>
		amd64,
		/// <summary>32-bit ARM, such as Raspberry Pi4 when running a 32-bit Linux</summary>
		arm71,
	}

	/// <summary>Some basic information about the native library</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sNativeLibraryInfo
	{
		sVersionInfo m_version;
		/// <summary>Version of the native DLL</summary>
		public Version version => m_version.asVersion();

		/// <summary>Capability flags specify what this native library can do for ya</summary>
		public eCapabilityFlags capabilityFlags;

		/// <summary>Architecture of the native library</summary>
		public eArchitecture architecture;

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			return $"Version {version}, {architecture}, {capabilityFlags}";
		}
	}
}