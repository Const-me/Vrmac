using System;

namespace VrmacVideo.Containers.MP4
{
	public sealed class MPEG4BitRateBox
	{
		public readonly int decodingBufferSize, maxBitrate, averageBitrate;

		internal MPEG4BitRateBox( ReadOnlySpan<byte> box )
		{
			ReadOnlySpan<int> span = box.Slice( 8, 12 ).cast<int>();
			decodingBufferSize = span[ 0 ].endian();
			maxBitrate = span[ 1 ].endian();
			averageBitrate = span[ 2 ].endian();
		}

		public override string ToString() => $"decodingBufferSize { decodingBufferSize }, maxBitrate { maxBitrate }, averageBitrate { averageBitrate }";
	}
}