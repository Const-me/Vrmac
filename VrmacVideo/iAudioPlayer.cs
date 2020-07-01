using System;
using VrmacVideo.Audio;

namespace VrmacVideo
{
	/// <summary>Interface to interact with the audio thread</summary>
	interface iAudioPlayer: IDisposable
	{
		iDecoderQueues decoderQueues { get; }

		void play();
		void pause();

		/// <summary>If audio thread fails with an exception it doesn’t crash the process, instead it captures the exception.
		/// This method does nothing when the thread is in good state, or throws that captured exception.</summary>
		void marshalException();

		/// <summary>Set an interface to call when audio presentation timestamp is advanced as the result of playing that audio.</summary>
		void setPresentationClock( iAudioPresentationClock clock );

		byte getVolume();
		void setVolume( byte volume );

		// 1. Stop playback and drain all queues.
		// 2. Notify with iAudioPresentationClock.drainComplete
		// 3. Receive + discard samples until there's one with the specified timestamp
		// 4. Once received that particular samples, queue it and the next ones to ALSA.
		// 5. Once ALSA decoded buffer full, notify with iAudioPresentationClock.seekComplete
		// You can then resume playback with play() call, if the destination video sample has arrived as well.
		void seek( TimeSpan where );
	}
}