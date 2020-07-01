using System;

namespace Vrmac.MediaEngine
{
	/// <summary>Media streams of a media resource</summary>
	[Flags]
	public enum eMediaStreams: byte
	{
		/// <summary>The current media resource contains an audio stream</summary>
		Audio = 1,
		/// <summary>The current media resource contains a video stream</summary>
		Video = 2,
	}
}