using Diligent.Graphics;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Vrmac;
using Vrmac.MediaEngine;
using Vrmac.Utils;
using VrmacVideo.Containers;
using VrmacVideo.Decoders;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	/// <summary>Implements most parts of the media engine, but relies on the derived class for integration with GLES.</summary>
	public abstract class LinuxEngine: iMediaEngine, iLinuxMediaEngine
	{
		protected abstract ITextureView createOutputTexture( IRenderDevice device, TextureFormat format, CSize size );
		protected abstract IRenderDevice getRenderDevice();
		protected abstract void initRendering( IRenderDevice device, Nv12Texture[] textures, ref sDecodedVideoSize videoSize );
		protected abstract void renderFrame( int bufferIndex );

		/// <summary>Count of encoded buffers for the video</summary>
		protected virtual int encodedBuffersCount => 2;

		/// <summary>Count of decoded buffers for the video. If RGB texture is used it doesn't count, only NV12 frames do.</summary>
		protected virtual int decodedBuffersCount => 4;

		/// <summary>Count of encoded buffers for the audio. They are much smaller, but come at about twice the frequency.</summary>
		/// <remarks>ALSA has a few more buffers.
		/// The compressed audio buffers initialized from this value are only for reading audio samples from mp4 file, and for the queue between the threads.</remarks>
		protected virtual int audioEncodedBuffers => 4;

		const string h264DecoderDevice = "/dev/video10";

		readonly H264 h264;
		iMediaFile file;
		readonly Rational? displayRefresh;

		public LinuxEngine( iSimdUtils simd, Rational? displayRefresh )
		{
			MiscUtils.simd = simd;
			h264 = new H264( VideoDevice.open( h264DecoderDevice ) );
			this.displayRefresh = displayRefresh;
			// Logger.logVerbose( "Decoder: {0}", h264.ToString() );
			// PrintSizeofs.print();
		}

		eMediaStreams getMediaStreams()
		{
			eMediaStreams ms = default;
			if( null != file?.videoTrack )
				ms |= eMediaStreams.Video;
			if( null != file?.audioTrack )
				ms |= eMediaStreams.Audio;
			return ms;
		}
		eMediaStreams iMediaEngine.mediaStreams => getMediaStreams();
		void iMediaEngine.getMediaStreams( out eMediaStreams mediaStreams ) => mediaStreams = getMediaStreams();

		CSize iMediaEngine.getNativeVideoSize() => decodedSize.cropRect.size;
		CSize iMediaEngine.nativeVideoSize => decodedSize.cropRect.size;

		eCanPlay iMediaEngine.canPlayType( string type )
		{
			switch( type )
			{
				case "video/mp4":
				case "video/mkv":
					return eCanPlay.Probably;
				case "application/octet-stream":
					return eCanPlay.Maybe;
				default:
					return eCanPlay.NotSupported;
			}
		}

		protected eChromaFormat chromaFormat => file.videoTrack.chromaFormat;

		StatefulVideoDecoder decoder;
		PresentationClock presentationClock;

		enum ePresentMode: byte
		{
			None,
			RGB,
			NV12,
		}
		ePresentMode presentMode = ePresentMode.None;

		protected void setNv12PresentMode()
		{
			if( presentMode != ePresentMode.None )
				throw new ApplicationException( "Video present mode can only be set once" );
			presentMode = ePresentMode.NV12;
		}

		ITextureView iMediaEngine.createFrameTexture( IRenderDevice device, TextureFormat format )
		{
			if( presentMode != ePresentMode.None )
				throw new ApplicationException( "Video present mode can only be set once" );
			presentMode = ePresentMode.RGB;
			return createOutputTexture( device, format, decodedSize.cropRect.size );
		}

		void IDisposable.Dispose()
		{
			if( shutdownHandle )
				shutdownHandle.set();

			decoder?.Dispose();
			decoder = null;
			audioPlayer?.Dispose();
			audioPlayer = null;
			file?.Dispose();
			file = null;
			h264?.Dispose();

			shutdownHandle.finalize();
		}

		int iMediaEngine.getError()
		{
			throw new NotImplementedException();
		}

		EventHandle shutdownHandle;
		protected sDecodedVideoSize decodedSize { get; private set; }
		iAudioPlayer audioPlayer;

		/// <summary>Load an mp4 media file; this runs on a background thread from the thread pool.</summary>
		void loadMediaImpl( string url, TaskCompletionSource<bool> completionSource )
		{
			try
			{
				// Deal with paths starting from "~/", transform that into user's home folder
				if( url.StartsWith( "~/" ) )
				{
					string rel = url.Substring( 2 );
					string home = Environment.GetFolderPath( Environment.SpecialFolder.UserProfile );
					url = Path.Combine( home, rel );
				}

				// Parse the complete MP4, except the largest `mdat` box which has the actual payload of the video.
				file = OpenMedia.openMedia( File.OpenRead( url ) );

				if( !shutdownHandle )
					shutdownHandle = EventHandle.create();

				// Initialize the video
				if( null == file.videoTrack )
					throw new ApplicationException( "The file doesn’t have a video track" );
				decoder?.Dispose();
				decoder = new StatefulVideoDecoder( h264.device, shutdownHandle );

				int videoEncodedBuffers = encodedBuffersCount;
				int audioEncodedBuffers = this.audioEncodedBuffers;

				decoder.initialize( file.videoTrack, videoEncodedBuffers );
				decodedSize = file.videoTrack.decodedSize;
				decoder.captureSetup( file.videoTrack, decodedBuffersCount, decodedSize );

				// Initialize the audio
				audioPlayer?.Dispose();
				audioPlayer = null;
				if( null != file.audioTrack )
				{
					try
					{
						audioPlayer = Audio.Player.create( file.audioTrack, audioEncodedBuffers, shutdownHandle );
					}
					catch( Exception ex )
					{
						// Logger.logError( ex.ToString() );
						ex.logError( "Error initializing audio" );
					}
				}
				else
					Logger.logWarning( "The file doesn’t have an audio track" );

				// Initialize presentation clock source
				if( null != audioPlayer )
				{
					// Use audio player for the source of presentation time
					if( displayRefresh.HasValue )
						presentationClock = new Clocks.AudioWithTimer( decoder, audioPlayer, displayRefresh.Value );
					else
						presentationClock = new Clocks.Audio( decoder, audioPlayer );
				}
				else
				{
					// Use eClock.Monotonic OS clock for presentation time
					presentationClock = new Clocks.Video( decoder );
				}
				m_state = eState.Prepared;
				if( m_autoPlay )
					play();

				completionSource.SetResult( true );
			}
			catch( Exception ex )
			{
				completionSource.SetException( ex );
			}
		}

		Task iLinuxMediaEngine.loadMedia( string url )
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			WaitCallback wcb = ( ttt ) => loadMediaImpl( url, (TaskCompletionSource<bool>)ttt );
			ThreadPool.QueueUserWorkItem( wcb, tcs );
			return tcs.Task;
		}

		void iMediaEngine.loadMedia( string url, iCompletionSource completionSource ) =>
			throw new NotSupportedException( "Use iLinuxMediaEngine.loadMedia instead" );

		bool iMediaEngine.onVideoStreamTick( out ulong presentationTime )
		{
			if( m_state == eState.Paused )
			{
				presentationTime = default;
				return false;
			}

			releasePlayedFrame();

			bool result = presentationClock.onVideoStreamTick( out presentationTime );
			if( !result )
				return false;

			if( decoder.needsTextures )
				initializeRendering();

			return true;
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		void initializeRendering()
		{
			using( var dev = getRenderDevice() )
			{
				Nv12Texture[] textures = decoder.exportTextures( dev );
				if( presentMode == ePresentMode.RGB )
				{
					var size = decoder.decodedSize;
					initRendering( dev, textures, ref size );
				}
			}
		}

		/// <summary>This method is no longer used. Was too slow on Pi4. Implemented zero-copy workflow instead, with dequeueTexture method</summary>
		void iMediaEngine.transferVideoFrame()
		{
			// Dequeue the next frame
			int frame = decoder.dequeDecoded();
			try
			{
				// Render NV12 into RGB. This also does cropping and color space conversions.
				renderFrame( frame );
			}
			finally
			{
				// Very important step: release the buffer back to the queue.
				decoder.enqueueDecodedBuffer( frame );
			}
		}

		int? renderingTexture = null;

		/// <summary>Keeping NV12 data in the decoded texture until the next frame seems to fix rare tearing-like artifacts, especially visible in the lower left corner of the video</summary>
		void releasePlayedFrame()
		{
			if( !renderingTexture.HasValue )
				return;
			decoder.enqueueDecodedBuffer( renderingTexture.Value );
			renderingTexture = null;
		}

		protected ITextureView dequeueTexture()
		{
			if( m_state == eState.Paused )
				return getLastFrameTexture();

			releasePlayedFrame();
			int frame = decoder.dequeDecoded();
			renderingTexture = frame;
			return decoder.textureView( frame );
		}

		protected ITextureView getLastFrameTexture()
		{
			if( null == renderingTexture )
			{
				Logger.logWarning( "No last frame texture" );
				return null;
			}
			return decoder.textureView( renderingTexture.Value );
		}

		protected void releaseTexture()
		{
			// Do nothing.
			// NV12 texture with the current video frame will be released back to V4L2 decoded queue on the next onVideoStreamTick() or dequeueTexture() call, whichever comes first.
		}

		#region Play and pause
		enum eState: byte
		{
			NotInitialized = 0,
			Prepared,
			Playing,
			Paused,
			Seeking,
		}
		eState m_state = eState.NotInitialized;

		Exception unsupportedTransition() => new ApplicationException( $"Unsupported state transition from { m_state } state" );

		public void play()
		{
			switch( m_state )
			{
				case eState.Prepared:
					audioPlayer.play();
					decoder.startDecoder( file, audioPlayer.decoderQueues );
					decoder.thread.setPresentationClock( presentationClock );
					m_state = eState.Playing;
					return;
				case eState.Playing:
					Logger.logWarning( "The media engine is already playing a video" );
					return;
				case eState.Paused:
					presentationClock.resume();
					m_state = eState.Playing;
					return;
			}
			throw unsupportedTransition();
		}

		void iMediaEngine.pause()
		{
			switch( m_state )
			{
				case eState.Playing:
					presentationClock.pause();
					m_state = eState.Paused;
					return;
				case eState.Paused:
					Logger.logWarning( "The media engine is already paused" );
					return;
			}
			throw unsupportedTransition();
		}

		bool m_autoPlay = false;
		bool iMediaEngine.getAutoPlay() => m_autoPlay;
		void iMediaEngine.setAutoPlay( bool AutoPlay ) => m_autoPlay = AutoPlay;
		bool iMediaEngine.autoPlay
		{
			get => m_autoPlay;
			set => m_autoPlay = value;
		}
		#endregion

		#region Duration and seek
		void iMediaEngine.getDuration( out TimeSpan duration ) => duration = file.duration;
		TimeSpan iMediaEngine.duration => file.duration;

		void iMediaEngine.getCurrentTime( out TimeSpan time ) => time = presentationClock.getCurrentTime();
		void iMediaEngine.setCurrentTime( TimeSpan time ) => presentationClock.seek( time );
		TimeSpan iMediaEngine.currentTime
		{
			get => presentationClock.getCurrentTime();
			set => presentationClock.seek( value );
		}
		#endregion

		#region Miscellaneous, audio volume controls
		bool iMediaEngine.muted
		{
			get => getMuted();
			set => setMuted( value );
		}

		byte prevVolume = 0xFF;
		byte currentVolume = 0xFF;

		public bool getMuted() => currentVolume == 0;

		public void setMuted( bool muted )
		{
			if( muted )
			{
				byte cv = currentVolume;
				if( cv > 0 )
					prevVolume = cv;
				currentVolume = 0;
			}
			else
				currentVolume = prevVolume;

			audioPlayer?.setVolume( currentVolume );
		}

		public void getVolume( out float volume )
		{
			volume = (float)currentVolume * ( 1.0f / 255.0f );
		}
		public void setVolume( float volume )
		{
			if( volume < 0 || volume > 1 )
				throw new ArgumentOutOfRangeException();
			// https://docs.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-data-conversion
			volume *= 255.0f;
			int i = (int)MathF.Round( volume );
			prevVolume = currentVolume = (byte)i;
			audioPlayer?.setVolume( currentVolume );
		}
		float iMediaEngine.volume
		{
			get { getVolume( out float f ); return f; }
			set => setVolume( value );
		}
		#endregion

		// Implemented by the derived class; it needs shader compiler and other GPU interop infrastructure missing from either VrmacVideo.dll or VrmacInterop.dll assemblies
		public abstract iVideoRenderState createRenderer( IRenderDevice device, CSize renderTargetSize, SwapChainFormats formats, Vector4 borderColor );

		volatile iMediaEngineEvents m_events;

		void iMediaEngine.setEventsSink( iMediaEngineEvents sink )
		{
			Interlocked.Exchange( ref m_events, sink );
		}
	}
}