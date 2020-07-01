using System;

namespace VrmacVideo.Audio
{
	/// <summary>Interface for Alsa player to interact with the rest of the software</summary>
	interface iAudioThread
	{
		/// <summary>Count of encoded frames pending in the thread-local queue</summary>
		int encodedFrames { get; }

		/// <summary>Dequeue a pending frame, decode it to PCM to the specified location, return presentation timestamp of that frame.</summary>
		/// <remarks>Alsa player uses memory mapped IO, the destination location = slice of some circular buffer deep inside Linux audio subsystem.</remarks>
		TimeSpan decodeFrame( Span<short> data );

		/// <summary>Same as above, returns null instead of generating silence</summary>
		TimeSpan? tryDecodeFrame( Span<short> data );

		/// <summary>Signal that playback of an audio frame has just started. For obvious reason (buffering) it happens slightly later than decodeFrame method.</summary>
		void updateTimestamp( TimeSpan timestamp );
	}
}