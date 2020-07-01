namespace VrmacVideo.IO.ALSA
{
	/// <summary>PCM access type</summary>
	enum ePcmAccessType: int
	{
		/// <summary>mmap access with simple interleaved channels</summary>
		MemoryMapInterleaved = 0,
		/// <summary>mmap access with simple non interleaved channels</summary>
		MemoryMapNonInterleaved,
		/// <summary>mmap access with complex placement</summary>
		MemoryMapComplex,
		/// <summary>snd_pcm_readi/snd_pcm_writei access</summary>
		ReadWriteInterleaved,
		/// <summary>snd_pcm_readn/snd_pcm_writen access</summary>
		ReadWriteNonInterleaved,
	}
}