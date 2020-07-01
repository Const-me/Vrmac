using System;
using Vrmac.Utils;
using System.Runtime.CompilerServices;
using VrmacVideo.IO.Dolby;

namespace VrmacVideo.Decoders.Audio
{
	class DolbyDecoder: iAudioDecoder
	{
		const byte channelsCount = 2;
		const eFrameFlags channelFlags = eFrameFlags.Stereo;

		byte iAudioDecoder.channelsCount => channelsCount;
		// The underlying decoder uses 0x100 = 256 long blocks.
		// The value is too small for ALSA's period, this class doubles the size.
		int iAudioDecoder.blockSize => 0x200;

		// Undocumented, but from testing that thing it's apparent the liba52 library does NOT copy the compressed data.
		// a52_frame needs the source data available at the old memory addresses.
		bool iAudioDecoder.copiesCompressedData => false;

		IntPtr ac3 = default;
		readonly pfnInterleaveFunc interleaveSamples;
		public int sampleRate { get; }

		public DolbyDecoder( int sampleRate )
		{
			ac3 = liba52.a52_init( 0 );
			interleaveSamples = MiscUtils.simd.interleaveDolby( channelsCount );
			this.sampleRate = sampleRate;
		}

		public void Dispose()
		{
			if( ac3 != default )
			{
				liba52.a52_free( ac3 );
				ac3 = default;
			}
			GC.SuppressFinalize( this );
		}

		~DolbyDecoder()
		{
			if( ac3 != default )
				liba52.a52_free( ac3 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		int iAudioDecoder.sync( ReadOnlySpan<byte> data )
		{
			int cb = liba52.a52_syncinfo( data, out var flags, out var sample_rate, out var bit_rate );
			if( 0 == cb )
				throw new ArgumentException( "Invalid Dolby AC3 stream" );
			if( sample_rate != sampleRate )
				throw new ApplicationException( $"DTS audio stream sample rate mismatch: expected { sampleRate }, got { sample_rate }" );
			// Logger.logVerbose( "A52 sync completed: flags {0}, sample rate {1}, bit rate {2}", flags, sample_rate, bit_rate );
			return cb;
		}

		/// <summary>Count of pending blocks in the current frame</summary>
		public int blocksLeft { get; private set; } = 0;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void iAudioDecoder.decodeFrame( ReadOnlySpan<byte> data, byte volume )
		{
			eFrameFlags flags = channelFlags;
			liba52.a52_frame( ac3, data, flags, volume );
			// Logger.logVerbose( "a52_frame" );

			// The decoder makes 6, but since we double the blocks tell player 3 are available.
			blocksLeft = 3;
		}

		bool logged = false;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void a52_block()
		{
			int r = liba52.a52_block( ac3 );
			if( 0 == r )
			{
				// Logger.logVerbose( "Decoded A52 block" );
				return;
			}
			if( logged )
				return;
			logged = true;
			Logger.logWarning( "liba52.a52_block failed with exit code {0}", r );
		}

		/// <summary>Decode next block in frame</summary>
		void iAudioDecoder.decodeBlock( Span<short> pcm )
		{
			if( blocksLeft <= 0 )
				throw new ApplicationException( "No blocks left in the frame" );
			blocksLeft--;

			unsafe
			{
				fixed ( short* p = pcm )
				{
					a52_block();
					IntPtr source = liba52.a52_samples( ac3 );
					interleaveSamples( p, source );

					a52_block();
					source = liba52.a52_samples( ac3 );
					interleaveSamples( p + 0x100 * channelsCount, source );
				}
			}
		}
	}
}