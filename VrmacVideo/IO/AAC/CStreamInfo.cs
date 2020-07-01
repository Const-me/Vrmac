#pragma warning disable CS0649
using Diligent;
using System;
using VrmacVideo.Containers.MP4;

namespace VrmacVideo.IO.AAC
{
	/// <summary>Provides information about the currently decoded audio data. All fields are read-only.</summary>
	struct CStreamInfo
	{
		// These members are the only really relevant ones for the user.

		/// <summary>The sample rate in Hz of the fully decoded PCM audio signal (after SBR processing).</summary>
		public readonly int sampleRate;

		/// <summary>The frame size of the decoded PCM audio signal. 1024 or 960 for AAC-LC, 2048 or 1920 for HE-AAC (v2), 512 or 480 for AAC-LD and AAC-ELD.</summary>
		/// <remarks>The unit for that value ain’t documented. But based on the source code, it looks like the unit = PCM sample * output channels count,
		/// for stereo with frameSize = 1024 you probably need 2048 short values = 4kb RAM</remarks>
		public readonly int frameSize;

		/// <summary>The number of output audio channels in the decoded and interleaved PCM audio signal</summary>
		public readonly int numChannels;

		readonly IntPtr m_channelTypes;
		public ReadOnlySpan<eAudioChannel> channelTypes => Unsafe.readSpan<eAudioChannel>( m_channelTypes, numChannels );

		readonly IntPtr m_channelIndices;
		public ReadOnlySpan<byte> channelIndices => Unsafe.readSpan<byte>( m_channelIndices, numChannels );

		// Decoder internal members

		/// <summary>Sampling rate in Hz without SBR (from configuration info)</summary>
		public readonly int aacSampleRate;

		/// <summary>MPEG-2 profile from file header. Or -1 if not applicable, happens with MPEG-4 audio.</summary>
		public readonly int profile;

		readonly int m_aot;
		/// <summary>Audio object type</summary>
		public eAudioObjectType audioObjectType => unchecked((eAudioObjectType)m_aot);

		/// <summary>Channel configuration (0: PCE defined, 1: mono, 2: stereo, ...</summary>
		public readonly int channelConfig;
		/// <summary>Instantaneous bit rate</summary>
		public readonly int bitRate;
		/// <summary>Samples per frame for the AAC core (from ASC).</summary>
		/// <remarks>1024 or 960 for AAC-LC; 512 or 480 for AAC-LD and AAC-ELD</remarks>
		public readonly int aacSamplesPerFrame;
		/// <summary>The number of audio channels after AAC core processing (before PS or MPS processing). This are <b>not</b> the final number of output channels</summary>
		public readonly int aacNumChannels;

		readonly int m_extAot;
		/// <summary>Extension Audio Object Type (from ASC)</summary>
		public eAudioObjectType extensionAudioObjectType => unchecked((eAudioObjectType)m_extAot);

		/// <summary>Extension sampling rate in Hz (from ASC)</summary>
		public readonly int extSamplingRate;
		/// <summary>The number of samples the output is additionally delayed by the decoder</summary>
		public readonly uint outputDelay;
		/// <summary>Copy of internal flags. Only to be written by the decoder, and only to be read externally.</summary>
		public readonly uint flags;
		/// <summary>epConfig level (from ASC): only level 0 supported, -1 means no ER (e. g. AOT=2, MPEG-2 AAC, etc.)</summary>
		public readonly sbyte epConfig;

		// Statistics

		/// <summary>This integer will reflect the estimated amount of lost access units in case aacDecoder_DecodeFrame() returns AAC_DEC_TRANSPORT_SYNC_ERROR.It will be &lt; 0 if the estimation failed</summary>
		public readonly int numLostAccessUnits;
		/// <summary>Total bytes passed through the decoder</summary>
		public readonly uint numTotalBytes;
		/// <summary>Bytes out of numTotalBytes that were considered errors</summary>
		public readonly uint numBadBytes;
		/// <summary>Total access units passed through the decoder</summary>
		public readonly int numTotalAccessUnits;

		// Metadata

		/// <summary>DRC program reference level. Defines the reference level below full-scale.</summary>
		/// <remarks>Quantized in steps of 0.25dB. The valid values range from 0 (0 dBFS) to 127 (-31.75 dBFS).
		/// It is used to reflect the average loudness of the audio in LKFS accoring to ITU-R BS 1770.
		/// If no level has been found in the bitstream the value is -1.</remarks>
		public readonly sbyte drcProgRefLev;

		/// <summary>DRC presentation mode</summary>
		/// <remarks>According to ETSI TS 101 154, this field indicates whether light ( MPEG-4 Dynamic Range Control tool ) or heavy compression( DVB heavy compression )
		/// dynamic range control shall take priority on the outputs.
		/// For details, see ETSI TS 101 154, table C.33. Possible values are:
		/// -1: No corresponding metadata found in the bitstream;
		/// 0: DRC presentation mode not indicated;
		/// 1: DRC presentation mode 1;
		/// 2: DRC presentation mode 2</remarks>
		public readonly sbyte drcPresMode;

		public override string ToString() => 
			$"sampleRate { sampleRate }, frameSize { frameSize }, numChannels { numChannels }, channelTypes { string.Join(' ', channelTypes.ToArray() ) }";
	}
}