// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System.Runtime.InteropServices;

namespace Vrmac.Input
{
	[ComInterface( "9eaeddc6-12d5-4fc0-8d7a-13080ac16f73", eMarshalDirection.ToNative )]
	public interface iMouseHandler
	{
		void buttonDown( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs );
		void buttonUp( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs );

		void captureChanged( [MarshalAs( UnmanagedType.U1 )] bool hasCapture );

		void mouseMove( int x, int y, eMouseButtonsState bs );
		void mouseEnter();
		void mouseLeave();

		void wheel( int x, int y, int delta, eMouseButtonsState bs );
		void horizontalWheel( int x, int y, int delta, eMouseButtonsState bs );
	}
}