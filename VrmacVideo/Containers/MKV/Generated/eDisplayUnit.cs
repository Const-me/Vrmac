namespace VrmacVideo.Containers.MKV
{
	/// <summary>How DisplayWidth &amp; DisplayHeight are interpreted.</summary>
	public enum eDisplayUnit: byte
	{
		Pixels = 0,
		Centimeters = 1,
		Inches = 2,
		DisplayAspectRatio = 3,
		Unknown = 4,
	}
}