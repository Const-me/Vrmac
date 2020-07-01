using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vrmac.Utils;
using size_t = System.UIntPtr;
using ssize_t = System.IntPtr;

namespace VrmacVideo.IO
{
	public static class LibC
	{
		const string libc = "libc.so.6";

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int open( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string fileName, eFileFlags flags = eFileFlags.O_CLOEXEC );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int close( int fd );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static unsafe extern int ioctl( int fd, uint request, void* pointer );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int poll( pollfd* fds, int nfds, int timeout );

		/// <summary>Wait for events on a set of file descriptors. Return value is the count of signaled handles.</summary>
		/// <remarks>If the timeout is zero it will not wait, but will update these bits in <see cref="pollfd.revents" />.</remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static int poll( ReadOnlySpan<pollfd> fileDescriptors, int msTimeout = -1 )
		{
			int ret;
			unsafe
			{
				fixed ( pollfd* pointer = fileDescriptors )
					ret = poll( pointer, fileDescriptors.Length, msTimeout );
			}
			if( ret >= 0 )
				return ret;
			throw exception( "poll", ret );
		}

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int eventfd( uint initval, eEventFdFlags flags );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static unsafe extern ssize_t write( int fd, void* buffer, size_t count );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static unsafe extern ssize_t read( int fd, void* buffer, size_t count );

		public static void read( int fd, Span<byte> span )
		{
			unsafe
			{
				fixed ( byte* p = span )
				{
					int cb = (int)read( fd, p, (size_t)span.Length );
					if( cb < 0 )
						throw exception( "read", cb );
					if( cb != span.Length )
						throw new EndOfStreamException();
				}
			}
		}

		public static readonly IntPtr MAP_FAILED = new IntPtr( -1 );
		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern IntPtr mmap( IntPtr addr, size_t length, eMemoryProtection prot, int flags, int fd, ssize_t off );
		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int munmap( IntPtr addr, size_t length );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		static extern int clock_gettime( eClock clock, out sTimeNano value );

		/// <summary>Get a clock reading, measured since Unix epoch</summary>
		/// <seealso cref="LinuxTime.getTime(TimeSpan)" />
		public static TimeSpan gettime( eClock clock )
		{
			int ret = clock_gettime( clock, out var time );
			if( ret >= 0 )
				return time;

			int errno = Marshal.GetLastWin32Error();
			if( 0 != errno )
				NativeErrorMessages.throwForHR( hresultFromLinux( errno ) );
			throw new ApplicationException();
		}

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int timerfd_create( eClock clock, eTimerCreateFlags flags );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int timerfd_settime( int fd, eTimerSetFlag flags, [In] ref sTimerDesc desc, IntPtr oldDescOrNull = default );

		[DllImport( libc, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		static extern int usleep( uint microseconds );

		public static void sleep( TimeSpan ts )
		{
			uint us = checked((uint)( ts.Ticks / 10 ));
			int res = usleep( us );
			if( 0 == res )
				return;
			throw exception( "usleep", res );
		}

		const int linuxErrorCode = unchecked((int)0xA0010000);

		public static int hresultFromLinux( int errno )
		{
			return errno | linuxErrorCode;
		}

		public const int ENOENT = 2;
		public const int EAGAIN = 11;

		/// <summary>Create a .NET exception from errno global variable</summary>
		public static Exception exception( string what, int returnedValue )
		{
			if( returnedValue == -1 )
			{
				int errno = Marshal.GetLastWin32Error();
				string message = NativeErrorMessages.lookupLinuxError( errno );
				if( null != message )
					return new COMException( $"{ what }: { message }", hresultFromLinux( errno ) );
				return new COMException( $"{ what }: undocumented Linux error code { errno }", hresultFromLinux( errno ) );
			}
			return new ApplicationException( $"{ what }: unexpected result { returnedValue }" );
		}
	}
}