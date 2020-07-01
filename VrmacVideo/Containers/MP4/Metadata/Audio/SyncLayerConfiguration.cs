using System;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.MP4
{
	struct ConstantDuration
	{
		public readonly uint timeScale;
		public readonly ushort accessUnitDuration;
		public readonly ushort compositionUnitDuration;

		internal ConstantDuration( ref Reader reader )
		{
			var cd = reader.readStructure<ElementaryStream.ConstantDuration>();
			timeScale = cd.timeScale;
			accessUnitDuration = cd.accessUnitDuration;
			compositionUnitDuration = cd.compositionUnitDuration;
		}
	}

	struct SyncLayerConfiguration
	{
		public readonly ePredefinedSyncLayerConfig predefined;
		public readonly eSyncLayerFlags flags;
		public readonly uint timeStampResolution;
		public readonly uint OCRResolution;
		public readonly byte timeStampLength;
		public readonly byte OCRLength;
		public readonly byte AU_Length;
		public readonly byte instantBitrateLength;
		public readonly byte degradationPriorityLength;
		public readonly byte AU_seqNumLength;
		public readonly byte packetSeqNumLength;

		public readonly ConstantDuration? constantDuration;

		internal SyncLayerConfiguration( ref Reader reader )
		{
			predefined = (ePredefinedSyncLayerConfig)reader.readByte();
			SyncLayerConfig slc;
			switch( predefined )
			{
				case ePredefinedSyncLayerConfig.Custom:
					slc = reader.readStructure<SyncLayerConfig>();
					break;
				case ePredefinedSyncLayerConfig.Mpeg4:
				case ePredefinedSyncLayerConfig.NullSyncLayerHeader:
					slc = SyncLayerConfig.getPredefined( predefined );
					break;
				default:
					throw new NotImplementedException( $"Predefined sync.layer configuration index { predefined } is not supported by the library" );
			}

			flags = slc.flags;
			timeStampResolution = slc.timeStampResolution;
			OCRResolution = slc.OCRResolution;
			timeStampLength = slc.timeStampLength;
			OCRLength = slc.OCRLength;
			AU_Length = slc.AU_Length;
			instantBitrateLength = slc.instantBitrateLength;
			degradationPriorityLength = slc.degradationPriorityLength;
			AU_seqNumLength = slc.AU_Length;
			packetSeqNumLength = slc.packetSeqNumLength;

			if( flags.HasFlag( eSyncLayerFlags.ConstantDuration ) )
				constantDuration = new ConstantDuration( ref reader );
			else
				constantDuration = null;
		}
	}
}