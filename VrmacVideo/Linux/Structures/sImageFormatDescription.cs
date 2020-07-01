using VrmacVideo.IO;

namespace VrmacVideo.Linux
{
	// C++ type v4l2_fmtdesc: https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/vidioc-enum-fmt.html?highlight=v4l2_fmtdesc#c.v4l2_fmtdesc
	public unsafe struct sImageFormatDescription
	{
		public int index;

		/// <summary>Type of the data stream, set by the application.</summary>
		/// <remarks>Valid values: VideoCapture, VideoCaptureMPlane, VideoOutput, VideoOutputMPlane, VideoOverlay, SdrCapture, SdrOutput, MetadataCapture.</remarks>
		public eBufferType type;
		public eImageFormatDescriptionFlags flags;
		/// <summary>Description of the format, a NUL-terminated ASCII string. This information is intended for the user, for example: “YUV 4:2:2”.</summary>
		fixed byte m_description[ 32 ];
		public ePixelFormat pixelFormat;

		/// <summary>Reserved for future extensions</summary>
		fixed uint reserved[ 4 ];

		public string description
		{
			get
			{
				unsafe
				{
					fixed ( byte* p = m_description )
						return StringMarshal.copy( p, 32 );
				}
			}
		}

		public override string ToString() =>
			$"index { index }, type { type }, flags { flags }, description { description }, pixelFormat { pixelFormat }";
	}
}