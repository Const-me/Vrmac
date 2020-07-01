namespace VrmacVideo.Containers.MKV
{
	/// <summary>The AES cipher mode used in the encryption.</summary>
	public enum eAESSettingsCipherMode: byte
	{
		/// <summary>AES-CTR / Counter, NIST SP 800-38A</summary>
		CTR = 1,
		/// <summary>AES-CBC / Cipher Block Chaining, NIST SP 800-38A</summary>
		CBC = 2,
	}
}