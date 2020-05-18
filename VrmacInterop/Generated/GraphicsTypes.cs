// This source file was automatically generated from "GraphicsTypes.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>
	/// <para>This enumeration describes value type. It is used by</para>
	/// <para>- BufferDesc structure to describe value type of a formatted buffer</para>
	/// <para>- DrawAttribs structure to describe index type for an indexed draw call</para>
	/// </summary>
	public enum GpuValueType : byte
	{
		/// <summary>Undefined type</summary>
		Undefined = 0,
		/// <summary>Signed 8-bit integer</summary>
		Int8 = 1,
		/// <summary>Signed 16-bit integer</summary>
		Int16 = 2,
		/// <summary>Signed 32-bit integer</summary>
		Int32 = 3,
		/// <summary>Unsigned 8-bit integer</summary>
		Uint8 = 4,
		/// <summary>Unsigned 16-bit integer</summary>
		Uint16 = 5,
		/// <summary>Unsigned 32-bit integer</summary>
		Uint32 = 6,
		/// <summary>Half-precision 16-bit floating point</summary>
		Float16 = 7,
		/// <summary>Full-precision 32-bit floating point</summary>
		Float32 = 8,
		/// <summary>Helper value storing total number of types in the enumeration</summary>
		NumTypes = 9
	}

	/// <summary>[D3D11_BIND_FLAG]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476085(v=vs.85).aspx</summary>
	/// <remarks>
	/// <para>This enumeration describes which parts of the pipeline a resource can be bound to.</para>
	/// <para>It generally mirrors [D3D11_BIND_FLAG][] enumeration. It is used by</para>
	/// <para>- BufferDesc to describe bind flags for a buffer</para>
	/// <para>- TextureDesc to describe bind flags for a texture</para>
	/// </remarks>
	[Flags]
	public enum BindFlags : uint
	{
		/// <summary>Undefined binding</summary>
		None = 0x0,
		/// <summary>A buffer can be bound as a vertex buffer</summary>
		VertexBuffer = 0x1,
		/// <summary>A buffer can be bound as an index buffer</summary>
		IndexBuffer = 0x2,
		/// <summary>A buffer can be bound as a uniform buffer</summary>
		UniformBuffer = 0x4,
		/// <summary>A buffer or a texture can be bound as a shader resource</summary>
		ShaderResource = 0x8,
		/// <summary>A buffer can be bound as a target for stream output stage</summary>
		StreamOutput = 0x10,
		/// <summary>A texture can be bound as a render target</summary>
		RenderTarget = 0x20,
		/// <summary>A texture can be bound as a depth-stencil target</summary>
		DepthStencil = 0x40,
		/// <summary>A buffer or a texture can be bound as an unordered access view</summary>
		UnorderedAccess = 0x80,
		/// <summary>A buffer can be bound as the source buffer for indirect draw commands</summary>
		IndirectDrawArgs = 0x100
	}

	/// <summary>
	/// <para>[D3D11_USAGE]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476259(v=vs.85).aspx</para>
	/// <para>This enumeration describes expected resource usage. It generally mirrors [D3D11_USAGE] enumeration.</para>
	/// <para>The enumeration is used by</para>
	/// <para>- BufferDesc to describe usage for a buffer</para>
	/// <para>- TextureDesc to describe usage for a texture</para>
	/// </summary>
	public enum Usage : byte
	{
		/// <summary>
		/// <para>A resource that can only be read by the GPU. It cannot be written by the GPU,</para>
		/// <para>and cannot be accessed at all by the CPU. This type of resource must be initialized</para>
		/// <para>when it is created, since it cannot be changed after creation.</para>
		/// <para>D3D11 Counterpart: D3D11_USAGE_IMMUTABLE. OpenGL counterpart: GL_STATIC_DRAW</para>
		/// </summary>
		Static = 0,
		/// <summary>
		/// <para>A resource that requires read and write access by the GPU and can also be occasionally</para>
		/// <para>written by the CPU.</para>
		/// <para>D3D11 Counterpart: D3D11_USAGE_DEFAULT. OpenGL counterpart: GL_DYNAMIC_DRAW</para>
		/// </summary>
		Default = 1,
		/// <summary>
		/// <para>A resource that can be read by the GPU and written at least once per frame by the CPU.</para>
		/// <para>D3D11 Counterpart: D3D11_USAGE_DYNAMIC. OpenGL counterpart: GL_STREAM_DRAW</para>
		/// </summary>
		Dynamic = 2,
		/// <summary>
		/// <para>A resource that facilitates transferring data from GPU to CPU.</para>
		/// <para>D3D11 Counterpart: D3D11_USAGE_STAGING. OpenGL counterpart: GL_DYNAMIC_READ</para>
		/// </summary>
		Staging = 3
	}

	/// <summary>
	/// <para>The enumeration is used by</para>
	/// <para>- BufferDesc to describe CPU access mode for a buffer</para>
	/// <para>- TextureDesc to describe CPU access mode for a texture</para>
	/// </summary>
	/// <remarks>Only USAGE_DYNAMIC resources can be mapped</remarks>
	[Flags]
	public enum CpuAccessFlags : byte
	{
		/// <summary>No CPU access</summary>
		None = 0x0,
		/// <summary>A resource can be mapped for reading</summary>
		Read = 0x1,
		/// <summary>A resource can be mapped for writing</summary>
		Write = 0x2
	}

	/// <summary>
	/// <para>[D3D11_MAP]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476181(v=vs.85).aspx</para>
	/// <para>Describes how a mapped resource will be accessed. This enumeration generally</para>
	/// <para>mirrors [D3D11_MAP][] enumeration. It is used by</para>
	/// <para>- IBuffer::Map to describe buffer mapping type</para>
	/// <para>- ITexture::Map to describe texture mapping type</para>
	/// </summary>
	public enum MapType : byte
	{
		/// <summary>
		/// <para>The resource is mapped for reading.</para>
		/// <para>D3D11 counterpart: D3D11_MAP_READ. OpenGL counterpart: GL_MAP_READ_BIT</para>
		/// </summary>
		Read = 0x1,
		/// <summary>
		/// <para>The resource is mapped for writing.</para>
		/// <para>D3D11 counterpart: D3D11_MAP_WRITE. OpenGL counterpart: GL_MAP_WRITE_BIT</para>
		/// </summary>
		Write = 0x2,
		/// <summary>
		/// <para>The resource is mapped for reading and writing.</para>
		/// <para>D3D11 counterpart: D3D11_MAP_READ_WRITE. OpenGL counterpart: GL_MAP_WRITE_BIT | GL_MAP_READ_BIT</para>
		/// </summary>
		ReadWrite = 0x3
	}

	/// <summary>
	/// <para>Describes special arguments for a map operation.</para>
	/// <para>This enumeration is used by</para>
	/// <para>- IBuffer::Map to describe buffer mapping flags</para>
	/// <para>- ITexture::Map to describe texture mapping flags</para>
	/// </summary>
	[Flags]
	public enum MapFlags : byte
	{
		None = 0x0,
		/// <summary>
		/// <para>Specifies that map operation should not wait until previous command that</para>
		/// <para>using the same resource completes. Map returns null pointer if the resource</para>
		/// <para>is still in use.</para>
		/// <para>D3D11 counterpart:  D3D11_MAP_FLAG_DO_NOT_WAIT</para>
		/// </summary>
		/// <remarks>: OpenGL does not have corresponding flag, so a buffer will always be mapped</remarks>
		DoNotWait = 0x1,
		/// <summary>
		/// <para>Previous contents of the resource will be undefined. This flag is only compatible with MAP_WRITE</para>
		/// <para>D3D11 counterpart: D3D11_MAP_WRITE_DISCARD. OpenGL counterpart: GL_MAP_INVALIDATE_BUFFER_BIT</para>
		/// </summary>
		/// <remarks>OpenGL implementation may orphan a buffer instead</remarks>
		Discard = 0x2,
		/// <summary>
		/// <para>The system will not synchronize pending operations before mapping the buffer. It is responsibility</para>
		/// <para>of the application to make sure that the buffer contents is not overwritten while it is in use by</para>
		/// <para>the GPU.</para>
		/// <para>D3D11 counterpart:  D3D11_MAP_WRITE_NO_OVERWRITE. OpenGL counterpart: GL_MAP_UNSYNCHRONIZED_BIT</para>
		/// </summary>
		NoOverwrite = 0x4
	}

	/// <summary>
	/// <para>This enumeration is used by</para>
	/// <para>- TextureDesc to describe texture type</para>
	/// <para>- TextureViewDesc to describe texture view type</para>
	/// </summary>
	public enum ResourceDimension : byte
	{
		/// <summary>Texture type undefined</summary>
		Undefined = 0,
		/// <summary>Buffer</summary>
		Buffer = 1,
		/// <summary>One-dimensional texture</summary>
		Tex1d = 2,
		/// <summary>One-dimensional texture array</summary>
		Tex1dArray = 3,
		/// <summary>Two-dimensional texture</summary>
		Tex2d = 4,
		/// <summary>Two-dimensional texture array</summary>
		Tex2dArray = 5,
		/// <summary>Three-dimensional texture</summary>
		Tex3d = 6,
		/// <summary>Cube-map texture</summary>
		TexCube = 7,
		/// <summary>Cube-map array texture</summary>
		TexCubeArray = 8,
		/// <summary>Helper value that stores the total number of texture types in the enumeration</summary>
		NumDimensions = 9
	}

	/// <summary>
	/// <para>This enumeration describes allowed view types for a texture view. It is used by TextureViewDesc</para>
	/// <para>structure.</para>
	/// </summary>
	public enum TextureViewType : byte
	{
		/// <summary>Undefined view type</summary>
		Undefined = 0,
		/// <summary>
		/// <para>A texture view will define a shader resource view that will be used</para>
		/// <para>as the source for the shader read operations</para>
		/// </summary>
		ShaderResource = 1,
		/// <summary>
		/// <para>A texture view will define a render target view that will be used</para>
		/// <para>as the target for rendering operations</para>
		/// </summary>
		RenderTarget = 2,
		/// <summary>
		/// <para>A texture view will define a depth stencil view that will be used</para>
		/// <para>as the target for rendering operations</para>
		/// </summary>
		DepthStencil = 3,
		/// <summary>
		/// <para>A texture view will define an unordered access view that will be used</para>
		/// <para>for unordered read/write operations from the shaders</para>
		/// </summary>
		UnorderedAccess = 4,
		/// <summary>Helper value that stores that total number of texture views</summary>
		NumViews = 5
	}

	/// <summary>
	/// <para>This enumeration describes allowed view types for a buffer view. It is used by BufferViewDesc</para>
	/// <para>structure.</para>
	/// </summary>
	public enum BufferViewType : byte
	{
		/// <summary>Undefined view type</summary>
		Undefined = 0,
		/// <summary>
		/// <para>A buffer view will define a shader resource view that will be used</para>
		/// <para>as the source for the shader read operations</para>
		/// </summary>
		ShaderResource = 1,
		/// <summary>
		/// <para>A buffer view will define an unordered access view that will be used</para>
		/// <para>for unordered read/write operations from the shaders</para>
		/// </summary>
		UnorderedAccess = 2,
		/// <summary>Helper value that stores that total number of buffer views</summary>
		NumViews = 3
	}

	/// <summary>
	/// <para>This enumeration describes available texture formats and generally mirrors DXGI_FORMAT enumeration.</para>
	/// <para>The table below provides detailed information on each format. Most of the formats are widely supported</para>
	/// <para>by all modern APIs (DX10+, OpenGL3.3+ and OpenGLES3.0+). Specific requirements are additionally indicated.</para>
	/// </summary>
	/// <remarks>
	/// <para>DXGI_FORMAT enumeration on MSDN,</para>
	/// <para>OpenGL Texture Formats</para>
	/// </remarks>
	public enum TextureFormat : ushort
	{
		/// <summary>Unknown format</summary>
		Unknown = 0,
		/// <summary>
		/// <para>Four-component 128-bit typeless format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32A32_TYPELESS. OpenGL does not have direct counterpart, GL_RGBA32F is used.</para>
		/// </summary>
		Rgba32Typeless = 1,
		/// <summary>
		/// <para>Four-component 128-bit floating-point format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32A32_FLOAT. OpenGL counterpart: GL_RGBA32F.</para>
		/// </summary>
		Rgba32Float = 2,
		/// <summary>
		/// <para>Four-component 128-bit unsigned-integer format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32A32_UINT. OpenGL counterpart: GL_RGBA32UI.</para>
		/// </summary>
		Rgba32Uint = 3,
		/// <summary>
		/// <para>Four-component 128-bit signed-integer format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32A32_SINT. OpenGL counterpart: GL_RGBA32I.</para>
		/// </summary>
		Rgba32Sint = 4,
		/// <summary>
		/// <para>Three-component 96-bit typeless format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32_TYPELESS. OpenGL does not have direct counterpart, GL_RGB32F is used.</para>
		/// </summary>
		/// <remarks>This format has weak hardware support and is not recommended</remarks>
		Rgb32Typeless = 5,
		/// <summary>
		/// <para>Three-component 96-bit floating-point format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32_FLOAT. OpenGL counterpart: GL_RGB32F.</para>
		/// </summary>
		/// <remarks>This format has weak hardware support and is not recommended</remarks>
		Rgb32Float = 6,
		/// <summary>
		/// <para>Three-component 96-bit unsigned-integer format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32_UINT. OpenGL counterpart: GL_RGB32UI.</para>
		/// </summary>
		/// <remarks>This format has weak hardware support and is not recommended</remarks>
		Rgb32Uint = 7,
		/// <summary>
		/// <para>Three-component 96-bit signed-integer format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32B32_SINT. OpenGL counterpart: GL_RGB32I.</para>
		/// </summary>
		/// <remarks>This format has weak hardware support and is not recommended</remarks>
		Rgb32Sint = 8,
		/// <summary>
		/// <para>Four-component 64-bit typeless format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16B16A16_TYPELESS. OpenGL does not have direct counterpart, GL_RGBA16F is used.</para>
		/// </summary>
		Rgba16Typeless = 9,
		/// <summary>
		/// <para>Four-component 64-bit half-precision floating-point format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16B16A16_FLOAT. OpenGL counterpart: GL_RGBA16F.</para>
		/// </summary>
		Rgba16Float = 10,
		/// <summary>
		/// <para>Four-component 64-bit unsigned-normalized-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16B16A16_UNORM. OpenGL counterpart: GL_RGBA16.</para>
		/// <para>[GL_EXT_texture_norm16]: https://www.khronos.org/registry/gles/extensions/EXT/EXT_texture_norm16.txt</para>
		/// <para>OpenGLES: [GL_EXT_texture_norm16][] extension is required</para>
		/// </summary>
		Rgba16Unorm = 11,
		/// <summary>
		/// <para>Four-component 64-bit unsigned-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16B16A16_UINT. OpenGL counterpart: GL_RGBA16UI.</para>
		/// </summary>
		Rgba16Uint = 12,
		/// <summary>
		/// <para>[GL_EXT_texture_norm16]: https://www.khronos.org/registry/gles/extensions/EXT/EXT_texture_norm16.txt</para>
		/// <para>Four-component 64-bit signed-normalized-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16B16A16_SNORM. OpenGL counterpart: GL_RGBA16_SNORM.</para>
		/// <para>[GL_EXT_texture_norm16]: https://www.khronos.org/registry/gles/extensions/EXT/EXT_texture_norm16.txt</para>
		/// <para>OpenGLES: [GL_EXT_texture_norm16][] extension is required</para>
		/// </summary>
		Rgba16Snorm = 13,
		/// <summary>
		/// <para>Four-component 64-bit signed-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16B16A16_SINT. OpenGL counterpart: GL_RGBA16I.</para>
		/// </summary>
		Rgba16Sint = 14,
		/// <summary>
		/// <para>Two-component 64-bit typeless format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32_TYPELESS. OpenGL does not have direct counterpart, GL_RG32F is used.</para>
		/// </summary>
		Rg32Typeless = 15,
		/// <summary>
		/// <para>Two-component 64-bit floating-point format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32_FLOAT. OpenGL counterpart: GL_RG32F.</para>
		/// </summary>
		Rg32Float = 16,
		/// <summary>
		/// <para>Two-component 64-bit unsigned-integer format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32_UINT. OpenGL counterpart: GL_RG32UI.</para>
		/// </summary>
		Rg32Uint = 17,
		/// <summary>
		/// <para>Two-component 64-bit signed-integer format with 32-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G32_SINT. OpenGL counterpart: GL_RG32I.</para>
		/// </summary>
		Rg32Sint = 18,
		/// <summary>
		/// <para>Two-component 64-bit typeless format with 32-bits for R channel and 8 bits for G channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32G8X24_TYPELESS. OpenGL does not have direct counterpart, GL_DEPTH32F_STENCIL8 is used.</para>
		/// </summary>
		R32g8x24Typeless = 19,
		/// <summary>
		/// <para>Two-component 64-bit format with 32-bit floating-point depth channel and 8-bit stencil channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_D32_FLOAT_S8X24_UINT. OpenGL counterpart: GL_DEPTH32F_STENCIL8.</para>
		/// </summary>
		D32FloatS8x24Uint = 20,
		/// <summary>
		/// <para>Two-component 64-bit format with 32-bit floating-point R channel and 8+24-bits of typeless data.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS. OpenGL does not have direct counterpart, GL_DEPTH32F_STENCIL8 is used.</para>
		/// </summary>
		R32FloatX8x24Typeless = 21,
		/// <summary>
		/// <para>Two-component 64-bit format with 32-bit typeless data and 8-bit G channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_X32_TYPELESS_G8X24_UINT</para>
		/// </summary>
		/// <remarks>This format is currently not implemented in OpenGL version</remarks>
		X32TypelessG8x24Uint = 22,
		/// <summary>
		/// <para>Four-component 32-bit typeless format with 10 bits for RGB and 2 bits for alpha channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R10G10B10A2_TYPELESS. OpenGL does not have direct counterpart, GL_RGB10_A2 is used.</para>
		/// </summary>
		Rgb10a2Typeless = 23,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-normalized-integer format with 10 bits for each color and 2 bits for alpha channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R10G10B10A2_UNORM. OpenGL counterpart: GL_RGB10_A2.</para>
		/// </summary>
		Rgb10a2Unorm = 24,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-integer format with 10 bits for each color and 2 bits for alpha channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R10G10B10A2_UINT. OpenGL counterpart: GL_RGB10_A2UI.</para>
		/// </summary>
		Rgb10a2Uint = 25,
		/// <summary>
		/// <para>Three-component 32-bit format encoding three partial precision channels using 11 bits for red and green and 10 bits for blue channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R11G11B10_FLOAT. OpenGL counterpart: GL_R11F_G11F_B10F.</para>
		/// </summary>
		R11g11b10Float = 26,
		/// <summary>
		/// <para>Four-component 32-bit typeless format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8B8A8_TYPELESS. OpenGL does not have direct counterpart, GL_RGBA8 is used.</para>
		/// </summary>
		Rgba8Typeless = 27,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-normalized-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8B8A8_UNORM. OpenGL counterpart: GL_RGBA8.</para>
		/// </summary>
		Rgba8Unorm = 28,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-normalized-integer sRGB format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8B8A8_UNORM_SRGB. OpenGL counterpart: GL_SRGB8_ALPHA8.</para>
		/// </summary>
		Rgba8UnormSrgb = 29,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8B8A8_UINT. OpenGL counterpart: GL_RGBA8UI.</para>
		/// </summary>
		Rgba8Uint = 30,
		/// <summary>
		/// <para>Four-component 32-bit signed-normalized-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8B8A8_SNORM. OpenGL counterpart: GL_RGBA8_SNORM.</para>
		/// </summary>
		Rgba8Snorm = 31,
		/// <summary>
		/// <para>Four-component 32-bit signed-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8B8A8_SINT. OpenGL counterpart: GL_RGBA8I.</para>
		/// </summary>
		Rgba8Sint = 32,
		/// <summary>
		/// <para>Two-component 32-bit typeless format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16_TYPELESS. OpenGL does not have direct counterpart, GL_RG16F is used.</para>
		/// </summary>
		Rg16Typeless = 33,
		/// <summary>
		/// <para>Two-component 32-bit half-precision floating-point format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16_FLOAT. OpenGL counterpart: GL_RG16F.</para>
		/// </summary>
		Rg16Float = 34,
		/// <summary>
		/// <para>Two-component 32-bit unsigned-normalized-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16_UNORM. OpenGL counterpart: GL_RG16.</para>
		/// <para>[GL_EXT_texture_norm16]: https://www.khronos.org/registry/gles/extensions/EXT/EXT_texture_norm16.txt</para>
		/// <para>OpenGLES: [GL_EXT_texture_norm16][] extension is required</para>
		/// </summary>
		Rg16Unorm = 35,
		/// <summary>
		/// <para>Two-component 32-bit unsigned-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16_UINT. OpenGL counterpart: GL_RG16UI.</para>
		/// </summary>
		Rg16Uint = 36,
		/// <summary>
		/// <para>Two-component 32-bit signed-normalized-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16_SNORM. OpenGL counterpart: GL_RG16_SNORM.</para>
		/// <para>[GL_EXT_texture_norm16]: https://www.khronos.org/registry/gles/extensions/EXT/EXT_texture_norm16.txt</para>
		/// <para>OpenGLES: [GL_EXT_texture_norm16][] extension is required</para>
		/// </summary>
		Rg16Snorm = 37,
		/// <summary>
		/// <para>Two-component 32-bit signed-integer format with 16-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16G16_SINT. OpenGL counterpart: GL_RG16I.</para>
		/// </summary>
		Rg16Sint = 38,
		/// <summary>
		/// <para>Single-component 32-bit typeless format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32_TYPELESS. OpenGL does not have direct counterpart, GL_R32F is used.</para>
		/// </summary>
		R32Typeless = 39,
		/// <summary>
		/// <para>Single-component 32-bit floating-point depth format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_D32_FLOAT. OpenGL counterpart: GL_DEPTH_COMPONENT32F.</para>
		/// </summary>
		D32Float = 40,
		/// <summary>
		/// <para>Single-component 32-bit floating-point format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32_FLOAT. OpenGL counterpart: GL_R32F.</para>
		/// </summary>
		R32Float = 41,
		/// <summary>
		/// <para>Single-component 32-bit unsigned-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32_UINT. OpenGL counterpart: GL_R32UI.</para>
		/// </summary>
		R32Uint = 42,
		/// <summary>
		/// <para>Single-component 32-bit signed-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R32_SINT. OpenGL counterpart: GL_R32I.</para>
		/// </summary>
		R32Sint = 43,
		/// <summary>
		/// <para>Two-component 32-bit typeless format with 24 bits for R and 8 bits for G channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R24G8_TYPELESS. OpenGL does not have direct counterpart, GL_DEPTH24_STENCIL8 is used.</para>
		/// </summary>
		R24g8Typeless = 44,
		/// <summary>
		/// <para>Two-component 32-bit format with 24 bits for unsigned-normalized-integer depth and 8 bits for stencil.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_D24_UNORM_S8_UINT. OpenGL counterpart: GL_DEPTH24_STENCIL8.</para>
		/// </summary>
		D24UnormS8Uint = 45,
		/// <summary>
		/// <para>Two-component 32-bit format with 24 bits for unsigned-normalized-integer data and 8 bits of unreferenced data.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R24_UNORM_X8_TYPELESS. OpenGL does not have direct counterpart, GL_DEPTH24_STENCIL8 is used.</para>
		/// </summary>
		R24UnormX8Typeless = 46,
		/// <summary>
		/// <para>Two-component 32-bit format with 24 bits of unreferenced data and 8 bits of unsigned-integer data.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_X24_TYPELESS_G8_UINT</para>
		/// </summary>
		/// <remarks>This format is currently not implemented in OpenGL version</remarks>
		X24TypelessG8Uint = 47,
		/// <summary>
		/// <para>Two-component 16-bit typeless format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8_TYPELESS. OpenGL does not have direct counterpart, GL_RG8 is used.</para>
		/// </summary>
		Rg8Typeless = 48,
		/// <summary>
		/// <para>Two-component 16-bit unsigned-normalized-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8_UNORM. OpenGL counterpart: GL_RG8.</para>
		/// </summary>
		Rg8Unorm = 49,
		/// <summary>
		/// <para>Two-component 16-bit unsigned-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8_UINT. OpenGL counterpart: GL_RG8UI.</para>
		/// </summary>
		Rg8Uint = 50,
		/// <summary>
		/// <para>Two-component 16-bit signed-normalized-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8_SNORM. OpenGL counterpart: GL_RG8_SNORM.</para>
		/// </summary>
		Rg8Snorm = 51,
		/// <summary>
		/// <para>Two-component 16-bit signed-integer format with 8-bit channels.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8_SINT. OpenGL counterpart: GL_RG8I.</para>
		/// </summary>
		Rg8Sint = 52,
		/// <summary>
		/// <para>Single-component 16-bit typeless format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16_TYPELESS. OpenGL does not have direct counterpart, GL_R16F is used.</para>
		/// </summary>
		R16Typeless = 53,
		/// <summary>
		/// <para>Single-component 16-bit half-precisoin floating-point format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16_FLOAT. OpenGL counterpart: GL_R16F.</para>
		/// </summary>
		R16Float = 54,
		/// <summary>
		/// <para>Single-component 16-bit unsigned-normalized-integer depth format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_D16_UNORM. OpenGL counterpart: GL_DEPTH_COMPONENT16.</para>
		/// </summary>
		D16Unorm = 55,
		/// <summary>
		/// <para>Single-component 16-bit unsigned-normalized-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16_UNORM. OpenGL counterpart: GL_R16.</para>
		/// <para>[GL_EXT_texture_norm16]: https://www.khronos.org/registry/gles/extensions/EXT/EXT_texture_norm16.txt</para>
		/// <para>OpenGLES: [GL_EXT_texture_norm16][] extension is required</para>
		/// </summary>
		R16Unorm = 56,
		/// <summary>
		/// <para>Single-component 16-bit unsigned-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16_UINT. OpenGL counterpart: GL_R16UI.</para>
		/// </summary>
		R16Uint = 57,
		/// <summary>
		/// <para>Single-component 16-bit signed-normalized-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16_SNORM. OpenGL counterpart: GL_R16_SNORM.</para>
		/// <para>[GL_EXT_texture_norm16]: https://www.khronos.org/registry/gles/extensions/EXT/EXT_texture_norm16.txt</para>
		/// <para>OpenGLES: [GL_EXT_texture_norm16][] extension is required</para>
		/// </summary>
		R16Snorm = 58,
		/// <summary>
		/// <para>Single-component 16-bit signed-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R16_SINT. OpenGL counterpart: GL_R16I.</para>
		/// </summary>
		R16Sint = 59,
		/// <summary>
		/// <para>Single-component 8-bit typeless format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8_TYPELESS. OpenGL does not have direct counterpart, GL_R8 is used.</para>
		/// </summary>
		R8Typeless = 60,
		/// <summary>
		/// <para>Single-component 8-bit unsigned-normalized-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8_UNORM. OpenGL counterpart: GL_R8.</para>
		/// </summary>
		R8Unorm = 61,
		/// <summary>
		/// <para>Single-component 8-bit unsigned-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8_UINT. OpenGL counterpart: GL_R8UI.</para>
		/// </summary>
		R8Uint = 62,
		/// <summary>
		/// <para>Single-component 8-bit signed-normalized-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8_SNORM. OpenGL counterpart: GL_R8_SNORM.</para>
		/// </summary>
		R8Snorm = 63,
		/// <summary>
		/// <para>Single-component 8-bit signed-integer format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8_SINT. OpenGL counterpart: GL_R8I.</para>
		/// </summary>
		R8Sint = 64,
		/// <summary>
		/// <para>Single-component 8-bit unsigned-normalized-integer format for alpha only.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_A8_UNORM</para>
		/// </summary>
		/// <remarks>This format is not availanle in OpenGL</remarks>
		A8Unorm = 65,
		/// <summary>
		/// <para>Single-component 1-bit format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R1_UNORM</para>
		/// </summary>
		/// <remarks>This format is not availanle in OpenGL</remarks>
		R1Unorm = 66,
		/// <summary>
		/// <para>Three partial-precision floating pointer numbers sharing single exponent encoded into a 32-bit value.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R9G9B9E5_SHAREDEXP. OpenGL counterpart: GL_RGB9_E5.</para>
		/// </summary>
		Rgb9e5Sharedexp = 67,
		/// <summary>
		/// <para>Four-component unsigned-normalized integer format analogous to UYVY encoding.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R8G8_B8G8_UNORM</para>
		/// </summary>
		/// <remarks>This format is not availanle in OpenGL</remarks>
		Rg8B8g8Unorm = 68,
		/// <summary>
		/// <para>Four-component unsigned-normalized integer format analogous to YUY2 encoding.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_G8R8_G8B8_UNORM</para>
		/// </summary>
		/// <remarks>This format is not availanle in OpenGL</remarks>
		G8r8G8b8Unorm = 69,
		/// <summary>
		/// <para>Four-component typeless block-compression format with 1:8 compression ratio.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC1_TYPELESS. OpenGL does not have direct counterpart, GL_COMPRESSED_RGB_S3TC_DXT1_EXT is used.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC1 on MSDN,</para>
		/// <para>DXT1 on OpenGL.org</para>
		/// </remarks>
		Bc1Typeless = 70,
		/// <summary>
		/// <para>Four-component unsigned-normalized-integer block-compression format with 5 bits for R, 6 bits for G, 5 bits for B, and 0 or 1 bit for A channel.</para>
		/// <para>The pixel data is encoded using 8 bytes per 4x4 block (4 bits per pixel) providing 1:8 compression ratio against RGBA8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC1_UNORM. OpenGL counterpart: GL_COMPRESSED_RGB_S3TC_DXT1_EXT.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC1 on MSDN,</para>
		/// <para>DXT1 on OpenGL.org</para>
		/// </remarks>
		Bc1Unorm = 71,
		/// <summary>
		/// <para>Four-component unsigned-normalized-integer block-compression sRGB format with 5 bits for R, 6 bits for G, 5 bits for B, and 0 or 1 bit for A channel.</para>
		/// <para>The pixel data is encoded using 8 bytes per 4x4 block (4 bits per pixel) providing 1:8 compression ratio against RGBA8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC1_UNORM_SRGB. OpenGL counterpart: GL_COMPRESSED_SRGB_S3TC_DXT1_EXT.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC1 on MSDN,</para>
		/// <para>DXT1 on OpenGL.org</para>
		/// </remarks>
		Bc1UnormSrgb = 72,
		/// <summary>
		/// <para>Four component typeless block-compression format with 1:4 compression ratio.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC2_TYPELESS. OpenGL does not have direct counterpart, GL_COMPRESSED_RGBA_S3TC_DXT3_EXT is used.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC2 on MSDN,</para>
		/// <para>DXT3 on OpenGL.org</para>
		/// </remarks>
		Bc2Typeless = 73,
		/// <summary>
		/// <para>Four-component unsigned-normalized-integer block-compression format with 5 bits for R, 6 bits for G, 5 bits for B, and 4 bits for low-coherent separate A channel.</para>
		/// <para>The pixel data is encoded using 16 bytes per 4x4 block (8 bits per pixel) providing 1:4 compression ratio against RGBA8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC2_UNORM. OpenGL counterpart: GL_COMPRESSED_RGBA_S3TC_DXT3_EXT.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC2 on MSDN,</para>
		/// <para>DXT3 on OpenGL.org</para>
		/// </remarks>
		Bc2Unorm = 74,
		/// <summary>
		/// <para>Four-component signed-normalized-integer block-compression sRGB format with 5 bits for R, 6 bits for G, 5 bits for B, and 4 bits for low-coherent separate A channel.</para>
		/// <para>The pixel data is encoded using 16 bytes per 4x4 block (8 bits per pixel) providing 1:4 compression ratio against RGBA8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC2_UNORM_SRGB. OpenGL counterpart: GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC2 on MSDN,</para>
		/// <para>DXT3 on OpenGL.org</para>
		/// </remarks>
		Bc2UnormSrgb = 75,
		/// <summary>
		/// <para>Four-component typeless block-compression format with 1:4 compression ratio.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC3_TYPELESS. OpenGL does not have direct counterpart, GL_COMPRESSED_RGBA_S3TC_DXT5_EXT is used.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC3 on MSDN,</para>
		/// <para>DXT5 on OpenGL.org</para>
		/// </remarks>
		Bc3Typeless = 76,
		/// <summary>
		/// <para>Four-component unsigned-normalized-integer block-compression format with 5 bits for R, 6 bits for G, 5 bits for B, and 8 bits for highly-coherent A channel.</para>
		/// <para>The pixel data is encoded using 16 bytes per 4x4 block (8 bits per pixel) providing 1:4 compression ratio against RGBA8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC3_UNORM. OpenGL counterpart: GL_COMPRESSED_RGBA_S3TC_DXT5_EXT.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC3 on MSDN,</para>
		/// <para>DXT5 on OpenGL.org</para>
		/// </remarks>
		Bc3Unorm = 77,
		/// <summary>
		/// <para>Four-component unsigned-normalized-integer block-compression sRGB format with 5 bits for R, 6 bits for G, 5 bits for B, and 8 bits for highly-coherent A channel.</para>
		/// <para>The pixel data is encoded using 16 bytes per 4x4 block (8 bits per pixel) providing 1:4 compression ratio against RGBA8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC3_UNORM_SRGB. OpenGL counterpart: GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT.</para>
		/// <para>[GL_EXT_texture_compression_s3tc]: https://www.khronos.org/registry/gles/extensions/EXT/texture_compression_s3tc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_EXT_texture_compression_s3tc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC3 on MSDN,</para>
		/// <para>DXT5 on OpenGL.org</para>
		/// </remarks>
		Bc3UnormSrgb = 78,
		/// <summary>
		/// <para>One-component typeless block-compression format with 1:2 compression ratio.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC4_TYPELESS. OpenGL does not have direct counterpart, GL_COMPRESSED_RED_RGTC1 is used.</para>
		/// <para>[GL_ARB_texture_compression_rgtc]: https://www.opengl.org/registry/specs/ARB/texture_compression_rgtc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_ARB_texture_compression_rgtc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC4 on MSDN,</para>
		/// <para>Compressed formats on OpenGL.org</para>
		/// </remarks>
		Bc4Typeless = 79,
		/// <summary>
		/// <para>One-component unsigned-normalized-integer block-compression format with 8 bits for R channel.</para>
		/// <para>The pixel data is encoded using 8 bytes per 4x4 block (4 bits per pixel) providing 1:2 compression ratio against R8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC4_UNORM. OpenGL counterpart: GL_COMPRESSED_RED_RGTC1.</para>
		/// <para>[GL_ARB_texture_compression_rgtc]: https://www.opengl.org/registry/specs/ARB/texture_compression_rgtc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_ARB_texture_compression_rgtc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC4 on MSDN,</para>
		/// <para>Compressed formats on OpenGL.org</para>
		/// </remarks>
		Bc4Unorm = 80,
		/// <summary>
		/// <para>One-component signed-normalized-integer block-compression format with 8 bits for R channel.</para>
		/// <para>The pixel data is encoded using 8 bytes per 4x4 block (4 bits per pixel) providing 1:2 compression ratio against R8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC4_SNORM. OpenGL counterpart: GL_COMPRESSED_SIGNED_RED_RGTC1.</para>
		/// <para>[GL_ARB_texture_compression_rgtc]: https://www.opengl.org/registry/specs/ARB/texture_compression_rgtc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_ARB_texture_compression_rgtc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC4 on MSDN,</para>
		/// <para>Compressed formats on OpenGL.org</para>
		/// </remarks>
		Bc4Snorm = 81,
		/// <summary>
		/// <para>Two-component typeless block-compression format with 1:2 compression ratio.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC5_TYPELESS. OpenGL does not have direct counterpart, GL_COMPRESSED_RG_RGTC2 is used.</para>
		/// <para>[GL_ARB_texture_compression_rgtc]: https://www.opengl.org/registry/specs/ARB/texture_compression_rgtc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_ARB_texture_compression_rgtc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC5 on MSDN,</para>
		/// <para>Compressed formats on OpenGL.org</para>
		/// </remarks>
		Bc5Typeless = 82,
		/// <summary>
		/// <para>Two-component unsigned-normalized-integer block-compression format with 8 bits for R and 8 bits for G channel.</para>
		/// <para>The pixel data is encoded using 16 bytes per 4x4 block (8 bits per pixel) providing 1:2 compression ratio against RG8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC5_UNORM. OpenGL counterpart: GL_COMPRESSED_RG_RGTC2.</para>
		/// <para>[GL_ARB_texture_compression_rgtc]: https://www.opengl.org/registry/specs/ARB/texture_compression_rgtc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_ARB_texture_compression_rgtc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC5 on MSDN,</para>
		/// <para>Compressed formats on OpenGL.org</para>
		/// </remarks>
		Bc5Unorm = 83,
		/// <summary>
		/// <para>Two-component signed-normalized-integer block-compression format with 8 bits for R and 8 bits for G channel.</para>
		/// <para>The pixel data is encoded using 16 bytes per 4x4 block (8 bits per pixel) providing 1:2 compression ratio against RG8 format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC5_SNORM. OpenGL counterpart: GL_COMPRESSED_SIGNED_RG_RGTC2.</para>
		/// <para>[GL_ARB_texture_compression_rgtc]: https://www.opengl.org/registry/specs/ARB/texture_compression_rgtc.txt</para>
		/// <para>OpenGL&amp;OpenGLES: [GL_ARB_texture_compression_rgtc][] extension is required</para>
		/// </summary>
		/// <remarks>
		/// <para>BC5 on MSDN,</para>
		/// <para>Compressed formats on OpenGL.org</para>
		/// </remarks>
		Bc5Snorm = 84,
		/// <summary>
		/// <para>Three-component 16-bit unsigned-normalized-integer format with 5 bits for blue, 6 bits for green, and 5 bits for red channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B5G6R5_UNORM</para>
		/// </summary>
		/// <remarks>This format is not available until D3D11.1 and Windows 8. It is also not available in OpenGL</remarks>
		B5g6r5Unorm = 85,
		/// <summary>
		/// <para>Four-component 16-bit unsigned-normalized-integer format with 5 bits for each color channel and 1-bit alpha.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B5G5R5A1_UNORM</para>
		/// </summary>
		/// <remarks>This format is not available until D3D11.1 and Windows 8. It is also not available in OpenGL</remarks>
		B5g5r5a1Unorm = 86,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-normalized-integer format with 8 bits for each channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B8G8R8A8_UNORM.</para>
		/// </summary>
		/// <remarks>This format is not available in OpenGL</remarks>
		Bgra8Unorm = 87,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-normalized-integer format with 8 bits for each color channel and 8 bits unused.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B8G8R8X8_UNORM.</para>
		/// </summary>
		/// <remarks>This format is not available in OpenGL</remarks>
		Bgrx8Unorm = 88,
		/// <summary>
		/// <para>Four-component 32-bit 2.8-biased fixed-point format with 10 bits for each color channel and 2-bit alpha.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM.</para>
		/// </summary>
		/// <remarks>This format is not available in OpenGL</remarks>
		R10g10b10XrBiasA2Unorm = 89,
		/// <summary>
		/// <para>Four-component 32-bit typeless format with 8 bits for each channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B8G8R8A8_TYPELESS.</para>
		/// </summary>
		/// <remarks>This format is not available in OpenGL</remarks>
		Bgra8Typeless = 90,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-normalized sRGB format with 8 bits for each channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B8G8R8A8_UNORM_SRGB.</para>
		/// </summary>
		/// <remarks>This format is not available in OpenGL.</remarks>
		Bgra8UnormSrgb = 91,
		/// <summary>
		/// <para>Four-component 32-bit typeless format that with 8 bits for each color channel, and 8 bits are unused.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B8G8R8X8_TYPELESS.</para>
		/// </summary>
		/// <remarks>This format is not available in OpenGL.</remarks>
		Bgrx8Typeless = 92,
		/// <summary>
		/// <para>Four-component 32-bit unsigned-normalized sRGB format with 8 bits for each color channel, and 8 bits are unused.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_B8G8R8X8_UNORM_SRGB.</para>
		/// </summary>
		/// <remarks>This format is not available in OpenGL.</remarks>
		Bgrx8UnormSrgb = 93,
		/// <summary>
		/// <para>Three-component typeless block-compression format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC6H_TYPELESS. OpenGL does not have direct counterpart, GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT is used.</para>
		/// <para>[GL_ARB_texture_compression_bptc]: https://cvs.khronos.org/svn/repos/ogl/trunk/doc/registry/public/specs/ARB/texture_compression_bptc.txt</para>
		/// <para>OpenGL: [GL_ARB_texture_compression_bptc][] extension is required. Not supported in at least OpenGLES3.1</para>
		/// </summary>
		/// <remarks>
		/// <para>BC6H on MSDN,</para>
		/// <para>BPTC Texture Compression on OpenGL.org</para>
		/// </remarks>
		Bc6hTypeless = 94,
		/// <summary>
		/// <para>Three-component unsigned half-precision floating-point format with 16 bits for each channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC6H_UF16. OpenGL counterpart: GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT.</para>
		/// <para>[GL_ARB_texture_compression_bptc]: https://cvs.khronos.org/svn/repos/ogl/trunk/doc/registry/public/specs/ARB/texture_compression_bptc.txt</para>
		/// <para>OpenGL: [GL_ARB_texture_compression_bptc][] extension is required. Not supported in at least OpenGLES3.1</para>
		/// </summary>
		/// <remarks>
		/// <para>BC6H on MSDN,</para>
		/// <para>BPTC Texture Compression on OpenGL.org</para>
		/// </remarks>
		Bc6hUf16 = 95,
		/// <summary>
		/// <para>Three-channel signed half-precision floating-point format with 16 bits per each channel.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC6H_SF16. OpenGL counterpart: GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT.</para>
		/// <para>[GL_ARB_texture_compression_bptc]: https://cvs.khronos.org/svn/repos/ogl/trunk/doc/registry/public/specs/ARB/texture_compression_bptc.txt</para>
		/// <para>OpenGL: [GL_ARB_texture_compression_bptc][] extension is required. Not supported in at least OpenGLES3.1</para>
		/// </summary>
		/// <remarks>
		/// <para>BC6H on MSDN,</para>
		/// <para>BPTC Texture Compression on OpenGL.org</para>
		/// </remarks>
		Bc6hSf16 = 96,
		/// <summary>
		/// <para>Three-component typeless block-compression format.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC7_TYPELESS. OpenGL does not have direct counterpart, GL_COMPRESSED_RGBA_BPTC_UNORM is used.</para>
		/// <para>[GL_ARB_texture_compression_bptc]: https://cvs.khronos.org/svn/repos/ogl/trunk/doc/registry/public/specs/ARB/texture_compression_bptc.txt</para>
		/// <para>OpenGL: [GL_ARB_texture_compression_bptc][] extension is required. Not supported in at least OpenGLES3.1</para>
		/// </summary>
		/// <remarks>
		/// <para>BC7 on MSDN,</para>
		/// <para>BPTC Texture Compression on OpenGL.org</para>
		/// </remarks>
		Bc7Typeless = 97,
		/// <summary>
		/// <para>Three-component block-compression unsigned-normalized-integer format with 4 to 7 bits per color channel and 0 to 8 bits of alpha.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC7_UNORM. OpenGL counterpart: GL_COMPRESSED_RGBA_BPTC_UNORM.</para>
		/// <para>[GL_ARB_texture_compression_bptc]: https://cvs.khronos.org/svn/repos/ogl/trunk/doc/registry/public/specs/ARB/texture_compression_bptc.txt</para>
		/// <para>OpenGL: [GL_ARB_texture_compression_bptc][] extension is required. Not supported in at least OpenGLES3.1</para>
		/// </summary>
		/// <remarks>
		/// <para>BC7 on MSDN,</para>
		/// <para>BPTC Texture Compression on OpenGL.org</para>
		/// </remarks>
		Bc7Unorm = 98,
		/// <summary>
		/// <para>Three-component block-compression unsigned-normalized-integer sRGB format with 4 to 7 bits per color channel and 0 to 8 bits of alpha.</para>
		/// <para>D3D counterpart: DXGI_FORMAT_BC7_UNORM_SRGB. OpenGL counterpart: GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM.</para>
		/// <para>[GL_ARB_texture_compression_bptc]: https://cvs.khronos.org/svn/repos/ogl/trunk/doc/registry/public/specs/ARB/texture_compression_bptc.txt</para>
		/// <para>OpenGL: [GL_ARB_texture_compression_bptc][] extension is required. Not supported in at least OpenGLES3.1</para>
		/// </summary>
		/// <remarks>
		/// <para>BC7 on MSDN,</para>
		/// <para>BPTC Texture Compression on OpenGL.org</para>
		/// </remarks>
		Bc7UnormSrgb = 99,
		/// <summary>Helper member containing the total number of texture formats in the enumeration</summary>
		NumFormats = 100
	}

	/// <summary>This enumeration defines filter type. It is used by SamplerDesc structure to define min, mag and mip filters.</summary>
	/// <remarks>
	/// <para>On D3D11, comparison filters only work with textures that have the following formats:</para>
	/// <para>R32_FLOAT_X8X24_TYPELESS, R32_FLOAT, R24_UNORM_X8_TYPELESS, R16_UNORM.</para>
	/// </remarks>
	public enum FilterType : byte
	{
		/// <summary>Unknown filter type</summary>
		Unknown = 0,
		/// <summary>Point filtering</summary>
		Point = 1,
		/// <summary>Linear filtering</summary>
		Linear = 2,
		/// <summary>Anisotropic filtering</summary>
		Anisotropic = 3,
		/// <summary>Comparison-point filtering</summary>
		ComparisonPoint = 4,
		/// <summary>Comparison-linear filtering</summary>
		ComparisonLinear = 5,
		/// <summary>Comparison-anisotropic filtering</summary>
		ComparisonAnisotropic = 6,
		/// <summary>Minimum-point filtering (DX12 only)</summary>
		MinimumPoint = 7,
		/// <summary>Minimum-linear filtering (DX12 only)</summary>
		MinimumLinear = 8,
		/// <summary>Minimum-anisotropic filtering (DX12 only)</summary>
		MinimumAnisotropic = 9,
		/// <summary>Maximum-point filtering (DX12 only)</summary>
		MaximumPoint = 10,
		/// <summary>Maximum-linear filtering (DX12 only)</summary>
		MaximumLinear = 11,
		/// <summary>Maximum-anisotropic filtering (DX12 only)</summary>
		MaximumAnisotropic = 12,
		/// <summary>Helper value that stores the total number of filter types in the enumeration</summary>
		NumFilters = 13
	}

	/// <summary>
	/// <para>[D3D11_TEXTURE_ADDRESS_MODE]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476256(v=vs.85).aspx</para>
	/// <para>[D3D12_TEXTURE_ADDRESS_MODE]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770441(v=vs.85).aspx</para>
	/// <para>Defines a technique for resolving texture coordinates that are outside of</para>
	/// <para>the boundaries of a texture. The enumeration generally mirrors [D3D11_TEXTURE_ADDRESS_MODE][]/[D3D12_TEXTURE_ADDRESS_MODE][] enumeration.</para>
	/// <para>It is used by SamplerDesc structure to define the address mode for U,V and W texture coordinates.</para>
	/// </summary>
	public enum TextureAddressMode : byte
	{
		/// <summary>Unknown mode</summary>
		Unknown = 0,
		/// <summary>
		/// <para>Tile the texture at every integer junction.</para>
		/// <para>Direct3D Counterpart: D3D11_TEXTURE_ADDRESS_WRAP/D3D12_TEXTURE_ADDRESS_MODE_WRAP. OpenGL counterpart: GL_REPEAT</para>
		/// </summary>
		Wrap = 1,
		/// <summary>
		/// <para>Flip the texture at every integer junction.</para>
		/// <para>Direct3D Counterpart: D3D11_TEXTURE_ADDRESS_MIRROR/D3D12_TEXTURE_ADDRESS_MODE_MIRROR. OpenGL counterpart: GL_MIRRORED_REPEAT</para>
		/// </summary>
		Mirror = 2,
		/// <summary>
		/// <para>Texture coordinates outside the range [0.0, 1.0] are set to the</para>
		/// <para>texture color at 0.0 or 1.0, respectively.</para>
		/// <para>Direct3D Counterpart: D3D11_TEXTURE_ADDRESS_CLAMP/D3D12_TEXTURE_ADDRESS_MODE_CLAMP. OpenGL counterpart: GL_CLAMP_TO_EDGE</para>
		/// </summary>
		Clamp = 3,
		/// <summary>
		/// <para>Texture coordinates outside the range [0.0, 1.0] are set to the border color specified</para>
		/// <para>specified in SamplerDesc structure.</para>
		/// <para>Direct3D Counterpart: D3D11_TEXTURE_ADDRESS_BORDER/D3D12_TEXTURE_ADDRESS_MODE_BORDER. OpenGL counterpart: GL_CLAMP_TO_BORDER</para>
		/// </summary>
		Border = 4,
		/// <summary>
		/// <para>Similar to TEXTURE_ADDRESS_MIRROR and TEXTURE_ADDRESS_CLAMP. Takes the absolute</para>
		/// <para>value of the texture coordinate (thus, mirroring around 0), and then clamps to</para>
		/// <para>the maximum value.</para>
		/// <para>Direct3D Counterpart: D3D11_TEXTURE_ADDRESS_MIRROR_ONCE/D3D12_TEXTURE_ADDRESS_MODE_MIRROR_ONCE. OpenGL counterpart: GL_MIRROR_CLAMP_TO_EDGE</para>
		/// </summary>
		/// <remarks>GL_MIRROR_CLAMP_TO_EDGE is only available in OpenGL4.4+, and is not available until at least OpenGLES3.1</remarks>
		MirrorOnce = 5,
		/// <summary>Helper value that stores the total number of texture address modes in the enumeration</summary>
		NumModes = 6
	}

	/// <summary>
	/// <para>[D3D11_COMPARISON_FUNC]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476101(v=vs.85).aspx</para>
	/// <para>[D3D12_COMPARISON_FUNC]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770349(v=vs.85).aspx</para>
	/// <para>This enumeartion defines a comparison function. It generally mirrors [D3D11_COMPARISON_FUNC]/[D3D12_COMPARISON_FUNC] enum and is used by</para>
	/// <para>- SamplerDesc to define a comparison function if one of the comparison mode filters is used</para>
	/// <para>- StencilOpDesc to define a stencil function</para>
	/// <para>- DepthStencilStateDesc to define a depth function</para>
	/// </summary>
	public enum ComparisonFunction : byte
	{
		/// <summary>Unknown comparison function</summary>
		Unknown = 0,
		/// <summary>
		/// <para>Comparison never passes.</para>
		/// <para>Direct3D counterpart: D3D11_COMPARISON_NEVER/D3D12_COMPARISON_FUNC_NEVER. OpenGL counterpart: GL_NEVER.</para>
		/// </summary>
		Never = 1,
		/// <summary>
		/// <para>Comparison passes if the source data is less than the destination data.</para>
		/// <para>Direct3D counterpart: D3D11_COMPARISON_LESS/D3D12_COMPARISON_FUNC_LESS. OpenGL counterpart: GL_LESS.</para>
		/// </summary>
		Less = 2,
		/// <summary>
		/// <para>Comparison passes if the source data is equal to the destination data.</para>
		/// <para>Direct3D counterpart: D3D11_COMPARISON_EQUAL/D3D12_COMPARISON_FUNC_EQUAL. OpenGL counterpart: GL_EQUAL.</para>
		/// </summary>
		Equal = 3,
		/// <summary>
		/// <para>Comparison passes if the source data is less than or equal to the destination data.</para>
		/// <para>Direct3D counterpart: D3D11_COMPARISON_LESS_EQUAL/D3D12_COMPARISON_FUNC_LESS_EQUAL. OpenGL counterpart: GL_LEQUAL.</para>
		/// </summary>
		LessEqual = 4,
		/// <summary>
		/// <para>Comparison passes if the source data is greater than the destination data.</para>
		/// <para>Direct3D counterpart: 3D11_COMPARISON_GREATER/D3D12_COMPARISON_FUNC_GREATER. OpenGL counterpart: GL_GREATER.</para>
		/// </summary>
		Greater = 5,
		/// <summary>
		/// <para>Comparison passes if the source data is not equal to the destination data.</para>
		/// <para>Direct3D counterpart: D3D11_COMPARISON_NOT_EQUAL/D3D12_COMPARISON_FUNC_NOT_EQUAL. OpenGL counterpart: GL_NOTEQUAL.</para>
		/// </summary>
		NotEqual = 6,
		/// <summary>
		/// <para>Comparison passes if the source data is greater than or equal to the destination data.</para>
		/// <para>Direct3D counterpart: D3D11_COMPARISON_GREATER_EQUAL/D3D12_COMPARISON_FUNC_GREATER_EQUAL. OpenGL counterpart: GL_GEQUAL.</para>
		/// </summary>
		GreaterEqual = 7,
		/// <summary>
		/// <para>Comparison always passes.</para>
		/// <para>Direct3D counterpart: D3D11_COMPARISON_ALWAYS/D3D12_COMPARISON_FUNC_ALWAYS. OpenGL counterpart: GL_ALWAYS.</para>
		/// </summary>
		Always = 8,
		/// <summary>Helper value that stores the total number of comparison functions in the enumeration</summary>
		NumFunctions = 9
	}

	/// <summary>The enumeration is used by TextureDesc to describe misc texture flags</summary>
	[Flags]
	public enum MiscTextureFlags : byte
	{
		None = 0x0,
		/// <remarks>A texture must be created with BIND_RENDER_TARGET bind flag</remarks>
		GenerateMips = 0x1
	}

	/// <summary>This enumeration is used by DrawAttribs structure to define input primitive topology.</summary>
	public enum PrimitiveTopology : byte
	{
		/// <summary>Undefined topology</summary>
		Undefined = 0,
		/// <summary>
		/// <para>Interpret the vertex data as a list of triangles.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST. OpenGL counterpart: GL_TRIANGLES.</para>
		/// </summary>
		TriangleList = 1,
		/// <summary>
		/// <para>Interpret the vertex data as a triangle strip.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP. OpenGL counterpart: GL_TRIANGLE_STRIP.</para>
		/// </summary>
		TriangleStrip = 2,
		/// <summary>
		/// <para>Interpret the vertex data as a list of points.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_POINTLIST. OpenGL counterpart: GL_POINTS.</para>
		/// </summary>
		PointList = 3,
		/// <summary>
		/// <para>Interpret the vertex data as a list of lines.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_LINELIST. OpenGL counterpart: GL_LINES.</para>
		/// </summary>
		LineList = 4,
		/// <summary>
		/// <para>Interpret the vertex data as a line strip.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_LINESTRIP. OpenGL counterpart: GL_LINE_STRIP.</para>
		/// </summary>
		LineStrip = 5,
		/// <summary>
		/// <para>Interpret the vertex data as a list of one control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_1_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_1ControlPointPatchlist = 6,
		/// <summary>
		/// <para>Interpret the vertex data as a list of two control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_2_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_2ControlPointPatchlist = 7,
		/// <summary>
		/// <para>Interpret the vertex data as a list of three control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_3_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_3ControlPointPatchlist = 8,
		/// <summary>
		/// <para>Interpret the vertex data as a list of four control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_4_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_4ControlPointPatchlist = 9,
		/// <summary>
		/// <para>Interpret the vertex data as a list of five control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_5_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_5ControlPointPatchlist = 10,
		/// <summary>
		/// <para>Interpret the vertex data as a list of six control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_6_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_6ControlPointPatchlist = 11,
		/// <summary>
		/// <para>Interpret the vertex data as a list of seven control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_7_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_7ControlPointPatchlist = 12,
		/// <summary>
		/// <para>Interpret the vertex data as a list of eight control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_8_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_8ControlPointPatchlist = 13,
		/// <summary>
		/// <para>Interpret the vertex data as a list of nine control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_9_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_9ControlPointPatchlist = 14,
		/// <summary>
		/// <para>Interpret the vertex data as a list of ten control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_10_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_10ControlPointPatchlist = 15,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 11 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_11_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_11ControlPointPatchlist = 16,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 12 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_12_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_12ControlPointPatchlist = 17,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 13 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_13_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_13ControlPointPatchlist = 18,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 14 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_14_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_14ControlPointPatchlist = 19,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 15 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_15_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_15ControlPointPatchlist = 20,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 16 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_16_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_16ControlPointPatchlist = 21,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 17 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_17_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_17ControlPointPatchlist = 22,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 18 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_18_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_18ControlPointPatchlist = 23,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 19 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_19_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_19ControlPointPatchlist = 24,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 20 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_20_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_20ControlPointPatchlist = 25,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 21 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_21_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_21ControlPointPatchlist = 26,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 22 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_22_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_22ControlPointPatchlist = 27,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 23 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_23_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_23ControlPointPatchlist = 28,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 24 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_24_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_24ControlPointPatchlist = 29,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 25 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_25_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_25ControlPointPatchlist = 30,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 26 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_26_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_26ControlPointPatchlist = 31,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 27 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_27_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_27ControlPointPatchlist = 32,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 28 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_28_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_28ControlPointPatchlist = 33,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 29 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_29_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_29ControlPointPatchlist = 34,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 30 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_30_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_30ControlPointPatchlist = 35,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 31 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_31_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_31ControlPointPatchlist = 36,
		/// <summary>
		/// <para>Interpret the vertex data as a list of 32 control point patches.</para>
		/// <para>D3D counterpart: D3D_PRIMITIVE_TOPOLOGY_32_CONTROL_POINT_PATCHLIST. OpenGL counterpart: GL_PATCHES.</para>
		/// </summary>
		_32ControlPointPatchlist = 37,
		/// <summary>Helper value that stores the total number of topologies in the enumeration</summary>
		NumTopologies = 38
	}

	/// <summary>Hardware adapter type</summary>
	public enum AdapterType : byte
	{
		/// <summary>Adapter type is unknown</summary>
		Unknown = 0,
		/// <summary>Software adapter</summary>
		Software = 1,
		/// <summary>Hardware adapter</summary>
		Hardware = 2
	}

	/// <summary>Flags indicating how an image is stretched to fit a given monitor's resolution.</summary>
	/// <remarks>DXGI_MODE_SCALING enumeration on MSDN,</remarks>
	public enum ScalingMode
	{
		/// <summary>
		/// <para>Unspecified scaling.</para>
		/// <para>D3D Counterpart: DXGI_MODE_SCALING_UNSPECIFIED.</para>
		/// </summary>
		Unspecified = 0,
		/// <summary>
		/// <para>Specifies no scaling. The image is centered on the display.</para>
		/// <para>This flag is typically used for a fixed-dot-pitch display (such as an LED display).</para>
		/// <para>D3D Counterpart: DXGI_MODE_SCALING_CENTERED.</para>
		/// </summary>
		Centered = 1,
		/// <summary>
		/// <para>Specifies stretched scaling.</para>
		/// <para>D3D Counterpart: DXGI_MODE_SCALING_STRETCHED.</para>
		/// </summary>
		Stretched = 2
	}

	/// <summary>Flags indicating the method the raster uses to create an image on a surface.</summary>
	/// <remarks>DXGI_MODE_SCANLINE_ORDER enumeration on MSDN,</remarks>
	public enum ScanlineOrder
	{
		/// <summary>
		/// <para>Scanline order is unspecified</para>
		/// <para>D3D Counterpart: DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED.</para>
		/// </summary>
		Unspecified = 0,
		/// <summary>
		/// <para>The image is created from the first scanline to the last without skipping any</para>
		/// <para>D3D Counterpart: DXGI_MODE_SCANLINE_ORDER_PROGRESSIVE.</para>
		/// </summary>
		Progressive = 1,
		/// <summary>
		/// <para>The image is created beginning with the upper field</para>
		/// <para>D3D Counterpart: DXGI_MODE_SCANLINE_ORDER_UPPER_FIELD_FIRST.</para>
		/// </summary>
		UpperFieldFirst = 2,
		/// <summary>
		/// <para>The image is created beginning with the lower field</para>
		/// <para>D3D Counterpart: DXGI_MODE_SCANLINE_ORDER_LOWER_FIELD_FIRST.</para>
		/// </summary>
		LowerFieldFirst = 3
	}

	/// <summary>Defines allowed swap chain usage flags</summary>
	[Flags]
	public enum SwapChainUsageFlags : uint
	{
		/// <summary>No allowed usage</summary>
		None = 0x0,
		/// <summary>Swap chain can be used as render target ouput</summary>
		RenderTarget = 0x1,
		/// <summary>Swap chain images can be used as shader inputs</summary>
		ShaderInput = 0x2,
		/// <summary>Swap chain images can be used as source of copy operation</summary>
		CopySource = 0x4
	}

	/// <summary>Debug flags that can be specified when creating Direct3D11-based engine implementation.</summary>
	/// <remarks>CreateDeviceAndContextsD3D11Type, CreateSwapChainD3D11Type, LoadGraphicsEngineD3D11</remarks>
	[Flags]
	public enum D3d11DebugFlags : uint
	{
		/// <summary>No debug flag</summary>
		None = 0x0,
		/// <summary>Whether to create Direct3D11 debug device</summary>
		CreateDebugDevice = 0x1,
		/// <summary>
		/// <para>Before executing draw/dispatch command, verify that</para>
		/// <para>all required shader resources are bound to the device context</para>
		/// </summary>
		VerifyCommittedShaderResources = 0x2,
		/// <summary>
		/// <para>Verify that all committed cotext resources are relevant,</para>
		/// <para>i.e. they are consistent with the committed resource cache.</para>
		/// <para>This is very expensive and should generally not be necessary.</para>
		/// </summary>
		VerifyCommittedResourceRelevance = 0x4
	}

	/// <summary>Direct3D11/12 feature level</summary>
	public enum Direct3dFeatureLevel : byte
	{
		/// <summary>Feature level 10.0</summary>
		_100 = 0,
		/// <summary>Feature level 10.1</summary>
		_101 = 1,
		/// <summary>Feature level 11.0</summary>
		_110 = 2,
		/// <summary>Feature level 11.1</summary>
		_111 = 3,
		/// <summary>Feature level 12.0</summary>
		_120 = 4,
		/// <summary>Feature level 12.1</summary>
		_121 = 5
	}

	/// <summary>Describes texture format component type</summary>
	public enum ComponentType : byte
	{
		/// <summary>Undefined component type</summary>
		Undefined = 0,
		/// <summary>Floating point component type</summary>
		Float = 1,
		/// <summary>Signed-normalized-integer component type</summary>
		Snorm = 2,
		/// <summary>Unsigned-normalized-integer component type</summary>
		Unorm = 3,
		/// <summary>Unsigned-normalized-integer sRGB component type</summary>
		UnormSrgb = 4,
		/// <summary>Signed-integer component type</summary>
		Sint = 5,
		/// <summary>Unsigned-integer component type</summary>
		Uint = 6,
		/// <summary>Depth component type</summary>
		Depth = 7,
		/// <summary>Depth-stencil component type</summary>
		DepthStencil = 8,
		/// <summary>Compound component type (example texture formats: TEX_FORMAT_R11G11B10_FLOAT or TEX_FORMAT_RGB9E5_SHAREDEXP)</summary>
		Compound = 9,
		/// <summary>Compressed component type</summary>
		Compressed = 10
	}

	/// <summary>Resource usage state</summary>
	public enum ResourceState : uint
	{
		/// <summary>The resource state is not known to the engine and is managed by the application</summary>
		Unknown = 0x0,
		/// <summary>The resource state is known to the engine, but is undefined. A resource is typically in an undefined state right after initialization.</summary>
		Undefined = 0x1,
		/// <summary>The resource is accessed as vertex buffer</summary>
		VertexBuffer = 0x2,
		/// <summary>The resource is accessed as constant (uniform) buffer</summary>
		ConstantBuffer = 0x4,
		/// <summary>The resource is accessed as index buffer</summary>
		IndexBuffer = 0x8,
		/// <summary>The resource is accessed as render target</summary>
		RenderTarget = 0x10,
		/// <summary>The resource is used for unordered access</summary>
		UnorderedAccess = 0x20,
		/// <summary>The resource is used in a writable depth-stencil view or in clear operation</summary>
		DepthWrite = 0x40,
		/// <summary>The resource is used in a read-only depth-stencil view</summary>
		DepthRead = 0x80,
		/// <summary>The resource is accessed from a shader</summary>
		ShaderResource = 0x100,
		/// <summary>The resource is used as the destination for stream output</summary>
		StreamOut = 0x200,
		/// <summary>The resource is used as indirect draw/dispatch arguments buffer</summary>
		IndirectArgument = 0x400,
		/// <summary>The resource is used as the destination in a copy operation</summary>
		CopyDest = 0x800,
		/// <summary>The resource is used as the source in a copy operation</summary>
		CopySource = 0x1000,
		/// <summary>The resource is used as the destination in a resolve operation</summary>
		ResolveDest = 0x2000,
		/// <summary>The resource is used as the source in a resolve operation</summary>
		ResolveSource = 0x4000,
		/// <summary>The resource is used for present</summary>
		Present = 0x8000,
		/// <summary>The resource is used for present</summary>
		MaxBit = 0x8000,
		/// <summary>The resource is used for present</summary>
		GenericRead = 5390
	}

	/// <summary>State transition barrier type</summary>
	public enum StateTransitionType : byte
	{
		/// <summary>Perform state transition immediately.</summary>
		Immediate = 0,
		/// <summary>
		/// <para>Begin split barrier. This mode only has effect in Direct3D12 backend, and corresponds to</para>
		/// <para>[D3D12_RESOURCE_BARRIER_FLAG_BEGIN_ONLY](https://docs.microsoft.com/en-us/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_barrier_flags)</para>
		/// <para>flag. See https://docs.microsoft.com/en-us/windows/desktop/direct3d12/using-resource-barriers-to-synchronize-resource-states-in-direct3d-12#split-barriers.</para>
		/// <para>In other backends, begin-split barriers are ignored.</para>
		/// </summary>
		Begin = 1,
		/// <summary>
		/// <para>End split barrier. This mode only has effect in Direct3D12 backend, and corresponds to</para>
		/// <para>[D3D12_RESOURCE_BARRIER_FLAG_END_ONLY](https://docs.microsoft.com/en-us/windows/desktop/api/d3d12/ne-d3d12-d3d12_resource_barrier_flags)</para>
		/// <para>flag. See https://docs.microsoft.com/en-us/windows/desktop/direct3d12/using-resource-barriers-to-synchronize-resource-states-in-direct3d-12#split-barriers.</para>
		/// <para>In other backends, this mode is similar to STATE_TRANSITION_TYPE_IMMEDIATE.</para>
		/// </summary>
		End = 2
	}

	/// <summary>Display mode attributes</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DisplayModeAttribs
	{
		/// <summary>Display resolution width</summary>
		public int Width;

		/// <summary>Display resolution height</summary>
		public int Height;

		/// <summary>Display format</summary>
		public TextureFormat Format;

		/// <summary>Refresh rate numerator</summary>
		public int RefreshRateNumerator;

		/// <summary>Refresh rate denominator</summary>
		public int RefreshRateDenominator;

		/// <summary>The scanline drawing mode.</summary>
		public ScalingMode Scaling;

		/// <summary>The scaling mode.</summary>
		public ScanlineOrder ScanlineOrder;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DisplayModeAttribs( bool unused )
		{
			Width = 0;
			Height = 0;
			Format = TextureFormat.Unknown;
			RefreshRateNumerator = 0;
			RefreshRateDenominator = 0;
			Scaling = ScalingMode.Unspecified;
			ScanlineOrder = ScanlineOrder.Unspecified;
		}
	}

	/// <summary>Swap chain description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct SwapChainDesc
	{
		/// <summary>The swap chain width. Default value is 0</summary>
		public int Width;

		/// <summary>The swap chain height. Default value is 0</summary>
		public int Height;

		/// <summary>Back buffer format. Default value is Diligent::TEX_FORMAT_RGBA8_UNORM_SRGB</summary>
		public TextureFormat ColorBufferFormat;

		/// <summary>
		/// <para>Depth buffer format. Default value is Diligent::TEX_FORMAT_D32_FLOAT.</para>
		/// <para>Use Diligent::TEX_FORMAT_UNKNOWN to create the swap chain without depth buffer.</para>
		/// </summary>
		public TextureFormat DepthBufferFormat;

		/// <summary>Swap chain usage flags. Default value is Diligent::SWAP_CHAIN_USAGE_RENDER_TARGET</summary>
		public SwapChainUsageFlags Usage;

		/// <summary>Number of buffers int the swap chain</summary>
		public int BufferCount;

		/// <summary>Default depth value, which is used as optimized depth clear value in D3D12</summary>
		public float DefaultDepthValue;

		/// <summary>Default stencil value, which is used as optimized stencil clear value in D3D12</summary>
		public byte DefaultStencilValue;

		byte m_IsPrimary;
		/// <summary>
		/// <para>Indicates if this is a primary swap chain. When Present() is called</para>
		/// <para>for the primary swap chain, the engine releases stale resources.</para>
		/// </summary>
		public bool IsPrimary
		{
			get => ( 0 != m_IsPrimary );
			set => m_IsPrimary = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public SwapChainDesc( bool unused )
		{
			Width = 0;
			Height = 0;
			ColorBufferFormat = TextureFormat.Rgba8UnormSrgb;
			DepthBufferFormat = TextureFormat.D32Float;
			Usage = SwapChainUsageFlags.RenderTarget;
			BufferCount = 2;
			DefaultDepthValue = 1.0f;
			DefaultStencilValue = 0;
			m_IsPrimary = 1;
		}
	}

	/// <summary>Full screen mode description</summary>
	/// <remarks>DXGI_SWAP_CHAIN_FULLSCREEN_DESC structure on MSDN,</remarks>
	[StructLayout( LayoutKind.Sequential )]
	public struct FullScreenModeDesc
	{
		byte m_Fullscreen;
		/// <summary>A Boolean value that specifies whether the swap chain is in fullscreen mode.</summary>
		public bool Fullscreen
		{
			get => ( 0 != m_Fullscreen );
			set => m_Fullscreen = MiscUtils.byteFromBool( value );
		}

		/// <summary>Refresh rate numerator</summary>
		public int RefreshRateNumerator;

		/// <summary>Refresh rate denominator</summary>
		public int RefreshRateDenominator;

		/// <summary>The scanline drawing mode.</summary>
		public ScalingMode Scaling;

		/// <summary>The scaling mode.</summary>
		public ScanlineOrder ScanlineOrder;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public FullScreenModeDesc( bool unused )
		{
			m_Fullscreen = 0;
			RefreshRateNumerator = 0;
			RefreshRateDenominator = 0;
			Scaling = ScalingMode.Unspecified;
			ScanlineOrder = ScanlineOrder.Unspecified;
		}
	}

	/// <summary>Box</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Box
	{
		/// <summary>Minimal X coordinate. Default value is 0</summary>
		public int MinX;

		/// <summary>Maximal X coordinate. Default value is 0</summary>
		public int MaxX;

		/// <summary>Minimal Y coordinate. Default value is 0</summary>
		public int MinY;

		/// <summary>Maximal Y coordinate. Default value is 0</summary>
		public int MaxY;

		/// <summary>Minimal Z coordinate. Default value is 0</summary>
		public int MinZ;

		/// <summary>Maximal Z coordinate. Default value is 1</summary>
		public int MaxZ;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public Box( bool unused )
		{
			MinX = 0;
			MaxX = 0;
			MinY = 0;
			MaxY = 0;
			MinZ = 0;
			MaxZ = 1;
		}
	}

	/// <summary>
	/// <para>Describes invariant texture format attributes. These attributes are</para>
	/// <para>intrinsic to the texture format itself and do not depend on the</para>
	/// <para>format support.</para>
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureFormatAttribs
	{
		/// <summary>
		/// <para>Literal texture format name (for instance, for TEX_FORMAT_RGBA8_UNORM format, this</para>
		/// <para>will be &quot;TEX_FORMAT_RGBA8_UNORM&quot;)</para>
		/// </summary>
		public IntPtr Name;

		/// <summary>Texture format, see Diligent::TEXTURE_FORMAT for a list of supported texture formats</summary>
		public TextureFormat Format;

		/// <summary>
		/// <para>Size of one component in bytes (for instance, for TEX_FORMAT_RGBA8_UNORM format, this will be 1)</para>
		/// <para>For compressed formats, this is the block size in bytes (for TEX_FORMAT_BC1_UNORM format, this will be 8)</para>
		/// </summary>
		public byte ComponentSize;

		/// <summary>Number of components</summary>
		public byte NumComponents;

		/// <summary>Component type, see Diligent::COMPONENT_TYPE for details.</summary>
		public ComponentType ComponentType;

		byte m_IsTypeless;
		/// <summary>Bool flag indicating if the format is a typeless format</summary>
		public bool IsTypeless
		{
			get => ( 0 != m_IsTypeless );
			set => m_IsTypeless = MiscUtils.byteFromBool( value );
		}

		/// <summary>For block-compressed formats, compression block width</summary>
		public byte BlockWidth;

		/// <summary>For block-compressed formats, compression block height</summary>
		public byte BlockHeight;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public TextureFormatAttribs( bool unused )
		{
			Name = IntPtr.Zero;
			Format = TextureFormat.Unknown;
			ComponentSize = 0;
			NumComponents = 0;
			ComponentType = ComponentType.Undefined;
			m_IsTypeless = 0;
			BlockWidth = 0;
			BlockHeight = 0;
		}
	}

	/// <summary>This structure is returned by IRenderDevice::GetTextureFormatInfo()</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureFormatInfo
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public TextureFormatAttribs baseStruct;

		byte m_Supported;
		/// <summary>Indicates if the format is supported by the device</summary>
		public bool Supported
		{
			get => ( 0 != m_Supported );
			set => m_Supported = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public TextureFormatInfo( bool unused )
		{
			baseStruct = new TextureFormatAttribs( true );
			m_Supported = 0;
		}
	}

	/// <summary>This structure is returned by IRenderDevice::GetTextureFormatInfoExt()</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureFormatInfoExt
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public TextureFormatInfo baseStruct;

		byte m_Filterable;
		/// <summary>Indicates if the format can be filtered</summary>
		public bool Filterable
		{
			get => ( 0 != m_Filterable );
			set => m_Filterable = MiscUtils.byteFromBool( value );
		}

		byte m_ColorRenderable;
		/// <summary>Indicates if the format can be used as a render target format</summary>
		public bool ColorRenderable
		{
			get => ( 0 != m_ColorRenderable );
			set => m_ColorRenderable = MiscUtils.byteFromBool( value );
		}

		byte m_DepthRenderable;
		/// <summary>Indicates if the format can be used as a depth format</summary>
		public bool DepthRenderable
		{
			get => ( 0 != m_DepthRenderable );
			set => m_DepthRenderable = MiscUtils.byteFromBool( value );
		}

		byte m_Tex1DFmt;
		/// <summary>Indicates if the format can be used to create a 1D texture</summary>
		public bool Tex1DFmt
		{
			get => ( 0 != m_Tex1DFmt );
			set => m_Tex1DFmt = MiscUtils.byteFromBool( value );
		}

		byte m_Tex2DFmt;
		/// <summary>Indicates if the format can be used to create a 2D texture</summary>
		public bool Tex2DFmt
		{
			get => ( 0 != m_Tex2DFmt );
			set => m_Tex2DFmt = MiscUtils.byteFromBool( value );
		}

		byte m_Tex3DFmt;
		/// <summary>Indicates if the format can be used to create a 3D texture</summary>
		public bool Tex3DFmt
		{
			get => ( 0 != m_Tex3DFmt );
			set => m_Tex3DFmt = MiscUtils.byteFromBool( value );
		}

		byte m_TexCubeFmt;
		/// <summary>Indicates if the format can be used to create a cube texture</summary>
		public bool TexCubeFmt
		{
			get => ( 0 != m_TexCubeFmt );
			set => m_TexCubeFmt = MiscUtils.byteFromBool( value );
		}

		/// <summary>
		/// <para>A bitmask specifying all the supported sample counts for this texture format.</para>
		/// <para>If the format supports n samples, then (SampleCounts&amp;n) != 0</para>
		/// </summary>
		public int SampleCounts;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public TextureFormatInfoExt( bool unused )
		{
			baseStruct = new TextureFormatInfo( true );
			m_Filterable = 0;
			m_ColorRenderable = 0;
			m_DepthRenderable = 0;
			m_Tex1DFmt = 0;
			m_Tex2DFmt = 0;
			m_Tex3DFmt = 0;
			m_TexCubeFmt = 0;
			SampleCounts = 0;
		}
	}

	/// <summary>Resource state transition barrier description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct StateTransitionDesc
	{
		/// <summary>Texture to transition.</summary>
		/// <remarks>Exactly one of pTexture or pBuffer must be non-null.</remarks>
		public IntPtr pTexture;

		/// <summary>Buffer to transition.</summary>
		/// <remarks>Exactly one of pTexture or pBuffer must be non-null.</remarks>
		public IntPtr pBuffer;

		/// <summary>When transitioning a texture, first mip level of the subresource range to transition.</summary>
		public int FirstMipLevel;

		/// <summary>When transitioning a texture, number of mip levels of the subresource range to transition.</summary>
		public int MipLevelsCount;

		/// <summary>When transitioning a texture, first array slice of the subresource range to transition.</summary>
		public int FirstArraySlice;

		/// <summary>When transitioning a texture, number of array slices of the subresource range to transition.</summary>
		public int ArraySliceCount;

		/// <summary>
		/// <para>Resource state before transition. If this value is RESOURCE_STATE_UNKNOWN,</para>
		/// <para>internal resource state will be used, which must be defined in this case.</para>
		/// </summary>
		public ResourceState OldState;

		/// <summary>Resource state after transition.</summary>
		public ResourceState NewState;

		/// <remarks>
		/// <para>When issuing UAV barrier (i.e. OldState and NewState equal RESOURCE_STATE_UNORDERED_ACCESS),</para>
		/// <para>TransitionType must be STATE_TRANSITION_TYPE_IMMEDIATE.</para>
		/// </remarks>
		public StateTransitionType TransitionType;

		byte m_UpdateResourceState;
		/// <summary>
		/// <para>If set to true, the internal resource state will be set to NewState and the engine</para>
		/// <para>will be able to take over the resource state management. In this case it is the</para>
		/// <para>responsibility of the application to make sure that all subresources are indeed in</para>
		/// <para>designated state.</para>
		/// <para>If set to false, internal resource state will be unchanged.</para>
		/// </summary>
		/// <remarks>When TransitionType is STATE_TRANSITION_TYPE_BEGIN, this member must be false.</remarks>
		public bool UpdateResourceState
		{
			get => ( 0 != m_UpdateResourceState );
			set => m_UpdateResourceState = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public StateTransitionDesc( bool unused )
		{
			pTexture = IntPtr.Zero;
			pBuffer = IntPtr.Zero;
			FirstMipLevel = 0;
			MipLevelsCount = GraphicsTypes.RemainingMipLevels;
			FirstArraySlice = 0;
			ArraySliceCount = GraphicsTypes.RemainingArraySlices;
			OldState = ResourceState.Unknown;
			NewState = ResourceState.Unknown;
			TransitionType = StateTransitionType.Immediate;
			m_UpdateResourceState = 0;
		}
	}
}
