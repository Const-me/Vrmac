namespace VrmacVideo.Containers.MKV
{
	/// <summary>Clipping of the color ranges.</summary>
	public enum eRange: byte
	{
		/// <summary>unspecified</summary>
		Unspecified = 0,
		/// <summary>broadcast range</summary>
		BroadcastRange = 1,
		/// <summary>full range (no clipping)</summary>
		FullRange = 2,
		/// <summary>defined by MatrixCoefficients / TransferCharacteristics</summary>
		Custom = 3,
	}
}