using System;
using System.Linq.Expressions;
using System.Reflection;

namespace VrmacVideo
{
	struct TimeScaler
	{
		static ulong greatestCommonDivisor( ulong a, ulong b )
		{
			// https://stackoverflow.com/a/41766138/
			while( a != 0 && b != 0 )
			{
				if( a > b )
					a %= b;
				else
					b %= a;
			}
			return a == 0 ? b : a;
		}

		public readonly Func<long, TimeSpan> convert;

		readonly long mulTime, divTime;

		// The formula used: result.Ticks = input * mul / div
		TimeScaler( ulong mul, ulong div )
		{
			ulong gcd = greatestCommonDivisor( mul, div );
			mulTime = (long)( mul / gcd );
			divTime = (long)( div / gcd );

			ParameterExpression eParam = Expression.Parameter( typeof( long ) );

			Expression eFunc = Expression.Multiply( eParam, Expression.Constant( mulTime ) );
			if( divTime > 1 )
				eFunc = Expression.Divide( eFunc, Expression.Constant( divTime ) );

			MethodInfo mi = typeof( TimeSpan ).GetMethod( nameof( TimeSpan.FromTicks ) );
			eFunc = Expression.Call( mi, eFunc );

			convert = Expression.Lambda<Func<long, TimeSpan>>( eFunc, eParam ).Compile();
		}

		public static TimeScaler mkv( ulong scaleMkv )
		{
			// https://www.matroska.org/technical/elements.html, TimestampScale:
			// Timestamp scale in nanoseconds (1.000.000 means all timestamps in the Segment are expressed in milliseconds).

			// TimeSpan tick = 100 nanoseconds.
			return new TimeScaler( scaleMkv, 100 );
		}

		public ulong convertBack( TimeSpan ts )
		{
			return (ulong)( ts.Ticks * divTime / mulTime );
		}
	}
}