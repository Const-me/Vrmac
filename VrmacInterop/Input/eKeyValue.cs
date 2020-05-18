namespace Vrmac.Input
{
	/// <summary>Values for eEventType.Key events in raw input API, also used in iKeyboardHandler.keyEvent method.</summary>
	public enum eKeyValue: byte
	{
		/// <summary>The key has been released</summary>
		Released = 0,
		/// <summary>The key is just pressed</summary>
		Pressed = 1,
		/// <summary>The key has been pressed for a while, the OS-implemented autorepeat feature kicked in and is spamming more of these events</summary>
		AutoRepeat = 2,
	}
}