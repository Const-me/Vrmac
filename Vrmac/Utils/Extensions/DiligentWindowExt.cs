using Vrmac.Input;
using TimeSourcesCache = System.Runtime.CompilerServices.ConditionalWeakTable<Vrmac.iDiligentWindow, Vrmac.Input.iInputEventTimeSource>;

namespace Vrmac
{
	/// <summary>Extension methods for iDiligentWindow</summary>
	public static class DiligentWindowExt
	{
		/// <summary>Switch window state without moving the window</summary>
		public static void moveWindow( this iDiligentWindow wnd, eShowWindow newState )
		{
			CRect rect = CRect.empty;
			wnd.moveWindow( newState, ref rect );
		}

		static readonly TimeSourcesCache timeSources = new TimeSourcesCache();
		static iInputEventTimeSource createTimeSource( iDiligentWindow window )
		{
			return new InputEventTime( window.input );
		}
		static readonly TimeSourcesCache.CreateValueCallback callback = createTimeSource;

		/// <summary>Get a time source interface, to get timestamps of input events</summary>
		public static iInputEventTimeSource timeSource( this iDiligentWindow wnd )
		{
			return timeSources.GetValue( wnd, callback );
		}
	}
}