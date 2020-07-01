using System;

namespace VrmacVideo.Audio
{
	struct AudioFrame
	{
		public readonly int index;
		public readonly int payloadBytes;
		public readonly TimeSpan timestamp;

		public AudioFrame( int index, int payloadBytes, TimeSpan timestamp )
		{
			this.index = index;
			this.payloadBytes = payloadBytes;
			this.timestamp = timestamp;
		}

		public override string ToString() => $"index { index }, payloadBytes { payloadBytes }, timestamp { timestamp }";
	}

	/// <summary>Queues API for the decoder thread</summary>
	interface iDecoderQueues
	{
		int emptyQueueHandle { get; }
		// int maxEncodedBytes { get; }
		Span<byte> dequeueEmpty( out int idx, int length );
		void enqueueEncoded( int idx, int length, TimeSpan timestamp );
	}

	/// <summary>Queues API for the audio thread</summary>
	interface iPlayerQueues
	{
		int encodedQueueHandle { get; }
		int encodedBuffersCount { get; }
		AudioFrame dequeueEncoded();
		ReadOnlySpan<byte> encodedBuffer( AudioFrame frame );
		void enqueueEmpty( int idx );
	}
}