using System;

namespace VrmacVideo.Containers.MP4
{
	[Flags]
	enum eSyncLayerFlags: byte
	{
		/// <summary>accessUnitStartFlag is present in each SL packet header of this elementary stream</summary>
		UseAccessUnitStart = 0x80,
		/// <summary>accessUnitEndFlag is present in each SL packet header of this elementary stream</summary>
		UseAccessUnitEnd = 0x40,
		/// <summary>RandomAccessPointFlag is present in each SL packet header of this elementary stream.</summary>
		UseRandomAccessPoint = 0x20,
		/// <summary>each SL packet corresponds to a random access point. In that case the randomAccessPointFlag need not be used.</summary>
		HasRandomAccessUnitsOnly = 0x10,
		/// <summary>paddingFlag is present in each SL packet header of this elementary stream</summary>
		UsePadding = 8,
		/// <summary>Time stamps are used for synchronization of this elementary stream</summary>
		UseTimeStamps = 4,
		/// <summary>idleFlag is used in this elementary stream</summary>
		UseIdle = 2,
		/// <summary>The ConstantDuration will follow</summary>
		ConstantDuration = 1,
	}
}