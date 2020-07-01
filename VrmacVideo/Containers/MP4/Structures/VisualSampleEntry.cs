#pragma warning disable CS0169  // field is never used
using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac;

namespace VrmacVideo.Containers.MP4.Structures
{
	// 8 bytes
	unsafe struct SampleEntryBase
	{
		fixed byte reserved[ 6 ];
		ushort data_reference_index;
	}

	// ISO/IEC 14496-12 section 12.1.3 "Sample entry"
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	unsafe struct VisualSampleEntry
	{
		// 8 bytes
		SampleEntryBase baseEntry;

		// The following portion is 70 bytes = 0x46
		fixed uint padding[ 4 ];
		public ushort width, height;
		uint horizresolution, vertresolution;    // both 0x00480000, for 72 dpi
		uint padding2;
		ushort frame_count;
		public fixed byte compressorname[ 32 ];
		uint padding3;  // depth always 0x0018, and pre-defined -1

		// In total, this one is 78 = 0x4E bytes.
		// After that is 4-bytes length, avcC magic number, and AVC1SampleEntry

		public CSize size => new CSize( width.endian(), height.endian() );
		public Vector2 resolution => new Vector2( horizresolution.endian() * ( 1.0f / 0x10000 ), vertresolution.endian() * ( 1.0f / 0x10000 ) );
		public ushort frameCount => frame_count.endian();
	}

	// ISO/IEC 14496-15, 5.2.4.1.1
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	struct AVCDecoderConfigurationRecord
	{
		int m_length;
		public int length => m_length.endian();
		public eAVC1BoxType boxType;
		byte configurationVersion; // = 1

		public eAvcProfile profileCode;
		public byte profileCompatibility;
		/// <summary>Level multiplied by 10, e.g. 41 = 0x29 for the level 4.1</summary>
		public byte levelCode;

		byte m_lengthSizeMinusOne;
		public byte lengthSizeMinusOne => (byte)( m_lengthSizeMinusOne & 3 );

		byte m_numOfSequenceParameterSets;
		public byte numOfSequenceParameterSets => (byte)( m_numOfSequenceParameterSets & 0x1F );
	}
}