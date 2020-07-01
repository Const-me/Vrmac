using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.ALSA
{
	// ==== Software related parameters ====
	static partial class libasound
	{
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern UIntPtr snd_pcm_sw_params_sizeof();
		/// <summary>get size of snd_pcm_sw_params_t in bytes</summary>
		public static int pcm_sw_params_sizeof() => (int)snd_pcm_sw_params_sizeof();

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_sw_params_current( IntPtr pcm, byte* p );
		public static void snd_pcm_sw_params_current( IntPtr pcm, Span<byte> span )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_sw_params_current( pcm, p ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_sw_params_set_period_event( IntPtr pcm, byte* p, int enable );
		public static void snd_pcm_sw_params_set_period_event( IntPtr pcm, Span<byte> span, bool enable )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_sw_params_set_period_event( pcm, p, enable ? 1 : 0 ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_sw_params_set_avail_min( IntPtr pcm, byte* p, int value );
		public static void snd_pcm_sw_params_set_avail_min( IntPtr pcm, Span<byte> span, int value )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_sw_params_set_avail_min( pcm, p, value ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_sw_params_set_start_threshold( IntPtr pcm, byte* p, int value );
		public static void snd_pcm_sw_params_set_start_threshold( IntPtr pcm, Span<byte> span, int value )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_sw_params_set_start_threshold( pcm, p, value ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_sw_params( IntPtr pcm, byte* p );
		public static void snd_pcm_sw_params( IntPtr pcm, Span<byte> span )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_sw_params( pcm, p ).check();
			}
		}
	}
}