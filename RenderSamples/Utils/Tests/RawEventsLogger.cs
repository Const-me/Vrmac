using Vrmac.Input;
using Vrmac.Input.Linux;
using System;

namespace RenderSamples.Utils.Tests
{
	class RawEventsLogger: RawDeviceBase
	{
		string time
		{
			get
			{
				DateTime local = messageTime.ToLocalTime();
				return local.ToString( "hh:mm:ss.fffffff" );
			}
		}

		protected override void handleSyncro( eSynchroEvent synchroEvent )
		{
			Console.WriteLine( "Syncro: {0} {1}", synchroEvent, time );
		}
		protected override void handleKey( eKey key, eKeyValue keyValue )
		{
			Console.WriteLine( "Key: {0} {1} {2}", key, keyValue, time );
		}
		protected override void handleButton( eButton button, eKeyValue keyValue )
		{
			Console.WriteLine( "Button: {0} {1} {2}", button, keyValue, time );
		}
		protected override void handleRelative( eRelativeAxis axis, int value )
		{
			Console.WriteLine( "Relative: {0} {1} {2}", axis, value, time );
		}
		protected override void handleAbsolute( eAbsoluteAxis axis, int value )
		{
			Console.WriteLine( "Absolute: {0} {1} {2}", axis, value, time );
		}
		protected override void handleMiscellaneous( eMiscEvent miscEvent, int value )
		{
			Console.WriteLine( "Miscellaneous: {0} {1} {2}", miscEvent, value, time );
		}
		protected override void handleSwitch( eSwitch switchEvent, int value )
		{
			Console.WriteLine( "Switch: {0} {1} {2}", switchEvent, value, time );
		}
		protected override void handleLed( eLed led, int value )
		{
			Console.WriteLine( "LED: {0} {1} {2}", led, value, time );
		}
		protected override void updated()
		{
			Console.WriteLine();
		}
	}
}