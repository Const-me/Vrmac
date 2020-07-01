using Diligent;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vrmac;
using VrmacVideo.IO;

namespace VrmacVideo.Audio
{
	/// <summary>Utility class to move encoded audio frames between the threads.</summary>
	/// <remarks>No audio data is being copied of course, that would be a waste of resources.</remarks>
	sealed class Queues: IDisposable, iDecoderQueues, iPlayerQueues
	{
		/// <summary>While much less than video, audio stream still uses non-trivial bandwidth.
		/// We definitely don’t want that bandwidth through the GC.
		/// We also want to avoid overhead of calling fixed/unsafe multiple times for each sample as it’s loaded from the file and sent to the decoder.
		/// In addition to that, some audio decoders want the encoded source data to stay at fixed memory addresses.
		/// For these reasons, keeping audio frames in native memory. AFAIK on Linux, Marshal.AllocHGlobal should be a thin wrapper over malloc() from glibc.</summary>
		readonly IntPtr[] encodedBuffers;

		readonly int[] encodedBufferLengths = null;

		readonly int maxBytesInFrame;
		readonly MessageQueue<AudioFrame> encodedQueue;
		readonly MessageQueue<int> emptyQueue;

		static int computeBufferSize( int requiredSize )
		{
			// Encoded audio buffers move data between threads, and therefore between CPU cores.
			// For performance reasons we don't want any cache line sharing to happen.
			// The cache line size is undocumented, none of these methods work on ARM: https://stackoverflow.com/q/30207256
			// Assuming 64 bytes like it's on the PC.
			return ( requiredSize + 63 ) & ( ~63 );
		}

		public Queues( TrackInfo trackInfo, int encodedBuffersCount )
		{
			encodedQueue = new MessageQueue<AudioFrame>( encodedBuffersCount, "audio-queue-encoded" );
			emptyQueue = new MessageQueue<int>( encodedBuffersCount, "audio-queue-free" );
			maxBytesInFrame = trackInfo.maxBytesInFrame;

			encodedBuffers = new IntPtr[ encodedBuffersCount ];
			int bs = 0;
			if( maxBytesInFrame > 0 )
			{
				bs = computeBufferSize( maxBytesInFrame );
				for( int i = 0; i < encodedBuffersCount; i++ )
					encodedBuffers[ i ] = Marshal.AllocHGlobal( bs );
			}
			else
				encodedBufferLengths = new int[ encodedBuffersCount ];

#if true
			// On startup, enqueue all buffers to the empty queue
			// When that code is disabled, both sides of the cross-thread interop do nothing due to the lack of data.
			for( int i = 0; i < encodedBuffersCount; i++ )
				emptyQueue.enqueue( i );
#endif

			if( maxBytesInFrame > 0 )
				Logger.logVerbose( "Audio.Queues: {0}, {1} bytes / each, rounded up to {2} bytes", encodedBuffersCount.pluralString( "encoded buffer" ), maxBytesInFrame, bs );
			else
				Logger.logVerbose( "Audio.Queues: {0}, dynamically sized", encodedBuffersCount.pluralString( "encoded buffer" ) );
		}

		bool disposedValue = false;
		void Dispose( bool disposing )
		{
			if( !disposedValue )
			{
				disposedValue = true;

				if( disposing )
				{
					encodedQueue?.Dispose();
					emptyQueue?.Dispose();
					GC.SuppressFinalize( this );
				}

				if( null != encodedBuffers )
					foreach( var b in encodedBuffers )
						Marshal.FreeHGlobal( b );
			}
		}
		~Queues() => Dispose( false );
		public void Dispose() => Dispose( true );

		// ==== API for the decoder thread ====
		int iDecoderQueues.emptyQueueHandle => emptyQueue.handle;
		// int iDecoderQueues.maxEncodedBytes => maxBytesInFrame;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		Span<byte> iDecoderQueues.dequeueEmpty( out int idx, int length )
		{
			idx = emptyQueue.dequeue();
			IntPtr buffer = encodedBuffers[ idx ];
			if( maxBytesInFrame > 0 )
			{
				if( length > maxBytesInFrame )
					throw new ApplicationException( $"Audio sample is too large; container metadata says the limit is { maxBytesInFrame } bytes, the actual one is { length } bytes" );
			}
			else
			{
				int bufferSize = encodedBufferLengths[ idx ];
				if( length > bufferSize )
				{
					if( IntPtr.Zero != buffer )
						Marshal.FreeHGlobal( buffer );
					bufferSize = computeBufferSize( length );
					buffer = Marshal.AllocHGlobal( bufferSize );
					if( IntPtr.Zero == buffer )
						throw new OutOfMemoryException();

					encodedBuffers[ idx ] = buffer;
					encodedBufferLengths[ idx ] = bufferSize;
				}
			}

			return Unsafe.writeSpan<byte>( buffer, length );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void iDecoderQueues.enqueueEncoded( int idx, int length, TimeSpan timestamp )
		{
			if( maxBytesInFrame > 0 && length > maxBytesInFrame )
				throw new ArgumentOutOfRangeException( $"Audio sample is too large; container metadata says the limit is { maxBytesInFrame } bytes, trying to enqueue { length } bytes" );

			AudioFrame frame = new AudioFrame( idx, length, timestamp );
			encodedQueue.enqueue( frame );
		}

		// ==== API for the audio thread ====
		int iPlayerQueues.encodedBuffersCount => encodedBuffers.Length;
		int iPlayerQueues.encodedQueueHandle => encodedQueue.handle;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		AudioFrame iPlayerQueues.dequeueEncoded() =>
			encodedQueue.dequeue();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		ReadOnlySpan<byte> iPlayerQueues.encodedBuffer( AudioFrame frame )
		{
			return Unsafe.readSpan<byte>( encodedBuffers[ frame.index ], frame.payloadBytes );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void iPlayerQueues.enqueueEmpty( int idx ) =>
			emptyQueue.enqueue( idx );
	}
}