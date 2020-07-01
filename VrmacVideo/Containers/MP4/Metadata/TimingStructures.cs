namespace VrmacVideo.Containers.MP4
{
	/// <summary>Decoding time-to-sample table entry</summary>
	public struct sTimeToSampleEntry
	{
		/// <summary>Number of consecutive samples that have the given duration</summary>
		public readonly uint sampleCount;
		/// <summary>Delta of these samples in the time-scale of the media</summary>
		public readonly uint sampleDelta;

		public override string ToString() => $"sampleCount { sampleCount }, sampleDelta { sampleDelta }";
	}

	/// <summary>Sample to chunk table entry</summary>
	public struct sSampleToChunkEntry
	{
		/// <summary>Index of the first chunk in this run of chunks that share the same samples-per-chunk and sample-description-index.</summary>
		/// <remarks>The index of the first chunk in a track has value 1.
		/// The firstChunk field in the first item of <see cref="SampleTable.sampleToChunk"/> has the value 1 ‘coz the first sample is in the the first chunk./remarks>
		public readonly int firstChunk;

		/// <summary>Number of samples in each of these chunks</summary>
		public readonly int samplesPerChunk;

		/// <summary>Index of the sample entry that describes the samples in this chunk.</summary>
		/// <remarks>The index ranges from 1 to the number of sample entries in the Sample Description Box</remarks>
		public readonly uint sampleDescriptionIndex;

		public override string ToString() => $"firstChunk { firstChunk }, samplesPerChunk { samplesPerChunk }, sampleDescriptionIndex { sampleDescriptionIndex }";
	}

	public struct sCompositionToSampleEntry
	{
		/// <summary>Number of consecutive samples that have the given offset</summary>
		public readonly uint sampleCount;

		/// <summary>Offset between composition time CT and decoding time DT, such that CT[ n ] = DT[ n ] + sCompositionToSampleEntry[ n ].sampleOffset.</summary>
		public readonly uint sampleOffset;

		public override string ToString() => $"sampleCount { sampleCount }, sampleOffset { sampleOffset }";
	}
}