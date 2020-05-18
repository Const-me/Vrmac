using Diligent.Graphics;
using System;

namespace Vrmac.Input
{
	/// <summary>Utility class to produce mouse drag events</summary>
	public sealed class MouseDragEvent: iButtonHandler, iMouseMoveHandler, iMouseEnterLeaveHandler
	{
		void iMouseEnterLeaveHandler.mouseEnter() { }
		void iMouseEnterLeaveHandler.mouseLeave()
		{
			prevPoint = null;
		}
		void iButtonHandler.buttonDown( CPoint point, eMouseButton button, eMouseButtonsState bs )
		{
			if( button != this.button )
				return;
			prevPoint = point;
		}
		void iButtonHandler.buttonUp( CPoint point, eMouseButton button, eMouseButtonsState bs )
		{
			if( button != this.button )
				return;
			prevPoint = null;
		}

		void iMouseMoveHandler.mouseMove( CPoint point, eMouseButtonsState bs )
		{
			if( !prevPoint.HasValue )
				return;
			if( !bs.HasFlag( buttonBit ) )
			{
				prevPoint = null;
				return;
			}

			// Detected the event
			int dx = point.x - prevPoint.Value.x;
			int dy = point.y - prevPoint.Value.y;
			prevPoint = point;
			if( dx == 0 && dy == 0 )
				return;

			Vector2 delta = new Vector2( dx, dy );
			delta *= m_scaling;
			onDragDelta?.Invoke( delta );
		}

		readonly eMouseButton button;
		readonly eMouseButtonsState buttonBit;
		Vector2 m_scaling;
		CPoint? prevPoint = null;

		MouseDragEvent( eMouseButton button, float scaling )
		{
			this.button = button;
			buttonBit = (eMouseButtonsState)( 1 << (int)button );
			m_scaling = new Vector2( scaling );
		}

		/// <summary>Construct for the specified mouse button, unscaled</summary>
		public MouseDragEvent( eMouseButton button ) :
			this( button, 1 )
		{ }

		/// <summary>Construct for the specified mouse button, DPI-scaled</summary>
		public MouseDragEvent( eMouseButton button, double dpiScalingFactor ) :
			this( button, (float)( 1.0 / dpiScalingFactor ) )
		{ }

		/// <summary>Get or set speed multiplier caused by DPI scaling</summary>
		public float dpiSpeedMultiplier
		{
			get => m_scaling.X;
			set
			{
				if( value <= 0.001f || value >= 1000 )
					throw new ArgumentOutOfRangeException();
				m_scaling = new Vector2( value );
			}
		}

		/// <summary>This event gets fired when user moves mouse while the button is held down.</summary>
		public event Action<Vector2> onDragDelta;
	}
}