using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vrmac.Utils;

namespace VrmacVideo.Decoders.Audio
{
	sealed class DtsDecoder: iAudioDecoder
	{
		const byte channelsCount = 2;
		DTS.iAudioDecoder m_decoder;

		byte iAudioDecoder.channelsCount => channelsCount;
		public int blockSize { get; }
		public bool copiesCompressedData { get; }
		public int sampleRate { get; }

		// readonly DbgSaveFrames dbgSaveFrames = new DbgSaveFrames();

		public DtsDecoder( int sampleRate )
		{
			this.sampleRate = sampleRate;
			DTS.AudioDecoder.createDcaDecoder( out m_decoder, channelsCount, sampleRate );
			blockSize = m_decoder.blockSize();
			// Logger.logInfo( "Block size: {0}", blockSize );
			copiesCompressedData = m_decoder.copiesCompressedData();
		}

		public void Dispose()
		{
			m_decoder?.Dispose();
			m_decoder = null;
			GC.SuppressFinalize( this );
		}

		~DtsDecoder()
		{
			m_decoder?.Dispose();
		}

		int iAudioDecoder.sync( ReadOnlySpan<byte> data )
		{
			if( data.Length < 16 )
				throw new ApplicationException( "The buffer is too small" );

			// TODO [low]: might be a good idea to implement small subset of dcadec_frame_parse_header in C#.
			// Not too hard, it has 6 bit streams formats and after converting the damn endianness only 2 possible sync.words, core and EXSS.
			// We already have a suitable bit parser in this project, MP4.ElementaryStream.BitReader will do just fine, apart from the Golomb methods it's quite generic.
			// Will allow slightly better error messages, and support multiple concatenated DTS frames in a single MKV frame.
			// See Table 5-1: Bit-stream header in the spec, it has about 25 fields defined, all fixed-length, and we only need a couple initial ones.
			return data.Length;

			/* unsafe
			{
				fixed ( byte* p = data )
				{
					// Logger.logVerbose( "{0:x2} {1:x2} {2:x2} {3:x2}", p[ 0 ], p[ 1 ], p[ 2 ], p[ 3 ] );
					int r = m_decoder.sync( p );
					check( r );
					// Logger.logVerbose( "DTS frame synced OK: {0} bytes", r );
					return r;
				}
			} */
		}

		/// <summary>Count of pending blocks in the current frame</summary>
		public int blocksLeft => m_decoder.blocksLeft();

		void iAudioDecoder.decodeFrame( ReadOnlySpan<byte> data, byte volume )
		{
			// dbgSaveFrames.frame( data );
			unsafe
			{
				fixed ( byte* p = data )
				{
					// var sw = Stopwatch.StartNew();
					int hr = m_decoder.syncAndDecode( p, volume );
					// double ms = sw.Elapsed.TotalMilliseconds;
					// Logger.logVerbose( "iAudioDecoder.decodeFrame: {0} ms", ms );
					check( hr );
				}
			}
		}

		/// <summary>Decode next block in frame</summary>
		void iAudioDecoder.decodeBlock( Span<short> pcm )
		{
			if( blocksLeft <= 0 )
				throw new ApplicationException( "No blocks left in the frame" );
			unsafe
			{
				fixed ( short* p = pcm )
				{
					int hr = m_decoder.decodeBlock( p );
					check( hr );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static void check( int hr )
		{
			if( hr >= 0 )
				return;
			Exception ex = exception( hr );
			throw ex;
		}

		// stdafx.h of the decoder DLL, `constexpr HRESULT makeErrorCode()` function, see `FACILITY_DECODER_DTS` there
		const int FACILITY_DECODER_DTS = unchecked((int)0xA0040000);

		[MethodImpl( MethodImplOptions.NoInlining )]
		static Exception exception( int hr )
		{
			string msg;
			if( ( hr & NativeErrorMessages.facilityMask ) == FACILITY_DECODER_DTS )
			{
				IntPtr ptr = DTS.AudioDecoder.formatMessage( hr & 0xFFFF );
				msg = Marshal.PtrToStringUTF8( ptr );
				return new ApplicationException( $"DTS decoder failed: { msg }" );
			}
			msg = NativeErrorMessages.customErrorMessage( hr );
			if( null != msg )
				return new ApplicationException( $"DTS decoder failed: { msg }" );

			ComLight.ErrorCodes.throwForHR( hr );
			return null;
		}

		// ETSI TS 102 114 "DTS Coherent Acoustics; Core and Extensions with Additional Profiles", section 5.3.1 "Bit stream header"
		// (FSIZE+1) is the total byte size of the current frame including primary audio data as well as any extension audio data. Valid range for FSIZE: 95 to 16383
		// https://www.etsi.org/deliver/etsi_ts/102100_102199/102114/01.04.01_60/ts_102114v010401p.pdf
		public const int maxBytesPerFrame = 16384;
		public const int minBytesPerFrame = 96;
	}
}