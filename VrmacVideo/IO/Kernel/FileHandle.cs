using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Vrmac.Utils;
using VrmacVideo.Linux;

namespace VrmacVideo.IO
{
	/// <summary>A wrapper around file handle, used for video decoder device</summary>
	/// <remarks>In Linux everything is a file.
	/// The only thing you can do to that particular file is call IOCTL’s specified in hundreds of pages of documentation, but it’s still a file handle.</remarks>
	internal struct FileHandle
	{
		int fd;
		public static implicit operator int( FileHandle ff )
		{
			return ff.fd;
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
			throw LibC.exception( $"Error closing file descriptor { fd }", res );
		}

		public void finalize()
		{
			if( fd >= 0 )
			{
				LibC.close( fd );
				fd = -1;
			}
		}

		FileHandle( int fd )
		{
			this.fd = fd;
		}

		public static FileHandle invalid => new FileHandle( -1 );

		public static implicit operator bool( FileHandle file ) => file.fd >= 0;

		public static FileHandle openFile( string path, eFileFlags flags )
		{
			int fd = LibC.open( path, flags );
			if( fd >= 0 )
				return new FileHandle( fd );
			throw LibC.exception( $"Unable to open the file \"{ path }\"", fd );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public T read<T>( eControlCode code ) where T : unmanaged
		{
			T result = new T();
			unsafe
			{
				T* pointer = &result;
				int res = LibC.ioctl( fd, (uint)code, pointer );
				if( 0 == res )
					return result;
				handleError( code, res );
				return default;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void call<T>( eControlCode code, ref T structure ) where T : unmanaged
		{
			unsafe
			{
				fixed ( T* pointer = &structure )
				{
					int res = LibC.ioctl( fd, (uint)code, pointer );
					if( 0 == res )
						return;
					handleError( code, res );
				}
			}
		}

		// Should happens rarely, ideally never at all, but the compiler doesn't know that; inline would decrease performance because instructions decoder and micro-ops cache.
		[MethodImpl( MethodImplOptions.NoInlining )]
		static void handleError( eControlCode code, int returnedValue )
		{
			throw LibC.exception( $"I/O control code { code } failed", returnedValue );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool enumerate<T>( eControlCode code, ref T structure ) where T : unmanaged
		{
			unsafe
			{
				fixed ( T* pointer = &structure )
				{
					int res = LibC.ioctl( fd, (uint)code, pointer );
					if( 0 == res )
						return true;
					return handleEnumError( code, res );
				}
			}
		}

		const int EINVAL = 22;

		[MethodImpl( MethodImplOptions.NoInlining )]
		static bool handleEnumError( eControlCode code, int returnedValue )
		{
			if( returnedValue == -1 )
			{
				int errno = Marshal.GetLastWin32Error();
				if( 0 == errno || errno == EINVAL )
					return false;
				string message = NativeErrorMessages.lookupLinuxError( errno );
				if( null != message )
					throw new COMException( $"I/O control code { code } failed: { message }", LibC.hresultFromLinux( errno ) );
				throw new COMException( $"I/O control code { code } failed: undocumented Linux error code { errno }", LibC.hresultFromLinux( errno ) );
			}
			throw new ApplicationException( $"I/O control code { code } failed: unexpected result { returnedValue }" );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool tryCall<T>( eControlCode code, ref T structure ) where T : unmanaged
		{
			unsafe
			{
				fixed ( T* pointer = &structure )
				{
					int res = LibC.ioctl( fd, (uint)code, pointer );
					if( 0 == res )
						return true;
					return false;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool tryCallNonBlock<T>( eControlCode code, ref T structure ) where T : unmanaged
		{
			int res;
			unsafe
			{
				fixed ( T* pointer = &structure )
					res = LibC.ioctl( fd, (uint)code, pointer );
			}
			if( 0 == res )
				return true;

			int errno = Marshal.GetLastWin32Error();
			if( errno == LibC.EAGAIN )
				return false;

			string message = NativeErrorMessages.lookupLinuxError( errno );
			if( null != message )
				throw new COMException( $"I/O control code { code } failed: { message }", LibC.hresultFromLinux( errno ) );
			throw new COMException( $"I/O control code { code } failed: undocumented Linux error code { errno }", LibC.hresultFromLinux( errno ) );
		}

		public IntPtr memoryMapInput( int offset, int length )
		{
			var mappedAddress = LibC.mmap( IntPtr.Zero, (UIntPtr)length, eMemoryProtection.ReadWrite, (int)eMapFlags.Shared, fd, (IntPtr)offset );
			if( mappedAddress != LibC.MAP_FAILED )
				return mappedAddress;
			throw LibC.exception( "memoryMapInput", -1 );
		}

		public IntPtr memoryMapOutput( int offset, int length )
		{
			var mappedAddress = LibC.mmap( IntPtr.Zero, (UIntPtr)length, eMemoryProtection.Read, (int)eMapFlags.Shared, fd, (IntPtr)offset );
			if( mappedAddress != LibC.MAP_FAILED )
				return mappedAddress;
			throw LibC.exception( "memoryMapOutput", -1 );
		}
	}
}