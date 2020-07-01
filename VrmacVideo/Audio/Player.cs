using System;

namespace VrmacVideo.Audio
{
	sealed class Player: iAudioPlayer
	{
		public static iAudioPlayer create( iAudioTrack audioTrack, int encodedBuffers, int shutdownEvent )
		{
			var info = audioTrack.info;
			return new Player( info, encodedBuffers, shutdownEvent );
		}

		Player( TrackInfo trackInfo, int encodedBuffers, int shutdownEvent )
		{
			queues = new Queues( trackInfo, encodedBuffers );

			thread = new AudioThread( queues, shutdownEvent, ref trackInfo );
			Logger.logVerbose( "Audio track info: {0}", trackInfo );
			// Logger.logVerbose( "Decoded stream info: {0}", stmInfo );
		}

		public iDecoderQueues decoderQueues { get; }

		public void Dispose() { }

		readonly Queues queues;
		iDecoderQueues iAudioPlayer.decoderQueues => queues;

		readonly AudioThread thread;

		void iAudioPlayer.play()
		{
			thread.play();
		}

		void iAudioPlayer.pause()
		{
			thread.pause();
		}

		void iAudioPlayer.marshalException() => thread?.marshalException();

		void iAudioPlayer.setPresentationClock( iAudioPresentationClock clock ) => thread?.setPresentationClock( clock );

		byte iAudioPlayer.getVolume() => thread.pendingQueue.volume;
		void iAudioPlayer.setVolume( byte val ) => thread.pendingQueue.volume = val;
		void iAudioPlayer.seek( TimeSpan where ) => thread.seek( where );

		void IDisposable.Dispose()
		{

		}
	}
}