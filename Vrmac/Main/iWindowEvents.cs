namespace Vrmac
{
	/// <summary>Implement this interface in your <see cref="iScene" />-implementing class to be able to respond to some of the events happening to the window.</summary>
	public interface iWindowEvents
	{
		/// <summary>User wants to close the window, e.g. pressed Alt+F4. Return true to close, or false to ignore user input.</summary>
		bool shouldClose();
		/// <summary>The window has been closed. Return true to quit the application and return from Render.renderWindowed(), return false to keep the process running.</summary>
		bool shouldExit();
	}
}