using Vrmac.Input;
using Vrmac.Input.Linux;

namespace Vrmac
{
	/// <summary>Implement this interface in your <see cref="iScene" />-implementing class to get keyboard input.</summary>
	public interface iKeyboardInput
	{
		/// <summary>Handler for the keyboard</summary>
		iKeyboardHandler keyboardHandler { get; }

		/// <summary>Return raw keyboard device to use for full-screen mode on Linux, or null to use a default keyboard.</summary>
		/// <remarks>This method is only called in that mode.</remarks>
		RawDevice getKeyboardDevice();
	}
}