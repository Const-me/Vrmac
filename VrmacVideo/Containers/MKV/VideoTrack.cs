using System;
using VrmacVideo.Linux;

namespace VrmacVideo.Containers.MKV
{
	sealed class VideoTrack: iVideoTrack
	{
		public readonly MkvMediaFile file;
		public readonly TrackEntry track;
		/// <summary>Parsed content of h264 codec private data</summary>
		public readonly VideoParams videoParams;
		/// <summary>Unfortunately, MKV format lacks this piece of metadata. Using conservative upper estimate.</summary>
		public readonly int maxBytesInFrame;
		readonly eVideoCodec videoCodec;

		public VideoTrack( MkvMediaFile file, TrackEntry track )
		{
			this.file = file;
			this.track = track;
			videoCodec = file.segment.videoCodec;
			switch( videoCodec )
			{
				case eVideoCodec.h264:
					videoParams = new VideoParams264( track );
					break;
				case eVideoCodec.h265:
					videoParams = new VideoParams265( track );
					break;
				default:
					throw new ApplicationException( "Unexpected eVideoCodec value" );
			}

			maxBytesInFrame = MaxEncodedSize.find( file, track, videoParams.decodedSize.size );
		}

		eVideoCodec iVideoTrack.codec => file.segment.videoCodec;

		eChromaFormat iVideoTrack.chromaFormat => videoParams.chromaFormat;

		sDecodedVideoSize iVideoTrack.decodedSize => videoParams.decodedSize;

		int iVideoTrack.maxBytesInFrame => maxBytesInFrame;

		sPixelFormatMP iVideoTrack.getEncodedFormat()
		{
			sPixelFormatMP res = new sPixelFormatMP();
			res.size = videoParams.decodedSize.size;
			res.pixelFormat = ePixelFormat.H264;
			res.field = eField.Progressive;
			videoParams.setColorAttributes( ref res );
			return res;
		}

		sPixelFormatMP iVideoTrack.getDecodedFormat()
		{
			sPixelFormatMP res = new sPixelFormatMP();
			res.size = videoParams.decodedSize.size;
			res.pixelFormat = ePixelFormat.NV12;
			res.field = eField.Progressive;
			videoParams.setColorAttributes( ref res );
			return res;
		}

		iVideoTrackReader iVideoTrack.createReader( EncodedQueue queue ) => new VideoReader( file, track, videoParams, queue );
	}
}