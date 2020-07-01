using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.ALSA
{
	// ==== Debugging support ====
	static partial class libasound
	{
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_output_stdio_open( out IntPtr output, [MarshalAs( UnmanagedType.LPUTF8Str )] string file, [MarshalAs( UnmanagedType.LPUTF8Str )] string mode );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_output_close( IntPtr output );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern int snd_pcm_dump( IntPtr pcm, IntPtr output );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern int snd_pcm_dump_setup( IntPtr pcm, IntPtr output );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_status_dump( IntPtr status, IntPtr output );

		public static void pcm_dump( IntPtr pcm, string path )
		{
			snd_output_stdio_open( out var output, path, "w" ).check();
			try
			{
				snd_pcm_dump( pcm, output ).check();
			}
			finally
			{
				snd_output_close( output );
			}
		}

		public static void pcm_dump_setup( IntPtr pcm, string path )
		{
			snd_output_stdio_open( out var output, path, "w" ).check();
			try
			{
				snd_pcm_dump_setup( pcm, output ).check();
			}
			finally
			{
				snd_output_close( output );
			}
		}
	}
}