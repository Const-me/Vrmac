namespace VrmacVideo.Audio
{
	enum eAudioCodec: byte
	{
		AAC,
		DTS,
		AC3,
		EAC3,
	}

	partial struct TrackInfo
	{
		public readonly eAudioCodec audioCodec;
		/// <summary>This can be 0 if the container doesn't have that data; when zero, encoded buffers are sized dynamically.</summary>
		public readonly int maxBytesInFrame;
		public readonly int sampleRate;
		/// <summary>This can be 0 if provided by decoder, as opposed to metadata in the container</summary>
		public readonly int samplesPerFrame;
		public readonly ushort bitsPerSample;
		public readonly byte channelsCount;
		public readonly byte[] decoderConfigBlob;
		public readonly byte[] strippedHeader;

		public override string ToString() =>
			$"codec { audioCodec }, maxBytesInFrame { maxBytesInFrame }, sampleRate { sampleRate }, samplesPerFrame { samplesPerFrame }, bitsPerSample { bitsPerSample }, channelsCount { channelsCount }, decoderConfigBlob { decoderConfigBlob?.Length ?? 0 } bytes";
	}
}