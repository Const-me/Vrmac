using System;
using VrmacVideo.Audio;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	interface iMediaFile: IDisposable
	{
		iVideoTrack videoTrack { get; }
		iAudioTrack audioTrack { get; }
		TimeSpan duration { get; }
	}

	interface iVideoTrack
	{
		eVideoCodec codec { get; }
		eChromaFormat chromaFormat { get; }
		sDecodedVideoSize decodedSize { get; }

		int maxBytesInFrame { get; }
		sPixelFormatMP getEncodedFormat();
		sPixelFormatMP getDecodedFormat();

		/// <summary>Create the reader, and enqueue initial parameter sets</summary>
		/// <remarks>The min. allowed queue length is 2, h264 decoder needs SPS and PPS to initialize.</remarks>
		iVideoTrackReader createReader( EncodedQueue queue );
	}

	interface iAudioTrack
	{
		TrackInfo info { get; }
		iAudioTrackReader createReader();
	}
}