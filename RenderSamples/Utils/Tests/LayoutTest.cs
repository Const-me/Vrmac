using Vrmac.Input;
using Vrmac.Input.KeyboardLayout;
using Vrmac.Input.Linux;
using System;

namespace RenderSamples.Utils.Tests
{
	class LayoutTest: RawDeviceBase
	{
		string time
		{
			get
			{
				DateTime local = messageTime.ToLocalTime();
				return local.ToString( "hh:mm:ss.fffffff" );
			}
		}

		readonly iKeyboardLayout layout;

		public LayoutTest()
		{
			// layout = new UsEnglishLayout();
			layout = new MontenegrinLayout();
		}

		protected override void handleLed( eLed led, int value )
		{
			layout.updateState( led, value );
		}

		static string charString( char c )
		{
			if( c == '\n' ) return @"'\n'";
			if( c == '\t' ) return @"'\t'";
			return $"'{ c }'";
		}

		protected override void handleKey( eKey key, eKeyValue keyValue )
		{
			layout.updateState( key, keyValue );

			char c = layout.keyChar( key );
			if( 0 != c )
				Console.WriteLine( "KeyChar: {0} {1} {2} {3}", charString( c ), key, keyValue, time );
			else
				Console.WriteLine( "Key: {0} {1} {2}", key, keyValue, time );
		}
	}
}