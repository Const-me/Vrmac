namespace Vrmac.MediaEngine
{
	/// <summary>Defines event codes for the Media Engine</summary>
	public enum eMediaEngineEvent: byte
	{
		/// <summary>The Media Engine has started to load the source</summary>
		LoadStart = 0,
		/// <summary>The Media Engine is loading the source</summary>
		Progress,
		/// <summary>The Media Engine has suspended a load operation</summary>
		Suspend,
		/// <summary>The Media Engine cancelled a load operation that was in progress</summary>
		Abort,
		/// <summary>The Media Engine has switched to the MF_MEDIA_ENGINE_NETWORK_EMPTY state. This can occur when the IMFMediaEngine::Load method is called, or if an error occurs during the Load method.</summary>
		Emptied,
		/// <summary>The Load algorithm is stalled, waiting for data</summary>
		Stalled,
		/// <summary>The Media Engine is switching to the playing state</summary>
		Play,
		/// <summary>The media engine has paused</summary>
		Pause,
		/// <summary>The Media Engine has loaded enough source data to determine the duration and dimensions of the source</summary>
		LoadedMetadata,
		/// <summary>The Media Engine has loaded enough data to render some content (for example, a video frame)</summary>
		LoadedData,
		/// <summary>Playback has stopped because the next frame is not available</summary>
		Waiting,
		/// <summary>Playback has started</summary>
		Playing,
		/// <summary>Playback can start, but the Media Engine might need to stop to buffer more data</summary>
		Canplay,
		/// <summary>The Media Engine can probably play through to the end of the resource, without stopping to buffer data</summary>
		CanPlayThrough,
		/// <summary>The Media Engine has started seeking to a new playback position</summary>
		Seeking,
		/// <summary>The Media Engine has seeked to a new playback position</summary>
		Seeked,
		/// <summary>The playback position has changed</summary>
		TimeUpdate,
		/// <summary>Playback has reached the end of the source</summary>
		Ended,
		/// <summary>The playback rate has changed</summary>
		RateChange,
		/// <summary>The duration of the media source has changed</summary>
		DurationChange,
		/// <summary>The audio volume changed</summary>
		VolumeChange,
		/// <summary>This never happens, delivered as <see cref="iMediaEngineEvents.formatChanged(eMediaStreams)" /> call.</summary>
		FormatChange,
		/// <summary>The Media Engine flushed any pending events from its queue</summary>
		PurgeQueuedEvents,
		/// <summary>The playback position reached a timeline marker</summary>
		TimelineMarker,
		/// <summary>The audio balance changed</summary>
		BalanceChange,
		/// <summary>The Media Engine has finished downloading the source data</summary>
		DownloadComplete,
		/// <summary>The media source has started to buffer data</summary>
		BufferingStarted,
		/// <summary>The media source has stopped buffering data</summary>
		BufferingEnded,
		/// <summary>The IMFMediaEngineEx::FrameStep method completed</summary>
		FrameStepCompleted,
		/// <summary>The Media Engine's Load algorithm is waiting to start</summary>
		NotifyStableState,
		/// <summary>The first frame of the media source is ready to render</summary>
		FirstFrameReady,
		/// <summary>Raised when a new track is added or removed</summary>
		TracksChange,
		/// <summary>Raised when there is new information about the Output Protection Manager (OPM)</summary>
		OpmInfo,
		/// <summary>MF_MEDIA_ENGINE_EVENT_RESOURCELOST, undocumented</summary>
		ResourceLost,
		/// <summary>MF_MEDIA_ENGINE_EVENT_DELAYLOADEVENT_CHANGED, undocumented</summary>
		DelayLoadEventChanged,
		/// <summary>Raised when one of the component streams of a media stream fails. This event is only raised if the media stream contains other component streams that did not fail.</summary>
		StreamRenderingError,
		/// <summary>MF_MEDIA_ENGINE_EVENT_SUPPORTEDRATES_CHANGED, undocumented</summary>
		SupportedRatesChanged,
		/// <summary>MF_MEDIA_ENGINE_EVENT_AUDIOENDPOINTCHANGE, undocumented</summary>
		AudioEndpointChange,
	}
}