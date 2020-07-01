using System;

namespace VrmacVideo.Linux
{
	[Flags]
	public enum eBufferCapabilityFlags: uint
	{
		None = 0,

		/// <summary>Buffer supports <see cref="eMemory.MemoryMap" /> streaming mode.</summary>
		MemoryMap = 0x00000001,

		/// <summary>Buffer supports <see cref="eMemory.UserPointer" /> streaming mode.</summary>
		UserPointer = 0x00000002,

		/// <summary>Buffer supports <see cref="eMemory.DmaSharedBuffer" /> streaming mode.</summary>
		DmaSharedBuffer = 0x00000004,

		/// <summary>This buffer type supports <see href="https://www.kernel.org/doc/html/v5.1/media/uapi/mediactl/request-api.html#media-request-api">requests</see>.</summary>
		Requests = 0x00000008,

		/// <summary>The kernel allows calling <see cref="eControlCode.REQBUFS"/> while buffers are still mapped or exported via DMABUF.
		/// These orphaned buffers will be freed when they are unmapped or when the exported DMABUF fds are closed.</summary>
		OrphanedBuffers = 0x00000010,
	}

	/// <summary>Payload structure for <see cref="eControlCode.REQBUFS" />, the C++ type is v4l2_requestbuffers structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/vidioc-reqbufs.html" />
	public unsafe struct sRequestBuffers
	{
		/// <summary>The number of buffers requested or granted.</summary>
		public int count;
		/// <summary>Type of the stream or buffers</summary>
		public eBufferType type;

		public eMemory memory;

		/// <summary>A placeholder for future extensions</summary>
		fixed uint reserved[ 2 ];
	}
}