using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vrmac;
using VrmacVideo.Decoders;

namespace VrmacVideo.Audio
{
	/// <summary>The queue is private to audio thread. Holds encoded samples received by the thread, also drives the decoder.</summary>
	sealed class PendingQueue
	{
		readonly Queue<AudioFrame> encodedFrames;
		public readonly iAudioDecoder decoder;
		public readonly iPlayerQueues queues;
		readonly bool copiesCompressedData;

		public int pendingFrames
		{
			get
			{
				int res = encodedFrames.Count;
				if( decoder.blocksLeft > 0 || partialFrame.HasValue )
					res++;
				return res;
			}
		}

		public PendingQueue( int encodedBuffersCount, iAudioDecoder decoder, iPlayerQueues queues )
		{
			this.decoder = decoder;
			this.queues = queues;
			encodedFrames = new Queue<AudioFrame>( encodedBuffersCount );
			copiesCompressedData = decoder.copiesCompressedData;
		}

		public byte channelsCount => decoder.channelsCount;

		public int blockSize => decoder.blockSize;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void enqueue()
		{
			var d = queues.dequeueEncoded();
			encodedFrames.Enqueue( d );
		}

		public void enqueue( AudioFrame frame ) =>
			encodedFrames.Enqueue( frame );

		AudioFrame? partialFrame = null;
		int consumedBytes = 0;
		public volatile byte volume = 0xFF;

		enum eBlockAction: byte
		{
			Decode,
			// Theoretically may be empty blocks: params, timestamps, etc., which don't produce audio
			Retry,
			Fail,
		}

		public bool haveFrameData => decoder.blocksLeft > 0 || partialFrame.HasValue || encodedFrames.Count > 0;

		eBlockAction prepareNextBlock()
		{
			if( decoder.blocksLeft > 0 )
			{
				// E.g. DTS produces 6 blocks per frame
				// Logger.logVerbose( "The decoder has {0} left", decoder.blocksLeft.pluralString( "block" ) );
				return eBlockAction.Decode;
			}

			if( partialFrame.HasValue && partialFrame.Value.payloadBytes > consumedBytes )
			{
				// Have partial frame from the previous call.
				// Not sure if happens in reality, but technically 1 mp4/mkv frame can contain multiple decoder frames.
				var source = queues.encodedBuffer( partialFrame.Value );
				source = source.Slice( consumedBytes );
				int cb = decoder.sync( source );
				consumedBytes += cb;
				decoder.decodeFrame( source, volume );

				eBlockAction act = eBlockAction.Retry;
				if( decoder.blocksLeft > 0 )
					act = eBlockAction.Decode;

				if( consumedBytes >= source.Length && copiesCompressedData )
				{
					// Finished decoding the frame, release the buffer for refill
					queues.enqueueEmpty( partialFrame.Value.index );
					partialFrame = null;
				}
				return act;
			}

			if( encodedFrames.TryDequeue( out var frame ) )
			{
				if( !copiesCompressedData && partialFrame.HasValue )
				{
					queues.enqueueEmpty( partialFrame.Value.index );
					partialFrame = null;
				}

				// Have a new frame on the local queue
				timestamp = frame.timestamp;

				var source = queues.encodedBuffer( frame );
				int cb = decoder.sync( source );

				decoder.decodeFrame( source, volume );
				eBlockAction act = eBlockAction.Retry;
				if( decoder.blocksLeft > 0 )
				{
					// Logger.logVerbose( "The decoder has decoded a frame {0}, it has {1}", timestamp, decoder.blocksLeft.pluralString( "audio block" ) );
					act = eBlockAction.Decode;
				}
				else
				{
					// Logger.logVerbose( "The decoder has decoded a frame {0} but it had no audio blocks ", timestamp );
				}

				if( cb >= source.Length && copiesCompressedData )
				{
					// Logger.logVerbose( "The decoder has consumed the complete frame" );
					// The decoder has consumed the whole frame, release for another thread to refill
					queues.enqueueEmpty( frame.index );
				}
				else
				{
					// Logger.logVerbose( "The decoder only consumed {0} out of {1} bytes of the frame", cb, source.Length );
					// The decoder only consumed a part of it, keep the buffer.
					consumedBytes = cb;
					partialFrame = frame;
				}
				return act;
			}
			return eBlockAction.Fail;
		}

		public bool nextBlock( Span<short> pcm )
		{
			while( true )
			{
				var a = prepareNextBlock();
				if( a == eBlockAction.Decode )
				{
					decoder.decodeBlock( pcm );
					return true;
				}
				if( a == eBlockAction.Retry )
					continue;
				return false;
			}
		}

		public TimeSpan timestamp { get; private set; }

		public void discardAndFlush()
		{
			if( partialFrame.HasValue )
			{
				queues.enqueueEmpty( partialFrame.Value.index );
				partialFrame = null;
			}
			while( encodedFrames.TryDequeue( out var frame ) )
				queues.enqueueEmpty( frame.index );
		}
	}
}