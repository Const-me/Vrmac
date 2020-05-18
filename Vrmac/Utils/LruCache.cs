using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Vrmac
{
	sealed class LruCache<K, V>
	{
		readonly int capacity;

		struct Entry
		{
			public readonly LinkedListNode<K> node;
			public readonly V value;

			public Entry( LinkedListNode<K> node, V value )
			{
				this.node = node;
				this.value = value;
			}
		}

		readonly Dictionary<K, Entry> dict;
		readonly LinkedList<K> list;

		public LruCache( int capacity )
		{
			this.capacity = capacity;
			dict = new Dictionary<K, Entry>();
			list = new LinkedList<K>();
		}

		public V lookup( K key )
		{
			if( dict.TryGetValue( key, out var entry ) )
			{
				list.Remove( entry.node );
				list.AddLast( entry.node );
				return entry.value;
			}
			return default;
		}

		public void add( K key, V val )
		{
			Debug.Assert( !dict.ContainsKey( key ) );

			LinkedListNode<K> node;

			if( dict.Count >= capacity )
			{
				node = list.First;
				if( dict.TryGetValue( node.Value, out Entry entry ) )
				{
					( entry.value as IDisposable )?.Dispose();
					dict.Remove( node.Value );
				}
				list.RemoveFirst();
			}
			node = list.AddLast( key );
			dict.Add( key, new Entry( node, val ) );
		}
	}
}