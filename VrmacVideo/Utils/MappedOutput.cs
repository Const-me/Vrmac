using Diligent;
using System;
using System.IO;
using Vrmac;
using VrmacVideo.IO;

namespace VrmacVideo
{
	/// <summary>Utility class to save luma channel from NV12 into grayscale TGA format</summary>
	struct MappedOutput
	{
		readonly int bufferIndex;
		readonly int mappedLength;
		readonly IntPtr mappedAddress;
		readonly int stride;
		readonly CSize size;

		public MappedOutput( int idx, int length, IntPtr ptr, int stride, CSize size )
		{
			bufferIndex = idx;
			mappedLength = length;
			mappedAddress = ptr;
			this.stride = stride;
			this.size = size;
		}

		ReadOnlySpan<byte> span => Unsafe.readSpan<byte>( mappedAddress, mappedLength );

		public void saveLuma( string path )
		{
			using( var f = File.Create( path ) )
			{
				ReadOnlySpan<byte> src = span.Slice( 0, stride * size.cy );
				TrueVision.saveGrayscale( f, src, size, stride );
			}
		}

		public void finalize()
		{
			if( mappedAddress == default )
				return;
			int returnedValue = LibC.munmap( mappedAddress, (UIntPtr)mappedLength );
			if( 0 == returnedValue )
				return;
			var ex = LibC.exception( "MappedOutput.finalize", returnedValue );
			Logger.logWarning( "{0}", ex.Message );
		}
	}
}