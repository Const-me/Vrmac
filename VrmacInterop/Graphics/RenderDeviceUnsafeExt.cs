using System;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>Couple extension methods for <see cref="IRenderDevice" /> interface.</summary>
	public static class RenderDeviceUnsafeExt
	{
		/// <summary>Create buffer with initial content from the specified structure</summary>
		public static IBuffer CreateBuffer<T>( this IRenderDevice device, BufferDesc desc, ref T data, string name = null ) where T : unmanaged
		{
			unsafe
			{
				fixed ( T* pointer = &data )
					return device.CreateBuffer( ref desc, name, (IntPtr)pointer, Marshal.SizeOf<T>() );
			}
		}

		/// <summary>Create buffer with initial content from array of structures</summary>
		public static IBuffer CreateBuffer<T>( this IRenderDevice device, BufferDesc desc, T[] data, string name = null ) where T : unmanaged
		{
			if( null == data || data.Length <= 0 )
				return device.CreateBuffer( ref desc, name, IntPtr.Zero, 0 );

			unsafe
			{
				int cb = Marshal.SizeOf<T>() * data.Length;
				fixed ( T* pointer = data )
					return device.CreateBuffer( ref desc, name, (IntPtr)pointer, cb );
			}
		}

		/// <summary>Create buffer with initial content from a span</summary>
		public static IBuffer CreateBuffer<T>( this IRenderDevice device, BufferDesc desc, ReadOnlySpan<T> data, string name = null ) where T : unmanaged
		{
			if( null == data || data.Length <= 0 )
				return device.CreateBuffer( ref desc, name, IntPtr.Zero, 0 );

			unsafe
			{
				int cb = Marshal.SizeOf<T>() * data.Length;
				fixed ( T* pointer = data )
					return device.CreateBuffer( ref desc, name, (IntPtr)pointer, cb );
			}
		}

		/// <summary>Create a texture with initial content from system RAM, with 1 subresource.</summary>
		public static ITexture CreateTexture<T>( this IRenderDevice device, ref TextureDesc desc, T[] array, int stride, string name = null ) where T : unmanaged
		{
			TextureSubResData[] srd = new TextureSubResData[ 1 ];
			srd[ 0 ].Stride = stride;
			unsafe
			{
				fixed ( T* sourceData = array )
				{
					srd[ 0 ].pData = (IntPtr)sourceData;
					return device.CreateTexture( ref desc, srd, 1, name );
				}
			}
		}
	}
}