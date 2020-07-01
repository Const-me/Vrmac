using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VrmacVideo.IO.AAC
{
	enum eAacStatus: int
	{
		Success = 0,
		OutOfMemory = 2,
	}

	static class ErrorCodes
	{
		static readonly Dictionary<int, string> dict = new Dictionary<int, string>( 33 )
		{
			{ 0, "No error occurred. Output buffer is valid and error free." },
			{ (int)eAacStatus.OutOfMemory, "Heap returned NULL pointer. Output buffer is invalid." },
			{ 0x0005, "Error condition is of unknown reason, or from a another module. Output buffer is invalid." },

			// Synchronization errors. Output buffer is invalid.
			{ 0x1001, "The transport decoder had synchronisation problems. Do not exit decoding. Just feed new bitstream data." },
			{ 0x1002, "The input buffer ran out of bits." },

			// Initialization errors. Output buffer is invalid.
			{ 0x2001, "The handle passed to the function call was invalid (null)." },
			{ 0x2002, "The AOT found in the configuration is not supported." },
			{ 0x2003, "The bitstream format is not supported." },
			{ 0x2004, "The error resilience tool format is not supported." },
			{ 0x2005, "The error protection format is not supported." },
			{ 0x2006, "More than one layer for AAC scalable is not supported." },
			{ 0x2007, "The channel configuration (either number or arrangement) is not supported." },
			{ 0x2008, "The sample rate specified in the configuration is not supported." },
			{ 0x2009, "The SBR configuration is not supported." },
			{ 0x200A, "The parameter could not be set. Either the value was out of range or the parameter does not exist." },
			{ 0x200B, "The decoder needs to be restarted, since the required configuration change cannot be performed." },
			{ 0x200C, "The provided output buffer is too small." },

			// Decode errors. Output buffer is valid but concealed.
			{ 0x4001, "The transport decoder encountered an unexpected error." },
			{ 0x4002, "Error while parsing the bitstream. Most probably it is corrupted, or the system crashed." },
			{ 0x4003, "Error while parsing the extension payload of the bitstream. The extension payload type found is not supported." },
			{ 0x4004, "The parsed bitstream value is out of range. Most probably the bitstream is corrupt, or has a wrong format." },
			{ 0x4005, "The embedded CRC did not match." },
			{ 0x4006, "An invalid codebook was signaled. Most probably the bitstream is corrupt, or the system crashed." },
			{ 0x4007, "Predictor found, but not supported in the AAC Low Complexity profile. Most probably the bitstream is corrupt, or has a wrong format." },
			{ 0x4008, "A CCE element was found which is not supported. Most probably the bitstream is corrupt, or has a wrong format." },
			{ 0x4009, "A LFE element was found which is not supported. Most probably the bitstream is corrupt, or has a wrong format." },
			{ 0x400A, "Gain control data found but not supported. Most probably the bitstream is corrupt, or has a wrong format." },
			{ 0x400B, "SBA found, but currently not supported in the BSAC profile." },
			{ 0x400C, "Error while reading TNS data. Most probably the bitstream is corrupt or the system crashed." },
			{ 0x400D, "Error while decoding error resilient data." },

			// Ancillary data errors. Output buffer is valid.
			{ 0x8001, "Non severe error concerning the ancillary data handling." },
			{ 0x8002, "The registered ancillary data buffer is too small to receive the parsed data." },
			{ 0x8003, "More than the allowed number of ancillary data elements should be written to buffer." },
		};

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void check( this eAacStatus code, string what )
		{
			if( code == eAacStatus.Success )
				return;
			throwOrLog( (int)code, what );
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		static void throwOrLog( int code, string what )
		{
			string message;
			if( !dict.TryGetValue( code, out message ) )
				message = $"Undocumented AAC decoder error code { code }";

			if( ( code & 0xF000 ) == 0x8000 )
			{
				// Since it's ancillary and doesn't screw up the output buffers, don't fail with exception, log a warning instead.
				Logger.logWarning( "{0}: {1}", what, message );
				return;
			}

			if( code == (int)eAacStatus.OutOfMemory )
				throw new OutOfMemoryException( message );
			throw new ApplicationException( message );
		}
	}
}