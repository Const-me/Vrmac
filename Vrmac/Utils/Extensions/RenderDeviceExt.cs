using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Vrmac;
using Vrmac.Utils;

namespace Diligent.Graphics
{
	/// <summary>Couple extension methods for <see cref="IRenderDevice" /> interface.</summary>
	public static class RenderDeviceExt
	{
		/// <summary>Create a buffer without initial data</summary>
		public static IBuffer CreateBuffer( this IRenderDevice device, BufferDesc desc, string name = null )
		{
			return device.CreateBuffer( ref desc, name, IntPtr.Zero, 0 );
		}

		/// <summary>Create dynamic buffer for shader constants</summary>
		public static IBuffer CreateDynamicUniformBuffer( this IRenderDevice device, int cb, string name = null )
		{
			BufferDesc CBDesc = new BufferDesc( false );
			CBDesc.uiSizeInBytes = cb;
			CBDesc.Usage = Usage.Dynamic;
			CBDesc.BindFlags = BindFlags.UniformBuffer;
			CBDesc.CPUAccessFlags = CpuAccessFlags.Write;
			return device.CreateBuffer( CBDesc, name );
		}

		/// <summary>Create dynamic buffer for shader constants, sized for the structure passed in generic argument.</summary>
		public static IBuffer CreateDynamicUniformBuffer<T>( this IRenderDevice device, string name = null ) where T : unmanaged
		{
			int cb = Marshal.SizeOf<T>();
			if( 0 != cb % 16 )
				throw new ArgumentException();
			return device.CreateDynamicUniformBuffer( cb, name );
		}

		/// <summary>Create immutable buffer for shader constants.</summary>
		public static IBuffer CreateImmutableUniformBuffer<T>( this IRenderDevice device, ref T data, string name = null ) where T : unmanaged
		{
			int cb = Marshal.SizeOf<T>();
			if( 0 != cb % 16 )
				throw new ArgumentException( $"Constant buffer size must be a multiple of 16 bytes, yet sizeof( { typeof( T ).FullName } ) = { cb }" );
			BufferDesc desc = new BufferDesc( false )
			{
				uiSizeInBytes = cb,
				BindFlags = BindFlags.UniformBuffer,
				Usage = Usage.Static,
			};
			return device.CreateBuffer( desc, ref data, name );
		}

		/// <summary>True if the device uses OpenGL or OpenGL ES backend</summary>
		public static bool isGlDevice( this IRenderDevice device )
		{
			var caps = device.GetDeviceCaps();
			switch( caps.DevType )
			{
				case RenderDeviceType.GL:
				case RenderDeviceType.GLES:
					return true;
			}
			return false;
		}

		/// <summary>Create mesh from arrays.</summary>
		/// <remarks>This is the recommended way of creating them when the data size is small, a few kilobytes.</remarks>
		public static IndexedMesh createIndexedMesh<TVertex, TIndex>( this IRenderDevice device, TVertex[] vertices, TIndex[] indices, string name = null )
			where TVertex : unmanaged
			where TIndex : unmanaged
		{
			return MeshLoader.createIndexed( device, vertices, indices, name );
		}

		/// <summary>Load a binary STL file.</summary>
		/// <seealso href="https://en.wikipedia.org/wiki/STL_%28file_format%29" />
		public static IndexedMesh loadStl( this IRenderDevice device, Stream stream, float? minCosAngle, string name = null )
		{
			return MeshLoader.loadStl( device, stream, minCosAngle, name );
		}

		/// <summary>Load a binary STL file.</summary>
		/// <seealso href="https://en.wikipedia.org/wiki/STL_%28file_format%29" />
		public static Task<IndexedMesh> loadStlAsync( this IRenderDevice device, Stream stream, float? minCosAngle, string name = null )
		{
			if( null == Dispatcher.currentDispatcher )
			{
				throw new ApplicationException( "You must call loadStlAsync on the GUI thread." );
				// CPU-bound heavy lifting is offloaded to the pool, however once it all done we have to resume on the thread which can create GPU resources.
				// To resume on the correct thread, the calling code, here, need to be on the thread with the dispatcher.
			}

			return MeshLoader.loadStlAsync( device, stream, minCosAngle, name );
		}
	}
}