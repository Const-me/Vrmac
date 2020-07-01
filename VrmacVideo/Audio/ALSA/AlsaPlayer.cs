using System;
using VrmacVideo.Decoders;
using VrmacVideo.IO;
using VrmacVideo.IO.ALSA;

namespace VrmacVideo.Audio.ALSA
{
	sealed class AlsaPlayer: iRenderer
	{
		/// <summary>Duration of uncompressed audio in the ALSA’s circular buffer.</summary>
		const int msDecodedBufferLength = 125;

		/// <summary>Min.count of decoded ALSA’s periods. The player will use max of the two, minDecodedBuffers and msDecodedBufferLength.</summary>
		const int minDecodedBuffers = 4;

		PcmHandle handle;
		readonly int pollHandlesCount;
		readonly int samplesPerFrame;
		readonly Queue queue;

		enum eState: byte
		{
			NotInitialized,
			Prepared,
			Playing,
			Seek,
			Paused,
		}
		volatile eState state = eState.NotInitialized;

		public AlsaPlayer( iAudioDecoder decoder )
		{
			try
			{
				handle = new PcmHandle( Setup.openDefaultOutput() );
			}
			catch( Exception ex )
			{
				throw new ApplicationException( "Unable to open the default audio device", ex );
			}

			int computedCount = (int)Math.Ceiling( 0.001 * msDecodedBufferLength * decoder.sampleRate / decoder.blockSize );
			int decodedBuffers = Math.Max( computedCount, minDecodedBuffers );

			// Logger.logVerbose( "Got the wave output device" );
			try
			{
				Setup.setupHardware( handle, decoder, decodedBuffers );
				Setup.setupSoftware( handle, decoder, decodedBuffers );
				// Logger.logVerbose( "Set up the wave output device" );
				handle.prepare();
			}
			catch( Exception ex )
			{
				// Logger.logError( "Unable to setup audio playback\n" + ex.ToString() );
				throw new ApplicationException( "Unable to setup audio playback", ex );
			}

			// handle.dbgDumpPcm();
			// handle.dbgDumpSetup();
			// handle.dbgDumpStatus();

			pollHandlesCount = handle.pollDescriptorsCount;
			samplesPerFrame = decoder.blockSize;
			queue = new Queue( decodedBuffers );
			state = eState.Prepared;
			Logger.logVerbose( "Initialized ALSA player with {0} decoded buffers; state = {1}; poll handles count {2}", decodedBuffers, handle.state, pollHandlesCount );

			// handle.dbgLogAvailableSamples();
		}

		int iRenderer.pollHandlesCount => pollHandlesCount;

		void iRenderer.setupPollHandles( Span<pollfd> pollHandles )
		{
			handle.setupPollHandles( pollHandles, pollHandlesCount );
			// handle.start();
			Logger.logInfo( "Started the ALSA player; state = {0}", handle.state );
		}

