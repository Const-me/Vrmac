#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true

namespace VrmacVideo.Linux
{
	/// <summary>Payload structure for <see cref="eControlCode.CREATE_BUFS" />, the C++ type is v4l2_create_buffers</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.12/media/uapi/v4l/vidioc-create-bufs.html" />
	unsafe struct sCreateBuffers
	{
		public int index;
		public int count;
		public eMemory memory;
		public sStreamDataFormat format;
		fixed uint reserved[ 8 ];
	}
}