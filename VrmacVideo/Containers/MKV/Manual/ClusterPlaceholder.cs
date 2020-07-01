using System;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	public struct ClusterPlaceholder
	{
		/// <summary>Absolute timestamp of the cluster (based on TimestampScale).</summary>
		public readonly ulong timestamp;
		public readonly long segmentPosition;

		internal ClusterPlaceholder( Stream stream, long segmentOffset )
		{
			long filePosition = stream.Position - 4;    // 4 for the cluster header.
			segmentPosition = filePosition - segmentOffset;

			timestamp = default;

			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				bool fastForward = false;
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.Timestamp:
						timestamp = reader.readUlong();
						break;
					default:
						fastForward = true;
						break;
				}
				if( fastForward )
					break;
			}
			stream.Seek( reader.endPosition, SeekOrigin.Begin );
		}

		internal ReusableCluster load( Stream stream, long segmentOffset, ReusableCluster cluster )
		{
			if( null == cluster )
				cluster = new ReusableCluster();
			stream.Seek( segmentOffset + segmentPosition, SeekOrigin.Begin );
			var id = stream.readElementId();
			if( id != eElement.Cluster )
				throw new ApplicationException( $"Expected a cluster, found { id }" );

			cluster.read( stream );
			return cluster;
		}

		/// <summary>Create a fake one with just the timestamp; need this version to avoid re-implementing Array.BinarySearch framework method.</summary>
		internal ClusterPlaceholder( ulong timestamp )
		{
			this.timestamp = timestamp;
			segmentPosition = -1;
		}
	}
}