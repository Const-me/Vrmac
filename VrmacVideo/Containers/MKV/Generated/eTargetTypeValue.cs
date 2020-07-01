namespace VrmacVideo.Containers.MKV
{
	/// <summary>A number to indicate the logical level of the target.</summary>
	public enum eTargetTypeValue: byte
	{
		/// <summary>COLLECTION
		/// The highest hierarchical level that tags can describe.</summary>
		Collection = 70,
		/// <summary>EDITION / ISSUE / VOLUME / OPUS / SEASON / SEQUEL
		/// A list of lower levels grouped together.</summary>
		Edition = 60,
		/// <summary>ALBUM / OPERA / CONCERT / MOVIE / EPISODE / CONCERT
		/// The most common grouping level of music and video (equals to an episode for TV series).</summary>
		Album = 50,
		/// <summary>PART / SESSION
		/// When an album or episode has different logical parts.</summary>
		Part = 40,
		/// <summary>TRACK / SONG / CHAPTER
		/// The common parts of an album or movie.</summary>
		Track = 30,
		/// <summary>SUBTRACK / PART / MOVEMENT / SCENE
		/// Corresponds to parts of a track for audio (like a movement).</summary>
		SubTrack = 20,
		/// <summary>SHOT
		/// The lowest hierarchy found in music or movies.</summary>
		Shot = 10,
	}
}