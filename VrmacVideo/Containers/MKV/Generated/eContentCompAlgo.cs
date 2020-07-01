namespace VrmacVideo.Containers.MKV
{
	/// <summary>The compression algorithm used.</summary>
	public enum eContentCompAlgo: byte
	{
		Zlib = 0,
		Bzlib = 1,
		Lzo1x = 2,
		HeaderStripping = 3,
	}
}