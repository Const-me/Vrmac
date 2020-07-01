using System;

namespace VrmacVideo.IO
{
	enum eBufferState: byte
	{
		User,
		Kernel,
	}

	/// <summary>Base class for buffer, for both media queues</summary>
	abstract class BufferBase
	{
		public eBufferState state { get; private set; } = eBufferState.User;
		public readonly int bufferIndex;

		public BufferBase( int bufferIndex )
		{
			this.bufferIndex = bufferIndex;
		}

		/// <summary>Submit buffer to the decoder</summary>
		public void enqueueBuffer( FileHandle videoDevice )
		{
			if( state != eBufferState.User )
				throw new ApplicationException( $"Can't enqueue buffer #{ bufferIndex }, it’s already being processed by Linux kernel" );
			enqueue( videoDevice );
			state = eBufferState.Kernel;
		}

		protected abstract void enqueue( FileHandle videoDevice );

		public void dequeueBuffer( FileHandle videoDevice )
		{
			if( state != eBufferState.Kernel )
				throw new ApplicationException( $"Can't dequeue buffer #{ bufferIndex }, it’s not being processed by Linux kernel" );
			dequeue( videoDevice );
			state = eBufferState.User;
		}

		protected abstract void dequeue( FileHandle videoDevice );

		public abstract string queryStatus( FileHandle videoDevice );
	}
}