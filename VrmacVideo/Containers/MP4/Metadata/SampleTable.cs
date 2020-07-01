using System;
using System.Diagnostics;

namespace VrmacVideo.Containers.MP4
{
	public abstract class SampleEntry { }

	/// <summary>Sample table box, container for the time/space map, parsed from <see cref="eBoxType.stbl" /></summary>
	/// <remarks>ISO/IEC 14496-12</remarks>
	public struct SampleTable
	{
		public SampleEntry[] entries;

		/// <summary>Presentation time for these samples, parsed from <see cref="eBoxType.stts" /></summary>
		/// <remarks>Allows indexing from decoding time to sample number, or vice versa.</remarks>
		public readonly sTimeToSampleEntry[] timeToSample;

		/// <summary>Samples within the media data are grouped into chunks. Chunks can be of different sizes, and the samples within a chunk can have different sizes.
		/// This table can be used to find the chunk that contains a sample, its position, and the associated sample description.</summary>
		public readonly sSampleToChunkEntry[] sampleToChunk;

		/// <summary>Sample count, and a table with size in bytes of each sample</summary>
		public readonly SampleSizeTable sampleSize;

		/// <summary>Index of every chunk in the file. Offsets are file offsets, not the offset into any box within the file.</summary>
		public readonly ChunkOffsetTable chunkOffset;

		/// <summary>Composition time to sample table is optional, must only be present if decoding time and composition time differ for any samples.</summary>
		public readonly CompositionTimeDeltas compositionToSample;

		/// <summary>Sync sample table, parsed from <see cref="eBoxType.stss" /> box</summary>
		/// <remarks>The samples indices in that table are 1-based instead of 0-based.
		/// The table provides a compact marking of the sync samples within the stream.
		/// The table is arranged in strictly increasing order of sample number.
		/// If this field is null, every sample in the track is a sync sample.</remarks>
		readonly uint[] syncSampleTable;

		internal SampleTable( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stbl );
			entries = null;
			timeToSample = null;
			sampleToChunk = null;
			sampleSize = null;
			chunkOffset = null;
			compositionToSample = null;
			syncSampleTable = null;

			foreach( eBoxType boxType in reader.readChildren() )
			{
				switch( boxType )
				{
					case eBoxType.stsd:
						entries = parseSampleTable( reader );
						break;
					case eBoxType.stts:
						timeToSample = TimingTables.readTimeToSample( reader );
						break;
					case eBoxType.ctts:
						compositionToSample = CompositionTimeDeltas.read( reader );
						break;
					case eBoxType.stsc:
						sampleToChunk = TimingTables.readSampleToChunk( reader );
						break;
					case eBoxType.stsz:
						sampleSize = TimingTables.readSampleSize( reader );
						break;
					case eBoxType.stz2:
						sampleSize = TimingTables.readSampleSizeCompact( reader );
						break;
					case eBoxType.stco:
						chunkOffset = TimingTables.readOffsets32( reader );
						break;
					case eBoxType.co64:
						chunkOffset = TimingTables.readOffsets64( reader );
						break;
					case eBoxType.stss:
						syncSampleTable = TimingTables.readSyncSample( reader );
						break;
					default:
						reader.skipCurrentBox();
						break;
				}
			}
		}

		static int parseSampleTableHeader( Mp4Reader reader )
		{
			Span<byte> span = stackalloc byte[ 8 ];
			reader.read( span );
			return BitConverter.ToInt32( span.Slice( 4 ) ).endian();
		}

		static SampleEntry[] parseSampleTable( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stsd );

			Span<byte> span = stackalloc byte[ 8 ];
			reader.read( span );

			int entryCount = BitConverter.ToInt32( span.Slice( 4 ) ).endian();
			SampleEntry[] result = new SampleEntry[ entryCount ];
			for( int i = 0; i < entryCount; i++ )
			{
				reader.read( span );
				int length = BitConverter.ToInt32( span ).endian();
				uint code = BitConverter.ToUInt32( span.Slice( 4 ) );
				switch( code )
				{
					case (uint)eFileType.avc1:
						result[ i ] = new AVC1SampleEntry( reader, length - 8 );
						break;
					case (uint)eAudioBoxType.mp4a:
						result[ i ] = new MP4AudioSampleEntry( reader, length - 8 );
						break;
					case (uint)eAudioBoxType.ac3:
						result[ i ] = new Ac3AudioSampleEntry( reader, length - 8 );
						break;
					default:
						throw new NotImplementedException( "The sample format is not currently supported" );
				}
			}
			return result;
		}

		public SampleEntry firstEntry => ( null != entries && entries.Length > 0 ) ? entries[ 0 ] : null;

		public int maxBytesInFrame
		{
			get
			{
				if( firstEntry is VideoSampleEntry vse )
					return Math.Max( vse.maxBytesInFrame, sampleSize.maxSampleSize );
				return sampleSize.maxSampleSize;
			}
		}

		/// <summary>Find a sync. sample index at or before the provided index.</summary>
		/// <param name="index">0-based index of the sample</param>
		/// <returns>Zero based index of the key frame sample, always ≤ of the argument</returns>
		public int findSyncSample( int index )
		{
			if( index < 0 )
				throw new ArgumentOutOfRangeException();

			if( null == syncSampleTable )
			{
				// Not an error, every sample in the track is a sync sample. Happens in practice for audio tracks.
				return index;
			}

			int bsr = Array.BinarySearch( syncSampleTable, (uint)( index + 1 ) );
			if( bsr < 0 )
			{
				// The negative number returned is the bitwise complement of the index of the first element that is larger than value.
				int indexLarger = ~bsr;
				int indexSmaller = indexLarger - 1;
				if( indexSmaller < 0 )
					return 0;

				indexSmaller = Math.Min( indexSmaller, syncSampleTable.Length - 1 );
				return (int)syncSampleTable[ indexSmaller ] - 1;
			}
			else
			{
				// The index of the specified value in the specified array
				return index;
			}
		}
	}
}