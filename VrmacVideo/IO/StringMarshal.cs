using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO
{
	static class StringMarshal
	{
		/// <summary>Equivalent of strncpy: copy UTF8 bytes up to the buffer length, but stop at '\0' </summary>
		public static unsafe string copy( byte* pointer, int length )
		{
			if( null == pointer )
				return null;
			if( length < 0 )
				throw new ArgumentOutOfRangeException();

			for( int i = 0; i < length; i++ )
			{
				if( 0 == pointer[ i ] )
				{
					length = i;
					break;
				}
			}
			return Marshal.PtrToStringUTF8( (IntPtr)pointer, length );
		}
	}
}