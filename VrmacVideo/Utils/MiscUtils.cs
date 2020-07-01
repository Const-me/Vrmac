using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Vrmac.Utils;
using VrmacVideo.Containers.MP4;

namespace VrmacVideo
{
	static class MiscUtils
	{
		public static IEnumerable<E> enumValues<E>() where E : struct
		{
			foreach( E e in Enum.GetValues( typeof( E ) ) )
				yield return e;
		}

		public static E first<E>( this IEnumerable<E> list, Func<E, bool> pred, string errorMessage )
		{
			foreach( E e in list )
			{
				if( pred( e ) )
					return e;
			}
			throw new ApplicationException( errorMessage );
		}

		public static string makeLines( this IEnumerable<string> lines )
		{
			return string.Join( '\n', lines );
		}

		public static iSimdUtils simd;

		/// <summary>Convert timestamp from weird media timescale into normal time units.</summary>
		public static TimeSpan timeFromTrack( long scaled, uint timeScale )
		{
			checked
			{
				long ticks = scaled * TimeSpan.TicksPerSecond;
				ticks /= timeScale;
				return TimeSpan.FromTicks( ticks );
			}
		}

		/// <summary>Drop the least significant 3.3219 bits of precision from the timestamp.</summary>
		/// <remarks>That precision drops occurs when samples are decoded.
		/// V4L2 uses microseconds (µs) for sample timestamps who could really use these couple extra bits of precision.
		/// And it’s using nanoseconds for events which don’t really need timing that accurate. Strange choice.</remarks>
		public static TimeSpan floorToMicro( this TimeSpan ts )
		{
			long micro = ts.Ticks / 10;
			return TimeSpan.FromTicks( micro * 10 );
		}

		public static string print( this TimeSpan ts )
		{
			if( ts.Ticks < 0 || ts.TotalHours > 24 )
				throw new ArgumentOutOfRangeException();
			return ts.ToString( @"h\:mm\:ss\.fffffff" );
		}

		/// <summary>Lowest 5 bits of the byte, casted to the enum</summary>
		public static eNaluType getNaluType( byte b )
		{
			// https://yumichan.net/video-processing/video-compression/introduction-to-h264-nal-unit/
			return (eNaluType)( b & 0x1F );
		}

		internal static ReadOnlySpan<T> cast<T>( this ReadOnlySpan<byte> span ) where T : unmanaged
		{
			return MemoryMarshal.Cast<byte, T>( span );
		}

		public static void writeBinaryText( string path, ReadOnlySpan<byte> bytes )
		{
			using( var f = File.CreateText( path ) )
				for( int i = 0; i < bytes.Length; i++ )
				{
					if( i != 0 )
						f.Write( ( 0 == i % 16 ) ? "\r\n" : " " );
					string bin = Convert.ToString( bytes[ i ], 2 ).PadLeft( 8, '0' );
					f.Write( bin );
				}
		}
	}
}