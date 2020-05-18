using System;

namespace Vrmac.Input
{
	/// <summary>State of keyboard's modifier keys and LEDs</summary>
	[Flags]
	public enum eKeyboardState: byte
	{
		/// <summary>None of the below</summary>
		None = 0,
		/// <summary>At least 1 of the shift keys is pressed</summary>
		ShiftDown = 1,
		/// <summary>At least 1 of the control keys is pressed</summary>
		ControlDown = 2,
		/// <summary>At least 1 of the alt keys is pressed</summary>
		AltDown = 4,
		/// <summary>CapsLock LED is lit</summary>
		CapsLock = 0x10,
		/// <summary>NumLock LED is lit</summary>
		NumLock = 0x20,
	};
}