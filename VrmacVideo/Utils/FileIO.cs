using System;
using System.IO;

namespace VrmacVideo
{
	static class FileIO
	{
		/// <summary>Stream.Read with proper error handling</summary>
		public static void read( this Stream stm, Span<byte> span )
		{
			while( true )
			{
				int cb = stm.Read( span );
				if( cb <= 0 )
					throw new EndOfStreamException();
				if( cb > span.Length )
					throw new ApplicationException( "Stream.Read returned unexpected length" );
				if( cb == span.Length )
					return;
				Logger.logWarning( "Incomplete file read: asked {0} bytes, got {1}", span.Length, cb );
				span = span.Slice( cb );
			}
		}
	}
}