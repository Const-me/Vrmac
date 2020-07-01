namespace VrmacVideo.IO.ALSA
{
	/// <summary>The ALSA PCM API design uses the states to determine the communication phase between application and library.</summary>
	/// <remarks> The actual state can be determined using <see cref="PcmHandle.state" /></remarks>
	enum ePcmState: int
	{
		/// <summary>The PCM device is in the open state.</summary>
		/// <remarks>After the snd_pcm_open() open call, the device is in this state.
		/// Also, when snd_pcm_hw_params() call fails, then this state is entered to force application calling snd_pcm_hw_params() function to set right communication parameters</remarks>
		Open = 0,
		/// <summary>The PCM device has accepted communication parameters and it is waiting for snd_pcm_prepare() call to prepare the hardware for selected operation (playback or capture).</summary>
		Setup,
		/// <summary>The PCM device is prepared for operation. Application can use snd_pcm_start() call, write or read data to start the operation.</summary>
		Prepared,
		/// <summary>The PCM device has been started and is running. It processes the samples. The stream can be stopped using the snd_pcm_drop() or snd_pcm_drain() calls.</summary>
		Running,
		/// <summary>The PCM device reached overrun (capture) or underrun (playback).
		/// You can use the -EPIPE return code from I/O functions (snd_pcm_writei(), snd_pcm_writen(), snd_pcm_readi(), snd_pcm_readn()) to determine this state without checking the actual state via snd_pcm_state() call.
		/// It is recommended to use the helper function snd_pcm_recover() to recover from this state, but you can also use snd_pcm_prepare(), snd_pcm_drop() or snd_pcm_drain() calls.</summary>
		XRun,
		/// <summary>The device is in this state when application using the capture mode called snd_pcm_drain() function.
		/// Until all data are read from the internal ring buffer using I/O routines (snd_pcm_readi(), snd_pcm_readn()), then the device stays in this state.</summary>
		Draining,
		/// <summary>The device is in this state when application called the <see cref="PcmHandle.pause" /> until the pause is released with.</summary>
		/// <remarks>Not all hardware supports this feature. Application should check the capability with the snd_pcm_hw_params_can_pause()</remarks>
		Paused,
		/// <summary>The device is in the suspend state provoked with the power management system.</summary>
		/// <remarks>The stream can be resumed using snd_pcm_resume() call, but not all hardware supports this feature.
		/// Application should check the capability with the snd_pcm_hw_params_can_resume().
		/// In other case, the calls snd_pcm_prepare(), snd_pcm_drop(), snd_pcm_drain() can be used to leave this state.</remarks>
		Suspended,
		/// <summary>The device is physically disconnected. It does not accept any I/O calls in this state.</summary>
		Disconnected,
	}
}