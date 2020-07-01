using System;
using VrmacVideo.IO;

namespace VrmacVideo.Audio
{
	/// <summary>Plays decoded PCM samples.</summary>
	/// <remarks>The library only has 1 implementation of this interface, <see cref="ALSA.AlsaPlayer" /> but technically should be possible to have more, e.g. Jack or Pulse.</remarks>
	interface iRenderer: IDisposable
	{
		int pollHandlesCount { get; }
		void setupPollHandles( Span<pollfd> pollHandles );
		void handlePollResult( iAudioThread decoder, ReadOnlySpan<pollfd> pollHandles );

		void pause();
		void resume();

		void beginSeek();
		void prepareEndSeek();
		void endSeek();

		bool isFull { get; }
		void decodeInitial( iAudioThread decoder );

		void dbgDumpStatus();
	}
}