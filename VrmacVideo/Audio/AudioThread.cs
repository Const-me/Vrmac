using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using VrmacVideo.Decoders;
using VrmacVideo.Decoders.Audio;
using VrmacVideo.IO;

namespace VrmacVideo.Audio
{
	sealed class AudioThread: iAudioThread
	{
		const ThreadPriority priority = ThreadPriority.AboveNormal;

		readonly int shutdownEvent;
		public readonly PendingQueue pendingQueue;
		readonly iRenderer render;

		readonly Thread thread;
		public bool running => thread.IsAlive;

		readonly EventHandle seekEventHandle;

		volatile iAudioPresentationClock m_clock;
		public void setPresentationClock( iAudioPresentationClock clock ) => Interlocked.Exchange( ref m_clock, clock );

		TimeSpan? seekDestination = null;

		public AudioThread( iPlayerQueues queues, int shutdownEvent, ref TrackInfo track )
		{
			this.shutdownEvent = shutdownEvent;

			var dec = AudioDecoders.create( ref track );
			render = new ALSA.AlsaPlayer( dec );
			pendingQueue = new PendingQueue( queues.encodedBuffersCount, dec, queues );
			seekEventHandle = EventHandle.create();

			thread = new Thread( threadMain )
			{
				IsBackground = true,
				Priority = priority,
				Name = "Audio thread"
			};
			// Logger.logVerbose( "AudioThread initialized OK" );
		}

		public void play()
		{
			if( !thread.IsAlive )
			{
				Logger.logInfo( "Launching the audio thread" );
				thread.Start();
				return;
			}
			render.resume();
		}

		public void pause()
		{
			render.pause();
		}

		volatile ExceptionDispatchInfo threadException = null;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void marshalException()
		{
			ExceptionDispatchInfo edi = Interlocked.Exchange( ref threadException, null );
			if( null == edi )
				return;
			edi.Throw();
		}

		void threadMain()
		{
			try
			{
				runThread();
			}
			catch( Exception ex )
			{
				ex.logError( "Audio thread failed" );
				threadException = ExceptionDispatchInfo.Capture( ex );
			}
		}

