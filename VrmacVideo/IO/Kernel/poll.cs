using System;

namespace VrmacVideo.IO
{
	[Flags]
	public enum ePollEvents: ushort
	{
		/// <summary>There is data to read</summary>
		POLLIN = 1,
		/// <summary>There is urgent data to read</summary>
		POLLPRI = 2,
		/// <summary>Writing now will not block</summary>
		POLLOUT = 4,

		/// <summary>Error condition, output only</summary>
		POLLERR = 8,
		/// <summary>Hang up, output only</summary>
		POLLHUP = 0x10,
		/// <summary></summary>
		POLLNVAL = 0x20,

		POLLRDNORM = 0x040,
		POLLWRNORM = 0x100,

		/// <summary>Undocumented</summary>
		POLLREMOVE = 0x1000,
		/// <summary>Stream socket peer closed connection, or shut down writing half of connection</summary>
		POLLRDHUP = 0x2000,
	}

	/// <summary>A file descriptor to be monitored with <see cref="LibC.poll(ReadOnlySpan{pollfd}, int)" /></summary>
	public struct pollfd
	{
		/// <summary>File descriptor</summary>
		public int fd;
		/// <summary>Requested events</summary>
		public ePollEvents events;
		/// <summary>Returned events</summary>
		public ePollEvents revents;
	}
}