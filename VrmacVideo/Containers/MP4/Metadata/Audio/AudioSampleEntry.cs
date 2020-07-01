using System.Runtime.InteropServices;

namespace VrmacVideo.Containers.MP4
{
	public abstract class AudioSampleEntry: SampleEntry
	{
		public readonly byte channelCount;
		public readonly ushort bitsPerSample;
		public readonly uint sampleRateInt;

		public const uint sampleRateDiv = 0x10000;
		const double sampleRateDmlMul = 1.0 / sampleRateDiv;
		public double sampleRate => sampleRateInt * sampleRateDmlMul;

		public AudioSampleEntry( Mp4Reader reader, ref int bytesLeft )
		{
			var ss = reader.readStructure<Structures.AudioSampleEntry>();
			bytesLeft -= Marshal.SizeOf<Structures.AudioSampleEntry>();

			checked
			{
				channelCount = (byte)ss.channelcount.endian();
				bitsPerSample = ss.samplesize.endian();
				sampleRateInt = ss.sampleRate.endian();
			}
		}

		/// <summary>Payload of decoder configuration specific for the audio.</summary>
		// The outer thing is specified in ISO/IEC 14496-1 section 7.2.6.6 "DecoderConfigDescriptor"
		// The payload is in ISO/IEC 14496-3 section 1.6.2.1 "AudioSpecificConfig"
		// This library doesn't needs the payload parsed, it's sent to fdk-aac decoder as an opaque blob of bytes.
		// The DecoderConfigDescriptor contains enough stuff to setup the playback.
		public abstract byte[] audioSpecificConfig { get; }
	}
}