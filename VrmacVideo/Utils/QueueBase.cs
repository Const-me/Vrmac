using System;
using System.Diagnostics;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	/// <summary>Abstract base class for V4L2 queue objects</summary>
	abstract class QueueBase<T> where T : BufferBase
	{
		public int buffersCount => buffers.Length;
		readonly FileHandle videoDevice;

		// We need buffer indices before sending DQBUF control code.
		// The reason for that, raspberry developers failed to implement stateful video decoder kernel interface correctly, as specified on kernel.org.
		// Their current implementation requires multi-planar API despite only a single plane is used.
		// Buffers for multi-planar API include a user pointer in the structure, for planes array.
		// While a buffer is processed by the hardware, the hardware probably expects data at that pointer to be still valid.
		// For this reason, a stack variable won't work for that pointer, they need to point to the native heap.
		// And the last thing we want is malloc/free every frame.
		// Fortunately, the workaround seems to be OK performance wise, these tiny queues of 2-8 bytes in a single array should be quite fast.
		SmallQueue m_kernel, m_user;

		public QueueBase( int buffersCount, FileHandle videoDevice )
		{
			if( buffersCount > 32 )
				throw new ArgumentOutOfRangeException( "Max. buffers count is 32" );
			buffers = new T[ buffersCount ];
			this.videoDevice = videoDevice;

			m_kernel = new SmallQueue( buffersCount, false );
			m_user = new SmallQueue( buffersCount, true );
		}

		protected readonly T[] buffers;

		T getNextEnqueue()
		{
			if( m_user.any )
				return buffers[ m_user.first ];
			return null;
		}
		public T nextEnqueue => getNextEnqueue();

		public void enqueue( T buffer )
		{
			Debug.Assert( getNextEnqueue() == buffer );
			buffer.enqueueBuffer( videoDevice );
			// Dequeue from user, enqueue to kernel
			m_user.dequeue();
			m_kernel.enqueue( buffer.bufferIndex );
		}

		public bool anyKernelBuffer => m_kernel.any;

		public T dequeue()
		{
			if( !m_kernel.any )
				throw new ApplicationException( "Unable to dequeue a buffer: none of them are being processed by Linux kernel" );

			// Find the buffer, it's the first one in the kernel queue
			T buffer = buffers[ m_kernel.first ];

			buffer.dequeueBuffer( videoDevice );

			// Dequeue from kernel, enqueue to user
			m_kernel.dequeue();
			m_user.enqueue( buffer.bufferIndex );

			return buffer;
		}

		public void dbgPrintBuffers( VideoDevice device )
		{
			for( int i = 0; i < buffersCount; i++ )
				Logger.logInfo( "#{0}: {1}", i, buffers[ i ].queryStatus( device.file ) );
		}
	}
}