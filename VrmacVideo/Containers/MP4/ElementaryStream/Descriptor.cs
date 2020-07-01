using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.Containers.MP4.ElementaryStream
{
	[Flags]
	enum eDescriptorFlags: byte
	{
		/// <summary>Has ID of another elementary stream on which this elementary stream depends</summary>
		DependentStream = 0x80,
		/// <summary>Has URL</summary>
		URL = 0x40,
		OCRstream = 0x20
	}

	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	struct ElementaryStreamDescriptor
	{
		ushort m_id;
		public ushort id => m_id.endian();

		byte flagsAndPriority;
		public eDescriptorFlags flags => (eDescriptorFlags)( flagsAndPriority & 0xE0 );
		/// <summary>relative measure for the priority of this elementary stream.</summary>
		/// <remarks>An elementary stream with a higher streamPriority is more important than one with a lower streamPriority. The absolute values of priority are not normatively defined.</remarks>
		public byte priority => (byte)( flagsAndPriority & 0x1F );

		// Here's what follows:
		// if( eDescriptorFlags.DependentStream ) ushort dependentStreamId;
		// DecoderConfigDescriptor decConfigDescr;
		// SLConfigDescriptor slConfigDescr;
		// And a few others
	}

	// ISO/IEC 14496-1 section 7.2.6.6 "DecoderConfigDescriptor"
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	unsafe struct DecoderConfigDescriptor
	{
		public byte objectTypeIndication;
		byte m_streamMisc;
		public byte streamType => (byte)( m_streamMisc >> 2 );
		public bool upStream => 0 != ( m_streamMisc & 2 );

		fixed byte m_bufferSizeDB[ 3 ];

		int getBufferSize()
		{
			// We want abc number
			// Stored bytes: a, b, c, g
			// The g is garbage, but we guaranteed to have read access there 'coz the structure ain't ending yet, m_maxBitrate field follows

			int val;
			unsafe
			{
				fixed ( byte* pointer = m_bufferSizeDB )
					val = *(int*)( pointer );
				// Loaded into uint: gcba
			}

			// Flipped bytes: abcg
			val = val.endian();

			// Shifting by 1 byte makes the value we're after
			return val >> 8;
		}
		public int bufferSizeDB => getBufferSize();

		int m_maxBitrate, m_avgBitrate;
		public int maxBitrate => m_maxBitrate.endian();
		public int avgBitrate => m_avgBitrate.endian();
	}
}