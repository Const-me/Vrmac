using Diligent;
using System;
using VrmacVideo.IO;
using VrmacVideo.IO.ALSA;

namespace VrmacVideo.Audio.ALSA
{
	/// <summary>PCM handle from ALSA</summary>
	struct PcmHandle
	{
		IntPtr handle;

		public PcmHandle( IntPtr handle )
		{
			this.handle = handle;
		}

		public void finalize()
		{
			if( handle != default )
			{
				libasound.snd_pcm_close( handle );
				handle = default;
			}
		}

		public static implicit operator IntPtr( PcmHandle h ) => h.handle;

		/// <summary>PCM state</summary>
		public ePcmState state => libasound.snd_pcm_state( handle );

		const int readyTimeouty = 250;

		/// <summary>Prepare PCM for use</summary>
		public void prepare()
		{
			libasound.snd_pcm_prepare( handle ).check();
			/* int wr = libasound.snd_pcm_wait( handle, readyTimeouty );
			if( wr < 0 )
				throw new ApplicationException( $"snd_pcm_wait failed: { libasound.errorMessage( wr ) }" );
			if( 0 == wr )
				throw new TimeoutException( "Timeout waiting for the ALSA device to become ready for I/O" ); */
		}

		/// <summary>Start a PCM</summary>
		public void start() =>
			libasound.snd_pcm_start( handle ).check();

		public void pause( bool enable ) =>
			libasound.snd_pcm_pause( handle, enable ).check();

		/// <summary>Get count of poll descriptors</summary>
		public int pollDescriptorsCount => libasound.snd_pcm_poll_descriptors_count( handle );

		/// <summary>Fill the given poll descriptor structs</summary>
		public void setupPollHandles( Span<pollfd> pollHandles, int count )
		{
			if( count > pollHandles.Length )
				throw new ArgumentOutOfRangeException();
			unsafe
			{
				fixed ( pollfd* pointer = pollHandles )
				{
					int res = libasound.snd_pcm_poll_descriptors( handle, pointer, count );
					if( res != count )
						throw new ApplicationException( $"snd_pcm_poll_descriptors asked to setup { count } handles, filled { res } instead" );
				}
			}
		}

		/// <summary>Get returned events from the set of poll descriptors</summary>
		public ePollResult getPollResult( ReadOnlySpan<pollfd> pollHandles, int count )
		{
			unsafe
			{
				fixed ( pollfd* pointer = pollHandles )
				{
					ePollResult res;
					libasound.snd_pcm_poll_descriptors_revents( handle, pointer, count, out res ).check();
					return res;
				}
			}
		}

		/// <summary>Number of frames ready to be written</summary>
		public int availableFrames
		{
			get
			{
				int res = libasound.snd_pcm_avail_update( handle );
				// int res = libasound.snd_pcm_avail( handle );
				if( res >= 0 )
					return res;
				throw new ApplicationException( $"snd_pcm_avail_update failed: { libasound.errorMessage( res ) }" );
			}
		}

		public Span<short> memoryMap( out int offset, ref int samples )
		{
			IntPtr area;
			libasound.snd_pcm_mmap_begin( handle, out area, out offset, ref samples ).check();

			sPcmChannelArea channelArea = Unsafe.readSpan<sPcmChannelArea>( area, 1 )[ 0 ];
			if( channelArea.step != 32 )
				throw new ApplicationException( $"Expected 32 bit / sample for 16 bit stereo, got { channelArea.step } in channelArea.step" );
			if( 0 != ( channelArea.first % 32 ) )
				throw new ApplicationException( $"Audio memory map, the pointer is not aligned" );

			IntPtr startAddress = channelArea.baseAddress + ( channelArea.first + offset * channelArea.step ) / 8;
			int scalarsCount = samples * 2;
			return Unsafe.writeSpan<short>( startAddress, scalarsCount );
		}

		public void memoryCommit( int offset, int samples )
		{
			libasound.snd_pcm_mmap_commit( handle, offset, samples );
		}

		public void dbgDumpSetup( string path = "/tmp/alsa-setup.txt" ) =>
			libasound.pcm_dump_setup( handle, path );

		public void dbgDumpPcm( string path = "/tmp/alsa-pcm.txt" ) =>
			libasound.pcm_dump( handle, path );

		public void dbgDumpStatus( string path = "/tmp/alsa-status.txt" )
		{
			Span<byte> statusSpan = stackalloc byte[ libasound.statusSizeof ];
			unsafe
			{
				fixed ( byte* statusPtr = statusSpan )
				{
					IntPtr status = (IntPtr)statusPtr;
					libasound.snd_pcm_status( handle, status ).check();

					libasound.snd_output_stdio_open( out var output, path, "w" ).check();
					try
					{
						libasound.snd_pcm_status_dump( status, output ).check();
					}
					finally
					{
						libasound.snd_output_close( output );
					}
				}
			}
		}

		public void dbgLogAvailableSamples()
		{
			int avail = libasound.snd_pcm_avail( handle );
			if( avail < 0 )
				throw new ApplicationException( $"snd_pcm_avail failed: { libasound.errorMessage( avail ) }" );
			int avail2 = libasound.snd_pcm_avail_update( handle );
			if( avail2 < 0 )
				throw new ApplicationException( $"snd_pcm_avail_update failed: { libasound.errorMessage( avail2 ) }" );

			Logger.logVerbose( "snd_pcm_avail {0}, snd_pcm_avail_update {1}", avail, avail2 );

			/*
			int avail3;
			Span<byte> statusSpan = stackalloc byte[ libasound.statusSizeof ];
			unsafe
			{
				fixed ( byte* statusPtr = statusSpan )
				{
					IntPtr status = (IntPtr)statusPtr;
					libasound.snd_pcm_status( handle, status ).check();
					avail3 = libasound.snd_pcm_status_get_avail( status );
				}
			}

			Logger.logVerbose( "snd_pcm_avail {0}, snd_pcm_avail_update {1}, snd_pcm_status_get_avail {2}", avail, avail2, avail3 ); */
		}

		public void pause() => libasound.snd_pcm_pause( handle, true ).check();

		public void resume() => libasound.snd_pcm_pause( handle, false ).check();

		/// <summary>Stops the PCM immediately. The pending samples on the buffer are ignored.</summary>
		public void dropData() => libasound.snd_pcm_drop( handle ).check();

		public void resumeAfterSeek()
		{
			prepare();
			/* int res = libasound.snd_pcm_avail_update( handle );
			if( res >= 0 )
			{
				Logger.logVerbose( "resumeAfterSeek: snd_pcm_avail_update returned {0}", res );
				return;
			}
			libasound.snd_pcm_recover( handle, res, 0 ).check();
			Logger.logVerbose( "resumeAfterSeek: snd_pcm_recover completed, {0} frames available", availableFrames ); */
		}
	}
}