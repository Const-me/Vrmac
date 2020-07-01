namespace VrmacVideo.Containers.MKV
{
	/// <summary>Defines when the process command SHOULD be handled</summary>
	public enum eChapProcessTime: byte
	{
		DuringTheWholeChapter = 0,
		BeforeStartingPlayback = 1,
		AfterPlaybackOfTheChapter = 2,
	}
}