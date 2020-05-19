using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vrmac.Draw;
using SDColor = System.Drawing.Color;

namespace Vrmac
{
	/// <summary>Utility class to parse strings into colors</summary>
	public static class Color
	{
		const float inv255 = (float)( 1 / 255.0 );
		const float inv15 = (float)( 1 / 15.0 );

		static Vector4 parseHex( string str )
		{
			ReadOnlySpan<char> span = str.AsSpan().Slice( 1 );
			if( !uint.TryParse( span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint val ) )
				throw new ArgumentException( "Expected a hexadecimal color value" );

			float r, g, b, a, mul;
			switch( span.Length )
			{
				case 3: // #rgb
					r = ( val >> 8 );
					g = ( ( val >> 4 ) & 0xF );
					b = ( val & 0xF );
					return new Vector4( r * inv15, g * inv15, b * inv15, 1 );

				case 4: // #argb
					a = ( val >> 12 );
					r = ( ( val >> 8 ) & 0xF );
					g = ( ( val >> 4 ) & 0xF );
					b = ( val & 0xF );
					mul = a * ( inv15 * inv15 );
					return new Vector4( r * mul, g * mul, b * mul, a * inv15 );

				case 6: // #rrggbb
					r = ( val >> 16 );
					g = ( ( val >> 8 ) & 0xFF );
					b = ( val & 0xFF );
					return new Vector4( r * inv255, g * inv255, b * inv255, 1 );

				case 8: // #aarrggbb
					a = ( val >> 24 );
					r = ( ( val >> 16 ) & 0xFF );
					g = ( ( val >> 8 ) & 0xFF );
					b = ( val & 0xFF );
					mul = a * ( inv255 * inv255 );
					return new Vector4( r * mul, g * mul, b * mul, a * inv255 );

				default:
					throw new ArgumentException( "Unexpected length of hexadecimal color, it must be 3, 4, 6 or 8 hexadecimal digits" );
			}
		}

		static Vector4 parseNonPremultipliedHex( string str )
		{
			ReadOnlySpan<char> span = str.AsSpan().Slice( 1 );
			if( !uint.TryParse( span, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint val ) )
				throw new ArgumentException( "Expected a hexadecimal color value" );

			float r, g, b, a;
			switch( span.Length )
			{
				case 3: // #rgb
					r = ( val >> 8 );
					g = ( ( val >> 4 ) & 0xF );
					b = ( val & 0xF );
					return new Vector4( r * inv15, g * inv15, b * inv15, 1 );

				case 4: // #argb
					a = ( val >> 12 );
					r = ( ( val >> 8 ) & 0xF );
					g = ( ( val >> 4 ) & 0xF );
					b = ( val & 0xF );
					return new Vector4( r * inv15, g * inv15, b * inv15, a * inv15 );

				case 6: // #rrggbb
					r = ( val >> 16 );
					g = ( ( val >> 8 ) & 0xFF );
					b = ( val & 0xFF );
					return new Vector4( r * inv255, g * inv255, b * inv255, 1 );

				case 8: // #aarrggbb
					a = ( val >> 24 );
					r = ( ( val >> 16 ) & 0xFF );
					g = ( ( val >> 8 ) & 0xFF );
					b = ( val & 0xFF );
					return new Vector4( r * inv255, g * inv255, b * inv255, a * inv255 );

				default:
					throw new ArgumentException( "Unexpected length of hexadecimal color, it must be 3, 4, 6 or 8 hexadecimal digits" );
			}
		}

		static readonly ColorConverter converter = new ColorConverter();

		/// <summary>Parse string to color with pre-multiplied alpha.</summary>
		public static Vector4 parse( string str )
		{
			if( string.IsNullOrWhiteSpace( str ) )
				throw new ArgumentNullException();
			str = str.Trim();
			if( str[ 0 ] == '#' )
				return parseHex( str );

			SDColor c = (SDColor)converter.ConvertFromString( str );
			return new Vector4( c.R * inv255, c.G * inv255, c.B * inv255, 1 );
		}

		/// <summary>Parse string to color without pre-multiplied alpha.</summary>
		public static Vector4 parseNonPremultiplied( string str )
		{
			if( string.IsNullOrWhiteSpace( str ) )
				throw new ArgumentNullException();
			str = str.Trim();
			if( str[ 0 ] == '#' )
				return parseNonPremultipliedHex( str );

			SDColor c = (SDColor)converter.ConvertFromString( str );
			return new Vector4( c.R * inv255, c.G * inv255, c.B * inv255, 1 );
		}

		/// <summary>Opaque black color</summary>
		public static Vector4 black { get; } = new Vector4( 0, 0, 0, 1 );
		/// <summary>Opaque white color</summary>
		public static Vector4 white { get; } = new Vector4( 1, 1, 1, 1 );
		/// <summary>Completely transparent color</summary>
		public static Vector4 transparent { get; } = Vector4.Zero;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static float hueFromInteger( int id )
		{
			// Convert ID into hue: https://probablydance.com/2018/06/16/fibonacci-hashing-the-optimization-that-the-world-forgot-or-a-better-alternative-to-integer-modulo/
			// 2654435769.4972302964775847713924 = 2^32 / 1.61803398874989484820458683436563811772
			unchecked
			{
				id *= (int)2654435769u;
			}
			return id * ( 6.0f / ( (long)uint.MaxValue + 1 ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static Vector4 makeColor( float r, float g, float b )
		{
			return new Vector4( r, g, b, 1 );
		}

		/// <summary>Create random bright color from integer ID.</summary>
		/// <remarks>Uses Fibonacci hashing to convert ID into hue, then converts HSV to RGB</remarks>
		/// <seealso href="https://probablydance.com/2018/06/16/fibonacci-hashing-the-optimization-that-the-world-forgot-or-a-better-alternative-to-integer-modulo/" />
		public static Vector4 randomColor( int i, float saturation = 1, float value = 1 )
		{
			float hue = hueFromInteger( i );

			// Create RGBA color from HSV
			float hueFloor = MathF.Floor( hue );
			int hi = (int)hueFloor;
			float f = hue - hueFloor;

			// https://stackoverflow.com/a/1626175/126995
			float v = value;
			float p = value * ( 1 - saturation );
			float q = value * ( 1 - f * saturation );
			float t = value * ( 1 - ( 1 - f ) * saturation );

			switch( hi )
			{
				case 0:
				default:
					return makeColor( v, t, p );
				case 1: return makeColor( q, v, p );
				case 2: return makeColor( p, v, t );
				case 3: return makeColor( p, q, v );
				case 4: return makeColor( t, p, v );
				case 5: return makeColor( v, p, q );
			}
		}

		/// <summary>Create more random colors from integer ID.</summary>
		public static Vector4 moreRandomColor( int i )
		{
			float saturation = ( i.GetHashCode() & 3 ) * ( 0.5f / 3.0f ) + 0.5f;
			return randomColor( i, saturation, 1 );
		}

		internal static int colorIndex( this eNamedColor nc )
		{
			return (int)nc;
		}

		internal static int colorIndex( this ConsoleColor cc )
		{
			return (int)cc;
		}
	}
}