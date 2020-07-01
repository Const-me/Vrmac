using System;

namespace VrmacVideo.IO
{
	[Flags]
	public enum eFileFlags: int
	{
		None = 0,

		O_RDONLY = 0,
		O_WRONLY = 1,
		O_RDWR = 2,

		// See printEnums.cpp
		O_CREAT = 0x40,
		O_EXCL = 0x80,
		O_NOCTTY = 0x100,
		O_TRUNC = 0x200,
		O_APPEND = 0x400,
		O_NONBLOCK = 0x800,
		O_ASYNC = 0x2000,
		O_DIRECTORY = 0x4000,
		O_NOFOLLOW = 0x8000,
		O_DIRECT = 0x10000,
		O_LARGEFILE = 0x20000,
		O_NOATIME = 0x40000,
		O_CLOEXEC = 0x80000,
		O_SYNC = 0x101000,
	}

	/// <summary>Similar flags for eventfd handles</summary>
	[Flags]
	public enum eEventFdFlags: int
	{
		None = 0,
		/// <summary>EFD_SEMAPHORE</summary>
		Semaphore = 1,
		/// <summary>EFD_NONBLOCK</summary>
		NonBlocking = eFileFlags.O_NONBLOCK,
		/// <summary>EFD_CLOEXEC</summary>
		DisableHandleInheritance = eFileFlags.O_CLOEXEC,
	}
}