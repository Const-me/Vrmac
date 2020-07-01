using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

static class MiscUtils
{
	public static string print( this Guid guid )
	{
		// https://docs.microsoft.com/en-us/dotnet/api/system.guid.tostring?view=netframework-4.8
		return guid.ToString( "D" ).ToLowerInvariant();
	}

	public static bool isEmpty<T>( this T[] arr )
	{
		return null == arr || arr.Length <= 0;
	}

	public static IEnumerable<string> readLines( string path )
	{
		using( var r = File.OpenText( path ) )
			while( true )
			{
				string line = r.ReadLine();
				if( null == line )
					yield break;
				yield return line;
			}
	}

	public static V lookup<K, V>( this Dictionary<K, V> dict, K key )
	{
		if( null == dict )
			return default;
		if( dict.TryGetValue( key, out V val ) )
			return val;
		return default;
	}

	public static bool notEmpty( this string s )
	{
		return !string.IsNullOrWhiteSpace( s );
	}
	public static bool isEmpty( this string s )
	{
		return string.IsNullOrWhiteSpace( s );
	}

	public static IEnumerable<int> enumSetBits( this uint v )
	{
		if( 0 == v )
			yield break;
		for( int i = 0; v != 0; i++, v = v >> 1 )
		{
			if( 0 != ( v & 1 ) )
				yield return i;
		}
	}

	public static IEnumerable<int> enumSetBits( this uint[] v )
	{
		if( v.isEmpty() )
			yield break;

		for( int idxWord = 0; idxWord < v.Length; idxWord++ )
		{
			int idxBit = idxWord * 32;
			foreach( int bit in v[ idxWord ].enumSetBits() )
				yield return idxBit + bit;
		}
	}

	public static bool equals( this string a, string b )
	{
		return a.Equals( b, StringComparison.InvariantCultureIgnoreCase );
	}
	public static bool notEquals( this string a, string b )
	{
		return !a.Equals( b, StringComparison.InvariantCultureIgnoreCase );
	}
	public static bool startsWith( this string a, string b )
	{
		return a.StartsWith( b, StringComparison.InvariantCultureIgnoreCase );
	}
	public static bool endsWith( this string a, string b )
	{
		return a.EndsWith( b, StringComparison.InvariantCultureIgnoreCase );
	}

	public static string group( this Match m, int idx = 0 )
	{
		return m.Groups[ idx + 1 ].Value;
	}
	public static int indexAfter( this Match m )
	{
		Debug.Assert( m.Success );
		return m.Index + m.Length;
	}

	public static string print( this TimeSpan ts )
	{
		if( ts.Ticks < 0 )
			return "negative";
		if( ts.Ticks == 0 )
			return "zero";
		if( ts.Ticks < TimeSpan.TicksPerSecond )
			return string.Format( "{0:G3} ms", ts.TotalMilliseconds );
		if( ts.Ticks < TimeSpan.TicksPerMinute )
			return string.Format( "{0:G3} seconds", ts.TotalSeconds );
		if( ts.Ticks < TimeSpan.TicksPerHour )
			return ts.ToString( "mm' minutes 'ss' seconds'" );
		return ts.ToString( "g" );
	}

	public static Span<byte> asSpan<T>( ref T val ) where T : unmanaged
	{
		// https://stackoverflow.com/a/50406304/126995
		Span<T> valSpan = MemoryMarshal.CreateSpan( ref val, 1 );
		return MemoryMarshal.Cast<T, byte>( valSpan );
	}

