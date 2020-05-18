using System;

namespace Vrmac.Input
{
	/// <summary>Interface to query time of last messages</summary>
	public interface iInputEventTimeSource
	{
		/// <remarks>On Windows and X11 the precision is not great, they both pass integer milliseconds. On Linux without X11 it's really good, down to the last 100-nanosecond tick.</remarks>
		TimeSpan messageTime { get; }
	}
}