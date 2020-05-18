using System;

namespace Vrmac.Input.Linux
{
	/// <summary>Abstract base class for Linux raw input devices</summary>
	public abstract class RawDeviceBase: iRawInputSink, iInputEventTimeSource
	{
		/// <summary>Construct the object</summary>
		public RawDeviceBase()
		{
			if( RuntimeEnvironment.operatingSystem != eOperatingSystem.Linux )
				throw new ApplicationException( "Raw input classes from Diligent.Input.Linux namespace only work on Linux" );
		}

		void iRawInputSink.failed( int hResult )
		{
			// TODO: handle somehow, maybe expose an event
		}

		void iRawInputSink.handle( ulong timestamp, eEventType eventType, ushort code, int value )
		{
			try
			{
				nativeTimeStamp = timestamp;

				switch( eventType )
				{
					case eEventType.Synchro:
						handleSyncro( (eSynchroEvent)( code & 0xFF ) );
						return;
					case eEventType.Key:
						handleKeyOrButton( code, value );
						return;
					case eEventType.Relative:
						handleRelative( (eRelativeAxis)code, value );
						return;
					case eEventType.Absolute:
						handleAbsolute( (eAbsoluteAxis)( code & 0xFF ), value );
						return;
					case eEventType.Miscellaneous:
						handleMiscellaneous( (eMiscEvent)( code & 0xFF ), value );
						return;
					case eEventType.Switch:
						handleSwitch( (eSwitch)( code & 0xFF ), value );
						return;
					case eEventType.LED:
						handleLed( (eLed)( code & 0xFF ), value );
						return;
				}
			}
			catch( Exception ex )
			{
				// ComLight runtime marshals exceptions into failed HRESULT codes, in this case it would cause the app to shut down the dispatcher and quit.
				// Hopefully, an exception in user input handler is not a good reason to shut down.
				ex.logError( "RawDeviceBase.iRawInputSink failed" );
				ConsoleLogger.logDebug( "Moar info: {0}", ex.ToString() );
			}
		}

		bool handleKeyOrButton( ushort code, int value )
		{
			if( value < 0 || value > 0xFF )
				return true;
			eKeyValue keyValue = (eKeyValue)(byte)value;

			if( Enum.IsDefined( typeof( eKey ), code ) )
				handleKey( (eKey)code, keyValue );
			else if( Enum.IsDefined( typeof( eButton ), code ) )
				handleButton( (eButton)code, keyValue );
			return true;
		}

		/// <summary>Timestamp of the last received event, the original value from Linux, nanoseconds since Unix epoch.</summary>
		protected ulong nativeTimeStamp { get; private set; }

		/// <summary>Timestamp of the last received event, UTC</summary>
		public DateTime messageTime
		{
			get
			{
				long ticks = (long)( nativeTimeStamp / 100 ); // DateTime and TimeSpan ticks are 100 nanoseconds / each, a legacy from FILETIME in the original Windows NT
				ticks += DateTime.UnixEpoch.Ticks;
				return new DateTime( ticks, DateTimeKind.Utc );
			}
		}

		TimeSpan iInputEventTimeSource.messageTime => TimeSpan.FromTicks( (long)( nativeTimeStamp / 100 ) );

		/// <summary>Handle EV_SYN events</summary>
		protected virtual void handleSyncro( eSynchroEvent synchroEvent ) { }

		/// <summary>Handle EV_KEY events where the key symbolic name starts with "KEY_"</summary>
		protected virtual void handleKey( eKey key, eKeyValue keyValue ) { }

		/// <summary>Handle EV_KEY events where the key symbolic name starts with "BTN_"</summary>
		protected virtual void handleButton( eButton button, eKeyValue keyValue ) { }

		/// <summary>Handle EV_REL events</summary>
		protected virtual void handleRelative( eRelativeAxis axis, int value ) { }

		/// <summary>Handle EV_ABS events</summary>
		protected virtual void handleAbsolute( eAbsoluteAxis axis, int value ) { }

		/// <summary>Handle EV_MSC events</summary>
		protected virtual void handleMiscellaneous( eMiscEvent miscEvent, int value ) { }

		/// <summary>Handle EV_SW events</summary>
		protected virtual void handleSwitch( eSwitch switchEvent, int value ) { }

		/// <summary>Handle EV_LED events</summary>
		protected virtual void handleLed( eLed led, int value ) { }

		/// <summary>Called at the end of every burst of events</summary>
		protected virtual void updated() { }

		void iRawInputSink.updated() => updated();
	}
}