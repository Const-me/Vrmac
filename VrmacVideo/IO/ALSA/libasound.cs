using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.ALSA
{
	static partial class libasound
	{
		public const string dll = "libasound.so.2";

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern IntPtr snd_strerror( int errnum );

		/// <summary>Returns message for an error code</summary>
		public static string errorMessage( int code )
		{
			IntPtr pointer = snd_strerror( code );
			if( pointer != default )
			{
				string str = Marshal.PtrToStringUTF8( pointer );
				return str;
			}
			return $"error code { code }";
		}

		/// <summary>Handle errors for ALSA functions which say in the documentation "Returns zero if success, otherwise a negative error code"</summary>
		public static void check( this int code, [CallerMemberName] string callerName = "" )
		{
			if( 0 == code )
				return;
			throw new ApplicationException( $"{ callerName } failed: { errorMessage( code ) }" );
		}

		// ==== General PCM functions ====
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_open( out IntPtr pcm, string name, ePcmStream stream, ePcmOpenFlags flags );
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_close( IntPtr pcm );
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_wait( IntPtr pcm, int msTimeout );

		// ==== PCM I/O functions, poll interface ====
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_poll_descriptors_count( IntPtr pcm );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static unsafe extern int snd_pcm_poll_descriptors( IntPtr pcm, pollfd* pfds, int count );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static unsafe extern int snd_pcm_poll_descriptors_revents( IntPtr pcm, pollfd* pfds, int count, out ePollResult result );

		// ==== PCM I/O functions, memory map access interface ====
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_avail_update( IntPtr pcm );
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_avail( IntPtr pcm );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_mmap_begin( IntPtr pcm, out IntPtr areas, out int offset, ref int frames );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_mmap_commit( IntPtr pcm, int offset, int frames );

		// ==== PCM I/O functions, state management ====
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern ePcmState snd_pcm_state( IntPtr pcm );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_start( IntPtr pcm );
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_pause( IntPtr pcm, [MarshalAs( UnmanagedType.Bool )] bool enable );
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_drop( IntPtr pcm );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_prepare( IntPtr pcm );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_recover( IntPtr pcm, int err, int silent );
	}
}