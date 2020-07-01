namespace VrmacVideo.Containers.MKV
{
	/// <summary>A set of track types coded on 8 bits.</summary>
	public enum eTrackType: byte
	{
		Video = 1,
		Audio = 2,
		Complex = 3,
		Logo = 16,
		Subtitle = 17,
		Buttons = 18,
		Control = 32,
		Metadata = 33,
	}
}