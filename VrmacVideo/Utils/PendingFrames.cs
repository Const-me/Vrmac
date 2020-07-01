using System;
using System.Runtime.CompilerServices;

namespace VrmacVideo
{
	/// <summary>A collection for decoded video frames, orders them by increasing timestamps.</summary>
	/// <remarks>
	/// <para>The sorting’s stable, two frames with the same timestamp will maintain their relative order.</para>
	/// <para>This thing is required because video frames in containers are in decoding order, different from playback order.
	/// It’s quite common to see frames like 1,3,2 in the container, which must be played in 1,2,3 order.
	/// Happens when the encoder decides #2 should depend on #3, and flips them in the compressed stream.</para>
	/// </remarks>
	struct PendingFrames
	{
		// Unfortunately, SortedList<T> disallows duplicate keys and we need them here.
		// Fortunately, .NET is open source with MIT license, copy-pasted couple things from over there:
		// https://source.dot.net/#System.Collections/System/Collections/Generic/SortedList.cs,7d1685ab0a3aa069

		TimeSpan[] keys;
		int[] values;
		int _size;

		public PendingFrames( int capacity )
		{
			keys = new TimeSpan[ capacity ];
			values = new int[ capacity ];
			_size = 0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void insert( TimeSpan key, int value )
		{
			if( _size == keys.Length )
				throw new ApplicationException( $"Too many decoded video frames, the capacity is { keys.Length }" );

			int i = Array.BinarySearch( keys, 0, _size, key );
			if( i < 0 )
			{
				// bitwise complement of the index of the first element that is larger than value
				i = ~i;
			}
			else
			{
				// The index of the specified value in the specified array, if value is found.
				// We want the new frame to be the next one, to maintain the order.
				i++;
			}

			if( i < _size )
			{
				Array.Copy( keys, i, keys, i + 1, _size - i );
				Array.Copy( values, i, values, i + 1, _size - i );
			}
			keys[ i ] = key;
			values[ i ] = value;
			_size++;
		}

		public int capacity => keys?.Length ?? 0;

		public bool any => _size > 0;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public TimeSpan firstTimestamp()
		{
			if( _size <= 0 )
				throw new ApplicationException( "No decoded frames are available at the moment." );
			return keys[ 0 ];
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int removeFirst()
		{
			if( _size <= 0 )
				throw new ApplicationException( "No decoded frames are available at the moment." );
			int result = values[ 0 ];
			Array.Copy( keys, 1, keys, 0, _size - 1 );
			Array.Copy( values, 1, values, 0, _size - 1 );
			_size--;
			return result;
		}

		public ReadOnlySpan<int> Values => values.AsSpan().Slice( 0, _size );

		public void clear() => _size = 0;
	}
}