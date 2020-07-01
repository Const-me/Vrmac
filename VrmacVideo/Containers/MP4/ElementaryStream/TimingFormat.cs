namespace VrmacVideo.Containers.MP4
{
	struct TimingFormat
	{
		// These 4 are needed to decrypt SEI messages
		public readonly bool cpbDpbDelaysPresent;
		public readonly byte cpbRemovalDelayLength;
		public readonly byte dpbOutputDelayLength;
		public readonly byte timeOffsetLength;
		public readonly bool picStructPresent;

		// These 2 are to compute these timestamps
		public readonly int timeScale, numUnitsInTick;

		public TimingFormat( ref SequenceParameterSet sps )
		{
			cpbDpbDelaysPresent = sps.vui.CpbDpbDelaysPresent;
			if( cpbDpbDelaysPresent )
			{
				cpbRemovalDelayLength = sps.vui.hrd.CpbRemovalDelayLength;
				dpbOutputDelayLength = sps.vui.hrd.DpbOutputDelayLength;
			}
			else
				cpbRemovalDelayLength = dpbOutputDelayLength = 0;

			timeOffsetLength = sps.vui.hrd.TimeOffsetLength;
			timeScale = sps.vui.timeScale;
			numUnitsInTick = sps.vui.numUnitsInTick;

			picStructPresent = sps.vui.flags.HasFlag( eVuiFlags.PicStruct );
		}
	}
}