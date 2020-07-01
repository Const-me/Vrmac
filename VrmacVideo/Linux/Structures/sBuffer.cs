#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
#pragma warning disable CS0169  // field is never used
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VrmacVideo.Linux
{
	/// <summary>Payload structure for <see cref="eControlCode.QUERYBUF" />, the C++ type is v4l2_buffer structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/latest/media/uapi/v4l/buffer.html#c.v4l2_buffer" />
	struct sBuffer
	{
		/// <summary>Number of the buffer</summary>
		public int index;
		/// <summary>Type of the buffer</summary>
		public eBufferType type;
		/// <summary>The number of bytes occupied by the data in the buffer</summary>
		public int bytesUsed;
		/// <summary>Flags set by the application or driver</summary>
		public eBufferFlags flags;
		/// <summary>Indicates the field order of the image in the buffer</summary>
		public eField field;
		/// <summary>For capture streams this is time when the first data byte was captured.
		/// For output streams the driver stores the time at which the last data byte was actually sent out.
		/// This permits applications to monitor the drift between the video and system clock</summary>
		public sTimeMicro timestamp;
		/// <summary>When the V4L2_BUF_FLAG_TIMECODE flag is set in flags, this structure contains a frame timecode</summary>
		public sTimeCode timeCode;
		/// <summary>Set by the driver, counting the frames (not fields!) in sequence. This field is set for both input and output devices.</summary>
		public uint sequence;
		/// <summary>This field must be set by applications and/or drivers in accordance with the selected I/O method</summary>
		public eMemory memory;

		[StructLayout( LayoutKind.Explicit )]
		public struct sUnion
		{
			/// <summary>For the single-planar API and when memory is eMemory.MemoryMap this is the offset of the buffer from the start of the device memory.
			/// The value is returned by the driver and apart of serving as parameter to the mmap() function not useful for applications.</summary>
			[FieldOffset( 0 )] public uint offset;
			/// <summary>For the single-planar API and when memory is eMemory.UserPointer this is a pointer to the buffer (casted to unsigned long type) in virtual memory, set by the application.</summary>
			[FieldOffset( 0 )] public uint userptr;
			/// <summary>When using the multi-planar API, contains a userspace pointer to an array of struct sPlane. Need to be fixed, obviously.</summary>
			[FieldOffset( 0 )] public IntPtr planes;
			/// <summary>For the single-plane API and when memory is eMemory.DmaSharedBuffer this is the file descriptor associated with a DMABUF buffer.</summary>
			[FieldOffset( 0 )] public int fd;
		}
		public sUnion m;

		/// <summary>Size of the buffer (not the payload) in bytes for the single-planar API.
		/// For the multi-planar API the application sets this to the number of elements in the planes array. The driver will fill in the actual number of valid elements in that array.</summary>
		public int length;
		/// <summary>A place holder for future extensions</summary>
		uint reserved2;
		/// <summary>The file descriptor of the request to queue the buffer to.</summary>
		public uint requestFileDescriptor;

		IEnumerable<string> details()
		{
			yield return $"index { index }, type { type }, bytesUsed { bytesUsed }, timeCode { timeCode }, sequence { sequence }, length { length }";
			yield return $"	Flags { flags }, time { timestamp }, field { field }";
			for( int i = 0; i < length; i++ )
			{
				int offset = i * sPlane.size;
				sPlane p = Marshal.PtrToStructure<sPlane>( m.planes + offset );
				yield return $"	Plane #{ i }: { p }";
			}
		}

		public override string ToString() => details().makeLines();
	}
}