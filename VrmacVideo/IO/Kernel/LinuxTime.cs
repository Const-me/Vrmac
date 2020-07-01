using System;

namespace VrmacVideo
{
	static class LinuxTime
	{
		static readonly DateTime epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

		public static DateTime getTime( this TimeSpan ts ) => epoch + ts;

		public static DateTime getLocalTime( this TimeSpan ts ) => ( epoch + ts ).ToLocalTime();

		public static TimeSpan linuxTime( this DateTime dt )
		{
			switch( dt.Kind )
			{
				case DateTimeKind.Utc:
				case DateTimeKind.Unspecified:
					return dt - epoch;
				case DateTimeKind.Local:
					return dt.ToUniversalTime() - epoch;
			}
			throw new ArgumentException();
		}
	}

	// Linux developers failed to agree on even something as basic as consistent date + time formats.
	// Different structures in V4L2 use `struct timeval` in microseconds and `struct timespec` in nanoseconds.

	/// <summary>struct timeval in C++, microseconds</summary>
	public struct sTimeMicro
	{
		int seconds, microseconds;

		public static implicit operator TimeSpan( sTimeMicro tm )
		{
			long ticks = tm.seconds;
			ticks *= TimeSpan.TicksPerSecond;
			ticks += tm.microseconds * 10;
			return TimeSpan.FromTicks( ticks );
		}

		public static implicit operator sTimeMicro( TimeSpan value )
		{
			long ticks = value.Ticks;
			sTimeMicro tm = default;
			// AMD64 integer divide instructions compute both a / b and a % b in 1 shot.
			// On all platforms, compilers optimize division by constexpr with multiply + shift, the remainder is multiply + subtract
			tm.seconds = (int)( ticks / TimeSpan.TicksPerSecond );
			int remainder = (int)( ticks % TimeSpan.TicksPerSecond );
			tm.microseconds = remainder / 10;
			return tm;
		}

		public DateTime dateTime => ( (TimeSpan)this ).getLocalTime();

		public override string ToString() => dateTime.ToString( "HH:mm:ss.ffffff" );
	}

	/// <summary>struct timespec in C++, nanoseconds</summary>
	public struct sTimeNano
	{
		int seconds, nanoseconds;

		public static implicit operator TimeSpan( sTimeNano nano )
		{
			long ticks = nano.seconds;
			ticks *= TimeSpan.TicksPerSecond;
			ticks += nano.nanoseconds / 100;
			return TimeSpan.FromTicks( ticks );
		}

		public static implicit operator sTimeNano( TimeSpan value )
		{
			long ticks = value.Ticks;
			sTimeNano nano = default;
			nano.seconds = (int)( ticks / TimeSpan.TicksPerSecond );
			int remainder = (int)( ticks % TimeSpan.TicksPerSecond );
			nano.nanoseconds = remainder * 100;
			return nano;
		}

		public DateTime dateTime => ( (TimeSpan)this ).getLocalTime();

		public override string ToString() => dateTime.ToString( "HH:mm:ss.ffffff" );
	}
}