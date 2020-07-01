using System;

namespace VrmacVideo.IO
{
	/// <summary>Flags for creating a timer</summary>
	[Flags]
	public enum eTimerCreateFlags: int
	{
		/// <summary>TFD_NONBLOCK</summary>
		NonBlocking = eFileFlags.O_NONBLOCK,
		/// <summary>TFD_CLOEXEC</summary>
		DisableHandleInheritance = eFileFlags.O_CLOEXEC
	}

	/// <summary>Initial expiration and interval for the timer</summary>
	public struct sTimerDesc
	{
		/// <summary>Interval for periodic timer</summary>
		public sTimeNano interval;
		/// <summary>Initial expiration of the timer, in seconds and nanoseconds. Setting it to `default` disarms the timer.</summary>
		public sTimeNano value;
	}

	public enum eTimerSetFlag: int
	{
		/// <summary>Relative timer, <see cref="sTimerDesc.value" /> specifies a time relative to the current value of the clock</summary>
		Relative = 0,
		/// <summary>Absolute timer, <see cref="sTimerDesc.value" /> specifies an absolute time for the clock</summary>
		Absolute = 1,
	}
}