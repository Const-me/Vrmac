using System;
using System.Diagnostics;
using Vrmac;
using VrmacVideo.IO;

namespace VrmacVideo
{
	sealed partial class DecoderThread
	{
		MediaSeekPosition iDecoderThread.findStreamsPosition( TimeSpan where )
		{
			StreamPosition video = reader.findStreamPosition( where );
			StreamPosition audio = null;
			if( null != audioReader )
				audio = audioReader.findStreamPosition( where );
			return new MediaSeekPosition( video, audio );
		}

		readonly object syncRoot = new object();
		MediaSeekPosition? seekPosition;

		void iDecoderThread.seek( ref MediaSeekPosition pos )
		{
			lock( syncRoot )
			{
				seekPosition = pos;
				seekEventHandle.set();
			}
		}

		/// <summary>Called from the main loop of this thread in response to seekEventHandle event being signalled.</summary>
		bool handleSeek()
		{
			seekEventHandle.reset();
			MediaSeekPosition seekPosition;
			lock( syncRoot )
			{
				if( !this.seekPosition.HasValue )
					throw new ApplicationException();
				seekPosition = this.seekPosition.Value;
			}

			Logger.logVerbose( "Processing seek request: {0}", seekPosition );

			StreamPosition audioSample = null;
			if( null != seekPosition.audio && null != audioReader )
				audioSample = audioReader.findKeyFrame( seekPosition.audio );
			StreamPosition videoSample = reader.findKeyFrame( seekPosition.video );

			eventsSink.discardDecoded();
			presentationClock.waitForAudioDrain();

			if( null != audioReader )
				audioReader.seekToSample( audioSample );
			reader.seekToSample( videoSample );

			Stopwatch sw = Stopwatch.StartNew();
			if( !waitForVideoFrame( seekPosition.video.time ) )
				return false;
			TimeSpan elapsed = sw.Elapsed;

			/* int frames = seekPosition.video.Value.sample - videoSample + 1;
			Logger.logDebug( "DecoderThread.handleSeek decoded the target frame of the video. It took {0:G3} seconds and {1}",
				elapsed.TotalSeconds, frames.pluralString( "frame" ) ); */

			presentationClock.signalVideoReady();
			return true;
		}

		bool waitForVideoFrame( TimeSpan timestamp )
		{
			timestamp = timestamp.floorToMicro();

			Span<pollfd> pollHandles = stackalloc pollfd[ 4 ];
			DecoderPoll.initHandles( pollHandles, device.file, audioQueue.emptyQueueHandle, seekEventHandle, shutdownEvent );

			eDecoderBits decoderBits;
			eAudioBits audio;
			bool seek, shutdown;

			while( true )
			{
				DecoderPoll.poll( pollHandles, out decoderBits, out audio, out seek, out shutdown );

				if( shutdown )
					return false;
				if( seek )
					throw new ApplicationException( "Invalid state transition, seek request while still processing previous one." );

				if( audio.HasFlag( eAudioBits.FreeBuffer ) )
					dispatchAudio();

				if( decoderBits == eDecoderBits.None )
					continue;

				// Logger.logVerbose( "Video decoder poll result: {0}", decoderBits );

				if( decoderBits.HasFlag( eDecoderBits.Event ) )
				{
					dispatchEvents();
				}

				if( decoderBits.hasDecodedBits() )
				{
					Debug.Assert( decoded.anyKernelBuffer );
					DecodedBuffer buffer = decoded.dequeue();
					if( buffer.timestamp != timestamp )
					{
						if( buffer.timestamp > timestamp )
							throw new ApplicationException( @"There’s a bug somewhere in this library; time is kept in 64-bit integers and shouldn’t suffer from precision-related issues." );

						// Logger.logVerbose( "DecoderThread.waitForVideoFrame got {0}, need {1}", buffer.timestamp, timestamp );
						// Video starts from a keyframe, very likely need to decode + discard a few frames before getting the one we need.
						decoded.enqueue( buffer );
					}
					else
					{
						Logger.logVerbose( "DecoderThread.waitForVideoFrame got the target {0}", buffer.timestamp );
						eventsSink.onFrameDecoded( buffer );
						return true;
					}
				}

				if( decoderBits.hasEncodedBits() )
					dispatchInput();
			}
		}
	}
}