#pragma warning disable CS0649

namespace VrmacVideo.Linux
{
	/// <summary>Payload structure for <see cref="eControlCode.SUBSCRIBE_EVENT" /> and <see cref="eControlCode.UNSUBSCRIBE_EVENT" /> codez; v4l2_event_subscription in C++</summary>
	unsafe struct sEventSubscription
	{
		/// <summary>Type of the event</summary>
		public eEventType type;
		/// <summary>ID of the event source. If there is no ID associated with the event source, then set this to 0. Whether or not an event needs an ID depends on the event type.</summary>
		public uint id;
		/// <summary>Event flags</summary>
		public eEventFlags flags;
		/// <summary>Reserved for future extensions</summary>
		fixed uint reserved[ 5 ];
	}
}