		void setupInternalHandles( Span<pollfd> waitHandles, int startIndex )
		{
			waitHandles[ startIndex ] = new pollfd() { fd = pendingQueue.queues.encodedQueueHandle, events = ePollEvents.POLLIN };
			waitHandles[ startIndex + 1 ] = new pollfd() { fd = shutdownEvent, events = ePollEvents.POLLIN };
			waitHandles[ startIndex + 2 ] = new pollfd() { fd = seekEventHandle, events = ePollEvents.POLLIN };
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		void runThread()
		{
			int playerHandlesCount = render.pollHandlesCount;
			Span<pollfd> waitHandles = stackalloc pollfd[ playerHandlesCount + 3 ];
			render.setupPollHandles( waitHandles.Slice( 0, playerHandlesCount ) );
			setupInternalHandles( waitHandles, playerHandlesCount );

			while( true )
			{
				LibC.poll( waitHandles );
				if( waitHandles[ playerHandlesCount + 1 ].revents.HasFlag( ePollEvents.POLLIN ) )
				{
					// Asked to shut down
					return;
				}
				if( waitHandles[ playerHandlesCount + 2 ].revents.HasFlag( ePollEvents.POLLIN ) )
				{
					// Got a seek event
					if( !handleSeek() )
						return;
					Logger.logDebug( "Audio thread seek complete" );
					continue;
				}

				if( waitHandles[ playerHandlesCount ].revents.HasFlag( ePollEvents.POLLIN ) )
				{
					// Got a buffer with encoded frame.
					// Dequeue from Linux kernel so the poll() no longer triggers the data available bit for that particular frame, enqueue into the thread-local managed-only queue.
					// var d = queues.dequeueEncoded();
					// encodedFrames.Enqueue( d );
					pendingQueue.enqueue();
				}
				render.handlePollResult( this, waitHandles );
			}
		}

		int iAudioThread.encodedFrames => pendingQueue.pendingFrames;

#if false
		bool bFirstFrame = true;
		int dbgFrameSerial = 0;
		void dbgSaveFrame( ReadOnlySpan<byte> frame, bool decoded )
		{
			string dir = @"/tmp/audio";
			if( !Directory.Exists( dir ) )
				Directory.CreateDirectory( dir );

			int sn;
			if( decoded )
				sn = dbgFrameSerial++;
			else
				sn = dbgFrameSerial;

			string name = string.Format( "audio{0:D4}-{1}.bin", sn, decoded ? 'd' : 'e' );
			using( var f = File.Create( Path.Combine( dir, name ) ) )
				f.Write( frame );
		}
#endif

		bool underrun = false;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		TimeSpan iAudioThread.decodeFrame( Span<short> data )
		{
			if( pendingQueue.nextBlock( data ) )
				return pendingQueue.timestamp;

			if( !underrun )
			{
				underrun = true;
				Logger.logWarning( "Audio buffer underrun, the file was too slow to read; generating silence" );
			}

			data.Fill( 0 );
			return pendingQueue.timestamp;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		TimeSpan? iAudioThread.tryDecodeFrame( Span<short> data )
		{
			if( pendingQueue.nextBlock( data ) )
				return pendingQueue.timestamp;
			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void iAudioThread.updateTimestamp( TimeSpan timestamp )
		{
			// Logger.logVerbose( "iAudioThread.updateTimestamp: {0}", timestamp );
			iAudioPresentationClock clock = m_clock;
			clock?.updateTimestamp( timestamp );
		}

		public void seek( TimeSpan where )
		{
			lock( this )
			{
				seekDestination = where;
				seekEventHandle.set();
			}
		}

		// Feed all available data to decoder and ALSA. Return true if no free space left in the audio driver.
		bool fillBuffer()
		{
			while( true )
			{
				if( render.isFull )
				{
					Logger.logVerbose( "Audio thread filled decoded queue after seek; resuming playback" );
					render.endSeek();
					return true;
				}
				if( pendingQueue.haveFrameData )
				{
					render.decodeInitial( this );
					continue;
				}
				return false;
			}
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		bool handleSeek()
		{
			// Get seek destination timestamp
			TimeSpan seekDest;
			lock( this )
			{
				if( !seekDestination.HasValue )
					throw new ApplicationException( "Seek event was set, but no destination timestamp" );
				seekEventHandle.reset();
				seekDest = seekDestination.Value;
			}

			// Stop playing, drop data in the ALSA queue
			render.beginSeek();

			pendingQueue.discardAndFlush();
			// Tell decoder thread it can now send new samples
			m_clock?.drainComplete();

			// Wait for video decoder to catch up
			if( !m_clock.waitForVideoFrame() )
				return false;

			// Run the poll waiting for that special frame
			Span<pollfd> waitHandles = stackalloc pollfd[ 3 ];
			setupInternalHandles( waitHandles, 0 );

			bool foundTarget = false;
			while( true )
			{
				LibC.poll( waitHandles );

				if( waitHandles[ 1 ].revents.HasFlag( ePollEvents.POLLIN ) )
				{
					// Asked to shut down
					return false;
				}
				if( waitHandles[ 2 ].revents.HasFlag( ePollEvents.POLLIN ) )
				{
					// Got another seek event..
					Logger.logWarning( "Multiple seeks are not supported, probably won’t work" );
					lock( this )
					{
						if( !seekDestination.HasValue )
							throw new ApplicationException( "Seek event was set, but no destination timestamp" );
						seekEventHandle.reset();
						seekDest = seekDestination.Value;
						seekDestination = null;
					}
				}

				if( waitHandles[ 0 ].revents.HasFlag( ePollEvents.POLLIN ) )
				{
					// Got a buffer with encoded frame.
					var d = pendingQueue.queues.dequeueEncoded();

					if( foundTarget )
					{
						pendingQueue.enqueue( d );
						if( fillBuffer() )
							return true;
					}
					else
					{
						if( d.timestamp != seekDest )
						{
							Logger.logVerbose( "Audio thread got sample {0}, needs {1}", d.timestamp, seekDest );
							// Not the one we're looking for
							pendingQueue.queues.enqueueEmpty( d.index );
							continue;
						}
						// Received seek destination frame.
						Logger.logVerbose( "Audio thread got the destination audio sample after seek, preparing to resume the playback" );
						pendingQueue.enqueue( d );
						render.prepareEndSeek();
						foundTarget = true;
						if( fillBuffer() )
							return true;
					}
				}
			}
		}
	}
}