#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
#pragma warning disable CS0169  // field is never used
using System;

namespace VrmacVideo.Containers.MP4.ElementaryStream
{
	// Initial part of SLConfigDescriptor with ePredefinedConfig.Custom value
	struct SyncLayerConfig
	{
		public eSyncLayerFlags flags;
		uint m_timeStampResolution;
		public uint timeStampResolution => m_timeStampResolution.endian();

		uint m_OCRResolution;
		public uint OCRResolution => m_OCRResolution.endian();
		/// <summary></summary>
		public byte timeStampLength;
		public byte OCRLength;
		public byte AU_Length;
		public byte instantBitrateLength;
		// [ 0 - 3 ]: degradationPriorityLength, [ 4 - 8 ]: AU_seqNumLength, [ 9 - 14 ]: packetSeqNumLength, [ 15 - 16 ]: reserved = 0b11
		ushort m_last4;
		public byte degradationPriorityLength => (byte)( ( m_last4 >> 4 ) & 0xF );
		public byte AU_seqNumLength => (byte)( ( m_last4.endian() >> 7 ) & 0x1F );
		public byte packetSeqNumLength => (byte)( ( m_last4 >> 9 ) & 0x1F );

		static readonly SyncLayerConfig[] predefinedConfigs = new SyncLayerConfig[ 2 ]
		{
			// ePredefined.NullSyncLayerHeader
			new SyncLayerConfig(){ m_timeStampResolution = ((ushort)1000).endian(), timeStampLength = 32 },
			// ePredefined.Mpeg4
			new SyncLayerConfig(){ flags = eSyncLayerFlags.UseTimeStamps }
		};

		/// <summary>Get a predefined sync layer config</summary>
		internal static SyncLayerConfig getPredefined( ePredefinedSyncLayerConfig pc )
		{
			byte val = (byte)pc;
			if( 0 == val || val > 2 )
				throw new ArgumentException();
			return predefinedConfigs[ val - 1 ];
		}
	}

	// Duration data from the SyncLayerConfig, the offset layout depends on earlier fields
	struct ConstantDuration
	{
		uint m_timeScale;
		ushort m_accessUnitDuration, m_compositionUnitDuration;

		public uint timeScale => m_timeScale.endian();
		public ushort accessUnitDuration => accessUnitDuration.endian();
		public ushort compositionUnitDuration => m_compositionUnitDuration.endian();
	}
}