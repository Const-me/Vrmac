using System;
using System.Collections.Generic;

/// <summary>Implements couple miscellaneous utility functions.</summary>
public static class VrmacMiscUtils
{
	/// <summary>Index of the first element matching the predicate, or -1 when not found</summary>
	public static int findIndex<T>( this IEnumerable<T> list, Func<T, bool> predicate )
	{
		int i = 0;
		foreach( var e in list )
		{
			if( predicate( e ) )
				return i;
			i++;
		}
		return -1;
	}

	/// <summary>Convert Unix epoch nanoseconds into DateTime from .NET runtime</summary>
	public static DateTime timeFromUnixNano( this ulong nano )
	{
		long ticks = (long)( nano / 100 );
		ticks += DateTime.UnixEpoch.Ticks;
		return new DateTime( ticks, DateTimeKind.Utc );
	}
}