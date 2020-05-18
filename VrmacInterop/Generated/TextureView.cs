// This source file was automatically generated from "TextureView.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>Describes allowed unordered access view mode</summary>
	[Flags]
	public enum UavAccessFlag : byte
	{
		/// <summary>Access mode is unspecified</summary>
		Unspecified = 0x0,
		/// <summary>Allow read operations on the UAV</summary>
		FlagRead = 0x1,
		/// <summary>Allow write operations on the UAV</summary>
		FlagWrite = 0x2,
		/// <summary>Allow read and write operations on the UAV</summary>
		FlagReadWrite = 3
	}

	/// <summary>Texture view flags</summary>
	[Flags]
	public enum TextureViewFlags : byte
	{
		/// <summary>No flags</summary>
		None = 0x0,
		/// <summary>
		/// <para>Allow automatic mipmap generation for this view.</para>
		/// <para>This flag is only allowed for TEXTURE_VIEW_SHADER_RESOURCE view type.</para>
		/// <para>The texture must be created with MISC_TEXTURE_FLAG_GENERATE_MIPS flag.</para>
		/// </summary>
		AllowMipMapGeneration = 0x1
	}

	/// <summary>Texture view description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureViewDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		/// <summary>Describes the texture view type, see Diligent::TEXTURE_VIEW_TYPE for details.</summary>
		public TextureViewType ViewType;

		/// <summary>
		/// <para>View interpretation of the original texture. For instance,</para>
		/// <para>one slice of a 2D texture array can be viewed as a 2D texture.</para>
		/// <para>See Diligent::RESOURCE_DIMENSION for a list of texture types.</para>
		/// <para>If default value Diligent::RESOURCE_DIM_UNDEFINED is provided,</para>
		/// <para>the view type will match the type of the referenced texture.</para>
		/// </summary>
		public ResourceDimension TextureDim;

		/// <summary>
		/// <para>View format. If default value Diligent::TEX_FORMAT_UNKNOWN is provided,</para>
		/// <para>the view format will match the referenced texture format.</para>
		/// </summary>
		public TextureFormat Format;

		/// <summary>Most detailed mip level to use</summary>
		public int MostDetailedMip;

		/// <summary>
		/// <para>Total number of mip levels for the view of the texture.</para>
		/// <para>Render target and depth stencil views can address only one mip level.</para>
		/// <para>If 0 is provided, then for a shader resource view all mip levels will be</para>
		/// <para>referenced, and for a render target or a depth stencil view, one mip level</para>
		/// <para>will be referenced.</para>
		/// </summary>
		public int NumMipLevels;

		/// <summary>For a texture array, first array slice to address in the view</summary>
		public int FirstArraySlice;

		/// <summary>For a 3D texture, first depth slice to address the view</summary>
		public int FirstDepthSlice;

		/// <summary>
		/// <para>For an unordered access view, allowed access flags. See Diligent::UAV_ACCESS_FLAG</para>
		/// <para>for details.</para>
		/// </summary>
		public UavAccessFlag AccessFlags;

		/// <summary>Texture view flags, see Diligent::TEXTURE_VIEW_FLAGS.</summary>
		public TextureViewFlags Flags;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public TextureViewDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			ViewType = TextureViewType.Undefined;
			TextureDim = ResourceDimension.Undefined;
			Format = TextureFormat.Unknown;
			MostDetailedMip = 0;
			NumMipLevels = 0;
			FirstArraySlice = 0;
			FirstDepthSlice = default( int );
			AccessFlags = UavAccessFlag.Unspecified;
			Flags = TextureViewFlags.None;
		}
	}

	/// <remarks>
	/// <para>To create a texture view, call ITexture::CreateView().</para>
	/// <para>Texture view holds strong references to the texture. The texture</para>
	/// <para>will not be destroyed until all views are released.</para>
	/// <para>The texture view will also keep a strong reference to the texture sampler,</para>
	/// <para>if any is set.</para>
	/// </remarks>
	[ComInterface( "5b2ea04e-8128-45e4-aa4d-6dc7e70dc424", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface ITextureView: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] TextureViewDesc GetDesc();


		/// <summary>Sets the texture sampler to use for filtering operations when accessing a texture from shaders. Only shader resource views can be assigned a sampler.</summary>
		/// <remarks>The view will keep strong reference to the sampler.</remarks>
		void SetSampler( ISampler pSampler );

		/// <summary>Returns the pointer to the sampler object set by the ITextureView::SetSampler().</summary>
		/// <remarks>The method does *NOT* call AddRef() on the returned interface, so Release() must not be called.</remarks>
		[RetValIndex] ISampler GetSampler();

		/// <summary>Returns the pointer to the referenced texture object.</summary>
		/// <remarks>The method does *NOT* call AddRef() on the returned interface, so Release() must not be called.</remarks>
		[RetValIndex] ITexture GetTexture();
	}
}
