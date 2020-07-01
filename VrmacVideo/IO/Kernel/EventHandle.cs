using System;

namespace VrmacVideo.IO
{
	/// <summary>A special file handle exposing ManualResetEvent-like functionality</summary>
	/// <remarks>Implemented for poll API</remarks>
	struct EventHandle
	{
		int fd;

		public static EventHandle create()
		{
			int fd = LibC.eventfd( 0, eEventFdFlags.NonBlocking | eEventFdFlags.DisableHandleInheritance );
			if( fd >= 0 )
				return new EventHandle( fd );
			throw LibC.exception( $"Unable to create an event handle", fd );
		}
		public static EventHandle invalid() => new EventHandle( -1 );

		public void set()
		{
			unsafe
			{
				ulong increment = 1;
				int res = (int)LibC.write( fd, &increment, (UIntPtr)8 );
				if( 8 == res )
					return;
				throw LibC.exception( "EventHandle.set failed", res );
			}
		}

		public void reset()
		{
			unsafe
			{
				ulong counter = 0;
				int res = (int)LibC.read( fd, &counter, (UIntPtr)8 );
				if( 8 == res )
					return;
				throw LibC.exception( "EventHandle.reset failed", res );
			}
		}

		public static implicit operator int( EventHandle ff )
		{
			return ff.fd;
		}
		public static implicit operator bool( EventHandle ff )
		{
			return ff.fd >= 0;
		}

		public void dispose()
		{
			if( this.fd < 0 )
				return;
			int fd = this.fd;
			this.fd = -1;
			int res = LibC.close( fd );
			if( 0 == res )
				return;
			throw LibC.exception( $"Error closing event descriptor { fd }", res );
		}

		public void finalize()
		{
			if( fd >= 0 )
			{
				LibC.close( fd );
				fd = -1;
			}
		}

		EventHandle( int fd )
		{
			this.fd = fd;
		}
	}
}