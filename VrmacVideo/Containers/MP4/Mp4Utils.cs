using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace VrmacVideo.Containers.MP4
{
	static class Mp4Utils
	{
		// Table 1 — Box types, structure, and cross-reference (Informative), page 15
		static readonly HashSet<eBoxType> containerBoxes = new HashSet<eBoxType>()
		{
			eBoxType.moov,
			eBoxType.trak,
			eBoxType.edts,
			eBoxType.mdia,
			eBoxType.minf,
			eBoxType.stbl,
			eBoxType.mvex,
			eBoxType.moof,
			eBoxType.traf,
			eBoxType.mfra,
			eBoxType.skip,
			eBoxType.udta,
			eBoxType.strk,
			eBoxType.meta,
			eBoxType.dinf,
			eBoxType.ipro,
			eBoxType.sinf,
			eBoxType.fiin,
			eBoxType.paen,
			eBoxType.meco,
			eBoxType.mere,
		};

		public static bool isContainer( this eBoxType box )
		{
			return containerBoxes.Contains( box );
		}

		public static string print( this eBoxType box )
		{
			if( Enum.IsDefined( typeof( eBoxType ), box ) )
				return box.ToString();
			byte[] bytes = BitConverter.GetBytes( (uint)box );
			return $"\"{ Encoding.ASCII.GetString( bytes ) }\"";
		}

		public static Span<T> cast<T>( this Span<byte> src ) where T : unmanaged
		{
			return MemoryMarshal.Cast<byte, T>( src );
		}

		public static IEnumerable<eBoxType> readChildren( this Mp4Reader reader )
		{
			int lvl = reader.level;
			Debug.Assert( lvl > 0 );
			Debug.Assert( reader.currentBox.isContainer() );
			lvl--;

			while( true )
			{
				eBoxType boxType = reader.readBox();
				// Console.WriteLine( "{0}:   {1}", reader.currentBoxNames, boxType );
				if( boxType == eBoxType.ChildContainerEnd )
				{
					if( reader.level == lvl )
						yield break;
				}
				else
					yield return boxType;
			}
		}

		internal static uint readUInt( this Mp4Reader reader )
		{
			Span<byte> bytes = stackalloc byte[ 4 ];
			reader.read( bytes );
			return bytes.cast<uint>()[ 0 ];
		}

		internal static T readStructure<T>( this Mp4Reader reader ) where T : unmanaged
		{
			T result = default;
			Span<T> span1 = MemoryMarshal.CreateSpan( ref result, 1 );
			Span<byte> span2 = MemoryMarshal.Cast<T, byte>( span1 );
			reader.read( span2 );
			return result;
		}

		internal static Span<byte> asBytes<T>( this Span<T> span ) where T : unmanaged
		{
			return MemoryMarshal.Cast<T, byte>( span );
		}

		public static void moveToBox( this Mp4Reader reader, eBoxType boxType )
		{
			while( true )
			{
				var bt = reader.readBox();
				if( bt == boxType )
					return;
				if( bt == eBoxType.Empty )
					throw new EndOfStreamException();
			}
		}

		static readonly DateTime epoch = new DateTime( 1904, 1, 1, 0, 0, 0, DateTimeKind.Utc );

		public static DateTime time( long seconds ) =>
			epoch + TimeSpan.FromTicks( BinaryPrimitives.ReverseEndianness( seconds ) * TimeSpan.TicksPerSecond );
		public static DateTime time( uint seconds ) =>
			epoch + TimeSpan.FromTicks( BinaryPrimitives.ReverseEndianness( seconds ) * TimeSpan.TicksPerSecond );

		internal static TimeSpan duration( uint val, uint timescale )
		{
			double seconds = ( (double)BinaryPrimitives.ReverseEndianness( val ) ) / timescale;
			return TimeSpan.FromSeconds( seconds );
		}
		internal static TimeSpan duration( long val, uint timescale )
		{
			double seconds = ( (double)BinaryPrimitives.ReverseEndianness( val ) ) / timescale;
			return TimeSpan.FromSeconds( seconds );
		}

		static readonly Dictionary<ushort, CultureInfo> culturesCache = new Dictionary<ushort, CultureInfo>();

		static string decryptLanguageCode( ushort packedValue )
		{
			// 8.4.2.2
			// bit(1) pad = 0;
			// unsigned int( 5 )[ 3 ] language;
			// language declares the language code for this media. See ISO 639‐2 / T for the set of three character codes.
			// Each character is packed as the difference between its ASCII value and 0x60.
			// Since the code is confined to being three lowercase letters, these values are strictly positive.

			// 4.2
			// The fields in the objects are stored with the most significant byte first, commonly known as network byte order or big‐endian format.
			// When fields smaller than a byte are defined, or fields span a byte boundary, the bits are assigned from the most significant bits in each byte to the least significant.
			// For example, a field of two bits followed by a field of six bits has the two bits in the high order bits of the byte.

			// So, the damn bits are laid out like this in memory:
			// 0AAAAABB
			// BBBCCCCC
			// On input, the value has this: BBBCCCCC 0AAAAABB

			packedValue = BinaryPrimitives.ReverseEndianness( packedValue );
			// Now it has this 2 bytes: 0AAAAABB BBBCCCCC

			Span<byte> isoCode = stackalloc byte[ 3 ];
			checked
			{
				isoCode[ 0 ] = (byte)( ( ( packedValue >> 10 ) & 0x1F ) + 0x60 );
				isoCode[ 1 ] = (byte)( ( ( packedValue >> 5 ) & 0x1F ) + 0x60 );
				isoCode[ 2 ] = (byte)( ( packedValue & 0x1F ) + 0x60 );
			}
			return Encoding.ASCII.GetString( isoCode );
		}

		internal static CultureInfo culture( ushort packedValue )
		{
			if( culturesCache.TryGetValue( packedValue, out var ci ) )
				return ci;

			string iso = decryptLanguageCode( packedValue );

			foreach( var culture in CultureInfo.GetCultures( CultureTypes.AllCultures ) )
			{
				if( culture.ThreeLetterISOLanguageName == iso )
				{
					culturesCache.Add( packedValue, culture );
					return culture;
				}
			}

			culturesCache.Add( packedValue, null );
			return null;
		}
	}
}