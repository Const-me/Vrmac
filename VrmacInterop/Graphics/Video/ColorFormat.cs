namespace Diligent.Graphics.Video
{
	/// <summary>Values for EGL_YUV_COLOR_SPACE_HINT_EXT parameter</summary>
	public enum eVideoColorSpace: byte
	{
		/// <summary>EGL_ITU_REC601_EXT</summary>
		REC601,
		/// <summary>EGL_ITU_REC709_EXT</summary>
		REC709,
		/// <summary>EGL_ITU_REC2020_EXT</summary>
		REC2020,
	}

	/// <summary>Values for EGL_SAMPLE_RANGE_HINT_EXT parameter</summary>
	public enum eRange: byte
	{
		/// <summary>EGL_YUV_FULL_RANGE_EXT</summary>
		Full,
		/// <summary>EGL_YUV_NARROW_RANGE_EXT</summary>
		Narrow,
	}

	/// <summary>Values for EGL_YUV_CHROMA_HORIZONTAL_SITING_HINT_EXT and EGL_YUV_CHROMA_VERTICAL_SITING_HINT_EXT parameters</summary>
	public enum eChromaSiting: byte
	{
		/// <summary>EGL_YUV_CHROMA_SITING_0_EXT</summary>
		Zero,
		/// <summary>EGL_YUV_CHROMA_SITING_0_5_EXT</summary>
		PointFive
	}

	/// <summary>Color format of NV12 video</summary>
	public struct ColorFormat
	{
		/// <summary>YUV color space of NV12 data</summary>
		public readonly eVideoColorSpace colorSpace;

		/// <summary>Sample range of the NV12 data</summary>
		/// <remarks>BTW, no matter what I did, Pi4 hardware decoder refused to decode limited-range h264 into full-range YUV,
		/// despite it would probably result in slightly better quality of the output.</remarks>
		public readonly eRange range;

		/// <summary>These parts of EGL are completely undocumented,
		/// but I think 0 means chroma samples are aligned with the luma samples, 0.5 means chroma samples are located with half-pixel offset from luma samples.</summary>
		public readonly eChromaSiting chromaHorizontal, chromaVertical;

		/// <summary>Create the structure</summary>
		public ColorFormat( eVideoColorSpace cs, eRange r, eChromaSiting horiz, eChromaSiting vert )
		{
			colorSpace = cs;
			range = r;
			chromaHorizontal = horiz;
			chromaVertical = vert;
		}

		static string css( eChromaSiting cs )
		{
			switch( cs )
			{
				case eChromaSiting.Zero: return "0";
				case eChromaSiting.PointFive: return "0.5";
				default: return cs.ToString();
			}
		}

		/// <summary>A string for debugging</summary>
		public override string ToString() =>
			$"colorSpace { colorSpace }, range { range }, chroma siting { css( chromaHorizontal ) } horizontal, { css( chromaVertical ) } vertical";
	}
}