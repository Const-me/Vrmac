using Diligent.Graphics;
using System;
using System.Numerics;
using Vrmac.Draw;

namespace Diligent
{
	/// <summary>Miscellaneous utilities for native interop</summary>
	public static class Unsafe
	{
		/// <summary>Create a span to read or write native memory</summary>
		public static Span<T> writeSpan<T>( IntPtr pointer, int count ) where T : unmanaged
		{
			unsafe
			{
				T* p = (T*)pointer;
				return new Span<T>( p, count );
			}
		}

		/// <summary>Create span to read native memory</summary>
		public static ReadOnlySpan<T> readSpan<T>( IntPtr pointer, int count ) where T : unmanaged
		{
			unsafe
			{
				T* p = (T*)pointer;
				return new ReadOnlySpan<T>( p, count );
			}
		}

		/// <summary>Copy elements to native memory</summary>
		public static void copy<T>( ref T destReference, ReadOnlySpan<T> source ) where T : unmanaged
		{
			unsafe
			{
				fixed ( T* destPointer = &destReference )
				{
					Span<T> destSpan = new Span<T>( destPointer, source.Length );
					source.CopyTo( destSpan );
				}
			}
		}

		/// <summary>Append ID to 2D vectors and copy to native memory</summary>
		public static void copyWithId( ref sVertexWithId destReference, ReadOnlySpan<Vector2> source, uint id )
		{
			unsafe
			{
				fixed ( sVertexWithId* destPointer = &destReference )
				{
					int length = source.Length;
					Span<sVertexWithId> destSpan = new Span<sVertexWithId>( destPointer, length );
					for( int i = 0; i < length; i++ )
					{
						destSpan[ i ].position = source[ i ];
						destSpan[ i ].id = id;
					}
				}
			}
		}
	}
}