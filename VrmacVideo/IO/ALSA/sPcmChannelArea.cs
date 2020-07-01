#pragma warning disable CS0649  // Field is never assigned to
using System;

namespace VrmacVideo.IO.ALSA
{
	struct sPcmChannelArea
	{
		/// <summary>Base address of channel samples</summary>
		public readonly IntPtr baseAddress;
		/// <summary>offset to first sample in bits</summary>
		public readonly int first;
		/// <summary>samples distance in bits</summary>
		public readonly int step;
	}
}