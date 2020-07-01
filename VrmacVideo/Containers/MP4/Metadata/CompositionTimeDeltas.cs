#pragma warning disable CS0649 // Field is never assigned to
using Diligent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VrmacVideo.Containers.MP4
{
	/// <summary>Table with per-sample offsets between decoding time and composition time</summary>
	/// <remarks>The data comes from <see cref="eBoxType.ctts" />, and almost always present for video tracks, because h264 encoders generally reorder these frames.</remarks>
	public abstract class CompositionTimeDeltas
	{
		/// <summary>Iterator over this table</summary>
		public struct Iterator
		{
			/// <summary>Composition-to-decoding delta of the current sample in the time scale of the media</summary>
			public int delta { get; internal set; }
			internal uint samplesLeft;
			internal int entryIndex;
		}

		/// <summary>Seek to start of the video</summary>
		public abstract Iterator begin();
		/// <summary>Advance to the next sample</summary>
		public abstract bool advance( ref Iterator iter );
		/// <summary>Seek to arbitrary position in the stream, defined by 0-based sample index</summary>
		public abstract Iterator seek( int sample );

		struct sHeader
		{
			public int version;
			public int entry_count;
		}

		internal static CompositionTimeDeltas read( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.ctts );

			sHeader header = reader.readStructure<sHeader>();
			int count = header.entry_count.endian();
			if( count <= 0 )
				return null;

			bool signedIntegers;
			switch( header.version )
			{
				case 0:
					signedIntegers = false;
					break;
				case 1:
					signedIntegers = true;
					break;
				default:
					// Warning because videos actually play OK even when ignoring the data from that box.
					Logger.logWarning( "ctts box has unexpected version {0}", header.version );
					return null;
			}

			// That thing takes couple MB of RAM. Bypassing the GC with malloc/free.
			IntPtr nativePointer = Marshal.AllocHGlobal( count * 8 );
			try
			{
				Span<CttsEntryUnsigned> span = Unsafe.writeSpan<CttsEntryUnsigned>( nativePointer, count );
				reader.read( span.asBytes() );

				if( signedIntegers )
				{
					var signed = MemoryMarshal.Cast<CttsEntryUnsigned, CttsEntrySigned>( span );
					return parseSigned( signed );
				}
				return parseUnsigned( span );
			}
			finally
			{
				Marshal.FreeHGlobal( nativePointer );
			}
		}

		struct IndexEntry
		{
			/// <summary>0-based index of the sample</summary>
			public readonly int sample;
			/// <summary>0-based offset into the table from ctts box of the mpeg4</summary>
			public readonly int offset;
			public IndexEntry( int s, int o ) { sample = s; offset = o; }
			public override string ToString() => $"sample { sample }, offset { offset }";
		}

		sealed class IndexComparer: IComparer<IndexEntry>
		{
			int IComparer<IndexEntry>.Compare( IndexEntry x, IndexEntry y )
			{
				return x.offset.CompareTo( y.offset );
			}
		}

		/// <summary>Binary searchable index for seeks</summary>
		sealed class Index
		{
			readonly IndexEntry[] entries;
			readonly IComparer<IndexEntry> comparer = new IndexComparer();

			const int indexFrequency = 0x80;

			public Index( ReadOnlySpan<CttsEntryUnsigned> source )
			{
				// Sqrt makes no sense because the inner loop ain't logarithmic like the binary search, it's linear.
				// int indexFrequency = (int)Math.Round( Math.Sqrt( source.Length ) );
				int indexEntries = ( ( source.Length + indexFrequency - 1 ) / indexFrequency ) + 1;
				entries = new IndexEntry[ indexEntries ];

				int sample = 0;
				int offset = 0;
				int entry = 0;
				for( int i = 0; i < source.Length; i += indexFrequency, entry++ )
				{
					entries[ entry ] = new IndexEntry( sample, offset );
					int jMax = Math.Min( i + indexFrequency, source.Length );
					for( int j = i; j < jMax; j++, offset++ )
					{
						sample += (int)source[ j ].sample_count;
					}
				}
				if( entry < entries.Length )
				{
					entries[ entry ] = new IndexEntry( sample, offset );
					entry++;
				}
				Debug.Assert( entry == entries.Length );
				Debug.Assert( offset == source.Length );
			}

			public IndexEntry find( int sample )
			{
				IndexEntry ee = new IndexEntry( sample, 0 );
				int res = Array.BinarySearch( entries, ee, comparer );
				if( res < 0 )
				{
					int index = ~res;
					// When not found, Array.BinarySearch returns index of the first element that is larger than value.
					// We want the previous one, the last element smaller than value.
					Debug.Assert( index > 0 );
					return entries[ index - 1 ];
				}
				return entries[ res ];
			}
		}

		static int countSize( uint maxCount )
		{
			if( maxCount < 0x100 )
				return 0x10;
			else if( maxCount < 0x10000 )
				return 0x20;
			else
				return 0x40;
		}

		static CompositionTimeDeltas parseUnsigned( Span<CttsEntryUnsigned> source )
		{
			uint maxCount, maxOffset;
			ComputeUtils.swapEndiannessAndComputeRange( source, out maxCount, out maxOffset );

			int index = countSize( maxCount );
			if( maxOffset < 0x100 )
				index |= 1;
			else if( maxOffset < 0x10000 )
				index |= 2;
			else
				index |= 4;

			switch( index )
			{
				// byte count
				case 0x11: return new Deltas<byte, byteCountTraits, byte, byteValueTraits>( source );
				case 0x12: return new Deltas<byte, byteCountTraits, ushort, ushortValueTraits>( source );
				case 0x14: return new Deltas<byte, byteCountTraits, uint, uintValueTraits>( source );

				// ushort count
				case 0x21: return new Deltas<ushort, ushortCountTraits, byte, byteValueTraits>( source );
				case 0x22: return new Deltas<ushort, ushortCountTraits, ushort, ushortValueTraits>( source );
				case 0x24: return new Deltas<ushort, ushortCountTraits, uint, uintValueTraits>( source );

				// uint count
				case 0x41: return new Deltas<uint, uintCountTraits, byte, byteValueTraits>( source );
				case 0x42: return new Deltas<uint, uintCountTraits, ushort, ushortValueTraits>( source );
				case 0x44: return new Deltas<uint, uintCountTraits, uint, uintValueTraits>( source );
			}
			throw new ApplicationException();
		}

		static CompositionTimeDeltas parseSigned( Span<CttsEntrySigned> source )
		{
			uint maxCount;
			int maxOffset, minOffset;
			ComputeUtils.swapEndiannessAndComputeRange( source, out maxCount, out minOffset, out maxOffset );

			int index = countSize( maxCount );

			if( maxOffset < 0x80 && minOffset >= -0x80 )
				index |= 1;
			else if( maxOffset < 0x8000 && minOffset >= -0x8000 )
				index |= 2;
			else
				index |= 4;

			switch( index )
			{
				// byte count
				case 0x11: return new Deltas<byte, byteCountTraits, sbyte, sbyteValueTraits>( source );
				case 0x12: return new Deltas<byte, byteCountTraits, short, shortValueTraits>( source );
				case 0x14: return new Deltas<byte, byteCountTraits, int, intValueTraits>( source );

				// ushort count
				case 0x21: return new Deltas<ushort, ushortCountTraits, sbyte, sbyteValueTraits>( source );
				case 0x22: return new Deltas<ushort, ushortCountTraits, short, shortValueTraits>( source );
				case 0x24: return new Deltas<ushort, ushortCountTraits, int, intValueTraits>( source );

				// uint count
				case 0x41: return new Deltas<uint, uintCountTraits, sbyte, sbyteValueTraits>( source );
				case 0x42: return new Deltas<uint, uintCountTraits, short, shortValueTraits>( source );
				case 0x44: return new Deltas<uint, uintCountTraits, int, intValueTraits>( source );
			}
			throw new ApplicationException();
		}

		interface iCountTraits<T>
		{
			uint convert( T val );
			T create( uint src );
		}
		interface iValueTraits<T>
		{
			int convert( T val );
			T create( int val );
		}

		sealed class Deltas<TCount, TCountTraits, TValue, TValueTraits>: CompositionTimeDeltas
			where TCountTraits : struct, iCountTraits<TCount>
			where TValueTraits : struct, iValueTraits<TValue>
		{
			readonly Index index;

			[StructLayout( LayoutKind.Sequential, Pack = 1 )]
			struct Entry
			{
				public readonly TCount count;
				public readonly TValue value;
				public Entry( TCount count, TValue value )
				{
					this.count = count;
					this.value = value;
				}
				public override string ToString() => $"count { count }, value { value }";
			}
			readonly Entry[] entries;

			public Deltas( ReadOnlySpan<CttsEntryUnsigned> source )
			{
				index = new Index( source );

				entries = new Entry[ source.Length ];
				TCountTraits KT = default;
				TValueTraits VT = default;
				for( int i = 0; i < source.Length; i++ )
				{
					CttsEntryUnsigned src = source[ i ];
					int off = checked((int)src.sample_offset);
					entries[ i ] = new Entry( KT.create( src.sample_count ), VT.create( off ) );
				}
			}

			public Deltas( ReadOnlySpan<CttsEntrySigned> source )
			{
				index = new Index( MemoryMarshal.Cast<CttsEntrySigned, CttsEntryUnsigned>( source ) );

				entries = new Entry[ source.Length ];
				TCountTraits KT = default;
				TValueTraits VT = default;
				for( int i = 0; i < source.Length; i++ )
				{
					CttsEntrySigned src = source[ i ];
					entries[ i ] = new Entry( KT.create( src.sample_count ), VT.create( src.sample_offset ) );
				}
			}

			Iterator makeIterator( int index )
			{
				var e = entries[ index ];
				TCountTraits KT = default;
				TValueTraits VT = default;
				return new Iterator()
				{
					delta = VT.convert( e.value ),
					samplesLeft = KT.convert( e.count ),
					entryIndex = index
				};
			}

			public override Iterator begin() => makeIterator( 0 );

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			public override bool advance( ref Iterator iter )
			{
				if( iter.samplesLeft > 1 )
				{
					iter.samplesLeft--;
					return true;
				}
				iter.entryIndex++;
				if( iter.entryIndex >= entries.Length )
					return false;
				iter = makeIterator( iter.entryIndex );
				return true;
			}

			public override Iterator seek( int sample )
			{
				var entry = index.find( sample );
				Iterator iter = makeIterator( entry.offset );
				int samplesLeft = sample - entry.sample;
				Debug.Assert( samplesLeft >= 0 );
				for( ; samplesLeft > 0; samplesLeft-- )
					advance( ref iter );
				return iter;
			}
		}

		// Traits for count generic argument
		struct byteCountTraits: iCountTraits<byte>
		{
			public uint convert( byte v ) => v;
			public byte create( uint v ) => checked((byte)v);
		}
		struct ushortCountTraits: iCountTraits<ushort>
		{
			public uint convert( ushort val ) => val;
			public ushort create( uint v ) => checked((ushort)v);
		}
		struct uintCountTraits: iCountTraits<uint>
		{
			public uint convert( uint val ) => val;
			public uint create( uint v ) => v;
		}

		// Traits for time delta generic argument
		struct byteValueTraits: iValueTraits<byte>
		{
			public int convert( byte val ) => val;
			public byte create( int v ) => checked((byte)v);
		}
		struct sbyteValueTraits: iValueTraits<sbyte>
		{
			public int convert( sbyte val ) => val;
			public sbyte create( int v ) => checked((sbyte)v);
		}
		struct ushortValueTraits: iValueTraits<ushort>
		{
			public int convert( ushort val ) => val;
			public ushort create( int v ) => checked((ushort)v);
		}
		struct shortValueTraits: iValueTraits<short>
		{
			public int convert( short val ) => val;
			public short create( int v ) => checked((short)v);
		}
		struct uintValueTraits: iValueTraits<uint>
		{
			public int convert( uint val ) => (int)val;
			public uint create( int v ) => checked((uint)v);
		}
		struct intValueTraits: iValueTraits<int>
		{
			public int convert( int val ) => val;
			public int create( int v ) => v;
		}
	}

	struct CttsEntryUnsigned
	{
		public uint sample_count, sample_offset;
		public override string ToString() => $"count { sample_count }, offset { sample_offset }";
	}

	struct CttsEntrySigned
	{
		public uint sample_count;
		public int sample_offset;
		public override string ToString() => $"count { sample_count }, offset { sample_offset }";
	}
}