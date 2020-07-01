using System;

namespace VrmacVideo.Linux
{
	/// <summary>Event types</summary>
	public enum eEventType: uint
	{
		/// <summary>All events. Valid only for VIDIOC_UNSUBSCRIBE_EVENT for unsubscribing all events at once.</summary>
		All = 0,
		/// <summary>This event is triggered on the vertical sync.</summary>
		VSync = 1,
		/// <summary>Triggered when the end of a stream is reached. This is typically used with MPEG decoders to report to the application when the last of the MPEG stream has been decoded.</summary>
		EndOfStream = 2,
		/// <summary>The documentation fails to specify WTF is the control</summary>
		Control = 3,
		/// <summary>Triggered immediately when the reception of a frame has begun.</summary>
		FrameSync = 4,
		/// <summary>A source parameter change is detected during runtime by the video device</summary>
		SourceChange = 5,
		/// <summary>Motion detection state for one or more of the regions changes</summary>
		MotionDetector = 6,

		/// <summary>Base event number for driver-private events</summary>
		PrivateStart = 0x08000000,
	}

	/// <summary>Flags for <see cref="eEventType.SourceChange" /> event.</summary>
	[Flags]
	public enum eSourceChanges: uint
	{
		Resolution = 1
	}

	/// <summary>Event Flags for get/subscribe/unsubscribe control codes</summary>
	[Flags]
	enum eEventFlags: uint
	{
		None = 0,

		/// <summary>When this event is subscribed an initial event will be sent containing the current status.
		/// This only makes sense for events that are triggered by a status change such as <see cref="eEventType.Control" />. Other events will ignore this flag.</summary>
		SendInitial = 1,

		/// <summary>Events directly caused by an ioctl will also be sent to the file handle that called that ioctl</summary>
		/// <remarks>Normally such events are suppressed to prevent feedback loops where an application changes a control to a one value and then another,
		/// and then receives an event telling it that that control has changed to the first value.</remarks>
		AllowFeedback = 2,
	}
}