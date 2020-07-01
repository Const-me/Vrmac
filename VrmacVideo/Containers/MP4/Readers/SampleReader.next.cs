namespace VrmacVideo
{
	sealed partial class SampleReader
	{
		/// <summary>Advance to the next sample, return false when EOF</summary>
		public bool advance()
		{
			// Advance the timestamp iterator
			timestampIter.timestamp += timestampIter.sampleDelta;
			if( timestampIter.samplesLeft > 0 )
				timestampIter.samplesLeft--;
			else
			{
				timestampIter.entryIndex++;
				if( timestampIter.entryIndex < timeToSampleEntries.Length )
				{
					var e = timeToSampleEntries[ timestampIter.entryIndex ];
					timestampIter.samplesLeft = e.sampleCount;
					timestampIter.sampleDelta = e.sampleDelta;
				}
			}

			// Advance time deltas iterator, if the track has that table. h264 video tracks have 'coz decoder reorders these samples quite often.
			timeDeltas?.advance( ref timeDeltaIter );

			// Advance sample iterator
			int sample = sampleIter.sample;
			sampleIter.sample = sample + 1;
			sampleIter.sampleInChunk++;
			if( sampleIter.sampleInChunk < chunkIter.samplesPerChunk )
			{
				// Within the same chunk
				sampleIter.sampleOffset += sampleSize[ sample ];
				return true;
			}

			// Moved to a next chunk
			sampleIter.chunk++;
			sampleIter.sampleInChunk = 0;
			sampleIter.sampleOffset = 0;
			if( sampleIter.chunk < chunkIter.chunkEnd )
			{
				// Within the same run of chunks
				return true;
			}

			chunkIter.index++;
			// Moved to a next run of chunks
			if( EOF )
				return false;

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
			return true;
		}
	}
}