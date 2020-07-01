namespace VrmacVideo
{
	public enum eChromaFormat: byte
	{
		Unknown = 0,

		/// <summary>4:2:0, 1 chroma value per 2×2 block of pixels</summary>
		/// <remarks>The only one supported so far. Most h264 profiles only support this mode of operation.</remarks>
		/// <seealso href="https://en.wikipedia.org/wiki/Advanced_Video_Coding#Feature_support_in_particular_profiles" />
		c420 = 1,

		/// <summary>4:2:2, 1 chroma value per 2×1 horizontal block of pixels</summary>
		c422 = 2,

		/// <summary>4:4:4, no chroma downsampling whatsoever, chroma channel has the same resolution as the video.</summary>
		c444 = 3,
	}
}