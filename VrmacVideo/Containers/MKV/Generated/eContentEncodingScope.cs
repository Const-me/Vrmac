using System;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>A bit field that describes which Elements have been modified in this way. Values (big endian) can be OR'ed.</summary>
	[Flags]
	public enum eContentEncodingScope: byte
	{
		/// <summary>All frame contents, excluding lacing data</summary>
		FrameContent = 1,
		/// <summary>The track's private data</summary>
		PrivateData = 2,
		/// <summary>The next ContentEncoding (next `ContentEncodingOrder`. Either the data inside `ContentCompression` and/or `ContentEncryption`)</summary>
		NextContentEncoding = 4,
	}
}