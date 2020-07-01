namespace VrmacVideo.Linux
{
	/// <summary>C++ enum: v4l2_buf_type</summary>
	public enum eBufferType: int
	{
		/// <summary>Buffer of a single-planar video capture stream</summary>
		VideoCapture = 1,

		/// <summary>Buffer of a single-planar video output stream</summary>
		VideoOutput = 2,

		/// <summary>Buffer for video overlay</summary>
		VideoOverlay = 3,

		/// <summary>Buffer of a raw VBI capture stream</summary>
		VbiCapture = 4,
		/// <summary>Buffer of a raw VBI output stream</summary>
		VbiOutput = 5,
		/// <summary>Buffer of a sliced VBI capture stream</summary>
		SlicedVbiCapture = 6,
		/// <summary>Buffer of a sliced VBI output stream</summary>
		SlicedVbiOutput = 7,

		/// <summary>Buffer for video output overlay (OSD)</summary>
		VideoOutputOverlay = 8,
		/// <summary>Buffer of a multi-planar video capture stream</summary>
		VideoCaptureMPlane = 9,
		/// <summary>Buffer of a multi-planar video output stream</summary>
		VideoOutputMPlane = 10,
		/// <summary>Buffer for Software Defined Radio (SDR) capture stream</summary>
		SdrCapture = 11,
		/// <summary>Buffer for Software Defined Radio (SDR) output stream</summary>
		SdrOutput = 12,
		/// <summary>Buffer for metadata capture</summary>
		MetadataCapture = 13,
		/// <summary>Buffer for metadata output</summary>
		MetadataOutput = 14,
	}
}