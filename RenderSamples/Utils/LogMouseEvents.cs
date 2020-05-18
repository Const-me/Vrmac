using Vrmac.Input;
using static Vrmac.ConsoleLogger;

namespace RenderSamples.Utils
{
	class LogMouseEvents: iMouseHandler
	{
		void iMouseHandler.buttonDown( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs ) =>
			logInfo( "{0} button down at {1}, {2}, state {3}", changedButtons, x, y, bs );

		void iMouseHandler.buttonUp( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs ) =>
			logInfo( "{0} button up at {1}, {2}, state {3}", changedButtons, x, y, bs );

		void iMouseHandler.captureChanged( bool hasCapture ) =>
			logInfo( "Mouse capture changed, {0}", hasCapture ? "now captured" : "no longer captured" );

		void iMouseHandler.mouseEnter() =>
			logInfo( "mouseEnter" );

		void iMouseHandler.mouseLeave() =>
			logInfo( "mouseLeave" );

		void iMouseHandler.mouseMove( int x, int y, eMouseButtonsState bs ) =>
			logInfo( "mouseMove at {0}, {1}, state {2}", x, y, bs );

		void iMouseHandler.wheel( int x, int y, int delta, eMouseButtonsState bs ) =>
			logInfo( "mouse wheel at {0}, {1}, delta {2}, state {3}", x, y, delta, bs );

		void iMouseHandler.horizontalWheel( int x, int y, int delta, eMouseButtonsState bs ) =>
			logInfo( "horizontalWheel at {0}, {1}, delta {2}, state {3}", x, y, delta, bs );
	}
}