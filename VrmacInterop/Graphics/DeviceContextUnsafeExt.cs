using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Vrmac;

namespace Diligent.Graphics
{
	/// <summary>Couple extension methods for <see cref="IDeviceContext" /> interface.</summary>
	public static class DeviceContextUnsafeExt
	{
		/// <summary>Updates the data in the buffer</summary>
		/// <param name="context">GPU device context</param>
		/// <param name="buffer">Pointer to the buffer to updates</param>
		/// <param name="offset">Offset in bytes from the beginning of the buffer to the update region</param>
		/// <param name="data">data to write to the buffer</param>
		/// <param name="stateTransitionMode">Buffer state transition mode, <seealso cref="ResourceStateTransitionMode" />.</param>
		public static void UpdateBuffer( this IDeviceContext context, IBuffer buffer, int offset, ReadOnlySpan<byte> data, ResourceStateTransitionMode stateTransitionMode )
		{
			unsafe
			{
				fixed ( byte* pointer = &data.GetPinnableReference() )
				{
					context.UpdateBuffer( buffer, offset, data.Length, (IntPtr)pointer, stateTransitionMode );
				}
			}
		}

		/// <summary>Update a CPU-writable buffer by mapping it with write+discard flags, and calling memcpy.</summary>
		public static void writeBuffer<T>( this IDeviceContext context, IBuffer buffer, ref T value ) where T : unmanaged
		{
			int cb = Marshal.SizeOf<T>();
			unsafe
			{
				fixed ( T* pointer = &value )
					context.MapBufferWriteDiscard( buffer, (IntPtr)pointer, cb );
			}
		}

		/// <summary>Update a CPU-writable buffer by mapping it with write+discard flags, and calling memcpy.</summary>
		public static void writeBuffer<T>( this IDeviceContext context, IBuffer buffer, T[] array ) where T : unmanaged
		{
			int cb = Marshal.SizeOf<T>() * array.Length;
			unsafe
			{
				fixed ( T* pointer = array )
					context.MapBufferWriteDiscard( buffer, (IntPtr)pointer, cb );
			}
		}

		/// <summary>Update a CPU-writable buffer by mapping it with write+discard flags, and calling memcpy.</summary>
		public static void writeBuffer( this IDeviceContext context, IBuffer buffer, ReadOnlySpan<byte> data )
		{
			unsafe
			{
				fixed ( byte* pointer = &data.GetPinnableReference() )
					context.MapBufferWriteDiscard( buffer, (IntPtr)pointer, data.Length );
			}
		}

		static Box createBox( ref CRect rect )
		{
			Box updateBox = new Box( false );
			updateBox.MinX = rect.left;
			updateBox.MaxX = rect.right;
			updateBox.MinY = rect.top;
			updateBox.MaxY = rect.bottom;
			return updateBox;
		}

		/// <summary>Overwrite a box of 2D texture with supplied data in system RAM</summary>
		public static void updateTexture<T>( this IDeviceContext context, ITexture texture, ReadOnlySpan<T> source, ref CRect destBox ) where T : unmanaged
		{
			CSize boxSize = destBox.size;
			Debug.Assert( boxSize.cx * boxSize.cy == source.Length );
			TextureSubResData tsrd = new TextureSubResData( false );

			tsrd.Stride = Marshal.SizeOf<T>() * boxSize.cx;
			Box box = createBox( ref destBox );
			unsafe
			{
				fixed ( T* pointer = source )
				{
					tsrd.pData = (IntPtr)pointer;
					context.UpdateTexture( texture, 0, 0, ref box, ref tsrd );
				}
			}
		}

		/// <summary>Copy data from one texture to another</summary>
		public static void copyTexture( this IDeviceContext context, ITexture dest, ITexture source )
		{
			CopyTextureAttribs copyAttribs = new CopyTextureAttribs( false );
			copyAttribs.pSrcTexture = source.nativeCast();
			copyAttribs.pDstTexture = dest.nativeCast();
			context.CopyTexture( ref copyAttribs );
		}
	}
}