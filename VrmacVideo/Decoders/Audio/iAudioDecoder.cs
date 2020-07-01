using System;
using VrmacVideo.Audio;
using VrmacVideo.Decoders.Audio;

namespace VrmacVideo.Decoders
{
	interface iAudioDecoder: IDisposable
	{
		int sampleRate { get; }

		/// <summary>Output audio channels. Always 2 i.e. stereo, the decoders are expected to downmix internally.</summary>
		byte channelsCount { get; }

		/// <summary>Samples per block; 256 for Dolby, 512 for DTS, variable number for AAC because the AAC decoder handles all of them, HE-AAC included.</summary>
		int blockSize { get; }

		/// <summary>True if the encoded buffer can be recycled immediately after <see cref="decodeFrame" />.
		/// False if the encoded data must stay at that memory address until all blocks from that frame are decoded by <see cref="decodeBlock" />.</summary>
		/// <remarks>Generally speaking, zero-copy decoders are more efficient.</remarks>
		bool copiesCompressedData { get; }

		/// <summary>If the data lacks the required headers, will throw an exception. Otherwise returns frame size.</summary>
		int sync( ReadOnlySpan<byte> data );

		/// <summary>Decode a frame while applying volume</summary>
		void decodeFrame( ReadOnlySpan<byte> data, byte volume );

		/// <summary>Count of pending blocks in the decoded frame</summary>
		/// <remarks>For AAC either 0 (need another frame) or 1 (decoded one and can supply the block).
		/// For Dolby, decodeFrame makes it 6.
		/// For DTS, decodeFrame writes a number there which depends on the stream.</remarks>
		int blocksLeft { get; }

		/// <summary>Produce interleaved PCM samples, decrement blocksLeft by 1.</summary>
		void decodeBlock( Span<short> pcm );
	}

	/// <summary>Class factory for audio decoders</summary>
	static class AudioDecoders
	{
		/// <summary>Class factory for audio decoders</summary>
		public static iAudioDecoder create( ref TrackInfo track )
		{
			switch( track.audioCodec )
			{
				case eAudioCodec.AAC:
					return new AacDecoder( ref track );

				case eAudioCodec.AC3:
					return new DolbyDecoder( track.sampleRate );

				case eAudioCodec.DTS:
					Audio.DTS.LoadLibrary.load();
					return new DtsDecoder( track.sampleRate );

				case eAudioCodec.EAC3:
					// The only Linux implementation appears to be in ffmpeg.
					// That thing is huge, 48 MB of archives, 163 MB of shared libraries.
					// Even libavcodec58 alone is huge, 25 MB of archives, 113 MB of shared libraries.
					// Individual decoders are borderline impossible to isolate from the rest of the framework, tons of internal dependencies.
					// E.g. all decoders handle ref.counted buffers instead of just memory, not what we want because zero-copy memory mapped I/O is strictly better than memcpy.
					// Apparently, the project grew organically over the course of 2 decades, and no one bothered to update the overall API/architecture.
					// Ideally, need to create an LGPL-licensed DLL by copy-pasting just the EAC3 decoder from libavcodec, with absolute minimum of dependencies from libavutil.
					// Without that ref.counted buffers BS.
					throw new NotImplementedException( "Dolby Digital Plus support is not yet implemented" );
			}
			throw new ArgumentException( $"Unexpected audio codec value { track.audioCodec }" );
		}
	}
}