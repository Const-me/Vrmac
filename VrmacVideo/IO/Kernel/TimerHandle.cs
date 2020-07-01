using System;
using System.Runtime.InteropServices;
using Vrmac.Utils;

namespace VrmacVideo.IO
{
	/// <summary>A special file handle exposing waitable timer-like functionality</summary>
	/// <remarks>Implemented for poll API</remarks>
	struct TimerHandle
	{
		public static implicit operator int( TimerHandle ff )
		{
			return ff.fd;
		}

		public static TimerHandle create()
		{
			int fd = LibC.timerfd_create( eClock.Monotonic, eTimerCreateFlags.DisableHandleInheritance );
			if( fd >= 0 )
				return new TimerHandle( fd );
			throwError( $"Unable to create a timer handle", fd );
			throw new ApplicationException();
		}

		public void setTimeout( TimeSpan timeout )
		{
			if( fd < 0 )
				throw new ApplicationException( "The timer was not created" );
			if( timeout.Ticks <= 0 )
				throw new ArgumentOutOfRangeException( "The time is in the past" );

			sTimerDesc desc = default;
			desc.value = timeout;
			int res = LibC.timerfd_settime( fd, eTimerSetFlag.Relative, ref desc );
			if( res >= 0 )
				return;
			throwError( $"Unable to set the time", res );
			throw new ApplicationException();
		}

		public static TimerHandle create( TimeSpan timeout )
		{
			TimerHandle timer = create();
			try
			{
				timer.setTimeout( timeout );
				return timer;
			}
			catch( Exception )
			{
				timer.finalize();
				throw;
			}
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
			throwError( $"Error closing timer handle { fd }", res );
		}

		public void finalize()
		{
			if( fd >= 0 )
			{
				LibC.close( fd );
				fd = -1;
			}
		}

		int fd;

		static void throwError( string what, int returnedValue )
		{
			if( returnedValue == -1 )
			{
				int errno = Marshal.GetLastWin32Error();
				string message = NativeErrorMessages.lookupLinuxError( errno );
				if( null != message )
					throw new COMException( $"{ what }: { message }", LibC.hresultFromLinux( errno ) );
				throw new COMException( $"{ what }: undocumented Linux error code { errno }", LibC.hresultFromLinux( errno ) );
			}
			throw new ApplicationException( $"{ what }: unexpected result { returnedValue }" );
		}

		TimerHandle( int fd )
		{
			this.fd = fd;
		}
	}
}