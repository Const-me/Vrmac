using System;

namespace VrmacVideo
{
	/// <summary>A small FIFO queue of bytes</summary>
	struct SmallQueue
	{
		readonly byte[] data;
		int length;

		public SmallQueue( int count, bool isFilled )
		{
			if( count > 0x100 )
				throw new ArgumentOutOfRangeException();

			data = new byte[ count ];
			if( isFilled )
			{
				// Creating a completely filled queue
				for( byte i = 0; i < count; i++ )
					data[ i ] = i;
				length = count;
			}
			else
			{
				// Creating an empty one
				length = 0;
			}
		}

		public void enqueue( int value )
		{
			// No need to check for overflow, arrays in .NET already do bound checking
			data[ length ] = checked((byte)value);
			length++;
		}

		public bool any => length > 0;

		public int dequeue()
		{
			if( length <= 0 )
				throw new ApplicationException( "The queue is empty" );
			int result = data[ 0 ];
			if( length > 1 )
			{
				// Let's hope JIT will turn this into memmove CRT call
				Array.Copy( data, 1, data, 0, length - 1 );
			}
			length--;
			return result;
		}

		public int first
		{
			get
			{
				if( length > 0 )
					return data[ 0 ];
				throw new ApplicationException( "The queue is empty" );
			}
		}
	}
}