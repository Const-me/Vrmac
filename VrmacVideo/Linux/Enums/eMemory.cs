namespace VrmacVideo.Linux
{
	/// <summary>C++ enum: v4l2_memory</summary>
	public enum eMemory: int
	{
		/// <summary>The buffer is used for memory mapping I/O</summary>
		MemoryMap = 1,
		/// <summary>The buffer is used for user pointer I/O</summary>
		UserPointer = 2,
		/// <summary>Undocumented</summary>
		Overlay = 3,
		/// <summary>The buffer is used for DMA shared buffer I/O</summary>
		DmaSharedBuffer = 4,
	}
}