using System;

namespace VrmacVideo.Linux
{
	// V4L2_PIX_FMT_FLAG_*
	[Flags]
	public enum ePixelFormatFlags: byte
	{
		/// <summary>No flags specified</summary>
		None = 0,
		/// <summary>The video has premultiplied alpha</summary>
		PremultipliedAlpha = 1,
	}
}