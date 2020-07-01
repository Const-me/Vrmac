using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Clusters have quite a few stuff inside. We don’t want to new/delete them while playing the video.
	/// This class is functionally equivalent to <see cref="Cluster" /> (copy-pasted from there) but it's mutable, allowing it to be re-initialized from another MKV cluster.</summary>
	class ReusableCluster
	{
		/// <summary>Absolute timestamp of the cluster (based on TimestampScale).</summary>
		public ulong timestamp;
		/// <summary>The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking or to decide what track to use.</summary>
		public List<ulong> silentTracks = null;
		/// <summary>The Segment Position of the Cluster in the Segment (0 in live streams). It might help to resynchronise offset on damaged streams.</summary>
		public ulong position;
		/// <summary>Size of the previous Cluster, in octets. Can be useful for backward playing.</summary>
		public ulong prevSize;

		// Blob and BlockGroup are structures, not classes. Clearing and re-populating lists with them doesn't cause GC allocations, as long as the new length is not much longer.

		/// <summary>Similar to <see cref="blockGroup" /> but without all the extra information, mostly used to reduced overhead when no extra feature is needed.</summary>
		/// <seealso href="https://www.matroska.org/technical/basics.html#simpleblock-structure" />
		public List<Blob> simpleBlock = null;

		/// <summary>Basic container of information containing a single Block and information specific to that Block.</summary>
		public List<BlockGroup> blockGroup = null;

		/// <summary>Similar to <see cref="simpleBlock" /> but the data inside the Block are Transformed (encrypt and/or signed).</summary>
		public List<Blob> encryptedBlock = null;

		void readSilentTracks( Stream stream, ref List<ulong> list )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.SilentTrackNumber:
						if( null == list )
							list = new List<ulong>();
						list.Add( reader.readUlong() );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}

		internal void read( Stream stream )
		{
			timestamp = default;
			silentTracks?.Clear();
			position = default;
			prevSize = default;
			simpleBlock?.Clear();
			blockGroup?.Clear();
			encryptedBlock?.Clear();

			ElementReader reader = new ElementReader( stream );

			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.Timestamp:
						timestamp = reader.readUlong();
						break;
					case eElement.SilentTracks:
						readSilentTracks( stream, ref silentTracks );
						break;
					case eElement.Position:
						position = reader.readUlong();
						break;
					case eElement.PrevSize:
						prevSize = reader.readUlong();
						break;
					case eElement.SimpleBlock:
						if( null == simpleBlock )
							simpleBlock = new List<Blob>();
						simpleBlock.Add( Blob.read( reader ) );
						break;
					case eElement.BlockGroup:
						if( null == blockGroup )
							blockGroup = new List<BlockGroup>();
						blockGroup.Add( new BlockGroup( stream ) );
						break;
					case eElement.EncryptedBlock:
						if( null == encryptedBlock )
							encryptedBlock = new List<Blob>();
						encryptedBlock.Add( Blob.read( reader ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}