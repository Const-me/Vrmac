using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VrmacVideo.Audio.ALSA
{
	/// <summary>Queue with timestamps of frames in Alsa’s circular buffers.</summary>
	/// <remarks>We no longer retain the payload of these frames, the audio for them is decompressed and owned by Alsa.
	/// These frames are not played just yet, but they will be, pretty soon.
	/// Need this queue to find out when exactly each frame is played, and get their presentation timestamps to synchronize video with audio.</remarks>
	struct Queue
	{
		readonly Queue<TimeSpan> bufferedFrames;
		readonly int decodedBuffers;

		public Queue( int decodedBuffers )
		{
			this.decodedBuffers = decodedBuffers;
			bufferedFrames = new Queue<TimeSpan>( decodedBuffers );
		}

		/// <summary>Enqueue a frame</summary>
		public void enqueue( TimeSpan ts )
		{
			bufferedFrames.Enqueue( ts );
		}

		public void enqueueSilence()
		{
			bufferedFrames.Enqueue( TimeSpan.FromTicks( -1 ) );
		}

		/// <summary>If size of this queue is equal to the size of the Alsa’s queue, do nothing and return null.
		/// Otherwise dequeue frames from the queue until it’s of the same size as Alsa’s, and return presentation timestamp of the last dequeued one.</summary>
		public TimeSpan? update( int alsaAvailableFrames )
		{
			int alsaQueueSize = decodedBuffers - alsaAvailableFrames;
			Debug.Assert( alsaQueueSize >= 0 );

			if( bufferedFrames.Count <= alsaQueueSize )
			{
				// Alsa hasn't played any new frames, all frames in the queue are still pending.
				Debug.Assert( bufferedFrames.Count == alsaQueueSize );
				return null;
			}

			// Alsa has played a frames or two of the audio; return presentation timestamp of the latest one of them, that's not silence
			TimeSpan? result = null;
			TimeSpan ts = bufferedFrames.Dequeue();
			if( ts.Ticks >= 0 )
				result = ts;
			while( bufferedFrames.Count > alsaQueueSize )
			{
				ts = bufferedFrames.Dequeue();
				if( ts.Ticks >= 0 )
					result = ts;
			}
			return result;
		}

		/// <summary>True when the queue is completely filled</summary>
		public bool isFull => bufferedFrames.Count == decodedBuffers;

		public void clear() => bufferedFrames.Clear();
	}
}