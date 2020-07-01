using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using VrmacVideo.IO;

namespace VrmacVideo
{
	[Flags]
	enum eDecoderBits: ushort
	{
		None = 0,
		Error = ePollEvents.POLLERR,
		Event = ePollEvents.POLLPRI,

		// Capture devices set the POLLIN and POLLRDNORM flags in the revents field
		Decoded1 = ePollEvents.POLLIN,
		Decoded2 = ePollEvents.POLLRDNORM,

		// Output devices the POLLOUT and POLLWRNORM flags
		Encoded1 = ePollEvents.POLLOUT,
		Encoded2 = ePollEvents.POLLWRNORM,
	}

	[Flags]
	enum eAudioBits: ushort
	{
		Error = ePollEvents.POLLERR,
		FreeBuffer = ePollEvents.POLLIN,
	}

	[Flags]
	enum eEventBits: ushort
	{
		Error = ePollEvents.POLLERR,
		Set = ePollEvents.POLLIN,
	}

	static class DecoderPoll
	{
		const eDecoderBits decoderPollBits = eDecoderBits.Event | eDecoderBits.Decoded1 | eDecoderBits.Decoded2 | eDecoderBits.Encoded1 | eDecoderBits.Encoded2;

		[MethodImpl( MethodImplOptions.NoInlining )]
		public static void initHandles( Span<pollfd> waitHandles, int fdDecoder, int fdAudio, int fdSeekEvent, int fdQuitEvent )
		{
			Debug.Assert( waitHandles.Length == 4 );
			waitHandles[ 0 ].fd = fdDecoder;
			waitHandles[ 0 ].events = (ePollEvents)decoderPollBits;

			waitHandles[ 1 ].fd = fdAudio;
			waitHandles[ 1 ].events = ePollEvents.POLLIN;

			waitHandles[ 2 ].fd = fdSeekEvent;
			waitHandles[ 2 ].events = ePollEvents.POLLIN;

			waitHandles[ 3 ].fd = fdQuitEvent;
			waitHandles[ 3 ].events = ePollEvents.POLLIN;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void poll( ReadOnlySpan<pollfd> waitHandles, out eDecoderBits decoderBits, out eAudioBits audio, out bool seekRequest, out bool shutdownRequest )
		{
			LibC.poll( waitHandles );
			decoderBits = (eDecoderBits)waitHandles[ 0 ].revents;
			audio = (eAudioBits)waitHandles[ 1 ].revents;
			ePollEvents seek = waitHandles[ 2 ].revents;
			ePollEvents shutdown = waitHandles[ 3 ].revents;

			if( decoderBits.HasFlag( eDecoderBits.Error ) )
				throw new ApplicationException( "poll(3) returned error status for the decoder device, probably the decoder failed" );
			if( audio.HasFlag( eAudioBits.Error ) )
				throw new ApplicationException( "poll(3) returned error status for the audio queue" );
			if( seek.HasFlag( ePollEvents.POLLERR ) )
				throw new ApplicationException( "poll(3) returned error status for the seek event handle" );
			if( shutdown.HasFlag( ePollEvents.POLLERR ) )
				throw new ApplicationException( "poll(3) returned error status for the shutdown event handle" );

			seekRequest = seek.HasFlag( ePollEvents.POLLIN );
			shutdownRequest = shutdown.HasFlag( ePollEvents.POLLIN );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool hasDecodedBits( this eDecoderBits bits )
		{
			return 0 != ( bits & ( eDecoderBits.Decoded1 | eDecoderBits.Decoded2 ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static bool hasEncodedBits( this eDecoderBits bits )
		{
			return 0 != ( bits & ( eDecoderBits.Encoded1 | eDecoderBits.Encoded2 ) );
		}
	}
}