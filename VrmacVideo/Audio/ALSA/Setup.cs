using System;
using VrmacVideo.Decoders;
using VrmacVideo.IO.ALSA;

namespace VrmacVideo.Audio.ALSA
{
	static class Setup
	{
		public static IntPtr openDefaultOutput()
		{
			IntPtr result;
			libasound.snd_pcm_open( out result, "default", ePcmStream.Playback, ePcmOpenFlags.NoAutoChannels | ePcmOpenFlags.NonBlocking ).check();
			return result;
		}

		public static void setupHardware( IntPtr pcm, iAudioDecoder decoder, int decodedBuffers )
		{
			Span<byte> hw_params = stackalloc byte[ libasound.pcm_hw_params_sizeof() ];
			libasound.snd_pcm_hw_params_any( pcm, hw_params );

			libasound.snd_pcm_hw_params_set_access( pcm, hw_params, ePcmAccessType.MemoryMapInterleaved );
			libasound.snd_pcm_hw_params_set_format( pcm, hw_params, ePcmFormat.S16 );
			// See ConfigureDecoder.configureAacDecoder method
			libasound.snd_pcm_hw_params_set_channels( pcm, hw_params, 2 );

			int sampleRate = decoder.sampleRate;
			if( libasound.snd_pcm_hw_params_test_rate( pcm, hw_params, sampleRate, eDirection.Exact ) )
			{
				libasound.snd_pcm_hw_params_set_rate( pcm, hw_params, sampleRate, eDirection.Exact );
				Logger.logVerbose( "The audio hardware supports {0}Hz sample rate of the audio track", sampleRate );
			}
			else
			{
				// Enable software resampling
				libasound.snd_pcm_hw_params_set_rate_resample( pcm, hw_params, true );
				libasound.snd_pcm_hw_params_set_rate( pcm, hw_params, sampleRate, eDirection.Exact );
				Logger.logVerbose( "The audio driver will resample {0}Hz sample rate of the audio track", sampleRate );
			}

			libasound.snd_pcm_hw_params_set_period_size( pcm, hw_params, decoder.blockSize, eDirection.Exact );
			libasound.snd_pcm_hw_params_set_buffer_size( pcm, hw_params, decoder.blockSize * decodedBuffers );

			libasound.snd_pcm_hw_params( pcm, hw_params );
		}

		public static void setupSoftware( IntPtr pcm, iAudioDecoder decoder, int decodedBuffers )
		{
			Span<byte> sw_params = stackalloc byte[ libasound.pcm_sw_params_sizeof() ];
			libasound.snd_pcm_sw_params_current( pcm, sw_params );

			libasound.snd_pcm_sw_params_set_avail_min( pcm, sw_params, decoder.blockSize );

			// libasound.snd_pcm_sw_params_set_start_threshold( pcm, sw_params, trackInfo.samplesPerFrame * ( decodedBuffers - 1 ) );
			libasound.snd_pcm_sw_params_set_start_threshold( pcm, sw_params, 0 );
			// libasound.snd_pcm_sw_params_set_period_event( pcm, sw_params, true );
			libasound.snd_pcm_sw_params( pcm, sw_params );
		}
	}
}