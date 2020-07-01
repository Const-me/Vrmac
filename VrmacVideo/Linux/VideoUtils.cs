namespace VrmacVideo.Linux
{
	static class VideoUtils
	{
		public static bool isMultiPlaneBufferType( this eBufferType bt )
		{
			return bt == eBufferType.VideoOutputMPlane || bt == eBufferType.VideoCaptureMPlane;
		}
	}
}