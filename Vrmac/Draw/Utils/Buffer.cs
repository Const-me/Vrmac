using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Unfortunately, Span and Memory can't grow, List can't be casted to spans.</summary>
	class Buffer<T> where T : struct
	{
		const int defaultCapacity = 256;
		T[] items;

		public Buffer()
		{
			items = new T[ defaultCapacity ];
		}

		public Buffer( int capacity )
		{
			items = new T[ capacity ];
		}

		public Span<T> append( int count )
		{
			if( length + count <= items.Length )
			{
				var res = items.AsSpan().Slice( length, count );
				length += count;
				return res;
			}
			else
			{
				int newSize = ( length + count ).nextPowerOf2();
				Array.Resize( ref items, newSize );

				var res = items.AsSpan().Slice( length, count );
				length += count;
				return res;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int add( ref T val )
		{
			int res = length;
			if( res + 1 <= items.Length )
			{

				items[ res ] = val;
				length = res + 1;
				return res;
			}
			return addWithResize( ref val );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int add( T val )
		{
			int res = length;
			if( res + 1 <= items.Length )
			{

				items[ res ] = val;
				length = res + 1;
				return res;
			}
			return addWithResize( ref val );
		}

		[MethodImpl( MethodImplOptions.NoInlining )]
		int addWithResize( ref T val )
		{
			int res = length;
			int newCapacity = ( res + 1 ).nextPowerOf2();
			Array.Resize( ref items, newCapacity );
			items[ res ] = val;
			length = res + 1;
			return res;
		}

		public int length { get; private set; } = 0;
		public int capacity => items.Length;
		public int sizeInBytes => length * Marshal.SizeOf<T>();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Span<T> read()
		{
			return items.AsSpan().Slice( 0, length );
		}

		public bool empty => length <= 0;

		public void clear() => length = 0;

		public ref T this[ int index ]
		{
			get
			{
#if DEBUG
				if( index >= length )
					throw new ArgumentOutOfRangeException();
#endif
				return ref items[ index ];
			}
		}

		public override string ToString()
		{
			return $"length { length }, capacity { items.Length }";
		}
	}
}