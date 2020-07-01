using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.ALSA
{
	// ==== Hardware related parameters ====
	static partial class libasound
	{
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern UIntPtr snd_pcm_hw_params_sizeof();
		/// <summary>get size of snd_pcm_hw_params_t in bytes</summary>
		public static int pcm_hw_params_sizeof() => (int)snd_pcm_hw_params_sizeof();

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_any( IntPtr pcm, byte* p );
		public static void snd_pcm_hw_params_any( IntPtr pcm, Span<byte> span )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_any( pcm, p ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_set_rate_resample( IntPtr pcm, byte* p, int enable );
		public static void snd_pcm_hw_params_set_rate_resample( IntPtr pcm, Span<byte> span, bool enable )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_set_rate_resample( pcm, p, enable ? 1 : 0 ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_set_channels( IntPtr pcm, byte* p, int count );
		public static void snd_pcm_hw_params_set_channels( IntPtr pcm, Span<byte> span, byte count )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_set_channels( pcm, p, count ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_set_access( IntPtr pcm, byte* p, ePcmAccessType type );
		public static void snd_pcm_hw_params_set_access( IntPtr pcm, Span<byte> span, ePcmAccessType type )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_set_access( pcm, p, type ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_set_format( IntPtr pcm, byte* p, ePcmFormat format );
		public static void snd_pcm_hw_params_set_format( IntPtr pcm, Span<byte> span, ePcmFormat format )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_set_format( pcm, p, format ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_set_rate( IntPtr pcm, byte* p, int val, eDirection dir );
		public static void snd_pcm_hw_params_set_rate( IntPtr pcm, Span<byte> span, int val, eDirection dir )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_set_rate( pcm, p, val, dir ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_set_period_size( IntPtr pcm, byte* p, int val, eDirection dir );
		public static void snd_pcm_hw_params_set_period_size( IntPtr pcm, Span<byte> span, int val, eDirection dir )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_set_period_size( pcm, p, val, dir ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_get_period_size( IntPtr pcm, byte* p, out int val, out eDirection dir );
		public static void snd_pcm_hw_params_get_period_size( IntPtr pcm, Span<byte> span, out int val, out eDirection dir )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_get_period_size( pcm, p, out val, out dir ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_set_buffer_size( IntPtr pcm, byte* p, int val );
		public static void snd_pcm_hw_params_set_buffer_size( IntPtr pcm, Span<byte> span, int val )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params_set_buffer_size( pcm, p, val ).check();
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params_test_rate( IntPtr pcm, byte* p, int val, eDirection dir );
		public static bool snd_pcm_hw_params_test_rate( IntPtr pcm, Span<byte> span, int val, eDirection dir )
		{
			unsafe
			{
				fixed ( byte* p = span )
					return 0 == snd_pcm_hw_params_test_rate( pcm, p, val, dir );
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int snd_pcm_hw_params( IntPtr pcm, byte* p );
		public static void snd_pcm_hw_params( IntPtr pcm, Span<byte> span )
		{
			unsafe
			{
				fixed ( byte* p = span )
					snd_pcm_hw_params( pcm, p ).check();
			}
		}
	}
}