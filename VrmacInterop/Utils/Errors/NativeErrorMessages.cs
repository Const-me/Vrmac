using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac.Utils
{
	/// <summary>Utility class to convert errors logged by C++ code into exception messages.</summary>
	/// <remarks>The public static methods are called from the proxy classes ComLight generates in runtime.</remarks>
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

		const int facilityMask = unchecked((int)0xFFFF0000);

		internal static string customErrorMessage( int hr )
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
	}
}