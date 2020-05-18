namespace Vrmac
{
	/// <summary>Controls how the window is shown</summary>
	public enum eShowWindow: byte
	{
		/// <summary>Neither minimized nor maximized</summary>
		Normal,
		/// <summary>Minimized</summary>
		Minimized,
		/// <summary>Maximized</summary>
		Maximized,
		/// <summary>Maximized and without borders or caption</summary>
		Fullscreen,
		/// <summary>Full-screen rendering, Windows only.</summary>
		/// <remarks>On Linux you can achieve same result if you stop desktop manager, and instead create a full screen rendering context.</remarks>
		TrueFullscreen
	}
}