using ComLight;
using Diligent.Graphics.Buffers;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>A buffer in native memory you can use to create one or more GPU buffer objects</summary>
	[ComInterface( "218e57e9-c6c9-449e-8d9a-8ee2498ef456", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface iUploadBuffer: IDisposable
	{
		/// <summary>Native pointer you can write into</summary>
		IntPtr getPointer();

		/// <summary>Length, in bytes, of the unmanaged buffer owned by this object. That's the value you have passed to <see cref="IRenderDevice.CreateBufferUploader(uint)" /> method.</summary>
		[RetValIndex] uint getLength();

		/// <summary>A thin wrapper around memcpy. Both length and offset are in bytes.</summary>
		void write( IntPtr source, uint length, uint destOffset = 0 );

		/// <summary>Write a sequence of elements, where source data has padding between elements.</summary>
		/// <remarks>
		/// <para>You can use this method to convert array of structures in system memory into multiple buffers in VRAM.</para>
		/// <para>For optimal performance, size should be one of the following: 1, 2, 4, 8, 12, 16, 20, 24, 28, 32, 48, 64.
		/// Anything else, and the implementation will be calling memcpy() in a loop, which is slower.</para>
		/// </remarks>
		void writeElements( IntPtr source, uint count, uint size, uint stride, uint destOffset = 0 );

		/// <summary>Convert to write-only stream</summary>
		void asStream( [Marshaller( typeof( UploadBufferStreamMarshaller ) )] out Stream stream );

		/// <summary>Upload the buffer to VRAM. lengthBytes can be less than the buffer capacity, if you do that the initial portion of the buffer will be uploaded.</summary>
		[RetValIndex] IBuffer create( uint lengthBytes, [In] ref BufferDesc BuffDesc, [In, MarshalAs( UnmanagedType.LPUTF8Str )] string name = null );
	}
}