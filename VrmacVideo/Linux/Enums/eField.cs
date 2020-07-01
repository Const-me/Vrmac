namespace VrmacVideo.Linux
{
	// v4l2_field
	public enum eField: uint
	{
		/// <summary>driver can choose from none, top, bottom, interlaced depending on whatever it thinks is approximate</summary>
		Any = 0,
		/// <summary>Images are in progressive format, not interlaced</summary>
		Progressive = 1,
		/// <summary>top field only</summary>
		Top = 2,
		/// <summary>bottom field only</summary>
		Bottom = 3,
		/// <summary>both fields interlaced</summary>
		Interlaced = 4,
		/// <summary>both fields sequential into one buffer, top-bottom order</summary>
		SequentialTopBottom = 5,
		/// <summary>same as above + bottom-top order</summary>
		SequentialBottomTop = 6,
		/// <summary>both fields alternating into separate buffers</summary>
		Alternate = 7,
		/// <summary>both fields interlaced, top field first and the top field is transmitted first</summary>
		InterlacedTopBottom = 8,
		/// <summary>both fields interlaced, top field first and the bottom field is transmitted first</summary>
		InterlacedBottomTop = 9,
	}
}