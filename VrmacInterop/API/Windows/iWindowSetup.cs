using ComLight;
using Vrmac.ModeSet;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Implement this interface for advanced control over window back buffer format, and/or it‘s initial position</summary>
	[ComInterface( "3a0d0e3d-321b-4962-9ee7-ad81dcb1d807", eMarshalDirection.ToNative )]
	public interface iWindowSetup
	{
		/// <summary>Select GLES configuration of the front buffer. Used by full-screen Linux contexts.</summary>
		/// <param name="configs">Configurations supported by the display and GPU driver</param>
		/// <param name="configsCount">Count of available configurations</param>
		/// <returns>Zero-based index of the config to use, or -1 to use built-in heuristics instead.</returns>
		/// <remarks>Only called on Linux.</remarks>
		int pickEglConfig( [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] sEglConfig[] configs,
			int configsCount );

		/// <summary>Select a screen to display the window.</summary>
		/// <remarks>Only called if you're using windowed rendering.</remarks>
		int pickScreen( [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] sScreen[] screens, int screensCount );

		/// <summary>Called while creating the window, you can adjust the default position here.</summary>
		/// <remarks>Only called if you're using windowed rendering.
		/// If you'll change nothing, the window will be shown at some reasonable default position, restored.</remarks>
		void adjustInitialPosition( [In] ref CRect displayRectangle, [In, Out] ref sWindowInitialPosition swp );

		/// <summary>Return true if you want video support on Windows</summary>
		bool needsVideoSupport();
	}
}