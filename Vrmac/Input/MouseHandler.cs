using System;
using System.Runtime.CompilerServices;
using Vrmac.Utils;

namespace Vrmac.Input
{
	/// <summary>Interface to receive mouse button up &amp; down events</summary>
	public interface iButtonHandler
	{
		/// <summary>A button has been pressed</summary>
		void buttonDown( CPoint point, eMouseButton button, eMouseButtonsState bs );
		/// <summary>A button has been released</summary>
		void buttonUp( CPoint point, eMouseButton button, eMouseButtonsState bs );
	}

	/// <summary>Interface to receive mouse move events</summary>
	public interface iMouseMoveHandler
	{
		/// <summary>A mouse was moved over the window</summary>
		void mouseMove( CPoint point, eMouseButtonsState bs );
	}

	/// <summary>Interface to receive mouse enter &amp; leave events</summary>
	public interface iMouseEnterLeaveHandler
	{
		/// <summary>Mouse pointer entered the window</summary>
		void mouseEnter();
		/// <summary>Mouse pointer left the window</summary>
		void mouseLeave();
	}
	/// <summary>Interface to receive vertical mouse wheel events</summary>
	public interface iMouseWheelHandler
	{
		/// <summary>Mouse wheel was rotated</summary>
		void wheel( CPoint point, int delta, eMouseButtonsState bs );
	}

	/// <summary>Unlike keyboard, iMouseHandler has quote a few events. It's unlikely you want all of them, yet alone all of them in a single object. This utility class helps.</summary>
	public sealed class MouseHandler: iMouseHandler, iInputEventTime, iInputEventTimeSource
	{
		Context context;

		/// <summary>Construct the object, and optionally set up cursor rendering.</summary>
		/// <remarks>Pass null for the context argument to skip the rendering.</remarks>
		public MouseHandler( Context context, eCursor cursor = eCursor.None )
		{
			this.context = context;
			if( null != context )
				context.mouseCursor = cursor;
		}

		// Buttons
		readonly ConditionalWeakTable<iButtonHandler, object> buttonHandlers = new ConditionalWeakTable<iButtonHandler, object>();
		void upDown( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs, bool down )
		{
			CPoint point = new CPoint( x, y );
			foreach( var kvp in buttonHandlers )
			{
				if( down )
					kvp.Key.buttonDown( point, changedButtons, bs );
				else
					kvp.Key.buttonUp( point, changedButtons, bs );
			}
		}
		void iMouseHandler.buttonDown( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs )
		{
			upDown( x, y, changedButtons, bs, true );
		}
		void iMouseHandler.buttonUp( int x, int y, eMouseButton changedButtons, eMouseButtonsState bs )
		{
			upDown( x, y, changedButtons, bs, false );
		}

		// Move
		readonly ConditionalWeakTable<iMouseMoveHandler, object> moveHandlers = new ConditionalWeakTable<iMouseMoveHandler, object>();
		void iMouseHandler.mouseMove( int x, int y, eMouseButtonsState bs )
		{
			CPoint point = new CPoint( x, y );
			foreach( var kvp in moveHandlers )
				kvp.Key.mouseMove( point, bs );

			// In addition to user handlers, also update position in the context, to render it.
			context?.setMouseCursorPosition( point );
		}

		// Vertical wheel
		readonly ConditionalWeakTable<iMouseWheelHandler, object> wheelHandlers = new ConditionalWeakTable<iMouseWheelHandler, object>();
		void iMouseHandler.wheel( int x, int y, int delta, eMouseButtonsState bs )
		{
			CPoint point = new CPoint( x, y );
			foreach( var kvp in wheelHandlers )
				kvp.Key.wheel( point, delta, bs );
		}

		// Enter & Leave
		readonly ConditionalWeakTable<iMouseEnterLeaveHandler, object> enterLeaveHandlers = new ConditionalWeakTable<iMouseEnterLeaveHandler, object>();
		void onEnterLeave( bool enter )
		{
			foreach( var kvp in enterLeaveHandlers )
				if( enter )
					kvp.Key.mouseEnter();
				else
					kvp.Key.mouseLeave();
		}
		void iMouseHandler.mouseEnter()
		{
			onEnterLeave( true );
		}
		void iMouseHandler.mouseLeave()
		{
			onEnterLeave( false );
		}
		// TODO [low]: capture changed and horizontal wheel
		void iMouseHandler.captureChanged( bool hasCapture )
		{
		}
		void iMouseHandler.horizontalWheel( int x, int y, int delta, eMouseButtonsState bs )
		{
		}

		/// <summary>Test object’s support of mouse handling interfaces, subscribe what's supported.</summary>
		public void subscribe( object obj )
		{
			if( null == obj )
				throw new ArgumentNullException();
			object dummy = true;	// Pretty sure .NET runtime has that boxed value cached somewhere
			if( obj is iButtonHandler bh )
				buttonHandlers.AddOrUpdate( bh, dummy );
			if( obj is iMouseMoveHandler mv )
				moveHandlers.AddOrUpdate( mv, dummy );
			if( obj is iMouseWheelHandler mwh )
				wheelHandlers.AddOrUpdate( mwh, dummy );
			if( obj is iMouseEnterLeaveHandler elh )
				enterLeaveHandlers.AddOrUpdate( elh, dummy );
		}

		/// <summary>Unsubscribe the object from mouse events</summary>
		public void unsubscribe( object obj )
		{
			if( null == obj )
				throw new ArgumentNullException();
			if( obj is iButtonHandler bh )
				buttonHandlers.Remove( bh );
			if( obj is iMouseMoveHandler mv )
				moveHandlers.Remove( mv );
			if( obj is iMouseWheelHandler mwh )
				wheelHandlers.Remove( mwh );
			if( obj is iMouseEnterLeaveHandler elh )
				enterLeaveHandlers.Remove( elh );
		}

		/// <summary>Provides timestamps for mouse events</summary>
		public iInputEventTimeSource timeSource { get; private set; }

		/// <summary>Date + time of the currently handled message.</summary>
		public TimeSpan messageTime => timeSource?.messageTime ?? TimeSpan.Zero;

		void iInputEventTime.sourceInitialized( iInputEventTimeSource source )
		{
			timeSource = source;
		}
	}
}