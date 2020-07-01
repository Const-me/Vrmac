#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
#pragma warning disable CS0169  // field is never used
using System;
using System.Diagnostics;

namespace VrmacVideo.Containers.MP4
{
	static class TimingTables
	{
		struct sHeader
		{
			uint unused;
			public int entry_count;
		}

		public static T[] readArray<T>( Mp4Reader reader, int count ) where T : unmanaged
		{
			T[] result = new T[ count ];
			var span = result.AsSpan().asBytes();
			reader.read( span );

			Span<uint> values = span.cast<uint>();
			for( int i = 0; i < values.Length; i++ )
				values[ i ] = values[ i ].endian();

			return result;
		}

		static T[] readArray<T>( Mp4Reader reader ) where T : unmanaged
		{
			sHeader header = reader.readStructure<sHeader>();
			int count = header.entry_count.endian();
			return readArray<T>( reader, count );
		}

		public static sTimeToSampleEntry[] readTimeToSample( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stts );
			return readArray<sTimeToSampleEntry>( reader );
		}

		public static sSampleToChunkEntry[] readSampleToChunk( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stsc );
			return readArray<sSampleToChunkEntry>( reader );
		}

		public static uint[] readSyncSample( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stss );
			return readArray<uint>( reader );
		}

		struct sSampleSizeBox
		{
			uint unused;
			public int sample_size, sample_count;
		}

		public static SampleSizeTable readSampleSize( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stsz );
			sSampleSizeBox box = reader.readStructure<sSampleSizeBox>();
			if( box.sample_size != 0 )
				return new SampleSizeFixed( box.sample_size.endian(), box.sample_count.endian() );

			return SampleSizeTable.createVariable( reader, box.sample_count.endian() );
		}

		public static SampleSizeTable readSampleSizeCompact( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stz2 );

			Span<int> header = stackalloc int[ 3 ];
			reader.read( header.asBytes() );
			int fieldSize = ( header[ 1 ] & 0xFF );
			int count = header[ 2 ].endian();
			switch( fieldSize )
			{
				case 4:
					// bits / sample, i.e. each value is in [ 0 .. 15 ] interval. I wonder which codec they have designed it for..
					throw new NotImplementedException();
				case 8:
					return new SampleSizeVariable8( reader, count );
				case 16:
					return new SampleSizeVariable16( reader, count );
				default:
					throw new ArgumentException();
			}
		}

		public static ChunkOffsetTable readOffsets32( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.stco );

			sHeader header = reader.readStructure<sHeader>();
			int count = header.entry_count.endian();

			uint[] entries = readArray<uint>( reader, count );
			return new ChunkOffsetTable32( entries );
		}

		public static ChunkOffsetTable readOffsets64( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.co64 );

			sHeader header = reader.readStructure<sHeader>();
			int count = header.entry_count.endian();

			long[] entries = new long[ count ];
			reader.read( entries.AsSpan().asBytes() );
			for( int i = 0; i < entries.Length; i++ )
				entries[ i ] = entries[ i ].endian();

			return new ChunkOffsetTable64( entries );
		}

		public static sCompositionToSampleEntry[] readCompositionOffsets( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.ctts );
			return readArray<sCompositionToSampleEntry>( reader );
		}
	}
}