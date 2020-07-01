#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.Linux
{
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/buffer.html#c.v4l2_plane" />
	unsafe struct sPlane
	{
		/// <summary>The number of bytes occupied by data in the plane (its payload)</summary>
		/// <remarks>Drivers must set this field when type refers to a capture stream, applications when it refers to an output stream.
		/// If the application sets this to 0 for an output stream, then bytesUsed will be set to the size of the plane (see the length field of this struct) by the driver.</remarks>
		public int bytesUsed;

		/// <summary>Size in bytes of the plane (not its payload). This is set by the driver based on the calls to ioctl VIDIOC_REQBUFS and/or ioctl VIDIOC_CREATE_BUFS.</summary>
		public int length;

		[StructLayout( LayoutKind.Explicit )]
		public struct Union
		{
			/// <summary>When the memory type in the containing struct v4l2_buffer is V4L2_MEMORY_MMAP, this is the value that should be passed to mmap(), similar to the offset field in struct v4l2_buffer.</summary>
			[FieldOffset( 0 )] public int memoryOffset;

			/// <summary>When the memory type in the containing struct v4l2_buffer is V4L2_MEMORY_USERPTR, this is a userspace pointer to the memory allocated for this plane by an application.</summary>
			[FieldOffset( 0 )] public IntPtr userPointer;

			/// <summary>When the memory type in the containing struct v4l2_buffer is V4L2_MEMORY_DMABUF, this is a file descriptor associated with a DMABUF buffer, similar to the fd field in struct v4l2_buffer.</summary>
			[FieldOffset( 0 )] public int fd;
		}
		public Union union;

		/// <summary>Offset in bytes to video data in the plane. Drivers must set this field when type refers to a capture stream, applications when it refers to an output stream.</summary>
		/// <remarks>That dataOffset is included in bytesUsed.</remarks>
		public int dataOffset;

		/// <summary>Size of the image in the plane</summary>
		public int imageSize => bytesUsed - dataOffset;

		/// <summary>Reserved for future use. Should be zeroed by drivers and applications.</summary>
		fixed uint reserved[ 11 ];

		public override string ToString() => 
			$"bytesUsed { bytesUsed }, length { length }, memoryOffset { union.memoryOffset }, dataOffset { dataOffset }";

		public static readonly int size = Marshal.SizeOf<sPlane>();
	}
}