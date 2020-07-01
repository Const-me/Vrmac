using System;

namespace VrmacVideo.IO.ALSA
{
	[Flags]
	enum ePollResult: ushort
	{
		None = 0,
		Read = ePollEvents.POLLIN,
		Write = ePollEvents.POLLOUT,
		Error = ePollEvents.POLLERR,
	}
}