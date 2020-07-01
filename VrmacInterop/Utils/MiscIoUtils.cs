using System;
using System.IO;

namespace Vrmac
{
	/// <summary>Utility functions related to I/O</summary>
	public static class MiscIoUtils
	{
		const int skipBufferLength = 1024 * 4;

		/// <summary>Seek the stream forward from current position. If the stream doesn’t support seek like it happens with GZipStream, will read and discard bytes.</summary>
		public static void skip( this Stream stm, int cbSkip )
		{
			if( 0 == cbSkip )
				return;

			if( stm.CanSeek )
			{
				stm.Seek( cbSkip, SeekOrigin.Current );
				return;
			}

			if( cbSkip < 0 )
				throw new ArgumentException( "The stream is forward only, can’t rewind it." );

			if( cbSkip <= skipBufferLength )
			{
				Span<byte> buffer = stackalloc byte[ cbSkip ];
				if( cbSkip != stm.Read( buffer.Slice( 0, cbSkip ) ) )
					throw new EndOfStreamException();
			}
			else
			{
				Span<byte> buffer = stackalloc byte[ skipBufferLength ];

				while( cbSkip >= skipBufferLength )
				{
					if( skipBufferLength != stm.Read( buffer ) )
						throw new EndOfStreamException();
					if( cbSkip == skipBufferLength )
						return;
					cbSkip -= skipBufferLength;
				}

				if( cbSkip != stm.Read( buffer.Slice( 0, cbSkip ) ) )
					throw new EndOfStreamException();
			}
		}

		/// <summary>Format a string like "1 byte" or "11 bytes"</summary>
		public static string pluralString( this int i, string what )
		{
			if( 1 != i )
				return $"{ i } { what }s";
			return $"1 { what }";
		}
	}
}