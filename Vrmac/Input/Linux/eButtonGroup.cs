#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace Vrmac.Input.Linux
{
	/// <summary>Buttons are grouped into categories.</summary>
	public enum eButtonGroup: byte
	{
		Miscellaneous,
		Mouse,
		Joystick,
		Gamepad,
		Digitizer,
		/// <summary>Wheel controllers might have 2 buttons for shifting gears</summary>
		Wheel,
		/// <summary>A flat, usually thumb-operated, usually digital, four-way directional control with one button on each point,
		/// found on nearly all modern video game console gamepads, game controllers, on the remote control units of some television and DVD players, and smart phones.</summary>
		DirectionalPad,
		/// <summary>Generic joystick events for devices with more than 16 buttons, such as Saitek X52 Pro Flight System</summary>
		TriggerHappy,
	}

	static class ButtonGroupExt
	{
		static eButtonGroup? buttonGroup( int index )
		{
			int mask = ( index & 0xFFF0 );
			switch( mask )
			{
				case 0x100:
					return eButtonGroup.Miscellaneous;
				case 0x110:
					return eButtonGroup.Mouse;
				case 0x120:
					return eButtonGroup.Joystick;
				case 0x130:
					return eButtonGroup.Gamepad;
				case 0x140:
					return eButtonGroup.Digitizer;
				case 0x150:
					return eButtonGroup.Wheel;
				case 0x220:
					return eButtonGroup.DirectionalPad;
				case 0x2c0:
				case 0x2d0:
				case 0x2e0:
					return eButtonGroup.TriggerHappy;
			}
			return null;
		}

		public static eButtonGroup? buttonGroup( this eButton button )
		{
			return buttonGroup( (ushort)button );
		}

		const uint lowWordMask = 0xFFFF;
		const uint highWordMask = 0xFFFF0000;

		public static IEnumerable<eButtonGroup> buttonGroups( this uint[] keyBits )
		{
			// Ignore first 0x100 bits = 8 values
			if( null == keyBits || keyBits.Length <= 8 )
				yield break;

			uint u = keyBits[ 8 ]; // 8 * 32 = 256 = 0x100
			if( 0 != ( u & lowWordMask ) )
				yield return eButtonGroup.Miscellaneous;
			if( 0 != ( u & highWordMask ) )
				yield return eButtonGroup.Mouse;

			if( keyBits.Length <= 9 )
				yield break;
			u = keyBits[ 9 ];  // 9 * 32 = 0x120
			if( 0 != ( u & lowWordMask ) )
				yield return eButtonGroup.Joystick;
			if( 0 != ( u & highWordMask ) )
				yield return eButtonGroup.Gamepad;

			if( keyBits.Length <= 10 )
				yield break;
			u = keyBits[ 10 ]; // 10 * 32 = 0x140
			if( 0 != ( u & lowWordMask ) )
				yield return eButtonGroup.Digitizer;
			if( 0 != ( u & highWordMask ) )
				yield return eButtonGroup.Wheel;

			if( keyBits.Length <= 17 )
				yield break;
			u = keyBits[ 17 ];	// 17 * 32 = 0x220
			if( 0 != ( u & lowWordMask ) )
				yield return eButtonGroup.DirectionalPad;

			if( keyBits.Length <= 22 )
				yield break;
			bool triggerHappy = ( 0 != keyBits[ 22 ] );    // 22 * 32 = 0x2C0
			if( keyBits.Length >= 24 )
			{
				if( 0 != ( keyBits[ 23 ] & lowWordMask ) )
					triggerHappy = true;
			}
			if( triggerHappy )
				yield return eButtonGroup.TriggerHappy;
		}
	}
}