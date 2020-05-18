using ComLight;
using Vrmac.Input.KeyboardLayout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vrmac.Input.Linux
{
	/// <summary>Implements extension methods of Dispatcher to open raw devices</summary>
	public static class RawInput
	{
		/// <summary>True if the device is likely to be a mouse</summary>
		public static bool isMouse( this RawDevice device )
		{
			if( !device.otherHandlers.Any( s => s.startsWith( "mouse" ) ) )
				return false;
			if( !device.buttonGroups.Contains( eButtonGroup.Mouse ) )
				return false;
			return true;
		}

		static readonly Dictionary<eKey, byte> requiredKeyboardKeys = new Dictionary<eKey, byte>()
		{
			{ eKey.Q, 1 },
			{ eKey.W, 2 },
			{ eKey.E, 4 },
			{ eKey.R, 8 },
			{ eKey.T, 0x10 },
			{ eKey.Y, 0x20 },
		};
		const byte requiredKeysMask = 0x3F;

		/// <summary>True if the device is likely to be a QUERTY keyboard</summary>
		public static bool isQwertyKeyboard( this RawDevice device )
		{
			if( !device.otherHandlers.Any( s => s.equals( "kbd" ) ) )
				return false;
			byte foundKeys = 0;
			foreach( var k in device.keys )
			{
				if( !requiredKeyboardKeys.TryGetValue( k, out byte bit ) )
					continue;
				foundKeys |= bit;
				if( foundKeys == requiredKeysMask )
					return true;
			}
			return false;
		}

		static RawDevice findDevice( Func<RawDevice, bool> selector, int index )
		{
			int i = 0;
			foreach( var dev in RawDevice.list() )
			{
				if( !selector( dev ) )
					continue;
				if( i == index )
					return dev;
				i++;
			}
			return null;
		}

		/// <summary>Open a raw mouse device and wrap it into an adapter that will call the provided object when input happens.</summary>
		public static iInputEventTimeSource openRawMouse( this Dispatcher dispatcher, iMouseHandler handler, CRect clipRect, RawDevice device = null )
		{
			// Find the mouse
			if( null == device )
			{
				device = RawDevice.list().FirstOrDefault( isMouse );
				if( null == device )
					throw new ApplicationException( "No mice are detected" );
			}

			// Create the adapter to translate raw events into mouse events
			RawMouse mouse = clipRect.isEmpty ? new RawMouse( device, handler ) : new RawMouseClipped( device, clipRect, handler );

			// Open the device
			using( iLinuxDispatcher linuxDispatcher = ComLightCast.cast<iLinuxDispatcher>( dispatcher.nativeDispatcher ) )
				linuxDispatcher.openInputDevice( device.eventInterface, mouse );

			return mouse;
		}

		/// <summary>Open a raw mouse device and wrap it into an adapter that will call the provided object when input happens.</summary>
		public static iInputEventTimeSource openRawMouse( this Dispatcher dispatcher, iMouseHandler handler, RawDevice device = null )
		{
			return openRawMouse( dispatcher, handler, CRect.empty, device );
		}

		/// <summary>Open a raw input device, interpret the input as a US English keyboard</summary>
		public static iInputEventTimeSource openRawKeyboard( this Dispatcher dispatcher, iKeyboardHandler handler, RawDevice device = null )
		{
			// Find the keyboard
			if( null == device )
			{
				device = RawDevice.list().FirstOrDefault( isQwertyKeyboard );
				if( null == device )
					throw new ApplicationException( "No keyboards found" );
			}

			// Create the layout. That object also owns the state, i.e. shift/numlock/etc.
			iKeyboardLayout layout = new UsEnglishLayout();
			// Create the adapter to translate raw events into keyboard events
			var keyboard = new RawKeyboard( device, layout, handler ); ;
			// Open the device
			using( iLinuxDispatcher linuxDispatcher = ComLightCast.cast<iLinuxDispatcher>( dispatcher.nativeDispatcher ) )
				linuxDispatcher.openInputDevice( device.eventInterface, keyboard );

			return keyboard;
		}
	}
}