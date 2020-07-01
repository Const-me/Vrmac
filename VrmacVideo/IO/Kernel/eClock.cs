namespace VrmacVideo.IO
{
	public enum eClock: int
	{
		/// <summary>Settable system-wide clock; CLOCK_REALTIME in C++</summary>
		Realtime = 0,
		/// <summary>Nonsettable clock that is not affected by discontinuous changes in the system clock (e.g., manual changes to system time); CLOCK_MONOTONIC in C++</summary>
		Monotonic = 1,
	}
}