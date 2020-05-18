static class LinuxErrors
{
	// Obviously, arrays are much faster than hash maps when the keys are densely packed.
	// Fortunately, Linux developers do just that with their error codes.

	// BTW, the source data came from /usr/include/asm-generic/errno-base.h + /usr/include/asm-generic/errno.h headers, I've wrote a small utility to parse C comments.
	static readonly string[] errorCodes = new string[ 134 ]
	{
		null, // 0
		"Operation not permitted", // 1
		"No such file or directory", // 2
		"No such process", // 3
		"Interrupted system call", // 4
		"I/O error", // 5
		"No such device or address", // 6
		"Argument list too long", // 7
		"Exec format error", // 8
		"Bad file number", // 9
		"No child processes", // 10
		"Operation would block", // 11
		"Out of memory", // 12
		"Permission denied", // 13
		"Bad address", // 14
		"Block device required", // 15
		"Device or resource busy", // 16
		"File exists", // 17
		"Cross-device link", // 18
		"No such device", // 19
		"Not a directory", // 20
		"Is a directory", // 21
		"Invalid argument", // 22
		"File table overflow", // 23
		"Too many open files", // 24
		"Not a typewriter", // 25
		"Text file busy", // 26
		"File too large", // 27
		"No space left on device", // 28
		"Illegal seek", // 29
		"Read-only file system", // 30
		"Too many links", // 31
		"Broken pipe", // 32
		"Math argument out of domain of func", // 33
		"Math result not representable", // 34
		"Resource deadlock would occur", // 35
		"File name too long", // 36
		"No record locks available", // 37
		"Invalid system call number", // 38
		"Directory not empty", // 39
		"Too many symbolic links encountered", // 40
		null, // 41
		"No message of desired type", // 42
		"Identifier removed", // 43
		"Channel number out of range", // 44
		"Level 2 not synchronized", // 45
		"Level 3 halted", // 46
		"Level 3 reset", // 47
		"Link number out of range", // 48
		"Protocol driver not attached", // 49
		"No CSI structure available", // 50
		"Level 2 halted", // 51
		"Invalid exchange", // 52
		"Invalid request descriptor", // 53
		"Exchange full", // 54
		"No anode", // 55
		"Invalid request code", // 56
		"Invalid slot", // 57
		null, // 58
		"Bad font file format", // 59
		"Device not a stream", // 60
		"No data available", // 61
		"Timer expired", // 62
		"Out of streams resources", // 63
		"Machine is not on the network", // 64
		"Package not installed", // 65
		"Object is remote", // 66
		"Link has been severed", // 67
		"Advertise error", // 68
		"Srmount error", // 69
		"Communication error on send", // 70
		"Protocol error", // 71
		"Multihop attempted", // 72
		"RFS specific error", // 73
		"Not a data message", // 74
		"Value too large for defined data type", // 75
		"Name not unique on network", // 76
		"File descriptor in bad state", // 77
		"Remote address changed", // 78
		"Can not access a needed shared library", // 79
		"Accessing a corrupted shared library", // 80
		".lib section in a.out corrupted", // 81
		"Attempting to link in too many shared libraries", // 82
		"Cannot exec a shared library directly", // 83
		"Illegal byte sequence", // 84
		"Interrupted system call should be restarted", // 85
		"Streams pipe error", // 86
		"Too many users", // 87
		"Socket operation on non-socket", // 88
		"Destination address required", // 89
		"Message too long", // 90
		"Protocol wrong type for socket", // 91
		"Protocol not available", // 92
		"Protocol not supported", // 93
		"Socket type not supported", // 94
		"Operation not supported on transport endpoint", // 95
		"Protocol family not supported", // 96
		"Address family not supported by protocol", // 97
		"Address already in use", // 98
		"Cannot assign requested address", // 99
		"Network is down", // 100
		"Network is unreachable", // 101
		"Network dropped connection because of reset", // 102
		"Software caused connection abort", // 103
		"Connection reset by peer", // 104
		"No buffer space available", // 105
		"Transport endpoint is already connected", // 106
		"Transport endpoint is not connected", // 107
		"Cannot send after transport endpoint shutdown", // 108
		"Too many references: cannot splice", // 109
		"Connection timed out", // 110
		"Connection refused", // 111
		"Host is down", // 112
		"No route to host", // 113
		"Operation already in progress", // 114
		"Operation now in progress", // 115
		"Stale file handle", // 116
		"Structure needs cleaning", // 117
		"Not a XENIX named type file", // 118
		"No XENIX semaphores available", // 119
		"Is a named type file", // 120
		"Remote I/O error", // 121
		"Quota exceeded", // 122
		"No medium found", // 123
		"Wrong medium type", // 124
		"Operation Canceled", // 125
		"Required key not available", // 126
		"Key has expired", // 127
		"Key has been revoked", // 128
		"Key was rejected by service", // 129
		"Owner died", // 130
		"State not recoverable", // 131
		"Operation not possible due to RF-kill", // 132
		"Memory page has hardware error", // 133
	};

	public static string tryLookup( int code )
	{
		if( code < 0 || code >= errorCodes.Length )
			return null;
		return errorCodes[ code ];
	}
}