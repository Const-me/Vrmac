using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Vrmac.Input.Linux
{
	/// <summary>Parses lines from /proc/bus/input/devices fake file</summary>
	struct DeviceParser
	{
		public eBusType? bus { get; private set; }
		public ushort? vendor { get; private set; }
		public ushort? product { get; private set; }
		public ushort? revision { get; private set; }
		public string name { get; private set; }
		public string eventIface { get; private set; }
		public List<string> otherHandlers { get; private set; }
		public uint eventTypes { get; private set; }

		public Dictionary<eEventType, uint[]> bits { get; private set; }

		public uint getSingleBits( eEventType et )
		{
			uint[] res = bits.lookup( et );
			if( res.isEmpty() )
				return 0;
			Debug.Assert( 1 == res.Length );
			return res[ 0 ];
		}

		static readonly Dictionary<string, eEventType> eventTypePrefixes;

		static DeviceParser()
		{
			eventTypePrefixes = new Dictionary<string, eEventType>( StringComparer.InvariantCultureIgnoreCase );

			Type t = typeof( eEventType );
			foreach( eEventType et in Enum.GetValues( t ) )
			{
				FieldInfo fi = t.GetField( et.ToString() );
				var a = fi.GetCustomAttribute<BitFieldsKeyAttribute>();
				if( null == a )
					continue;
				eventTypePrefixes.Add( a.key, et );
			}
		}

		public void reset()
		{
			bus = null;
			vendor = product = revision = null;
			name = null;
			eventIface = null;
			eventTypes = 0;
			if( null == bits )
				bits = new Dictionary<eEventType, uint[]>();
			else
				bits.Clear();
			if( null == otherHandlers )
				otherHandlers = new List<string>();
			else
				otherHandlers.Clear();
		}

		static ushort? parseUint16( string str )
		{
			if( ushort.TryParse( str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort num ) )
				return num;
			return null;
		}

		static eBusType? parseBusType( string str )
		{
			ushort? num = parseUint16( str );
			if( !num.HasValue )
				return null;
			ushort us = num.Value;
			if( us > 0xFF )
				return null;
			if( Enum.IsDefined( typeof( eBusType ), (byte)us ) )
				return (eBusType)us;
			return null;
		}

		bool parseIdentity( string line )
		{
			foreach( string kv in line.Split( ' ', StringSplitOptions.RemoveEmptyEntries ) )
			{
				int idx = kv.IndexOf( '=' );
				if( idx <= 0 )
					continue;
				string name = kv.Substring( 0, idx ).ToLowerInvariant();
				string value = kv.Substring( idx + 1 );
				switch( name )
				{
					case "bus":
						bus = parseBusType( value ); break;
					case "vendor":
						vendor = parseUint16( value ); break;
					case "product":
						product = parseUint16( value ); break;
					case "version":
						revision = parseUint16( value ); break;
				}
			}
			return true;
		}

		static string fieldValue( string line )
		{
			int eq = line.IndexOf( '=' );
			if( eq < 0 )
				return null;
			return line.Substring( eq + 1 );
		}

		bool parseName( string line )
		{
			line = fieldValue( line );
			if( null == line )
				return false;
			line = line.Trim();
			if( line.StartsWith( '\"' ) && line.EndsWith( '\"' ) )
				line = line.Substring( 1, line.Length - 2 );
			name = line;
			return true;
		}

		bool parseHandlers( string line )
		{
			line = fieldValue( line );
			string[] handlers = line.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
			foreach( string h in handlers )
			{
				if( h.StartsWith( "event" ) )
				{
					string iface = $"/dev/input/{ h }";
					if( File.Exists( iface ) )
					{
						eventIface = iface;
						otherHandlers.AddRange( handlers.Where( hh => hh != h ) );
						return true;
					}
				}
			}
			return false;
		}

		static uint? parseUint32( string str )
		{
			if( uint.TryParse( str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint val ) )
				return val;
			return null;
		}

		static uint parseEventTypes( string str )
		{
			return parseUint32( str ) ?? 0;
		}

		bool parseBitFields( string line )
		{
			// Parse the XX=YY part
			int eq = line.IndexOf( '=' );
			if( eq < 0 )
				return false;
			string key = line.Substring( 0, eq );
			line = line.Substring( eq + 1 ).Trim();

			// Event types line is a special case 
			if( key.equals( "EV" ) )
			{
				eventTypes = parseEventTypes( line );
				return true;
			}

			// Lookup the key
			if( !eventTypePrefixes.TryGetValue( key, out var et ) )
				return false;

			// Split into fields
			string[] fields = line.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
			if( fields.Length <= 0 )
				return false;

			// Parse fields into uint, flipping the order.
			// No idea why the order is flipped, it's undocumented, but it appears to work this way.
			uint[] bits = new uint[ fields.Length ];
			for( int i = 0; i < fields.Length; i++ )
			{
				uint? ui = parseUint32( fields[ i ] );
				if( !ui.HasValue )
					return false;
				bits[ fields.Length - i - 1 ] = ui.Value;
			}

			// Store in the dictionary
			this.bits.Add( et, bits );
			return true;
		}

		public bool parse( string line )
		{
			if( line.Length <= 2 || line[ 1 ] != ':' )
				return false;
			switch( char.ToLower( line[ 0 ] ) )
			{
				case 'i':
					return parseIdentity( line.Substring( 2 ).Trim() );
				case 'n':
					return parseName( line.Substring( 2 ).Trim() );
				case 'h':
					return parseHandlers( line.Substring( 2 ).Trim() );
				case 'b':
					return parseBitFields( line.Substring( 2 ).Trim() );
			}
			return false;
		}

		public bool isGoodEnough()
		{
			return bus.HasValue && null != eventIface;
		}
	}
}