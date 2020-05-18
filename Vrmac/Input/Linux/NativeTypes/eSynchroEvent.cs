namespace Vrmac.Input.Linux
{
	/// <summary>Codes for <see cref="eEventType.Synchro" /> events</summary>
	public enum eSynchroEvent: byte
	{
		/// <summary>Used to synchronize and separate events into packets of input data changes occurring at the same moment in time.</summary>
		/// <remarks>For example, motion of a mouse may set the REL_X and REL_Y values for one motion, then emit a SYN_REPORT. The next motion will emit more REL_X and REL_Y values and send another SYN_REPORT.</remarks>
		Report = 0,

		/// <summary>Undocumented</summary>
		Config = 1,

		/// <summary>Used to synchronize and separate touch events</summary>
		MultiTouchReport = 2,

		/// <summary>Used to indicate buffer overrun in the evdev client’s event queue.</summary>
		Dropped = 3,
	}
}