using System.Collections.Generic;
using System.Linq;

namespace Vrmac.Input.Linux
{
	class RawMouse: RawDeviceBase
	{
		protected readonly iMouseHandler handler;

		public readonly string deviceName;

		public readonly eButton[] buttons;

		internal RawMouse( RawDevice device, iMouseHandler handler )
		{
			this.handler = handler;

			buttons = device.buttons.ToArray();

			deviceName = device.name;
			if( deviceName.isEmpty() )
				deviceName = device.productDescription;
			if( deviceName.isEmpty() )
			{
				string mfg = device.manufacturer;
				if( mfg.notEmpty() )
					deviceName = $"A mouse made by { mfg }";
				else
					deviceName = $"Unidentified { device.bus } mouse";
			}
		}

		/// <summary>This hash map maps from raw input enum into regular mouse button enum.</summary>
		static readonly Dictionary<eButton, eMouseButton> buttonsMap = new Dictionary<eButton, eMouseButton>()
		{
			{  eButton.Left, eMouseButton.Left },
			{  eButton.Middle, eMouseButton.Middle },
			{  eButton.Right, eMouseButton.Right },
			// Not sure these 2 are always what's written here, only tested with Logitech G700s
			{  eButton.Side, eMouseButton.XButton1 },
			{  eButton.Extra, eMouseButton.XButton2 },
		};

		protected eMouseButtonsState buttonsState { get; private set; } = eMouseButtonsState.None;

		void setStateBit( eMouseButton button )
		{
			int bit = ( 1 << (byte)button );
			buttonsState |= (eMouseButtonsState)bit;
		}
		void clearStateBit( eMouseButton button )
		{
			int bit = ( 1 << (byte)button );
			buttonsState &= ( ~(eMouseButtonsState)bit );
		}

		protected override void handleButton( eButton button, eKeyValue keyValue )
		{
			if( buttonsMap.TryGetValue( button, out var res ) )
			{
				switch( keyValue )
				{
					case eKeyValue.Pressed:
						setStateBit( res );
						handler.buttonDown( position.x, position.y, res, buttonsState );
						break;
					case eKeyValue.Released:
						clearStateBit( res );
						handler.buttonUp( position.x, position.y, res, buttonsState );
						break;
				}
			}
		}

		protected CPoint position = new CPoint( 0, 0 );
		protected CPoint prevPosition = new CPoint( 0, 0 );
		protected const int WHEEL_DELTA = 120;

		protected override void handleRelative( eRelativeAxis axis, int value )
		{
			// Console.WriteLine( "RawMouse.handleRelative: {0} {1}", axis, value );
			switch( axis )
			{
				case eRelativeAxis.X:
					position.x += value;
					break;
				case eRelativeAxis.Y:
					position.y += value;
					break;
				case eRelativeAxis.VerticalWheel:
					handler.wheel( position.x, position.y, value * WHEEL_DELTA, buttonsState );
					break;
				case eRelativeAxis.HorizontalWheel:
					handler.horizontalWheel( position.x, position.y, value * WHEEL_DELTA, buttonsState );
					break;
			}
		}

		protected override void handleSyncro( eSynchroEvent synchroEvent )
		{
			if( synchroEvent != eSynchroEvent.Report || position == prevPosition )
				return;
			// ConsoleLogger.logDebug( "RawMouse.handleSyncro, updated" );
			handler.mouseMove( position.x, position.y, buttonsState );
			prevPosition = position;
		}
	}
}