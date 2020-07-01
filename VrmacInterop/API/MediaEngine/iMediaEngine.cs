using ComLight;
using Diligent.Graphics;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Vrmac.MediaEngine
{
	/// <summary>Enables an application to play audio or video files</summary>
	/// <remarks>
	/// <para>This COM interface is a thin wrapper around the corresponding OS component. My implementation takes a few pages of code, and only 24kb in native binary size.
	/// The real implementation is in MFMediaEngine.dll COM library, 4.8MB of binary code written and supported by Microsoft.</para>
	/// <para>Sadly, that native component fails to support D3D12. Unlike Direct2D, it uses internally created threads to mess with GPU resources,
	/// which renders it incompatible with that <see href="https://docs.microsoft.com/en-us/windows/win32/direct3d12/direct3d-11-on-12">11-on-12</see> layer.
	/// For this reason, the C++ implementation creates a D3D11 device on the same GPU,
	/// shares video frame texture through <see href="https://docs.microsoft.com/en-us/windows/win32/direct3darticles/surface-sharing-between-windows-graphics-apis">DXGI</see>, Diligent Engine takes it from there.</para>
	/// </remarks>
	/// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/mfmediaengine/nn-mfmediaengine-imfmediaengine" />
	[ComInterface( "97ab1528-8833-475b-8455-a3f0cc4962a5", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iMediaEngine: IDisposable
	{
		/// <summary>Queries how likely it is that the Media Engine can play a specified type of media resource.</summary>
		[RetValIndex] eCanPlay canPlayType( [NativeString] string type );

		/// <summary>Queries whether the Media Engine automatically begins playback</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		bool getAutoPlay();
		/// <summary>Specifies whether the Media Engine automatically begins playback</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		void setAutoPlay( [MarshalAs( UnmanagedType.U1 )] bool AutoPlay );
		/// <summary>Whether the Media Engine automatically begins playback</summary>
		bool autoPlay { get; set; }

		/// <summary>Queries whether the audio is muted</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		bool getMuted();
		/// <summary>Mutes or unmutes the audio</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		void setMuted( [MarshalAs( UnmanagedType.U1 )] bool muted );
		/// <summary>Mutes or unmutes the audio</summary>
		bool muted { get; set; }

		/// <summary>Queries volume of the audio, 1.0 = 100%</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		void getVolume( out float volume );
		/// <summary>Set volume of the audio</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		void setVolume( float volume );
		/// <summary>Volume of the audio</summary>
		float volume { get; set; }

		/// <summary>Sets the URL of a media resource, and load the media</summary>
		/// <remarks>Not supported on Linux, call <see cref="iLinuxMediaEngine.loadMedia(string)" /> instead.</remarks>
		[EditorBrowsable( EditorBrowsableState.Never )]
		void loadMedia( [NativeString] string url, iCompletionSource completionSource );

		/// <summary>Returns the last error status, if any, that resulted from loading the media source</summary>
		int getError();

		/// <summary>Create video frame texture of the specified format, and return it’s shader resource view.</summary>
		/// <remarks>These video textures are a bit special ‘coz Microsoft neglected to integrate their IMFMediaEngine component with D3D12.
		/// The frame textures are created as D3D11 textures, then shared into D3D12. With other direction of sharing, media engine fails to deliver these precious video frames.</remarks>
		[RetValIndex]
		ITextureView createFrameTexture( IRenderDevice device, TextureFormat format );

		/// <summary>Queries for the streams in the current media resource</summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		void getMediaStreams( out eMediaStreams mediaStreams );
		/// <summary>The streams in the current media resource</summary>
		eMediaStreams mediaStreams { get; }

		/// <summary>Gets the size of the video frame, adjusted for aspect ratio.</summary>
		[RetValIndex, EditorBrowsable( EditorBrowsableState.Never )] CSize getNativeVideoSize();
		/// <summary>Size of the video frame, adjusted for aspect ratio</summary>
		CSize nativeVideoSize { get; }

		/// <summary>If a new frame is ready, returns true and received receives the presentation time of the frame. Otherwise returns false.</summary>
		bool onVideoStreamTick( out ulong presentationTime );

		/// <summary>Copies the current video frame into the frame texture.</summary>
		/// <remarks>You must call <see cref="createFrameTexture" /> first, this method gonna fail otherwise.</remarks>
		void transferVideoFrame();

		/// <summary>Starts playback</summary>
		void play();
		/// <summary>Pauses playback</summary>
		void pause();

		/// <summary>Set event sink to get notified what’s going on</summary>
		/// <remarks>The media engine component is asynchronous by nature.
		/// On windows, the events are called on some media foundation background thread.
		/// On Linux they are not implemented yet, but eventually they will also be called on internal VrmacVideo thread.</remarks>
		void setEventsSink( iMediaEngineEvents sink );

		/// <summary>Gets the duration of the media resource</summary>
		void getDuration( out TimeSpan duration );
		/// <summary>duration of the media resource</summary>
		TimeSpan duration { get; }

		/// <summary>Gets the current playback position</summary>
		void getCurrentTime( out TimeSpan time );
		/// <summary>Seeks to a new playback position</summary>
		void setCurrentTime( TimeSpan time );
		/// <summary>Current playback position</summary>
		TimeSpan currentTime { get; set; }
	}
}