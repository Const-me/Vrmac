using System;
using System.Runtime.InteropServices;
// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member

namespace Diligent.Graphics
{
	[StructLayout( LayoutKind.Sequential )]
	public struct MappedTextureSubresource
	{
		public IntPtr pData;
		public int Stride, DepthStride;

		/* public unsafe ReadOnlySpan<TPixel> readLine<TPixel>( int y ) where TPixel : struct
		{
			int offset = y * Stride;
			int length = Stride / Marshal.SizeOf<TPixel>();
			IntPtr p = pData + offset;
			return new ReadOnlySpan<TPixel>( p.ToPointer(), length );
		}

		public unsafe ReadOnlySpan<TPixel> readLine<TPixel>( int y, int z ) where TPixel : struct
		{
			int offset = y * Stride + z * DepthStride;
			int length = Stride / Marshal.SizeOf<TPixel>();
			IntPtr p = pData + offset;
			return new ReadOnlySpan<TPixel>( p.ToPointer(), length );
		}

		public unsafe Span<TPixel> writeLine<TPixel>( int y ) where TPixel : struct
		{
			int offset = y * Stride;
			int length = Stride / Marshal.SizeOf<TPixel>();
			IntPtr p = pData + offset;
			return new Span<TPixel>( p.ToPointer(), length );
		}

		public unsafe Span<TPixel> writeLine<TPixel>( int y, int z ) where TPixel : struct
		{
			int offset = y * Stride + z * DepthStride;
			int length = Stride / Marshal.SizeOf<TPixel>();
			IntPtr p = pData + offset;
			return new Span<TPixel>( p.ToPointer(), length );
		} */
	}
}