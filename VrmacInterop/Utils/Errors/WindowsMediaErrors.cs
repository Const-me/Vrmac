static class WindowsMediaErrors
{
	static readonly string[] errorCodes = new string[]
	{
		null, // 0
		"The process of fetching the media resource was stopped at the user's request", // MF_MEDIA_ENGINE_ERR_ABORTED 1
		"A network error occurred while fetching the media resource", // MF_MEDIA_ENGINE_ERR_NETWORK 2
		"An error occurred while decoding the media resource", // MF_MEDIA_ENGINE_ERR_DECODE 3
		"The media resource is not supported", // MF_MEDIA_ENGINE_ERR_SRC_NOT_SUPPORTED 4
		"An error occurred while encrypting the media resource", // MF_MEDIA_ENGINE_ERR_ENCRYPTED 5
	};

	public static string tryLookup( int code )
	{
		if( code < 0 || code >= errorCodes.Length )
			return null;
		return errorCodes[ code ];
	}
}