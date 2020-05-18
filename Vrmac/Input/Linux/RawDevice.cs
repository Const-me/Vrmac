using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vrmac.Input.Linux
{
	/// <summary>Information about found Linux input device. This class is immutable and doesn't do any IO, it only describes an input device detected by the OS kernel.</summary>
	public class RawDevice
	{
		/// <summary>Bus type</summary>
		public readonly eBusType bus;

		/// <summary>Vendor, product and revision IDs</summary>
		public readonly ushort? vendor, product, revision;

		/// <summary>Vendor ID resolved into string.</summary>
		/// <remarks>It's only implemented for USB devices</remarks>
		public string manufacturer
		{
			get
			{
				if( bus != eBusType.USB || !vendor.HasValue )
					return null;
				return UsbVendors.print( vendor.Value );
			}
		}

		/// <summary>Vendor and product IDs, resolved into string</summary>
		/// <remarks>It's only implemented for USB devices</remarks>
		public string productDescription
		{
			get
			{
				if( bus != eBusType.USB || !vendor.HasValue || !product.HasValue )
					return null;
				return UsbDevices.lookup( vendor.Value, product.Value );
			}
		}

		/// <summary>Name, as reported by the device</summary>
		public readonly string name;
		/// <summary>Event interface, pass this string to <see cref="iLinuxDispatcher.openInputDevice(string, iRawInputSink)" /> method.</summary>
		public readonly string eventInterface;

		/// <summary>Other values from "Handlers" field of the device, we use this to find keyboards which have "kbd" on that list</summary>
		public readonly string[] otherHandlers;

		const string source = @"/proc/bus/input/devices";

		// Bit mask of the event types the device might produce
		readonly uint eventTypesBits;
		/// <summary>Event types the device might produce</summary>
		public IEnumerable<eEventType> eventTypes =>
			eventTypesBits.enumSetBits().Select( i => (eEventType)i );

		// Bit mask of the keys and buttons the device has
		readonly uint[] keysBits;
		/// <summary>Keys the device has</summary>
		public IEnumerable<eKey> keys => keysOrButtons<eKey>().Select( i => (eKey)i );
		/// <summary>Buttons the device has</summary>
		public IEnumerable<eButton> buttons => keysOrButtons<eButton>().Select( i => (eButton)i );
		/// <summary>Button groups of the device</summary>
		public IEnumerable<eButtonGroup> buttonGroups => keysBits.buttonGroups();

		readonly uint ledBits;
		/// <summary>LEDs the device has</summary>
		public IEnumerable<eLed> leds => ledBits.enumSetBits().Select( i => (eLed)i );

		readonly uint relBits;
		/// <summary>Relative axes the device reports</summary>
		public IEnumerable<eRelativeAxis> relativeAxes => relBits.enumSetBits().Select( i => (eRelativeAxis)i );

		readonly uint[] absBits;
		/// <summary>Absolute axes the device reports</summary>
		public IEnumerable<eAbsoluteAxis> absoluteAxes => absBits.enumSetBits().Select( i => (eAbsoluteAxis)i );

		readonly uint switchBits;
		/// <summary>Switches the device has</summary>
		public IEnumerable<eSwitch> switches => switchBits.enumSetBits().Select( i => (eSwitch)i );

		readonly uint miscEventsBits;
		/// <summary>Miscellaneous events the device produces</summary>
		public IEnumerable<eMiscEvent> miscellaneousEvents => miscEventsBits.enumSetBits().Select( i => (eMiscEvent)i );

		RawDevice( ref DeviceParser parser )
		{
			bus = parser.bus.Value;
			vendor = parser.vendor;
			product = parser.product;
			revision = parser.revision;
			name = parser.name;
			eventInterface = parser.eventIface;
			otherHandlers = parser.otherHandlers.ToArray();
			eventTypesBits = parser.eventTypes;

			keysBits = parser.bits.lookup( eEventType.Key );
			ledBits = parser.getSingleBits( eEventType.LED );
			relBits = parser.getSingleBits( eEventType.Relative );
			absBits = parser.bits.lookup( eEventType.Absolute );
			switchBits = parser.getSingleBits( eEventType.Switch );
			miscEventsBits = parser.getSingleBits( eEventType.Miscellaneous );
		}

		/// <summary>Enumerate currently attached input devices</summary>
		public static IEnumerable<RawDevice> list()
		{
			DeviceParser parser = new DeviceParser();
			parser.reset();

			foreach( string line in MiscUtils.readLines( source ) )
			{
				if( string.IsNullOrWhiteSpace( line ) )
				{
					if( parser.isGoodEnough() )
						yield return new RawDevice( ref parser );
					parser.reset();
				}
				parser.parse( line );
			}
		}

		IEnumerable<ushort> keysOrButtons<T>()
		{
			foreach( int idx in keysBits.enumSetBits() )
			{
				ushort typeCode = (ushort)idx;
				if( !Enum.IsDefined( typeof( T ), typeCode ) )
					continue;
				yield return typeCode;
			}
		}

		IEnumerable<string> desc()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( "{0}\\VID_{1:X4}&PID_{1:X4}", bus, vendor, product );
			if( revision.HasValue )
				sb.AppendFormat( "&REV_{0:X4}", revision );
			yield return sb.ToString();
			yield return eventInterface;
			if( name.notEmpty() )
				yield return name;
			if( eventTypes.Any() )
				yield return string.Join( ", ", eventTypes );
			if( keys.Any() )
				yield return "Keys: " + string.Join( " ", keys );
			if( buttons.Any() )
				yield return "Buttons: " + string.Join( " ", buttons );
			if( buttonGroups.Any() )
				yield return "Button groups: " + string.Join( " ", buttonGroups );
			if( leds.Any() )
				yield return "LEDs: " + string.Join( ' ', leds );
			if( relativeAxes.Any() )
				yield return "Relative: " + string.Join( ' ', relativeAxes );
			if( absoluteAxes.Any() )
				yield return "Absolute: " + string.Join( ' ', absoluteAxes );
			if( switches.Any() )
				yield return "Switches: " + string.Join( ' ', switches );
			if( miscellaneousEvents.Any() )
				yield return "Misc. events: " + string.Join( ' ', miscellaneousEvents );

			yield return manufacturer;
			string pd = productDescription;
			if( null != pd )
				yield return pd;
		}

		/// <summary>Return detailed description of this device</summary>
		public string printDetails()
		{
			return string.Join( '\n', desc() );
		}
	}
}