#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
#pragma warning disable CS0169  // field is never used

namespace VrmacVideo.Containers.MP4.Structures
{
	// ISO/IEC 14496-12 section 12.2.3 "Sample entry"
	unsafe struct AudioSampleEntry
	{
		// 8 bytes
		SampleEntryBase baseEntry;

		fixed uint padding[ 2 ];
		public ushort channelcount;
		public ushort samplesize;
		uint padding2;
		public uint sampleRate;
	}
}