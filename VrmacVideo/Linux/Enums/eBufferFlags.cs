using System;

namespace VrmacVideo.Linux
{
	/// <summary>Flags for <see cref="sBuffer.flags" /> field.</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/buffer.html#buffer-flags" />
	[Flags]
	enum eBufferFlags: uint
	{
		/// <summary>The buffer resides in device memory and has been mapped into the application’s address space</summary>
		Mapped = 1,
		/// <summary>When this flag is set, the buffer is currently on the incoming queue.
		/// After successful <see cref="eControlCode.QBUF" /> call it is always set and after <see cref="eControlCode.DQBUF" /> always cleared.</summary>
		Queued = 2,
		/// <summary>The buffer is currently on the outgoing queue, ready to be dequeued from the driver</summary>
		Done = 4,
		/// <summary>The buffer has been dequeued successfully, although the data might have been corrupted.
		/// This is recoverable, streaming may continue as normal and the buffer may be reused normally. Drivers set this flag when <see cref="eControlCode.DQBUF" /> is called.</summary>
		Error = 0x40,
		/// <summary>This buffer is part of a request that hasn’t been queued yet.</summary>
		InRequest = 0x80,

		/// <summary>The buffer contains a compressed image which is a key frame (or field), i. e. can be decompressed on its own. Also known as an I-frame.</summary>
		KeyFrame = 8,
		/// <summary>Predicted frames or fields which contain only differences to a previous key frame</summary>
		PFrame = 0x10,
		/// <summary>A bi-directional predicted frame or field which contains only the differences between the current frame and both the preceding and following key frames to specify its content</summary>
		BFrame = 0x20,

		/// <summary>The <see cref="sBuffer.timeCode"/> field is valid.</summary>
		TimeCode = 0x100,
		/// <summary>The buffer has been prepared for I/O and can be queued by the application. Managed by the driver.</summary>
		Prepared = 0x400,
		/// <summary>Caches do not have to be invalidated for this buffer.
		/// Typically applications shall use this flag if the data captured in the buffer is not going to be touched by the CPU, instead the buffer will, probably, be passed on to a DMA-capable hardware unit for further processing or output.</summary>
		NoCacheInvalidate = 0x800,
		/// <summary>Caches do not have to be cleaned for this buffer. Typically applications shall use this flag for output buffers if the data in this buffer has not been created by the CPU but by some DMA-capable unit, in which case caches have not been used.</summary>
		NoCacheClean = 0x1000,
		/// <summary>Only valid if V4L2_BUF_CAP_SUPPORTS_M2M_HOLD_CAPTURE_BUF is set. It is typically used with stateless decoders where multiple output buffers each decode to a slice of the decoded frame. </summary>
		Mem2MemHoldCaptureBuffer = 0x00000200,
		/// <summary>Last buffer produced by the hardware. Due to hardware limitations, the last buffer may be empty.</summary>
		Last = 0x00100000,
		/// <summary>The <see cref="sBuffer.requestFileDescriptor"/> field is valid.</summary>
		RequestFd = 0x00800000,

		/// <summary>Mask for timestamp types below. To test the timestamp type, mask out bits not belonging to timestamp type by performing a logical and operation with buffer flags and timestamp mask.</summary>
		TimestampMask = 0x0000e000,
		/// <summary>Unknown timestamp type</summary>
		TimestampUnknown = 0,
		/// <summary>The buffer timestamp has been taken from <see cref="IO.eClock.Monotonic"/> clock.</summary>
		TimestampMonotonic = 0x00002000,
		/// <summary>The capture buffer timestamp has been taken from the corresponding output buffer. This flag applies only to mem2mem devices.</summary>
		TimestampCopy = 0x00004000,

		/// <summary>Mask for timestamp sources below.</summary>
		TimestampSourceMask = 0x00070000,
		/// <summary>The buffer timestamp has been taken when the last pixel of the frame has been received or the last pixel of the frame has been transmitted.</summary>
		TimestampSourceEndOfFrame = 0,
		/// <summary>The buffer timestamp has been taken when the exposure of the frame has begun. This is only valid for the <see cref="eBufferType.VideoCapture" /> type.</summary>
		TimestampSourceStartOfExposure = 0x00010000
	}
}