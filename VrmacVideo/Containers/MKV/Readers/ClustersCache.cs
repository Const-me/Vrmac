using System;
using System.Collections.Generic;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>LRU cache that keeps small number of recently used clusters</summary>
	/// <remarks>Video and audio readers read from different positions in the file, due to the different pace of buffering. When seeking, both need to do that.</remarks>
	class ClustersCache
	{
		// LRU cache capacity
		const int capacity = 4;

		struct Entry
		{
			public readonly LinkedListNode<int> node;
			public readonly ReusableCluster value;

			public Entry( LinkedListNode<int> node, ReusableCluster value )
			{
				this.node = node;
				this.value = value;
			}
		}

		readonly Dictionary<int, Entry> dict = new Dictionary<int, Entry>( capacity );
		readonly LinkedList<int> list = new LinkedList<int>();

		public readonly MkvMediaFile file;
		public readonly int clustersCount;

		public ClustersCache( MkvMediaFile file )
		{
			this.file = file;
			clustersCount = file.segment.cluster.Length;
			timeScaler = TimeScaler.mkv( file.segment.info[ 0 ].timestampScale );
		}

		// Monitor object that guards cache and the file.
		// Should be locked whenever the file is being read.
		// Fortunately, other threads only mess with the file in response to seek, i.e. 99.99% of times these locks will be a very fast noop.
		public readonly object syncRoot = new object();

		/// <summary>Load a cluster by 0-based index</summary>
		public ReusableCluster load( int idx )
		{
			Entry entry;
			if( dict.TryGetValue( idx, out entry ) )
			{
				list.Remove( entry.node );
				list.AddLast( entry.node );
				return entry.value;
			}
			ReusableCluster cluster = null;

			LinkedListNode<int> node;
			if( dict.Count >= capacity )
			{
				// Evicting a cluster from the cache
				node = list.First;
				if( dict.TryGetValue( node.Value, out entry ) )
				{
					cluster = entry.value;
					dict.Remove( node.Value );
				}
				list.RemoveFirst();
			}

			cluster = file.segment.cluster[ idx ].load( file.stream, file.segment.position, cluster );
			node = list.AddLast( idx );
			dict.Add( idx, new Entry( node, cluster ) );
			return cluster;
		}

		public readonly TimeScaler timeScaler;
	}
}