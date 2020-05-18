using System;
using System.Runtime.InteropServices;

static class MiscUtils
{
	public static unsafe byte byteFromBool( bool input )
	{
		return *( (byte*)( &input ) );
	}

	/// <summary>Read native string from unmanaged memory, UTF-16 on Windows, or UTF-8 on Linux.</summary>
	public static readonly Func<IntPtr, string> stringFromPointer;

	static MiscUtils()
	{
		if( Environment.OSVersion.Platform == PlatformID.Win32NT )
			stringFromPointer = Marshal.PtrToStringUni;
		else
			stringFromPointer = Marshal.PtrToStringUTF8;
	}
}