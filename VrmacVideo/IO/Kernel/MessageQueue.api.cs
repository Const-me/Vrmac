using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using size_t = System.UIntPtr;
using ssize_t = System.IntPtr;

namespace VrmacVideo.IO
{
	public enum eMessageQueueFlags: int
	{
		None,
		NonBlocking = 0x800,
	}

	// struct mq_attr in C++
	public unsafe struct MessageQueueAttributes
	{
		public eMessageQueueFlags flags;
		public int maxMessages;
		public int messageSize;
		public int currentMessages;
		// Undocumented field below: present in Linux headers, but the documentation is dead silent about that.
		fixed uint reserved[ 4 ];
	}

	/// <summary>Linux file permission bits, these S_IRWXU, S_IRUSR, etc.</summary>
	[Flags]
	public enum eFileAccess: int
	{
		/// <summary>others have execute permission</summary>
		OthersExec = 1,
		/// <summary>others have write permission</summary>
		OthersWrite = 2,
		/// <summary>others have read permission</summary>
		OthersRead = 4,
		/// <summary>others have read, write, and execute permissions</summary>
		OthersAllAccess = 7,

		/// <summary>group has read permission</summary>
		GroupExec = 8,
		/// <summary>group has write permission</summary>
		GroupWrite = 0x10,
		/// <summary>group has execute permission</summary>
		GroupRead = 0x20,
		/// <summary>group has read, write, and execute permissions</summary>
		GroupAllAccess = 0x38,

		/// <summary>user has execute permission</summary>
		OwnerExec = 0x40,
		/// <summary>user has write permission</summary>
		OwnerWrite = 0x80,
		/// <summary>user has read permission</summary>
		OwnerRead = 0x100,
		/// <summary>user (file owner) has read, write, and execute permissions</summary>
		OwnerAllAccess = 0x1C0,

		/// <summary>sticky bit, see inode(7)</summary>
		Sticky = 0x200,
		/// <summary>set-group-ID bit, see inode(7).</summary>
		SetGroupId = 0x400,
		/// <summary>set-user-ID bit</summary>
		SetUserId = 0x800,
	}

	public static class MQ
	{
		const string dll = "librt.so";

		// Message queues API
		[DllImport( dll, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int mq_open( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string name, eFileFlags flags, eFileAccess access, [In] ref MessageQueueAttributes attributes );
		[DllImport( dll, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int mq_close( int fd );
		[DllImport( dll, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		public static extern int mq_unlink( [In, MarshalAs( UnmanagedType.LPUTF8Str )] string name );

		[DllImport( dll, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		static extern unsafe int mq_send( int fd, void* pointer, size_t length, uint priority );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void mq_send<T>( int fd, T message, uint priority = 0 ) where T : unmanaged
		{
			size_t cb = (size_t)Marshal.SizeOf<T>();
			unsafe
			{
				T* ptr = &message;
				int res = mq_send( fd, ptr, cb, priority );
				if( 0 == res )
					return;
				throw LibC.exception( "mq_send", res );
			}
		}

		[DllImport( dll, SetLastError = true, CallingConvention = CallingConvention.Cdecl )]
		static extern unsafe ssize_t mq_receive( int fd, void* pointer, size_t length, out uint priority );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T mq_receive<T>( int fd, out uint priority ) where T : unmanaged
		{
			size_t cb = (size_t)Marshal.SizeOf<T>();
			T result;
			unsafe
			{
				T* ptr = &result;
				int res = (int)mq_receive( fd, ptr, cb, out priority );
				if( res == (int)cb )
					return result;
				throw LibC.exception( "mq_receive", res );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T mq_receive<T>( int fd ) where T : unmanaged
		{
			return mq_receive<T>( fd, out var unused );
		}
	}
}