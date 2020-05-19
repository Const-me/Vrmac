// This source file was automatically generated from "Texture.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Defines optimized depth-stencil clear value.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DepthStencilClearValue
	{
		/// <summary>Depth clear value</summary>
		public float Depth;

		/// <summary>Stencil clear value</summary>
		public byte Stencil;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DepthStencilClearValue( bool unused )
		{
			Depth = 1.0f;
			Stencil = 0;
		}
	}

	/// <summary>Defines optimized clear value.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public unsafe struct OptimizedClearValue
	{
		/// <summary>Format</summary>
		public TextureFormat Format;

		/// <summary>Render target clear value</summary>
		public fixed float Color[ 4 ];

		/// <summary>Depth stencil clear value</summary>
		public DepthStencilClearValue DepthStencil;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public OptimizedClearValue( bool unused )
		{
			Format = TextureFormat.Unknown;
			DepthStencil = new DepthStencilClearValue( true );
		}
	}

	/// <summary>Describes data for one subresource</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureSubResData
	{
		/// <summary>
		/// <para>Pointer to the subresource data in CPU memory.</para>
		/// <para>If provided, pSrcBuffer must be null</para>
		/// </summary>
		public IntPtr pData;

		/// <summary>
		/// <para>Pointer to the GPU buffer that contains subresource data.</para>
		/// <para>If provided, pData must be null</para>
		/// </summary>
		public IntPtr pSrcBuffer;

		/// <summary>
		/// <para>When updating data from the buffer (pSrcBuffer is not null),</para>
		/// <para>offset from the beginning of the buffer to the data start</para>
		/// </summary>
		public int SrcOffset;

		/// <summary>For 2D and 3D textures, row stride in bytes</summary>
		public int Stride;

		/// <summary>For 3D textures, depth slice stride in bytes</summary>
		/// <remarks>On OpenGL, this must be a mutliple of Stride</remarks>
		public int DepthStride;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public TextureSubResData( bool unused )
		{
			pData = IntPtr.Zero;
			pSrcBuffer = IntPtr.Zero;
			SrcOffset = 0;
			Stride = 0;
			DepthStride = 0;
		}
	}

	/// <summary>Texture inteface</summary>
	[ComInterface( "a64b0e60-1b5e-4cfd-b880-663a1adcbe98", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface ITexture: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] TextureDesc GetDesc();


		/// <summary>Creates a new texture view</summary>
		/// <param name="ViewDesc">View description. See <see cref="TextureViewDesc" /> for details.</param>
		/// <returns>Address of the memory location where the pointer to the view interface will be written to.</returns>
		/// <remarks>
		/// <para>To create a shader resource view addressing the entire texture, set only TextureViewDesc::ViewType member of the ViewDesc parameter to <see cref="TextureViewType.ShaderResource" /> and leave all other members in
		/// their default values. Using the same method, you can create render target or depth stencil view addressing the largest mip level. If texture view format is <see cref="TextureFormat.Unknown" /> , the view format will
		/// match the texture format. If texture view type is <see cref="TextureViewType.Undefined" /> , the type will match the texture type. If the number of mip levels is 0, and the view type is shader resource, the view will
		/// address all mip levels.</para>
		/// <para>For other view types it will address one mip level. If the number of slices is 0, all slices from FirstArraySlice or FirstDepthSlice will be referenced by the view.</para>
		/// <para>For non-array textures, the only allowed values for the number of slices are 0 and 1. Texture view will contain strong reference to the texture, so the texture will not be destroyed until all views are released.
		/// The function calls AddRef() for the created interface, so it must be released by a call to Release() when it is no longer needed.</para>
		/// </remarks>
		[RetValIndex( 1 )] ITextureView CreateView( [In] ref TextureViewDesc ViewDesc );

		/// <summary>Returns the pointer to the default view.</summary>
		/// <param name="ViewType">Type of the requested view. See <see cref="TextureViewType" /> .</param>
		/// <remarks>The function does not increase the reference counter for the returned interface, so Release() must *NOT* be called.</remarks>
		[RetValIndex( 1 )] ITextureView GetDefaultView( TextureViewType ViewType );

		/// <summary>Returns native texture handle specific to the underlying graphics API</summary>
		[RetValIndex] IntPtr GetNativeHandle();

		/// <summary>Sets the usage state for all texture subresources.</summary>
		/// <remarks>
		/// <para>This method does not perform state transition, but resets the internal texture state to the given value.</para>
		/// <para>This method should be used after the application finished manually managing the texture state and wants to hand over state management back to the engine.</para>
		/// </remarks>
		void SetState( ResourceState State );

		/// <summary>Returns the internal texture state</summary>
		[RetValIndex] ResourceState GetState();
	}
}
