using Diligent;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VrmacVideo.Containers.MP4
{
	public abstract class SampleSizeTable
	{
		public readonly int sampleCount;

		internal SampleSizeTable( int count )
		{
			sampleCount = count;
		}

		public static SampleSizeTable createVariable( Mp4Reader reader, int count )
		{
			// That's not a small amount of data, maybe a megabyte or 2. Bypassing the GC with malloc/free.
			IntPtr nativePointer = Marshal.AllocHGlobal( count * 4 );
			try
			{
				Span<int> entries = Unsafe.writeSpan<int>( nativePointer, count );

				reader.read( entries.asBytes() );
				ComputeUtils.flipEndiannessAndComputeMax( entries, out uint maxValue );

				if( maxValue > 0xFFFFFF )
					return new SampleSizeVariable32( entries.ToArray(), (int)maxValue );

				if( maxValue > 0xFFFF )
					return new SampleSizeVariable24( entries, (int)maxValue );

				return new SampleSizeVariable16( entries, (int)maxValue );
			}
			finally
			{
				Marshal.FreeHGlobal( nativePointer );
			}
		}

		public abstract int this[ int index ] { get; }
		public abstract int maxSampleSize { get; }
	}

	sealed class SampleSizeFixed: SampleSizeTable
	{
		public readonly int sampleSize;

		public SampleSizeFixed( int size, int count ) :
			base( count )
		{
			sampleSize = size;
		}

		public override int this[ int index ] => sampleSize;
		public override int maxSampleSize { get; }

		public override string ToString() =>
			$"Fixed sample size: { sampleCount } samples, { sampleSize } bytes / each";
	}

	sealed class SampleSizeVariable32: SampleSizeTable
	{
		readonly int[] entries;

		public SampleSizeVariable32( int[] entries, int maxValue ) :
			base( entries.Length )
		{
			this.entries = entries;
			maxSampleSize = maxValue;
		}

		public override int this[ int index ] => entries[ index ];

		public override int maxSampleSize { get; }

		public override string ToString() =>
			$"Variable sample size table, 32 bits, { sampleCount } samples, up to { maxSampleSize } bytes";
	}

	sealed class SampleSizeVariable24: SampleSizeTable
	{
		readonly byte[] entries;

		public SampleSizeVariable24( ReadOnlySpan<int> entries, int maxValue ) :
			base( entries.Length )
		{
			// For performance reasons we load 4 bytes, this way the indexer is just 2 instructions, load and bitwise and.
			// We don't want to crash reading the last element, that's why the "+1"
			int length = ( entries.Length * 3 ) + 1;
			this.entries = new byte[ length ];
			ComputeUtils.packIntegers24( this.entries.AsSpan(), entries );
			maxSampleSize = maxValue;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		int getEntry( int index )
		{
			if( index >= 0 && index < sampleCount )
			{
				int val = BitConverter.ToInt32( entries, index * 3 );
				return val & 0xFFFFFF;
			}
			throw new ArgumentOutOfRangeException();
		}

		public override int this[ int index ] => getEntry( index );

		public override int maxSampleSize { get; }

		public override string ToString() =>
			$"Variable sample size table, 24 bits, { sampleCount } samples, up to { maxSampleSize } bytes";
	}

	sealed class SampleSizeVariable16: SampleSizeTable
	{
		readonly ushort[] entries;

		public SampleSizeVariable16( ReadOnlySpan<int> entries, int maxValue ) :
			base( entries.Length )
		{
			this.entries = new ushort[ entries.Length ];
			ComputeUtils.packIntegers16( this.entries.AsSpan(), entries );
			maxSampleSize = maxValue;
		}

		public SampleSizeVariable16( Mp4Reader mp4, int count ) :
			base( count )
		{
			Debug.Assert( mp4.currentBox == eBoxType.stz2 );
			entries = new ushort[ count ];
			mp4.read( entries.AsSpan().asBytes() );
			maxSampleSize = ComputeUtils.swapEndiannessAndComputeMax( entries.AsSpan() );
		}

		public override int this[ int index ] => entries[ index ];

		public override int maxSampleSize { get; }

		public override string ToString() =>
			$"Variable sample size table, 16 bits, { sampleCount } samples, up to { maxSampleSize } bytes";
	}

	sealed class SampleSizeVariable8: SampleSizeTable
	{
		readonly byte[] entries;

		public SampleSizeVariable8( Mp4Reader mp4, int count ) : base( count )
		{
			Debug.Assert( mp4.currentBox == eBoxType.stz2 );
			entries = new byte[ count ];
			mp4.read( entries.AsSpan() );
			maxSampleSize = ComputeUtils.computeMax( entries.AsSpan() );
		}

		public override int this[ int index ] => entries[ index ];

		public override int maxSampleSize { get; }

		public override string ToString() =>
			$"Variable sample size table, 8 bits, { sampleCount } samples, up to { maxSampleSize } bytes";
	}
}