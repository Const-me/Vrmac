namespace VrmacVideo.Containers.MKV
{
	/// <summary>A flag to declare if the video is known to be progressive or interlaced and if applicable to declare details about the interlacement.</summary>
	public enum eFlagInterlaced: byte
	{
		Undetermined = 0,
		Interlaced = 1,
		Progressive = 2,
	}
}