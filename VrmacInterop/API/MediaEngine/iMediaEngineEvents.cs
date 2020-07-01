using ComLight;

namespace Vrmac.MediaEngine
{
	/// <summary>Defines error status codes for the Media Engine.</summary>
	public enum eMediaEngineError: byte
	{
		/// <summary>The process of fetching the media resource was stopped at the user's request</summary>
		Aborted = 1,
		/// <summary>A network error occurred while fetching the media resource</summary>
		Network = 2,
		/// <summary>An error occurred while decoding the media resource</summary>
		Decode = 3,
		/// <summary>The media resource is not supported</summary>
		SourceNotSupported = 4,
		/// <summary>An error occurred while encrypting the media resource</summary>
		Encrypted = 5
	}

	/// <summary>Implement this interface to receive events from media engine</summary>
	/// <remarks>These methods are called on some media engine internal background threads, you might need some threads syncronization in these handlers.</remarks>
	[ComInterface( "a285bc5f-cb49-4637-b909-b51224efbd89", eMarshalDirection.ToNative )]
	public interface iMediaEngineEvents
	{
		/// <summary>An error occurred</summary>
		/// <param name="error">A member of the <see cref="eMediaEngineError" /> enumeration.</param>
		/// <param name="hresult">An HRESULT error code, or zero.</param>
		void error( eMediaEngineError error, int hresult );

		/// <summary>Something else happened</summary>
		void mediaEngineEvent( eMediaEngineEvent mediaEvent );

		/// <summary>The output format of the media source has changed</summary>
		void formatChanged( eMediaStreams stream );
	}
}