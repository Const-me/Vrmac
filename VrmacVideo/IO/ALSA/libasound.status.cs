using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.ALSA
{
	// ==== Status Functions ====
	static partial class libasound
	{
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern UIntPtr snd_pcm_status_sizeof();

		/// <summary>size of snd_pcm_status_t in bytes</summary>
		public static readonly int statusSizeof = (int)snd_pcm_status_sizeof();

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_status( IntPtr pcm, IntPtr status );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int snd_pcm_status_get_avail( IntPtr status );
	}
}