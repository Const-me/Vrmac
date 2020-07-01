using System;
using System.Linq;
using VrmacVideo.Containers.MKV;

namespace VrmacVideo.Audio
{
	partial struct TrackInfo
	{
		internal TrackInfo( TrackEntry track )
		{
			sampleRate = (int)Math.Round( track.audio.samplingFrequency );
			channelsCount = (byte)track.audio.channels;
			bitsPerSample = checked((ushort)track.audio.bitDepth);

			strippedHeader = null;
			if( null != track.contentEncodings && null != track.contentEncodings.contentEncoding )
			{
				var ce = track.contentEncodings.contentEncoding;
				if( ce.Any( e => e.contentEncodingType == eContentEncodingType.Encryption ) )
					throw new NotSupportedException( "Encrypted audio is not supported" );
				var comp = ce.FirstOrDefault( e => e.contentEncodingType == eContentEncodingType.Compression );
				if( null != comp )
				{
					if( comp.contentCompression.contentCompAlgo != eContentCompAlgo.HeaderStripping )
						throw new NotSupportedException( "The only content compression supported so far is header stripping" );
					strippedHeader = comp.contentCompression.contentCompSettings;
				}
			}

			switch( track.codecID )
			{
				case "A_EAC3":
					audioCodec = eAudioCodec.EAC3;
					throw new NotImplementedException( "VrmacVideo can’t play Dolby Digital Plus audio" );
				case "A_AC3":
				case "A_AC3/BSID9":
				case "A_AC3/BSID10":
					audioCodec = eAudioCodec.AC3;
					maxBytesInFrame = IO.Dolby.liba52.maxEncodedBuffer;
					samplesPerFrame = IO.Dolby.liba52.samplesPerFrame;
					decoderConfigBlob = null;
					return;
				case "A_DTS":
					audioCodec = eAudioCodec.DTS;
					samplesPerFrame = 0;
					maxBytesInFrame = 0;
					decoderConfigBlob = null;
					return;
				case "A_AAC/MPEG2/MAIN":
				case "A_AAC/MPEG2/LC":
				case "A_AAC/MPEG2/LC/SBR":
				case "A_AAC/MPEG2/SSR":
				case "A_AAC/MPEG4/MAIN":
				case "A_AAC/MPEG4/LC":
				case "A_AAC/MPEG4/LC/SBR":
				case "A_AAC/MPEG4/SSR":
				case "A_AAC/MPEG4/LTP":
				case "A_AAC":
					audioCodec = eAudioCodec.AAC;
					maxBytesInFrame = 0;
					decoderConfigBlob = track.codecPrivate;
					samplesPerFrame = (int)Math.Round( track.defaultDuration * 1E-9 * track.audio.samplingFrequency );
					return;
				default:
					throw new NotSupportedException( $"The audio codec \"{ track.codecID }\" is not supported" );
			}
		}
	}
}