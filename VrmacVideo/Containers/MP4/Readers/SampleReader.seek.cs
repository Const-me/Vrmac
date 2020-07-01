using System;
using System.IO;
using VrmacVideo.Containers.MP4;

namespace VrmacVideo
{
	sealed partial class SampleReader
	{
		int findSampleTimeDelta( int sample )
		{
			if( null == timeDeltas )
				return 0;
			return timeDeltas.seek( sample ).delta;
		}

		/// <summary>0-based index of the 1st sample with presentation time before the argument</summary>
		public Mp4StreamPosition findStreamPosition( TimeSpan ts )
		{
			if( ts.Ticks <= 0 )
			{
				if( ts.Ticks < 0 )
					Logger.logWarning( "A seek operation has a negative argument; clipped to the start of the track" );
				return default;
			}

			// Convert from normal time into that weird track-specific timescale
			long searchTime = ( ts.Ticks * timeScale ) / TimeSpan.TicksPerSecond;
			searchTime = editList.trackTime( searchTime );

			// Iterate over entries in `stts` box of the mp4 file.
			// More often than not, it has a single entry spanning across the entire track.
			uint entryFirstFrame = 0;
			long entryStartTime = 0;
			foreach( sTimeToSampleEntry e in timeToSampleEntries )
			{
				long endTime = entryStartTime + (long)e.sampleDelta * e.sampleCount;
				if( searchTime >= endTime )
				{
					entryStartTime = endTime;
					entryFirstFrame += e.sampleCount;
					continue;
				}

				// The sample we're after is within the current entry of that list. Compute the result.
				long relativeTime = searchTime - entryStartTime;
				uint sample = (uint)( relativeTime / e.sampleDelta );
				int index = (int)( sample + entryFirstFrame );

				long scaledTime = entryStartTime + (long)sample * e.sampleDelta;
				TimeSpan destTime = getTimeStamp( scaledTime, findSampleTimeDelta( index ) );
				return new Mp4StreamPosition( index, destTime );
			}

			Logger.logWarning( "A seek operation has the argument past the end of the track" );
			return new Mp4StreamPosition( sampleSize.sampleCount, duration );
		}

		public Mp4StreamPosition findKeyFrame( Mp4StreamPosition seekFrame )
		{
			int idxSeekSample = sampleTable.findSyncSample( seekFrame.sample );
			if( idxSeekSample == seekFrame.sample )
			{
				Logger.logVerbose( "findKeyFrame: the seek destination frame, {0}, is a key frame", seekFrame );
				return seekFrame;
			}

			Logger.logVerbose( "findKeyFrame: {0}, found key frame {1}", seekFrame, idxSeekSample );

			// Not done yet, need the time for the keyframe sample
			uint skipSamples = (uint)idxSeekSample;
			long entryStartTime = 0;
			foreach( sTimeToSampleEntry e in timeToSampleEntries )
			{
				long endTime = entryStartTime + (long)e.sampleDelta * e.sampleCount;
				if( e.sampleCount <= skipSamples )
				{
					// The key frame is past the current chunk of that table, move to the next chunk
					skipSamples -= e.sampleCount;
					entryStartTime = endTime;
					continue;
				}

				long scaledTime = entryStartTime + (long)skipSamples * e.sampleDelta;
				TimeSpan destTime = getTimeStamp( scaledTime, findSampleTimeDelta( idxSeekSample ) );
				return new Mp4StreamPosition( idxSeekSample, destTime );
			}

			Logger.logWarning( "findKeyFrame has the argument past the end of the track" );
			return new Mp4StreamPosition( sampleSize.sampleCount, duration );
		}

		public void seekToSample( int sampleIndex )
		{
			rewind();

			// Set the state of SampleToChunkIterator iterator
			int chunkRunSample = 0;
			while( true )
			{
				int chunksInRun = chunkIter.chunkEnd - chunkIter.chunkBegin;
				int samplesInRun = chunksInRun * chunkIter.samplesPerChunk;
				int chunkRunEndSample = chunkRunSample + samplesInRun;
				if( sampleIndex < chunkRunEndSample )
				{
					// Found a run of chunks containing the frame we're after
					break;
				}
				// Move to a next run of chunks
				chunkIter.index++;
				if( EOF )
					return;

				chunkRunSample = chunkRunEndSample;
				chunkIter.chunkBegin = chunkIter.chunkEnd;
				chunkIter.samplesPerChunk = sampleToChunk[ chunkIter.index ].samplesPerChunk;

				if( chunkIter.index + 1 < sampleToChunk.Length )
				{
					// sampleToChunk has a next element
					chunkIter.chunkEnd = sampleToChunk[ chunkIter.index + 1 ].firstChunk - 1;
				}
				else
				{
					// We're at the last sampleToChunk row, use total count of chunks in the file for the end iterator value
					chunkIter.chunkEnd = chunkOffset.count;
				}
			}
			// Found a run of chunks in sampleToChunk table. Locate the sample within the run

			// Sample index relative to the current run of chunks
			int relativeSample = sampleIndex - chunkRunSample;

			sampleIter.chunk = chunkIter.chunkBegin + relativeSample / chunkIter.samplesPerChunk;
			sampleIter.sample = sampleIndex;
			sampleIter.sampleInChunk = relativeSample % chunkIter.samplesPerChunk;
			for( int i = sampleIndex - sampleIter.sampleInChunk; i < sampleIndex; i++ )
				sampleIter.sampleOffset += sampleSize[ i ];

			// Still not done, need to setup time iterator
			uint samplesLeft = (uint)sampleIndex;
			while( true )
			{
				if( samplesLeft < timestampIter.samplesLeft )
				{
					timestampIter.timestamp += (long)samplesLeft * timestampIter.sampleDelta;
					timestampIter.samplesLeft -= samplesLeft;
					break;
				}
				samplesLeft -= timestampIter.samplesLeft;
				timestampIter.timestamp += (long)timestampIter.samplesLeft * timestampIter.sampleDelta;
				timestampIter.entryIndex++;
				if( timestampIter.entryIndex < timeToSampleEntries.Length )
				{
					var e = timeToSampleEntries[ timestampIter.entryIndex ];
					timestampIter.samplesLeft = e.sampleCount;
					timestampIter.sampleDelta = e.sampleDelta;
				}
				else
					throw new EndOfStreamException();
			}

			if( null != timeDeltas )
				timeDeltaIter = timeDeltas.seek( sampleIndex );

			Logger.logVerbose( "SampleReader.seekToSample positioned at the sample {0} with timestamp {1}", sampleIter.sample, timestamp );
		}
	}
}