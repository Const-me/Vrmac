using System;
using Vrmac.Utils;
using VrmacVideo.Audio;
using VrmacVideo.IO.AAC;

namespace VrmacVideo.Decoders.Audio
{
	class AacDecoder: iAudioDecoder
	{
		const byte channelsCount = 2;
		public int sampleRate { get; }
		bool iAudioDecoder.copiesCompressedData => false;

		// readonly DbgSaveFrames dbgSaveAll = new DbgSaveFrames();

		public AacDecoder( ref TrackInfo track )
		{
			decoder = new Decoder( eTransportType.Raw, 1 );
			if( null != track.decoderConfigBlob )
				decoder.configRaw( track.decoderConfigBlob.AsSpan() );
			else
				throw new ArgumentException( "AAC decoder needs Audio Specific Config magic blob." );   // There's a special box in mpeg4 files for that data. Not sure about MKV, though.

			decoder.setParameter( eParameter.PcmMinOutputChannels, channelsCount );
			decoder.setParameter( eParameter.PcmMaxOutputChannels, channelsCount );
			blockSize = track.samplesPerFrame;
			simdUtils = MiscUtils.simd;
			sampleRate = track.sampleRate;

			// dbgSaveAll.parameters( track.decoderConfigBlob.AsSpan() );
		}

		readonly Decoder decoder;
		byte iAudioDecoder.channelsCount => channelsCount;
		readonly iSimdUtils simdUtils;

		public int blockSize { get; }
		byte m_volume = 0xFF;

		int iAudioDecoder.sync( ReadOnlySpan<byte> data )
		{
			int cb = decoder.fillInputBuffer( data );
			blocksLeft = 1;
			/* if( cb == data.Length )
				Logger.logVerbose( "AacDecoder.sync: consumed all {0} bytes of the frame", cb );
			else
				Logger.logVerbose( "AacDecoder.sync: consumed {0} bytes out of {1}", cb, data.Length ); */

			// dbgSaveAll.frame( data.Slice( 0, cb ) );
			return cb;
		}

		void iAudioDecoder.decodeFrame( ReadOnlySpan<byte> data, byte volume )
		{
			m_volume = volume;
		}

		public int blocksLeft { get; private set; }

		void iAudioDecoder.decodeBlock( Span<short> data )
		{
			if( blocksLeft <= 0 )
				throw new ApplicationException( "No blocks left in the frame" );
			// Logger.logVerbose( "AacDecoder.decodeBlock" );
			blocksLeft--;
			decoder.decodeFrame( data );
			MiscUtils.simd.applyPcmVolume( data, m_volume );
		}

		public void Dispose()
		{
			decoder?.Dispose();
		}
	}
}