using Vrmac.Input.KeyboardLayout;

namespace Vrmac.Input.Linux
{
	class RawKeyboard: RawDeviceBase
	{
		readonly iKeyboardLayout layout;
		readonly iKeyboardHandler handler;
		public readonly string deviceName;

		internal RawKeyboard( RawDevice device, iKeyboardLayout layout, iKeyboardHandler handler )
		{
			this.layout = layout;
			this.handler = handler;
			deviceName = device.name;

			if( deviceName.isEmpty() )
				deviceName = device.productDescription;
			if( deviceName.isEmpty() )
			{
				string mfg = device.manufacturer;
				if( mfg.notEmpty() )
					deviceName = $"A keyboard made by { mfg }";
				else
					deviceName = $"Unidentified { device.bus } keyboard";
			}
		}

		protected override void handleLed( eLed led, int value )
		{
			layout.updateState( led, value );
		}

		protected override void handleKey( eKey key, eKeyValue keyValue )
		{
			layout.updateState( key, keyValue );
			char c = layout.keyChar( key );
			handler.keyEvent( key, keyValue, c, layout.state );
		}
	}
}