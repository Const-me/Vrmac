using System;

namespace VrmacVideo.IO.ALSA
{
	/// <summary>Random flags for <see cref="libasound.snd_pcm_open"/> function.</summary>
	[Flags]
	enum ePcmOpenFlags: int
	{
		/// <summary>None of the below</summary>
		None = 0,

		/// <summary>Non blocking mode</summary>
		NonBlocking = 1,
		/// <summary>Async notification</summary>
		AsyncNotify = 2,
		/// <summary>Disable automatic (but not forced!) rate resamplinig</summary>
		NoAutoResample = 0x10000,
		/// <summary>Disable automatic (but not forced!) channel conversion</summary>
		NoAutoChannels = 0x20000,
		/// <summary>Disable automatic (but not forced!) format conversion</summary>
		NoAutoFormat = 0x40000,
		/// <summary>Disable software volume controls</summary>
		NoSoftVolume = 0x80000,
	}
}