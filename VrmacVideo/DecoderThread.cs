using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	/// <summary>Reads frames from mp4 file, both audio and video.
	/// Enqueues video frames into the corresponding V4L2 queue.
	/// Dequeues decoded video frames from another V4L2 queue, calls <see cref="iDecoderEvents.onFrameDecoded" /> when they arrive.
	/// For the audio stream, it posts encoded frames to the dedicated audio thread for decoding and playback.</summary>
	sealed partial class DecoderThread: iDecoderThread
	{
		readonly VideoDevice device;
		readonly iVideoTrackReader reader;
		readonly EncodedQueue encoded;
		readonly DecodedQueue decoded;
		readonly iAudioTrackReader audioReader;
		readonly Audio.iDecoderQueues audioQueue;
		readonly int shutdownEvent;
		readonly Thread thread;
		readonly iDecoderEvents eventsSink;
		readonly EventHandle seekEventHandle;
		PresentationClock presentationClock;

		public DecoderThread( VideoDevice device, iVideoTrackReader reader, EncodedQueue encoded, DecodedQueue decoded, int shutdownEvent, iDecoderEvents eventsSink,
			iAudioTrackReader audioReader, Audio.iDecoderQueues audioQueue )
		{
			this.device = device;
			this.reader = reader;
			this.encoded = encoded;
			this.decoded = decoded;
			this.shutdownEvent = shutdownEvent;
			this.audioReader = audioReader;
			this.audioQueue = audioQueue;

			// Enqueue decoded buffers
			decoded.enqueueAll();

			this.eventsSink = eventsSink;
			seekEventHandle = EventHandle.create();

			// Remaining work is done in the thread
			thread = new Thread( threadMain );
			thread.IsBackground = true;
			thread.Name = "Media player thread";
			Logger.logInfo( "Launching the video decoding thread" );
			thread.Start();
		}

		void iDecoderThread.setPresentationClock( PresentationClock clock ) => presentationClock = clock;

		volatile ExceptionDispatchInfo threadException = null;

		void threadMain()
		{
			try
			{
				runThread();
			}
			catch( Exception ex )
			{
				ex.logError( "Video decoding thread failed" );
				var edi = ExceptionDispatchInfo.Capture( ex );
				Interlocked.Exchange( ref threadException, edi );
			}
		}

		void runThread()
		{
			Span<pollfd> pollHandles = stackalloc pollfd[ 4 ];
			DecoderPoll.initHandles( pollHandles, device.file, audioQueue.emptyQueueHandle, seekEventHandle, shutdownEvent );

			eDecoderBits decoderBits;
			eAudioBits audio;
			bool seek, shutdown;

			while( true )
			{
				DecoderPoll.poll( pollHandles, out decoderBits, out audio, out seek, out shutdown );

				if( shutdown )
					return;
				if( seek )
				{
					if( !handleSeek() )
						return;
					continue;
				}

				if( audio.HasFlag( eAudioBits.FreeBuffer ) )
					dispatchAudio();

				if( decoderBits == eDecoderBits.None )
					continue;

				// Logger.logVerbose( "Video decoder poll result: {0}", decoderBits );

				if( decoderBits.HasFlag( eDecoderBits.Event ) )
				{
					dispatchEvents();
					// Logger.logVerbose( "Dispatched an event" );
				}

				if( decoderBits.hasDecodedBits() )
					dispatchOutput();

				if( decoderBits.hasEncodedBits() )
					dispatchInput();
			}
		}

		/// <summary>Dispatch output queue, called "capture" in V4L2</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void dispatchOutput()
		{
			Debug.Assert( decoded.anyKernelBuffer );
			DecodedBuffer buffer = decoded.dequeue();
			eventsSink.onFrameDecoded( buffer );
		}

		/// <summary>Dispatch input queue, called "output" in V4L2</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void dispatchInput()
		{
			Debug.Assert( encoded.anyKernelBuffer );
			var b = encoded.dequeue();
			while( true )
			{
				var act = reader.writeNextNalu( b );
				if( act == eNaluAction.EOF )
					return;
				if( act == eNaluAction.Ignore )
					continue;
				encoded.enqueue( b );
				return;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void dispatchEvents()
		{
			while( true )
			{
				sEvent evt = new sEvent();
				device.file.call( eControlCode.DQEVENT, ref evt );
				switch( evt.type )
				{
					case eEventType.EndOfStream:
						eventsSink.onEndOfStream();
						break;
					case eEventType.SourceChange:
						if( evt.u.sourceChanges.HasFlag( eSourceChanges.Resolution ) )
							eventsSink.onDynamicResolutionChange();
						break;
					default:
						// Logger.logVerbose( "Received a decoder event: {0}", evt );
						break;
				}

				if( evt.pending <= 0 )
				{
					// Logger.logVerbose( "Dispatched all events" );
					break;
				}
				else
				{
					// Logger.logVerbose( "Dispatched an event, {0} pending ones", evt.pending );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void dispatchAudio()
		{
			audioReader.read( audioQueue );
		}

		void iDecoderThread.marshalException()
		{
			ExceptionDispatchInfo edi = Interlocked.Exchange( ref threadException, null );
			if( null == edi )
				return;
			edi.Throw();
		}
	}
}