using Vrmac.Input.Linux;

namespace Vrmac.Input.KeyboardLayout
{
	/// <summary>Interface for a keyboard layout</summary>
	public interface iKeyboardLayout
	{
		/// <summary>State of the keyboard</summary>
		eKeyboardState state { get; }

		/// <summary>Update that state with a key</summary>
		void updateState( eKey key, eKeyValue keyValue );

		/// <summary>Update that state with an LED</summary>
		void updateState( eLed led, int value );

		/// <summary>Resolve key into Unicode character, returns '\0' if the key does not produce any characters.</summary>
		char keyChar( eKey key );
	}
}