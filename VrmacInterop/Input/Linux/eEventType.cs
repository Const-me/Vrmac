namespace Vrmac.Input.Linux
{
	/// <summary>Event types</summary>
	public enum eEventType: byte
	{
		/// <summary>Markers to separate events. Events may be separated in time or in space, such as with the multitouch protocol</summary>
		Synchro = 0,

		/// <summary>State changes of keyboards, buttons, or other key-like devices</summary>
		[BitFieldsKey( "KEY" )]
		Key = 1,

		/// <summary>Relative axis value changes, e.g. moving the mouse 5 units to the left</summary>
		[BitFieldsKey( "REL" )]
		Relative = 2,

		/// <summary>Absolute axis value changes, e.g. the coordinates of a touch on a touchscreen</summary>
		[BitFieldsKey( "ABS" )]
		Absolute = 3,

		/// <summary>Miscellaneous input data that do not fit into other types</summary>
		[BitFieldsKey( "MSC" )]
		Miscellaneous = 4,

		/// <summary>Binary state input switches</summary>
		[BitFieldsKey( "SW" )]
		Switch = 5,

		/// <summary>Turn LEDs on devices on and off</summary>
		[BitFieldsKey( "LED" )]
		LED = 0x11,

		/// <summary>Output sound to devices</summary>
		[BitFieldsKey( "SND" )]
		Sound = 0x12,

		/// <summary>Autorepeating devices</summary>
		[BitFieldsKey( "REP" )]
		Repeat = 0x14,

		/// <summary>Send force feedback commands to an input device</summary>
		[BitFieldsKey( "FF" )]
		ForceFeedback = 0x15,

		/// <summary>A special type for power button and switch input</summary>
		[BitFieldsKey( "PWR" )]
		Power = 0x16,

		/// <summary>Used to receive force feedback device status</summary>
		[BitFieldsKey( "FF_STATUS" )]
		ForceFeedbackStatus = 0x17
	}
}