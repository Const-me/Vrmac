using Diligent;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.HEVC
{
	struct DecoderDevice
	{
		FileHandle fileMem, fileInterrupts;
		IntPtr mapMem, mapInterrupts;

		const string devicePathHevc = @"/dev/argon-hevcmem";
		const string devicePathInterrupts = @"/dev/argon-intcmem";
		const int mappedSizeMem = 0x10000;
		// const int mappedSizeInterrupts = 0x10000;
		// const int mappedSizeInterrupts = 0x1000;
		const int mappedSizeInterrupts = 4;

		static void openEndpoint( string path, ref FileHandle file, ref IntPtr mapped, int size )
		{
			file = FileHandle.openFile( path, eFileFlags.O_RDWR | eFileFlags.O_SYNC );
			mapped = LibC.mmap( IntPtr.Zero, (UIntPtr)size, eMemoryProtection.ReadWrite, (int)eMapFlags.Shared, file, (IntPtr)0 );
			if( mapped == LibC.MAP_FAILED )
				throw LibC.exception( $"memory map failed for device \"{ path }\"", -1 );
		}

		public DecoderDevice( bool unused )
		{
			fileMem = fileInterrupts = FileHandle.invalid;
			mapMem = mapInterrupts = LibC.MAP_FAILED;

			openEndpoint( devicePathHevc, ref fileMem, ref mapMem, mappedSizeMem );
			openEndpoint( devicePathInterrupts, ref fileInterrupts, ref mapInterrupts, mappedSizeInterrupts );
		}

		public void finalize()
		{
			if( mapInterrupts != LibC.MAP_FAILED )
			{
				LibC.munmap( mapInterrupts, (UIntPtr)mappedSizeInterrupts );
				mapInterrupts = LibC.MAP_FAILED;
			}
			fileInterrupts.finalize();

			if( mapMem != LibC.MAP_FAILED )
			{
				LibC.munmap( mapMem, (UIntPtr)mappedSizeMem );
				mapMem = LibC.MAP_FAILED;
			}
			fileMem.finalize();
		}

		/* static uint mangleArgonBlock( uint val )
		{
			return val & ~0xc0000000u;
		} */

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void mapWrite( eDecoderRegister address, int value )
		{
			int offset = (int)address * 4;
			Marshal.WriteInt32( mapMem + offset, value );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void mapWrite( eDecoderRegister address, uint value )
		{
			int offset = (int)address * 4;
			Marshal.WriteInt32( mapMem + offset, unchecked((int)value) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int mapRead( ushort address )
		{
			int offset = address & ~3;
			return Marshal.ReadInt32( mapMem + offset );
		}

		public sDecoderStatus readStatus() =>
			new sDecoderStatus( Marshal.ReadInt32( mapInterrupts ) );

		static readonly TimeSpan sleepDuration = TimeSpan.FromMilliseconds( 1.0 / 8.0 );    // 125 microseconds
		static readonly int sleepRetries = (int)( TimeSpan.FromMilliseconds( 500 ).Ticks / sleepDuration.Ticks );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		int waitForStatusBit( int bit )
		{
			for( int i = 0; i < sleepRetries; i++ )
			{
				int s = Marshal.ReadInt32( mapInterrupts );
				if( 0 == ( s & phase1Complete ) )
				{
					LibC.sleep( sleepDuration );
					continue;
				}
				return s;
			}
			throw new TimeoutException( "HEVC decoder failed to produce result within reasonable timeout" );
		}

		const int phase1Complete = (int)eDecoderStatus.Interrupt;
		const int phase2Complete = (int)eDecoderStatus.Interrupt << 4;

		public void waitPhase1()
		{
			int r = waitForStatusBit( phase1Complete );
			r &= ~phase2Complete;
			Marshal.WriteInt32( mapInterrupts, r );
		}

		public void waitPhase2()
		{
			int r = waitForStatusBit( phase2Complete );
			r &= ~phase1Complete;
			Marshal.WriteInt32( mapInterrupts, r );
		}

		public Span<uint> scalingList()
		{
			const int offset = (int)eDecoderRegister.ScalingList0 * 4;
			const int count = DecoderScalingList.scalingListBytes / 4;
			return Unsafe.writeSpan<uint>( mapMem + offset, count );
		}
	}
}