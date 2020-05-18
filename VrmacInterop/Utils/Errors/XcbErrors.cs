static class XcbErrors
{
	static readonly string[] errorCodes = new string[]
	{
		null, // 0
		"xcb connection errors because of socket, pipe and other stream errors", // XCB_CONN_ERROR 1
		"xcb connection shutdown because of extension not supported", // XCB_CONN_CLOSED_EXT_NOTSUPPORTED 2
		"malloc(), calloc() and realloc() error upon failure, for eg ENOMEM", // XCB_CONN_CLOSED_MEM_INSUFFICIENT 3
		"Connection closed, exceeding request length that server accepts", // XCB_CONN_CLOSED_REQ_LEN_EXCEED 4
		"Connection closed, error during parsing display string", // XCB_CONN_CLOSED_PARSE_ERR 5
		"Connection closed because the server does not have a screen matching the display", // XCB_CONN_CLOSED_INVALID_SCREEN 6
		"Connection closed because some FD passing operation failed", // XCB_CONN_CLOSED_FDPASSING_FAILED 7
	};

	public static string tryLookup( int code )
	{
		if( code < 0 || code >= errorCodes.Length )
			return null;
		return errorCodes[ code ];
	}
}