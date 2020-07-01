using VrmacVideo.IO;

namespace VrmacVideo.Linux
{
	/// <summary>Payload structure for <see cref="eControlCode.EXPBUF"/>; the C++ type is v4l2_exportbuffer structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/vidioc-expbuf.html#c.v4l2_exportbuffer" />
	public unsafe struct sExportBuffer
	{
		/// <summary>Type of the buffer, set by the application.</summary>
		public eBufferType type;

		/// <summary>Number of the buffer, set by the application</summary>
		public int index;

		/// <summary>Index of the plane to be exported when using the multi-planar API</summary>
		public int plane;

		/// <summary>Flags for the newly created file, currently only O_CLOEXEC, O_RDONLY, O_WRONLY, and O_RDWR are supported</summary>
		public eFileFlags flags;

		/// <summary>The DMABUF file descriptor associated with a buffer. Set by the driver.</summary>
		public int fd;

		/// <summary>Reserved field for future use. Drivers and applications must set the array to zero.</summary>
		fixed uint reserved[ 11 ];
	}
}