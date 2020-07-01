using System;
using System.Collections.Generic;
using System.Linq;

namespace VrmacVideo.Containers.MKV
{
	static class FindSeekPosition
	{
		sealed class ClusterCompare: IComparer<ClusterPlaceholder>
		{
			int IComparer<ClusterPlaceholder>.Compare( ClusterPlaceholder x, ClusterPlaceholder y ) =>
				x.timestamp.CompareTo( y.timestamp );
		}
		static readonly ClusterCompare clusterCompare = new ClusterCompare();

		struct sSeekPos
		{
			public ulong time;
			public int cluster, blob;
		}

		/// <summary>Sequence of blobs in the cluster, with sequence numbers</summary>
		static IEnumerable<sSeekPos> listBlobsInCluster( ReusableCluster cluster, ulong trackNumber, int clusterIdx )
		{
			sSeekPos result = default;
			result.cluster = clusterIdx;
			result.blob = 0;
			long time = (long)cluster.timestamp;

			// The filtering code needs to match what's in ReaderBase.loadCluster or this gonna fail, miserably.
			if( null != cluster.simpleBlock )
			{
				foreach( var b in cluster.simpleBlock )
				{
					if( b.trackNumber != trackNumber )
						continue;
					result.time = checked((ulong)( time + b.timestamp ));
					yield return result;
					result.blob++;
				}
			}
			if( null != cluster.blockGroup )
			{
				foreach( var b in cluster.blockGroup )
				{
					if( b.block.trackNumber != trackNumber )
						continue;
					result.time = checked((ulong)( time + b.block.timestamp ));
					result.blob++;
				}
			}
		}

		static IEnumerable<sSeekPos> listBlobsInClusters( ClustersCache cache, ulong trackNumber, int idx, int count = 3 )
		{
			int endIdx = Math.Min( idx + count, cache.file.segment.cluster.Length );
			for( ; idx < endIdx; idx++ )
			{
				lock( cache.syncRoot )
					cache.file.loadCluster( idx, tempCluster );
				foreach( var sp in listBlobsInCluster( tempCluster, trackNumber, idx ) )
					yield return sp;
			}
		}

		static sSeekPos? nullable( sSeekPos sp ) => sp;

		// A temp cluster used for search, only used by GUI thread.
		static ReusableCluster tempCluster = null;

		public static MkvSeekPosition find( ClustersCache cache, TimeSpan where, ulong trackNumber )
		{
			ulong searchingTime = cache.timeScaler.convertBack( where );

			int idx = Array.BinarySearch( cache.file.segment.cluster, new ClusterPlaceholder( searchingTime ), clusterCompare );
			if( idx < 0 )
				idx = ( ~idx ) - 1;

			if( idx < 0 )
				return new MkvSeekPosition( TimeSpan.Zero, 0, 0 );

			if( null == tempCluster )
				tempCluster = new ReusableCluster();

			Logger.logDebug( "FindSeekPosition.find, looking for {0}, scaled {1}, starting from {2}", where, searchingTime, idx );

			// Create a new reusable one
			ReusableCluster rc = new ReusableCluster();
			sSeekPos? blob = listBlobsInClusters( cache, trackNumber, idx )
				.Where( sp => sp.time <= searchingTime )
				.Select( nullable )
				.LastOrDefault();

			if( !blob.HasValue )
				throw new ApplicationException( "Seek failed, might be trying to seek past the end" );

			TimeSpan time = cache.timeScaler.convert( (long)blob.Value.time );
			return new MkvSeekPosition( time, blob.Value.cluster, blob.Value.blob );
		}
	}
}