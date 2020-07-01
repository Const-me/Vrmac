namespace VrmacVideo.Containers.MKV
{
	/// <summary>The hash algorithm used for the signature.</summary>
	public enum eContentSigHashAlgo: byte
	{
		/// <summary>Not signed</summary>
		NotSigned = 0,
		/// <summary>SHA1-160</summary>
		SHA1 = 1,
		/// <summary>MD5</summary>
		MD5 = 2,
	}
}