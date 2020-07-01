using Diligent;
using System;
using System.Runtime.InteropServices;
using VrmacVideo.Linux;

namespace VrmacVideo.IO
{
	/// <summary>We don't want fixed/unsafe every frame, it has overhead. Instead we keep planes in native memory, allocated with Marshal.AllocHGlobal.
	/// We only gonna write a single integer there while playing the video, count of bytes used.</summary>
	struct PlanesArray
	{
		IntPtr pointer;
		public readonly int count;
		const int VIDEO_MAX_PLANES = 8;
		public PlanesArray( int count )
		{
			if( count < 1 || count > VIDEO_MAX_PLANES )
				throw new ArgumentOutOfRangeException();
			// pointer = Marshal.AllocHGlobal( sPlane.size * VIDEO_MAX_PLANES );
			pointer = Marshal.AllocHGlobal( sPlane.size * count );
			if( pointer == default )
				throw new OutOfMemoryException();
			this.count = count;
		}

		public void finalize()
		{
			// That thing is safe to call with nullptr argument, just like free() from C++
			Marshal.FreeHGlobal( pointer );
			pointer = default;
		}

		public static implicit operator IntPtr( PlanesArray pa ) => pa.pointer;

		public Span<sPlane> span => Unsafe.writeSpan<sPlane>( pointer, count );

		public void setBytesUsed( int cb, int planeIndex = 0 )
		{
			if( planeIndex < 0 || planeIndex >= count )
				throw new ArgumentOutOfRangeException();
			Marshal.WriteInt32( pointer + planeIndex * sPlane.size, cb );
		}

		public int getBytesUsed( int planeIndex = 0 )
		{
			if( planeIndex < 0 || planeIndex >= count )
				throw new ArgumentOutOfRangeException();
			return Marshal.ReadInt32( pointer + planeIndex * sPlane.size );
		}
	}
}