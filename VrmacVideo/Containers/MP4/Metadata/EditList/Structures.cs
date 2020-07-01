#pragma warning disable CS0649  // Field 'BoxHeader.entry_count' is never assigned to, and will always have its default value 0
using System.Buffers.Binary;

namespace VrmacVideo.Containers.MP4.EditList
{
	struct BoxHeader
	{
		public uint version;
		uint entry_count;
		public int entriesCount => (int)BinaryPrimitives.ReverseEndianness( entry_count );
	}

	struct Entry32
	{
		public uint segmentDuration;
		public int mediaTime;
		public override string ToString() => $"segmentDuration { segmentDuration }, mediaTime { mediaTime }";
	}

	struct Entry64
	{
		public ulong segmentDuration;
		public long mediaTime;
		public override string ToString() => $"segmentDuration { segmentDuration }, mediaTime { mediaTime }";
	}
}