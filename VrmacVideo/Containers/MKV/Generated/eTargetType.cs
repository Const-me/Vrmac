namespace VrmacVideo.Containers.MKV
{
	/// <summary>An informational string that can be used to display the logical level of the target like "ALBUM", "TRACK", "MOVIE", "CHAPTER", etc (see <a
	/// href="https://www.matroska.org/technical/tagging.html#targettypes">TargetType</a>).</summary>
	public enum eTargetType: byte
	{
		COLLECTION,
		EDITION,
		ISSUE,
		VOLUME,
		OPUS,
		SEASON,
		SEQUEL,
		ALBUM,
		OPERA,
		CONCERT,
		MOVIE,
		EPISODE,
		PART,
		SESSION,
		TRACK,
		SONG,
		CHAPTER,
		SUBTRACK,
		MOVEMENT,
		SCENE,
		SHOT,
	}
}