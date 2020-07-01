using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using VrmacVideo.Containers.MP4;

namespace VrmacVideo
{
	/// <summary>Using numerous lookup tables from MP4 header, iterate over samples in the file.</summary>
	/// <remarks>Reused for both audio and video streams.</remarks>
	sealed partial class SampleReader
	{
		readonly SampleSizeTable sampleSize;
		readonly sTimeToSampleEntry[] timeToSampleEntries;
		readonly sSampleToChunkEntry[] sampleToChunk;
		readonly ChunkOffsetTable chunkOffset;
		readonly CompositionTimeDeltas timeDeltas;
		readonly int sampleCount;
		readonly uint timeScale;
		readonly TimeSpan duration;
		readonly SampleTable sampleTable;
		readonly iEditList editList;

		public SampleReader( TrackMetadata track )
		{
			MediaInformation mi = track.info.mediaInformation;
			sampleCount = mi.sampleTable.sampleSize.sampleCount;
			sampleSize = mi.sampleTable.sampleSize;
			timeToSampleEntries = mi.sampleTable.timeToSample;
			sampleToChunk = mi.sampleTable.sampleToChunk;
			chunkOffset = mi.sampleTable.chunkOffset;
			timeScale = track.info.timeScale;
			duration = track.info.duration;
			sampleTable = mi.sampleTable;
			timeDeltas = mi.sampleTable.compositionToSample;
			editList = track.editList;
			rewind();
		}

		// Iterator state for reading from sampleToChunk
		struct SampleToChunkIterator
		{
			// Index of the current entry in sampleToChunk array
			public int index;
			// First chunk in the current entry of the array, 0-based
			public int chunkBegin;
			// Last chunk in the current entry plus one
			public int chunkEnd;
			// Copy of the value from the mpeg4 file
			public int samplesPerChunk;
		};
		SampleToChunkIterator chunkIter;

		// Iterator state to read chunks and samples
		struct SampleIter
		{
			public int chunk;
			public int sample;
			public long sampleOffset;
			public int sampleInChunk;
		}
		SampleIter sampleIter;

		/// <summary>Iterator state to advance the time, using source data from stts atom.</summary>
		struct TimestampIter
		{
			public uint samplesLeft;
			public uint sampleDelta;
			public long timestamp;
			public int entryIndex;
		}
		TimestampIter timestampIter;

		CompositionTimeDeltas.Iterator timeDeltaIter = default;

		/// <summary>Convert timestamp of the current sample from that weird media timescale into normal time units.</summary>
		public TimeSpan timestamp => getCurrentTimestamp();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		TimeSpan getTimeStamp( long sample, int delta )
		{
			long res = sample + delta;
			res = editList.presentationTime( res );
			return MiscUtils.timeFromTrack( res, timeScale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		TimeSpan getCurrentTimestamp()
		{
			return getTimeStamp( timestampIter.timestamp, timeDeltaIter.delta );
		}

		/// <summary>Reset all iterators to the start of the media</summary>
		public void rewind()
		{
			// Rewind SampleToChunkIterator
			chunkIter = default;
			sSampleToChunkEntry ce = sampleToChunk[ 0 ];
			Debug.Assert( 1 == ce.firstChunk );
			if( sampleToChunk.Length > 1 )
				chunkIter.chunkEnd = sampleToChunk[ 1 ].firstChunk - 1;
			else
				chunkIter.chunkEnd = chunkOffset.count;
			chunkIter.samplesPerChunk = ce.samplesPerChunk;

			// Rewind sample iterator, the initial state of that one is all 0.
			sampleIter = default;

			// Rewind timestamps iterator
			timestampIter = default;
			timestampIter.samplesLeft = timeToSampleEntries[ 0 ].sampleCount;
			timestampIter.sampleDelta = timeToSampleEntries[ 0 ].sampleDelta;

			if( null != timeDeltas )
				timeDeltaIter = timeDeltas.begin();
		}

		/// <summary>True when reached end of the file</summary>
		public bool EOF => chunkIter.index >= sampleToChunk.Length;

		/// <summary>Seek the stream to the position of the current sample, return sample size in bytes</summary>
		public int seek( Stream stream )
		{
			long offset = chunkOffset[ sampleIter.chunk ];
			offset += sampleIter.sampleOffset;
			stream.Seek( offset, SeekOrigin.Begin );
			int cb = sampleSize[ sampleIter.sample ];
			// Logger.logVerbose( "Seek to 0x{0:X}, sample size 0x{1:X}", offset, cb );
			return cb;
		}

		/// <summary>Seek the stream to the position of the current sample + NALU offset within that sample</summary>
		/// <remarks>Only used for video streams.
		/// Fortunately, audio samples don’t have any internal structure we have need to care about, fdk-aac decoder accepts the complete audio samples as encountered in the mp4.
		/// Too bad that’s not the case for the Pi4 h264 video decoder.</remarks>
		public void seekToNalu( Stream stream, int naluOffset )
		{
			long offset = chunkOffset[ sampleIter.chunk ];
			offset += sampleIter.sampleOffset;
			offset += naluOffset;
			stream.Seek( offset, SeekOrigin.Begin );
		}
	}
}