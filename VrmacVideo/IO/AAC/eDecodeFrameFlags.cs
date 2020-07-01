using System;

namespace VrmacVideo.IO.AAC
{
	/// <summary></summary>
	[Flags]
	enum eDecodeFrameFlags: uint
	{
		/// <summary>None of the below</summary>
		None = 0,
		/// <summary>Trigger the built-in error concealment module to generate a substitute signal for one lost frame. New input data will not be considered.</summary>
		Conceal = 1,
		/// <summary>Flush all filterbanks to get all delayed audio without having new input data. Thus new input data will not be considered.</summary>
		Flush = 2,
		/// <summary>Signal an input bit stream data discontinuity. Resync any internals as necessary.</summary>
		Interrupt = 4,
		/// <summary>Clear all signal delay lines and history buffers. This can cause discontinuities in the output signal.</summary>
		ClearHistory = 8,
	}
}