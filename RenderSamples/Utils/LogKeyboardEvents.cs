using Vrmac.Input;
using System;

namespace RenderSamples.Utils
{
	class LogKeyboardEvents: iKeyboardHandler
	{
		static string printKeyChar( char c )
		{
			switch( c )
			{
				case '\t': return "\\t";
				case '\r': return "\\r";
				case '\n': return "\\n";
				case '\b': return "\\b";
				case '\u00A0': return "\\uA0";  // non-breaking space
				case '\u00AD': return "\\uAD";  // soft hyphen, it's zero-width even in monospace console
			}
			return c.ToString();
		}

		static string printKey( char c )
		{
			string kc = printKeyChar( c );
			string hex = ( (int)c ).ToString( "x" );
			return $"'{ kc }' 0x{ hex }";
		}

		void iKeyboardHandler.keyEvent( eKey key, eKeyValue what, ushort unicodeChar, eKeyboardState ks )
		{
			if( 0 == unicodeChar )
				Console.WriteLine( "keyEvent: {0} {1} {2}", key, what, ks );
			else if( key == 0 )
				Console.WriteLine( "keyEvent: {0} {1} {2}", printKey( (char)unicodeChar ), what, ks );
			else
				Console.WriteLine( "keyEvent: {0} ({1}) {2} {3}", printKey( (char)unicodeChar ), key, what, ks );
		}
	}
}