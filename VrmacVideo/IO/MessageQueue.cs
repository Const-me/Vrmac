using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO
{
	/// <summary>Wraps Linux message queue API into something more .NET</summary>
	sealed class MessageQueue<T>: IDisposable
		where T : unmanaged
	{
		readonly string name;
		const eFileFlags queueFlags = eFileFlags.O_RDWR | eFileFlags.O_CLOEXEC | eFileFlags.O_CREAT | eFileFlags.O_EXCL | eFileFlags.O_NONBLOCK;

		public MessageQueue( int length, string queueName )
		{
			int cbMessage = Marshal.SizeOf<T>();
			if( cbMessage > 8192 )
				throw new ArgumentOutOfRangeException( "By default, Linux limits size of queue messages to 8kb" );
			if( length > 10 )
				throw new ArgumentOutOfRangeException( "By default, Linux limits the queue to 10 pending messages" );

			int pid = Process.GetCurrentProcess().Id;
			string name = $"/{ queueName }.{ pid }";

			MessageQueueAttributes mqa = new MessageQueueAttributes()
			{
				flags = eMessageQueueFlags.NonBlocking,
				maxMessages = length,
				messageSize = cbMessage
			};
			int fd = MQ.mq_open( name, queueFlags, eFileAccess.OwnerAllAccess, ref mqa );
			if( fd < 0 )
				throw LibC.exception( "mq_open", fd );

			handle = fd;
			this.name = name;
		}

		public void Dispose()
		{
			if( handle >= 0 )
			{
				int res = MQ.mq_close( handle );
				if( res < 0 )
					throw LibC.exception( "mq_close", res );
				res = MQ.mq_unlink( name );
				if( res < 0 )
					throw LibC.exception( "mq_unlink", res );
			}
			GC.SuppressFinalize( this );
		}

		~MessageQueue()
		{
			if( handle >= 0 )
			{
				MQ.mq_close( handle );
				MQ.mq_unlink( name );
			}
		}

		/// <summary>Queue handle for poll API</summary>
		public int handle { get; }

		/// <summary>Enqueue a message; if there’re too many pending messages, will fail with an exception.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void enqueue( T message ) => MQ.mq_send( handle, message );

		/// <summary>Dequeue a message; is there’s no pending messages, will fail with an exception.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public T dequeue() => MQ.mq_receive<T>( handle );
	}
}