using System;

namespace VrmacVideo.Containers.MP4
{
	static class ComputeUtils
	{
		// TODO [low]: move this to C++. We can easily have 150k-300k entries there.
		// AMD64 has _mm_shuffle_epi8 and _mm_max_epu32, NEON has vrev32q_u8 and vmaxq_u32.
		// Pretty sure gonna be faster by an order of magnitude, AFAIK .NET runtime doesn't have the performance budget for automatic vectorizations.
		// OTOH, this code only runs while opening the video, already fast enough even on Pi.
		public static void flipEndiannessAndComputeMax( Span<int> span, out uint maxValue )
		{
			if( span.IsEmpty )
			{
				maxValue = 0;
				return;
			}

			uint ax = 0;
			unsafe
			{
				fixed ( int* spanPointer = span )
				{
					uint* end = (uint*)spanPointer + span.Length;
					for( uint* ptr = (uint*)spanPointer; ptr < end; ptr++ )
					{
						uint v = *ptr;
						v = v.endian();
						ax = Math.Max( ax, v );
						*ptr = v;
					}
				}
			}
			maxValue = ax;
		}

		public static void packIntegers24( Span<byte> destSpan, ReadOnlySpan<int> sourceSpan )
		{
			unsafe
			{
				fixed ( int* sourcePointer = sourceSpan )
				fixed ( byte* destPointer = destSpan )
				{
					int* sourceEnd = sourcePointer + sourceSpan.Length;
					byte* dest = destPointer;
					for( int* p = sourcePointer; p < sourceEnd; p++, dest += 3 )
						*(int*)dest = *p;
				}
			}
		}

		public static void packIntegers16( Span<ushort> destSpan, ReadOnlySpan<int> sourceSpan )
		{
			unsafe
			{
				fixed ( int* sourcePointer = sourceSpan )
				fixed ( ushort* destPointer = destSpan )
				{
					int* sourceEndAligned = sourcePointer + ( sourceSpan.Length & ~1 );
					int* dest = (int*)destPointer;
					int* p = sourcePointer;
					for( ; p < sourceEndAligned; p += 2, dest++ )
					{
						int low = *p;
						int high = *( p + 1 );
						int both = low | ( high << 16 );
						*dest = both;
					}
					if( p < sourcePointer + sourceSpan.Length )
					{
						int last = *p;
						*(ushort*)( dest ) = (ushort)last;
					}
				}
			}
		}

		public static int swapEndiannessAndComputeMax( Span<ushort> span )
		{
			ushort result = 0;
			unsafe
			{
				fixed ( ushort* spanPointer = span )
				{
					ushort* end = spanPointer + span.Length;
					for( ushort* p = spanPointer; p < end; p++ )
					{
						ushort v = *p;
						v = v.endian();
						*p = v;
						result = Math.Max( result, v );
					}
				}
			}
			return result;
		}

		public static int computeMax( ReadOnlySpan<byte> bytes )
		{
			byte result = 0;
			foreach( byte v in bytes )
				result = Math.Max( result, v );
			return result;
		}

		public static void swapEndiannessAndComputeRange( Span<CttsEntryUnsigned> span, out uint maxCount, out uint maxOffset )
		{
			uint mc = 0, mo = 0;
			unsafe
			{
				fixed ( CttsEntryUnsigned* spanPointer = span )
				{
					CttsEntryUnsigned* end = spanPointer + span.Length;
					for( CttsEntryUnsigned* p = spanPointer; p < end; p++ )
					{
						uint c = p->sample_count;
						uint o = p->sample_offset;
						c = c.endian();
						o = o.endian();
						p->sample_count = c;
						p->sample_offset = o;
						mc = Math.Max( mc, c );
						mo = Math.Max( mo, o );
					}
				}
			}
			maxCount = mc;
			maxOffset = mo;
		}

		public static void swapEndiannessAndComputeRange( Span<CttsEntrySigned> span, out uint maxCount, out int minOffset, out int maxOffset )
		{
			uint mc = 0;
			int i = 0, ax = 0;
			unsafe
			{
				fixed ( CttsEntrySigned* spanPointer = span )
				{
					CttsEntrySigned* end = spanPointer + span.Length;
					for( CttsEntrySigned* p = spanPointer; p < end; p++ )
					{
						uint c = p->sample_count;
						int off = p->sample_offset;
						c = c.endian();
						off = off.endian();
						p->sample_count = c;
						p->sample_offset = off;
						mc = Math.Max( mc, c );
						i = Math.Min( i, off );
						ax = Math.Max( ax, off );
					}
				}
			}
			maxCount = mc;
			minOffset = i;
			maxOffset = ax;
		}
	}
}