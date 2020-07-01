namespace VrmacVideo.Containers.MKV
{
	/// <summary>Declare the field ordering of the video. If FlagInterlaced is not set to 1, this Element MUST be ignored.</summary>
	public enum eFieldOrder: byte
	{
		Progressive = 0,
		/// <summary>Top field displayed first. Top field stored first.</summary>
		TopFieldFirst = 1,
		Undetermined = 2,
		/// <summary>Bottom field displayed first. Bottom field stored first.</summary>
		BottomFieldFirst = 6,
		/// <summary>Top field displayed first. Fields are interleaved in storage with the top line of the top field stored first.</summary>
		BottomFieldFirstSwapped = 9,
		/// <summary>Bottom field displayed first. Fields are interleaved in storage with the top line of the top field stored first.</summary>
		TopFieldFirstSwapped = 14,
	}
}