	public static Span<To> cast<From, To>( this Span<From> span ) where To : unmanaged where From : unmanaged
	{
		return MemoryMarshal.Cast<From, To>( span );
	}
	public static Span<To> cast<From, To>( this Span<From> span, int start, int length ) where To : unmanaged where From : unmanaged
	{
		return MemoryMarshal.Cast<From, To>( span.Slice( start, length ) );
	}
	public static ReadOnlySpan<To> cast<From, To>( this ReadOnlySpan<From> span ) where To : unmanaged where From : unmanaged
	{
		return MemoryMarshal.Cast<From, To>( span );
	}
	public static ReadOnlySpan<To> cast<From, To>( this ReadOnlySpan<From> span, int start, int length ) where To : unmanaged where From : unmanaged
	{
		return MemoryMarshal.Cast<From, To>( span.Slice( start, length ) );
	}
	/// <summary>Just a = b but without the type checks</summary>
	public static void copy<A, B>( ref A a, ref B b ) where A : unmanaged where B : unmanaged
	{
		Span<byte> sourceBytes = asSpan( ref b );
		Span<byte> destBytes = asSpan( ref a );
		sourceBytes.CopyTo( destBytes );
	}

	public static void read<T>( this Stream stm, T[] arr ) where T : unmanaged
	{
		var span = MemoryMarshal.Cast<T, byte>( arr.AsSpan() );
		if( span.Length != stm.Read( span ) )
			throw new EndOfStreamException();
	}

	public static T read<T>( this Stream stream ) where T : unmanaged
	{
		T result = new T();
		var span = asSpan( ref result );
		if( span.Length != stream.Read( span ) )
			throw new EndOfStreamException();
		return result;
	}
	public static void rewind( this Stream s )
	{
		s.Seek( 0, SeekOrigin.Begin );
	}

	/// <summary>Index of the element with minimum key</summary>
	public static int minIndex<T, Key>( this IEnumerable<T> list, Func<T, Key> selector ) where Key : IComparable<Key>
	{
		int i = 0;
		Key bestKey = default;
		int bestIndex = -1;
		foreach( var e in list )
		{
			if( 0 == i )
			{
				bestIndex = 0;
				bestKey = selector( e );
			}
			else
			{
				Key k = selector( e );
				if( k.CompareTo( bestKey ) < 0 )
				{
					bestKey = k;
					bestIndex = i;
				}
			}
			i++;
		}
		return bestIndex;
	}

	public static Span<T> castSpan<T>( this Span<byte> bytes ) where T : unmanaged
	{
		return MemoryMarshal.Cast<byte, T>( bytes );
	}

	public static IEnumerable<TKey> keys<TKey, TValue>( this ConditionalWeakTable<TKey, TValue> table )
		where TKey : class
		where TValue : class
	{
		foreach( var kvp in table )
			yield return kvp.Key;
	}

	public static IEnumerable<TValue> values<TKey, TValue>( this ConditionalWeakTable<TKey, TValue> table )
		where TKey : class
		where TValue : class
	{
		foreach( var kvp in table )
			yield return kvp.Value;
	}

	/// <summary>Next power of 2 of the number. The argument is expected to be a positive number.</summary>
	public static int nextPowerOf2( this int v )
	{
		Debug.Assert( v > 0 );
		v--;
		v |= v >> 1;
		v |= v >> 2;
		v |= v >> 4;
		v |= v >> 8;
		v |= v >> 16;
		v++;
		return v;
	}

	/// <summary>Pack bytes</summary>
	public static ushort combine( this byte low, byte high )
	{
		return (ushort)( low | (int)high << 8 );
	}
	/// <summary>Pack bytes</summary>
	public static int combine( byte a, byte b, byte c )
	{
		return a | (int)b << 8 | (int)c << 16;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static T createOnFirstUse<T>( ref T cachedValue ) where T : class, new()
	{
		if( null != cachedValue )
			return cachedValue;
		return createOnFirstUseImpl( ref cachedValue );
	}

	static readonly object createOnFirstUseLock = new object();

	[MethodImpl( MethodImplOptions.NoInlining )]
	static T createOnFirstUseImpl<T>( ref T cachedValue ) where T : class, new()
	{
		lock( createOnFirstUseLock )
		{
			if( null != cachedValue )
				return cachedValue;
			cachedValue = new T();
			return cachedValue;
		}
	}

	// https://www.h-schmidt.net/FloatConverter/IEEE754.html
	public const int one = 0x3f800000;
}