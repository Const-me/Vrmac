namespace Vrmac.Input.Linux
{
	class RawMouseClipped: RawMouse
	{
		readonly CRect clipRect;

		internal RawMouseClipped( RawDevice device, CRect clipRect, iMouseHandler handler ) :
			base( device, handler )
		{
			this.clipRect = clipRect;
			position = prevPosition = clipRect.center;
		}

		static int clip( int x, int i, int ax )
		{
			if( x < i )
				return i;
			if( x > ax )
				return ax;
			return x;
		}

		protected override void handleRelative( eRelativeAxis axis, int value )
		{
			switch( axis )
			{
				case eRelativeAxis.X:
					position.x = clip( position.x + value, clipRect.left, clipRect.right );
					break;
				case eRelativeAxis.Y:
					position.y = clip( position.y + value, clipRect.top, clipRect.bottom );
					break;
				case eRelativeAxis.VerticalWheel:
					handler.wheel( position.x, position.y, value * WHEEL_DELTA, buttonsState );
					break;
				case eRelativeAxis.HorizontalWheel:
					handler.horizontalWheel( position.x, position.y, value * WHEEL_DELTA, buttonsState );
					break;
			}
		}
	}
}