namespace VrmacVideo.Containers.MP4
{
	/// <summary>Symbolic names of predefined sync layer configuration</summary>
	enum ePredefinedSyncLayerConfig: byte
	{
		/// <summary>It’s a custom one</summary>
		Custom = 0,
		/// <summary>Null SL packet header</summary>
		NullSyncLayerHeader = 1,
		/// <summary>Reserved for use in MP4 files</summary>
		Mpeg4 = 2,
	}
}