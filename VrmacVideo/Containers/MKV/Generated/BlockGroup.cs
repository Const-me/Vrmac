using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Basic container of information containing a single Block and information specific to that Block.</summary>
	public struct BlockGroup
	{
		/// <summary>Block containing the actual data to be rendered and a timestamp relative to the Cluster Timestamp. (see <a href="https://www.matroska.org/technical/basics.html#block-structure">Block Structure</a>)</summary>
		public readonly Blob block;
		/// <summary>A Block with no data. It MUST be stored in the stream at the place the real Block would be in display order. (see <a href="https://www.matroska.org/technical/elements.html#BlockVirtual">Block Virtual</a>)</summary>
		public readonly Blob blockVirtual;
		/// <summary>Contain additional blocks to complete the main one. An EBML parser that has no knowledge of the Block structure could still see and use/skip these data.</summary>
		public readonly BlockAdditions blockAdditions;
		/// <summary>The duration of the Block (based on TimestampScale). The BlockDuration Element can be useful at the end of a Track to define the duration of the last frame (as there is no subsequent Block available), or when there is a
		/// break in a track like for subtitle tracks.</summary>
		public readonly ulong blockDuration;
		/// <summary>This frame is referenced and has the specified cache priority. In cache only a frame of the same or higher priority can replace this frame. A value of 0 means the frame is not referenced.</summary>
		public readonly ulong referencePriority;
		/// <summary>Timestamp of another frame used as a reference (ie: B or P frame). The timestamp is relative to the block it's attached to.</summary>
		public readonly int[] referenceBlock;
		/// <summary>The Segment Position of the data that would otherwise be in position of the virtual block.</summary>
		public readonly int? referenceVirtual;
		/// <summary>The new codec state to use. Data interpretation is private to the codec. This information SHOULD always be referenced by a seek entry.</summary>
		public readonly byte[] codecState;
		/// <summary>Duration in nanoseconds of the silent data added to the Block (padding at the end of the Block for positive value, at the beginning of the Block for negative value). The duration of DiscardPadding is not calculated in
		/// the duration of the TrackEntry and SHOULD be discarded during playback.</summary>
		public readonly int? discardPadding;
		/// <summary>Contains slices description.</summary>
		public readonly Slices slices;
		/// <summary><a href="http://labs.divx.com/node/16601">DivX trick track extensions</a></summary>
		public readonly ReferenceFrame referenceFrame;

		internal BlockGroup( Stream stream )
		{
			block = default;
			blockVirtual = default;
			blockAdditions = default;
			blockDuration = default;
			referencePriority = 0;
			referenceBlock = default;
			referenceVirtual = default;
			codecState = default;
			discardPadding = default;
			slices = default;
			referenceFrame = default;
			List<int> referenceBlocklist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.Block:
						block = Blob.read( reader );
						break;
					case eElement.BlockVirtual:
						blockVirtual = Blob.read( reader );
						break;
					case eElement.BlockAdditions:
						blockAdditions = new BlockAdditions( stream );
						break;
					case eElement.BlockDuration:
						blockDuration = reader.readUlong();
						break;
					case eElement.ReferencePriority:
						referencePriority = reader.readUlong( 0 );
						break;
					case eElement.ReferenceBlock:
						if( null == referenceBlocklist ) referenceBlocklist = new List<int>();
						referenceBlocklist.Add( reader.readInt() );
						break;
					case eElement.ReferenceVirtual:
						referenceVirtual = reader.readInt();
						break;
					case eElement.CodecState:
						codecState = reader.readByteArray();
						break;
					case eElement.DiscardPadding:
						discardPadding = reader.readInt();
						break;
					case eElement.Slices:
						slices = new Slices( stream );
						break;
					case eElement.ReferenceFrame:
						referenceFrame = new ReferenceFrame( stream );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( referenceBlocklist != null ) referenceBlock = referenceBlocklist.ToArray();
		}
	}
}