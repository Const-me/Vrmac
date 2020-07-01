namespace VrmacVideo.Linux
{
	// enum v4l2_xfer_func
	public enum eTransferFunction: byte
	{
		Default = 0,
		BT_709 = 1,
		SRGB = 2,
		AdobeRGB = 3,
		SMPTE240M = 4,
		None = 5,
		DCI_P3 = 6,
		SMPTE2084 = 7,
	}
}