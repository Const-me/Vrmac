using ComLight;

namespace Vrmac.Input
{
	/// <summary>Interface to receive keyboard input.</summary>
	/// <remarks>In windowed mode this interface is called from C++ code.
	/// In full screen mode however this interface is called by C# code which wraps raw input devices.</remarks>
	[ComInterface( "c5b38ddb-0b26-47b6-b02a-6adfb30213b5", eMarshalDirection.ToNative )]
	public interface iKeyboardHandler
	{
		/// <summary>Handle a keyboard event</summary>
		/// <param name="key">Can be 0 on Linux in windowed mode, for keys which depend on keyboard layout.</param>
		/// <param name="what">What heppenned with that key</param>
		/// <param name="unicodeChar">Can be 0 when the key does not map to a Unicode symbol, e.g. backspace key does not.
		/// However, at least one of key and unicodeChar is guaranteed to be non-zero, this event makes no sense otherwise.</param>
		/// <param name="ks">Current state of shift, alt, control, numlock and caps lock.</param>
		void keyEvent( eKey key, eKeyValue what, ushort unicodeChar, eKeyboardState ks );
	}
}