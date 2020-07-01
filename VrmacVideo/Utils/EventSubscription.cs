using System;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	/// <summary>Utility class to subscribe/unsubscribe V4L2 events</summary>
	sealed class EventSubscription: IDisposable
	{
		readonly FileHandle device;

		public EventSubscription( VideoDevice videoDevice )
		{
			device = videoDevice.file;

			eEventType[] eventTypes = new eEventType[]
			{
				eEventType.EndOfStream,
				eEventType.SourceChange,
			};

			foreach( var e in eventTypes )
			{
				// Subscribe for the events
				sEventSubscription evt = new sEventSubscription() { type = e };
				device.call( eControlCode.SUBSCRIBE_EVENT, ref evt );
			}
		}

		void IDisposable.Dispose()
		{
			sEventSubscription events = default;
			device.call( eControlCode.UNSUBSCRIBE_EVENT, ref events );
		}
	}
}