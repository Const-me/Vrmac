using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>The Top-Level Element containing the (monolithic) Block structure.</summary>
	public struct Cluster
	{
		/// <summary>Absolute timestamp of the cluster (based on TimestampScale).</summary>
		public readonly ulong timestamp;
		/// <summary>The list of tracks that are not used in that part of the stream. It is useful when using overlay tracks on seeking or to decide what track to use.</summary>
		public readonly SilentTracks silentTracks;
		/// <summary>The Segment Position of the Cluster in the Segment (0 in live streams). It might help to resynchronise offset on damaged streams.</summary>
		public readonly ulong position;
		/// <summary>Size of the previous Cluster, in octets. Can be useful for backward playing.</summary>
		public readonly ulong prevSize;
		/// <summary>Similar to <a href="https://www.matroska.org/technical/basics.html#block-structure">Block</a> but without all the extra information, mostly used to reduced overhead when no extra feature is needed. (see <a
		/// href="https://www.matroska.org/technical/basics.html#simpleblock-structure">SimpleBlock Structure</a>)</summary>
		public readonly Blob[] simpleBlock;
		/// <summary>Basic container of information containing a single Block and information specific to that Block.</summary>
		public readonly BlockGroup[] blockGroup;
		/// <summary>Similar to <a href="https://www.matroska.org/technical/basics.html#simpleblock-structure">SimpleBlock</a> but the data inside the Block are Transformed (encrypt and/or signed).</summary>
		public readonly Blob[] encryptedBlock;

		internal Cluster( Stream stream )
		{
			timestamp = default;
			silentTracks = default;
			position = default;
			prevSize = default;
			simpleBlock = default;
			blockGroup = default;
			encryptedBlock = default;
			List<Blob> simpleBlocklist = null;
			List<BlockGroup> blockGrouplist = null;
			List<Blob> encryptedBlocklist = null;
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
						silentTracks = new SilentTracks( stream );
						break;
					case eElement.Position:
						position = reader.readUlong();
						break;
					case eElement.PrevSize:
						prevSize = reader.readUlong();
						break;
					case eElement.SimpleBlock:
						if( null == simpleBlocklist ) simpleBlocklist = new List<Blob>();
						simpleBlocklist.Add( Blob.read( reader ) );
						break;
					case eElement.BlockGroup:
						if( null == blockGrouplist ) blockGrouplist = new List<BlockGroup>();
						blockGrouplist.Add( new BlockGroup( stream ) );
						break;
					case eElement.EncryptedBlock:
						if( null == encryptedBlocklist ) encryptedBlocklist = new List<Blob>();
						encryptedBlocklist.Add( Blob.read( reader ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( simpleBlocklist != null ) simpleBlock = simpleBlocklist.ToArray();
			if( blockGrouplist != null ) blockGroup = blockGrouplist.ToArray();
			if( encryptedBlocklist != null ) encryptedBlock = encryptedBlocklist.ToArray();
		}
	}
}