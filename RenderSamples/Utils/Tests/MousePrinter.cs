using Vrmac.Input;
using System;

namespace RenderSamples.Utils.Tests
{
	class MousePrinter: iMouseHandler
	{
		void iMouseHandler.buttonDown( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs )
		{
			Console.WriteLine( "MousePrinter.buttonDown {0} {1} {2} {3}", x, y, changedButtons, bs );
		}
		void iMouseHandler.buttonUp( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs )
		{
			Console.WriteLine( "MousePrinter.buttonUp {0} {1} {2} {3}", x, y, changedButtons, bs );
		}
		void iMouseHandler.mouseMove( int x, int y, eMouseButtonsState bs )
		{
			Console.WriteLine( "MousePrinter.mouseMove {0} {1} {2}", x, y, bs );
		}
		void iMouseHandler.wheel( int x, int y, int delta, eMouseButtonsState bs )
		{
			Console.WriteLine( "MousePrinter.wheel {0} {1} {2} {3}", x, y, delta, bs );
		}
		void iMouseHandler.horizontalWheel( int x, int y, int delta, eMouseButtonsState bs )
		{
			Console.WriteLine( "MousePrinter.horizintalWheel {0} {1} {2} {3}", x, y, delta, bs );
		}

		void iMouseHandler.captureChanged( bool hasCapture )
		{ }

		void iMouseHandler.mouseEnter()
		{ }

		void iMouseHandler.mouseLeave()
		{ }
	}
}