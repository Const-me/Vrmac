using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using sample_t = System.Single;

namespace VrmacVideo.IO.Dolby
{
	// https://github.com/aholtzma/ac3dec/blob/master/doc/liba52.txt
	// Also /usr/include/a52dec/a52.h from there: https://packages.debian.org/buster/armhf/liba52-0.7.4-dev/filelist

	[Flags]
	enum eFrameFlags: int
	{
		Mono = 1,
		Stereo = 2,
		A52_3F = 3,
		A52_2F1R = 4,
		A52_3F1R = 5,
		A52_2F2R = 6,
		A52_3F2R = 7,
		A52_CHANNEL1 = 8,
		A52_CHANNEL2 = 9,
		A52_DOLBY = 10,

		/// <summary>Bit mask to get channels value from the flags</summary>
		ChannelMask = 0xF,
		/// <summary>There is an LFE channel coded in the stream</summary>
		LFE = 0x10,
		/// <summary>A52_ADJUST_LEVEL</summary>
		AdjustLevel = 0x20,
	}

	/// <summary>API of the Dolby AC3 audio software decoder library, shipped by Debian as an OS component.</summary>
	/// <seealso href="https://packages.debian.org/buster/liba52-0.7.4" />
	static class liba52
	{
		// sudo apt-get -y install liba52-0.7.4
		const string dll = "liba52-0.7.4.so";

		// Not exported
		// [DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		// public static extern uint mm_accel();

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr a52_init( uint mm_accel );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr a52_samples( IntPtr state );

		// The returned size is guaranteed to be an even number between 128 and 3840
		public const int minEncodedBuffer = 128;
		public const int maxEncodedBuffer = 3840;

		/// <summary>If the buffer looks like the start of a valid a52 frame, a52_syncinfo() returns the size of the coded frame in bytes,
		/// and fills flags, sampleRate and bitRate with the information encoded in the stream.</summary>
		/// <param name="buf">must contain at least 7 bytes from the input stream</param>
		/// <param name="flags"></param>
		/// <param name="sampleRate"></param>
		/// <param name="bitRate"></param>
		/// <returns>Size of the coded frame in bytes, guaranteed to be an even number between 128 and 3840. Or zero if the data is invalid.</returns>
		/// <remarks>It is recommended to call this function for each frame, for several reasons:
		/// this function detects errors that the other functions will not double-check, consecutive frames might have different lengths, and it helps you re-sync with the stream if you get de-synchronized.</remarks>
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int a52_syncinfo( byte* buffer, out eFrameFlags flags, out int sampleRate, out int bitRate );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static int a52_syncinfo( ReadOnlySpan<byte> span, out eFrameFlags flags, out int sampleRate, out int bitRate )
		{
			unsafe
			{
				fixed ( byte* p = span )
					return a52_syncinfo( p, out flags, out sampleRate, out bitRate );
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int a52_frame( IntPtr state, byte* buffer, ref eFrameFlags flags, float* level, float bias );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void a52_frame( IntPtr state, ReadOnlySpan<byte> span, eFrameFlags flags, byte volume )
		{
			flags |= eFrameFlags.AdjustLevel;
			// Set the bias so the complete range maps to [ 383.0f .. 385.0f ] interval.
			// These particular magic numbers result in 16-bit samples in the lowest 16 bits of the float mantissa: https://www.h-schmidt.net/FloatConverter/IEEE754.html
			// This way we only need a few integer SIMD instructions to convert, very fast.
			// Specifically, AMD64 code uses _mm_sub_epi32 and _mm_packs_epi32, NEON version vsubq_s32, vqmovn_s32 and vcombine_s16.
			// Both _mm_packs_epi32 and vqmovn_s32 instructions use saturation to clip values into the range of int16_t, exactly what we need for PCM audio.
			float level = volume * ( 1.0f / 255.0f );
			float bias = 384;
			unsafe
			{
				fixed ( byte* p = span )
					if( 0 != a52_frame( state, p, ref flags, &level, bias ) )
						throw new ApplicationException( "liba52.a52_frame failed" );
			}
		}

		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate sample_t pfnDynamicRange( sample_t compressionFactor, IntPtr callpackContext );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int a52_dynrng( IntPtr state, pfnDynamicRange callback, IntPtr callpackContext );

		public const int blocksPerFrame = 6;
		public const int samplesPerBlock = 256;
		public const int samplesPerFrame = blocksPerFrame * samplesPerBlock;

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern int a52_block( IntPtr state );

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		public static extern void a52_free( IntPtr state );
	}
}