		void iRenderer.handlePollResult( iAudioThread decoder, ReadOnlySpan<pollfd> pollHandles )
		{
			// In mpeg4 sample = what's in the sample rate, frame is much larger unit in the container, after decoding 1 frame can be 1024 samples = 4kb data = 21.33ms of audio.
			// In ALSA, frame = what's in sample rate = 4 bytes for stereo, sample = 16 bit in out case, and what mpeg4 calls "frame" ALSA developers call "period".
			// This code integrate them two together :-(

			ePollResult pollResult = handle.getPollResult( pollHandles, pollHandlesCount );
			int encodedFrames = decoder.encodedFrames;
			if( pollResult == ePollResult.None || 0 == encodedFrames )
			{
				// Nothing to do here: either no encoded frames, or no space for decoded ones
				return;
			}

			int alsaAvailableFrames = handle.availableFrames / samplesPerFrame;
			if( alsaAvailableFrames <= 0 )
				return;

			TimeSpan? playedFrame = queue.update( alsaAvailableFrames );
			if( playedFrame.HasValue )
				decoder.updateTimestamp( playedFrame.Value );

			if( state == eState.Paused )
			{
				int samples = alsaAvailableFrames * samplesPerFrame;
				var span = handle.memoryMap( out int offset, ref samples );
				if( 0 != samples % samplesPerFrame )
					throw new ApplicationException( $"ALSA mapped { samples } samples, incomplete count of frames" );

				int framesToCommit = samples / samplesPerFrame;
				for( int p = 0; p < framesToCommit; p++ )
				{
					span.Slice( p * samplesPerFrame * 2, samplesPerFrame * 2 ).Fill( 0 );
					queue.enqueueSilence();
				}
				handle.memoryCommit( offset, samples );
				return;
			}

			int enqueuedFrames = 0;
			while( true )
			{
				int framesToCommit = Math.Min( encodedFrames, alsaAvailableFrames );
				if( framesToCommit <= 0 )
					break;
				int samples = framesToCommit * samplesPerFrame;

				var span = handle.memoryMap( out int offset, ref samples );
				if( 0 != samples % samplesPerFrame )
					throw new ApplicationException( $"ALSA mapped { samples } samples, incomplete count of frames" );

				framesToCommit = samples / samplesPerFrame;
				for( int p = 0; p < framesToCommit; p++ )
				{
					TimeSpan ts = decoder.decodeFrame( span.Slice( p * samplesPerFrame * 2 ) );
					queue.enqueue( ts );
				}
				handle.memoryCommit( offset, samples );

				alsaAvailableFrames -= framesToCommit;
				encodedFrames -= framesToCommit;
				enqueuedFrames += framesToCommit;
			}

			if( 0 == enqueuedFrames )
			{
				Logger.logVerbose( "Alsa player is idle: encodedFrames {0}, alsaAvailableFrames {1}, state {2}", encodedFrames, alsaAvailableFrames, handle.state );
				return;
			}
			if( state == eState.Prepared && queue.isFull )
			{
				handle.start();
				state = eState.Playing;
				Logger.logInfo( "Filled Alsa’s buffer and started audio playback; state {0}", handle.state );
			}
			/* else if( state == eState.Seek && queue.isFull )
			{
				decoder.seekCompleted();
			} */
			// else Logger.logVerbose( "Enqueued {0} to Alsa; state {1}", enqueuedFrames.pluralString( "frame" ), handle.state );
		}

		void IDisposable.Dispose()
		{
			handle.finalize();
		}

		void iRenderer.pause()
		{
			switch( state )
			{
				case eState.Playing:
					state = eState.Paused;
					Logger.logVerbose( "Paused audio playback" );
					return;
				case eState.Paused:
					Logger.logWarning( "Already paused, doing nothing" );
					return;
				default:
					throw new ApplicationException( $"Pause request, unsupported state transition from { state }" );
			}
		}

		void iRenderer.resume()
		{
			switch( state )
			{
				case eState.Playing:
					Logger.logWarning( "Already playing, doing nothing" );
					return;
				case eState.Paused:
					state = eState.Playing;
					Logger.logVerbose( "Resumed audio playback" );
					return;
				default:
					throw new ApplicationException( $"Resume request, unsupported state transition from { state }" );
			}
		}

		void iRenderer.endSeek()
		{
			handle.start();
			state = eState.Playing;
		}

		void iRenderer.beginSeek()
		{
			handle.dropData();
			queue.clear();
			state = eState.Seek;
		}

		void iRenderer.prepareEndSeek()
		{
			// handle.dropData();
			// handle.prepare();
			handle.resumeAfterSeek();
			/* int avail = handle.availableFrames;
			if( avail < samplesPerFrame )
				throw new ApplicationException();

			int samples = samplesPerFrame;
			var span = handle.memoryMap( out int offset, ref samples );
			TimeSpan ts = decoder.decodeFrame( span );
			queue.enqueue( ts );
			handle.memoryCommit( offset, samples );

			/* handle.dbgDumpPcm();
			handle.dbgDumpSetup();
			handle.dbgDumpStatus();
			handle.dbgLogAvailableSamples(); */

			// Logger.logVerbose( "AlsaPlayer.prepareEndSeek, state = {0}, available = {1}", handle.state, handle.availableFrames );
		}

		bool iRenderer.isFull => queue.isFull;

		void iRenderer.decodeInitial( iAudioThread decoder )
		{
			int avail = handle.availableFrames;
			if( avail < samplesPerFrame )
				throw new ApplicationException();

			int samples = samplesPerFrame;
			var span = handle.memoryMap( out int offset, ref samples );
			TimeSpan ts = decoder.decodeFrame( span );
			handle.memoryCommit( offset, samples );
			queue.enqueue( ts );
		}

		void iRenderer.dbgDumpStatus()
		{
			handle.dbgDumpPcm();
			handle.dbgDumpSetup();
			handle.dbgDumpStatus();
			handle.dbgLogAvailableSamples();
		}
	}
}