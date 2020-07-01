using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac.Utils
{
	/// <summary>Utility class to convert errors logged by C++ code into exception messages.</summary>
	/// <remarks>Some of the public static methods are called from the proxy classes ComLight generates in runtime, because [CustomConventions( typeof( NativeErrorMessages ) )].</remarks>
	public static class NativeErrorMessages
	{
		[ThreadStatic]
		static string lastNativeErrorMessage = null;

		/// <summary>Called by COM runtime before every native method</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void prologue()
		{
			lastNativeErrorMessage = null;
		}

		/// <summary>Called by COM runtime to marshal HRESULT codes into exceptions</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void throwForHR( int hr )
		{
			if( hr >= 0 )
				return;
			throwForHResult( hr );
		}

		/// <summary>Called by COM runtime to marshal HRESULT codes into exceptions, for methods which return booleans.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool throwAndReturnBool( int hr )
		{
			if( hr >= 0 )
				return 0 == hr;
			throwForHResult( hr );
			return false;
		}

		/// <summary>Keep the error message for the current thread. Will be used for exception is the currently running native call will return a failed status code.</summary>
		public static void setNativeErrorMessage( string message )
		{
			lastNativeErrorMessage = message;
		}

		// See getLastHr() inline function in errorHandling.hpp
		const int linuxErrorCode = unchecked((int)0xA0010000);

		// See FACILITY_XCB constant in errorHandling.hpp
		const int xcbErrorCode = unchecked((int)0xA0020000);
		// See FACILITY_WINDOWS_MEDIA constant in errorHandling.hpp
		const int windowsMediaErrorCode = unchecked((int)0xA0030000);

		/// <summary>Bit mask to extract higher 16 bits of HRESULT, with facility, severity, and user bit</summary>
		public const int facilityMask = unchecked((int)0xFFFF0000);

		/// <summary>Error message from HRESULT code</summary>
		public static string customErrorMessage( int hr )
		{
			int mask = hr & facilityMask;
			int code = hr & 0xFFFF;
			string message;

			switch( mask )
			{
				case linuxErrorCode:
					message = LinuxErrors.tryLookup( code );
					if( null != message )
						return $"Linux error code { code }, { message }";
					return $"Undocumented Linux error code { code }";
				case xcbErrorCode:
					message = XcbErrors.tryLookup( code );
					if( null != message )
						return $"XCB error code { code }, { message }";
					return $"Undocumented XCB error code { code }";
				case windowsMediaErrorCode:
					message = WindowsMediaErrors.tryLookup( code );
					if( null != message )
						return $"Windows media error code { code }: { message }";
					return $"Undocumented Windows media error code { code }";
			}
			return null;
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		static void throwForHResult( int hr )
		{
			string printed = lastNativeErrorMessage;
			string custom = customErrorMessage( hr );

			int mask = 0;
			if( !string.IsNullOrWhiteSpace( printed ) )
			{
				printed = printed.Trim();
				mask |= 1;
			}
			if( null != custom )
				mask |= 2;

			switch( mask )
			{
				case 1:
					throw new COMException( printed, hr );
				case 2:
					throw new COMException( custom, hr );
				case 3:
					throw new COMException( $"{ printed }: { custom }", hr );
			}

			ComLight.ErrorCodes.throwForHR( hr );
		}

		/// <summary>Human-readable message in English, from Linux errno result code</summary>
		public static string lookupLinuxError( int code )
		{
			return LinuxErrors.tryLookup( code );
		}
	}
}