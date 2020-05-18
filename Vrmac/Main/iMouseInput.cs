using Vrmac.Input;
using Vrmac.Input.Linux;

namespace Vrmac
{
	/// <summary>Implement this interface in your <see cref="iScene" />-implementing class to get mouse input.</summary>
	public interface iMouseInput
	{
		/// <summary>Handler for the mouse</summary>
		iMouseHandler mouseHandler { get; }

		/// <summary>Return raw mouse device to use for full-screen mode on Linux, or null to use a default mouse.</summary>
		/// <remarks>This method is only called in that mode.</remarks>
		RawDevice getMouseDevice();
	}
}