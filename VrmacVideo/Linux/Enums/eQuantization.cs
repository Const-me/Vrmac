namespace VrmacVideo.Linux
{
	public enum eQuantization: byte
	{
		/// <summary>The default for R'G'B' quantization is always full range, except for the BT2020 colorspace.
		/// For Y'CbCr the quantization is always limited range, except for COLORSPACE_JPEG: this is full range.</summary>
		Default = 0,
		FullRange = 1,
		LimitedRange = 2,
	}
}