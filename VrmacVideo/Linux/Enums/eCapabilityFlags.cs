using System;

namespace VrmacVideo.Linux
{
	[Flags]
	public enum eCapabilityFlags: uint
	{
		/// <summary>Is a video capture device</summary>
		VideoCapture = 0x00000001,
		/// <summary>Is a video output device</summary>
		VideoOutput = 0x00000002,
		/// <summary>Can do video overlay</summary>
		VideoOverlay = 0x00000004,
		/// <summary>Is a raw VBI capture device</summary>
		VBICapture = 0x00000010,
		/// <summary>Is a raw VBI output device</summary>
		VBIOutput = 0x00000020,
		/// <summary>Is a sliced VBI capture device</summary>
		SlicedVBICapture = 0x00000040,
		/// <summary>Is a sliced VBI output device</summary>
		SlicedVBIOutput = 0x00000080,
		/// <summary>RDS data capture</summary>
		RDSCapture = 0x00000100,
		/// <summary>Can do video output overlay</summary>
		VideoOutputOverlay = 0x00000200,
		/// <summary>Can do hardware frequency seek</summary>
		HardwareFrequencySeek = 0x00000400,
		/// <summary>Is an RDS encoder</summary>
		RDSOutput = 0x00000800,

		/// <summary>Is a video capture device that supports multiplanar formats</summary>
		VideoCaptureMPlane = 0x00001000,
		/// <summary>Is a video output device that supports multiplanar formats</summary>
		VideoOutputMPlane = 0x00002000,
		/// <summary>Is a video mem-to-mem device that supports multiplanar formats</summary>
		VideoMem2memMPlane = 0x00004000,
		/// <summary>Is a video mem-to-mem device</summary>
		VideoMem2mem = 0x00008000,

		/// <summary>has a tuner</summary>
		Tuner = 0x00010000,
		/// <summary>has audio support </summary>
		Audio = 0x00020000,
		/// <summary>is a radio device </summary>
		Radio = 0x00040000,
		/// <summary>has a modulator </summary>
		Modulator = 0x00080000,

		/// <summary>Is a SDR capture device </summary>
		SDRCapture = 0x00100000,
		/// <summary>Supports the extended pixel format</summary>
		ExtendedPixelFormat = 0x00200000,
		/// <summary>Is a SDR output device</summary>
		SDROutput = 0x00400000,
		/// <summary>Is a metadata capture device</summary>
		MetadataCapture = 0x00800000,

		/// <summary>read/write systemcalls</summary>
		ReadWrite = 0x01000000,
		/// <summary>async I/O</summary>
		AsyncIO = 0x02000000,
		/// <summary>streaming I/O ioctls</summary>
		Streaming = 0x04000000,

		/// <summary>Is a touch device</summary>
		TouchDevice = 0x10000000,

		/// <summary>sets device capabilities field</summary>
		DeviceCaps = 0x80000000,
	}
}