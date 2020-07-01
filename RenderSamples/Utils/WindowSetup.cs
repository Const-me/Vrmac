using Vrmac;
using Vrmac.ModeSet;
using System;

namespace RenderSamples.Utils
{
	class WindowSetup: iWindowSetup
	{
		void iWindowSetup.adjustInitialPosition( ref CRect displayRectangle, ref sWindowInitialPosition swp )
		{
			// swp.show = eShowWindow.Minimized;
			// swp.show = eShowWindow.Maximized;
			// swp.show = eShowWindow.Fullscreen;
		}

		int iWindowSetup.pickEglConfig( sEglConfig[] configs, int configsCount )
		{
			Console.WriteLine( "iWindowSetup.pickEglConfig, following is available:\n\t{0}",
				string.Join( ";\n\t", configs ) );
			return -1;
		}

		int iWindowSetup.pickScreen( sScreen[] screens, int screensCount )
		{
			Console.WriteLine( "iWindowSetup.pickScreen, following is available:\n\t{0}",
				string.Join( ";\n\t", screens ) );
			return -1;
		}

		bool iWindowSetup.needsVideoSupport() => true;
	}
}