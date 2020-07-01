namespace VrmacVideo.Containers.MKV
{
	/// <summary>The encryption algorithm used. The value '0' means that the contents have not been encrypted but only signed.</summary>
	public enum eContentEncAlgo: byte
	{
		/// <summary>Not encrypted</summary>
		NotEncrypted = 0,
		/// <summary>DES - FIPS 46-3</summary>
		DES = 1,
		/// <summary>Triple DES - RFC 1851</summary>
		TripleDES = 2,
		/// <summary>Twofish</summary>
		Twofish = 3,
		/// <summary>Blowfish</summary>
		Blowfish = 4,
		/// <summary>AES - FIPS 187</summary>
		AES = 5,
	}
}