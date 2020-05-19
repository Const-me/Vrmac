// This source file was automatically generated from "RenderDevice.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Render device interface</summary>
	[ComInterface( "f0e9b607-ae33-4b2b-b1af-a8b2c3104022", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IRenderDevice: IDisposable
	{
		[RetValIndex] IBuffer CreateBuffer( [In] ref BufferDesc BuffDesc, [In, MarshalAs( UnmanagedType.LPUTF8Str )] string name, IntPtr dataPointer, int dataSize );

		/// <summary>Create a factory object which uploads larger chunks of memory into VRAM buffers.</summary>
		[RetValIndex] iUploadBuffer CreateBufferUploader( uint capacity );

		/// <summary>Create a factory object which creates indexed meshes from a stream of vertices</summary>
		[RetValIndex] iMeshIndexer CreateMeshIndexer( [In] ref sMeshIndexerSetup setup );

		/// <summary>Create a factory object which creates shaders.</summary>
		[RetValIndex] iShaderFactory GetShaderFactory();

		[RetValIndex] ITexture CreateTexture( [In] ref TextureDesc TexDesc, [MarshalAs( UnmanagedType.LPUTF8Str )] string name = null );

		/// <summary>Create a texture with initial data. If it’s from RAM, don’t forget to pin source array. If from VRAM, TextureSubResData.pSrcBuffer field must contain pointer from <see cref="ITexture.nativeCast()" /></summary>
		[RetValIndex] ITexture CreateTexture( [In] ref TextureDesc TexDesc, [In, MarshalAs( UnmanagedType.LPArray )] TextureSubResData[] data, uint length, [MarshalAs( UnmanagedType.LPUTF8Str )] string name = null );

		/// <summary>Create a texture by decoding an image</summary>
		[RetValIndex] ITexture LoadTexture( [ReadStream] System.IO.Stream source, eImageFileFormat format, [In] ref TextureLoadInfo loadInfo, [MarshalAs( UnmanagedType.LPUTF8Str )] string name = null );

		/// <summary>Creates a new sampler object</summary>
		/// <param name="SamDesc">Sampler description, see <see cref="SamplerDesc" /> for details.</param>
		/// <returns>
		/// <para>Address of the memory location where the pointer to the sampler interface will be stored.</para>
		/// <para>The function calls AddRef(), so that the new object will contain one reference.</para>
		/// </returns>
		/// <remarks>
		/// <para>If an application attempts to create a sampler interface with the same attributes as an existing interface, the same interface will be returned.</para>
		/// <para>In D3D11, 4096 unique sampler state objects can be created on a device at a time.</para>
		/// </remarks>
		[RetValIndex( 1 )] ISampler CreateSampler( [In] ref SamplerDesc SamDesc );

		/// <summary>Creates a new resource mapping</summary>
		/// <param name="MappingDesc">Resource mapping description, see <see cref="ResourceMappingDesc" /> for details.</param>
		/// <returns>
		/// <para>Address of the memory location where the pointer to the resource mapping interface will be stored.</para>
		/// <para>The function calls AddRef(), so that the new object will contain one reference.</para>
		/// </returns>
		[RetValIndex( 1 )] IResourceMapping CreateResourceMapping( [In] ref ResourceMappingDesc MappingDesc );

		/// <summary>Creates a new pipeline state object</summary>
		/// <param name="PipelineDesc">Pipeline state description, see <see cref="PipelineStateDesc" /> for details.</param>
		/// <returns>
		/// <para>Address of the memory location where the pointer to the pipeline state interface will be stored.</para>
		/// <para>The function calls AddRef(), so that the new object will contain one reference.</para>
		/// </returns>
		[RetValIndex( 1 )] IPipelineState CreatePipelineState( [In] ref PipelineStateDesc PipelineDesc );

		/// <summary>Create a factory object that helps making GPU pipeline states</summary>
		[RetValIndex] iPipelineStateFactory CreatePipelineStateFactory();

		/// <summary>Creates a new fence object</summary>
		/// <param name="Desc">Fence description, see <see cref="FenceDesc" /> for details.</param>
		/// <returns>
		/// <para>Address of the memory location where the pointer to the fence interface will be stored.</para>
		/// <para>The function calls AddRef(), so that the new object will contain one reference.</para>
		/// </returns>
		[RetValIndex( 1 )] IFence CreateFence( [In] ref FenceDesc Desc );

		/// <summary>Creates a new query object</summary>
		/// <param name="Desc">Query description, see <see cref="QueryDesc" /> for details.</param>
		/// <returns>
		/// <para>Address of the memory location where the pointer to the query interface will be stored.</para>
		/// <para>The function calls AddRef(), so that the new object will contain one reference.</para>
		/// </returns>
		[RetValIndex( 1 )] IQuery CreateQuery( [In] ref QueryDesc Desc );

		/// <summary>Gets the device capabilities, see <see cref="DeviceCaps" /> for details</summary>
		[RetValIndex] DeviceCaps GetDeviceCaps();

		/// <summary>Returns the basic texture format information.</summary>
		/// <param name="TexFormat">Texture format for which to provide the information</param>
		/// <remarks>See <see cref="TextureFormatInfo" /> for details on the provided information.</remarks>
		[RetValIndex( 1 )] TextureFormatInfo GetTextureFormatInfo( TextureFormat TexFormat );

		/// <summary>Returns the extended texture format information.</summary>
		/// <param name="TexFormat">Texture format for which to provide the information</param>
		/// <remarks>
		/// <para>See <see cref="TextureFormatInfoExt" /> for details on the provided information.</para>
		/// <para>The first time this method is called for a particular format, it may be considerably slower than GetTextureFormatInfo(). If you do not require extended information, call GetTextureFormatInfo() instead.</para>
		/// </remarks>
		[RetValIndex( 1 )] TextureFormatInfoExt GetTextureFormatInfoExt( TextureFormat TexFormat );

		/// <summary>Purges device release queues and releases all stale resources.</summary>
		/// <param name="ForceRelease">Forces release of all objects. Use this option with great care only if you are sure the resources are not in use by the GPU (such as when the device has just been idled).</param>
		/// <remarks>This method is automatically called by ISwapChain::Present() of the primary swap chain.</remarks>
		void ReleaseStaleResources( [MarshalAs( UnmanagedType.U1 )] bool ForceRelease );

		/// <summary>Waits until all outstanding operations on the GPU are complete.</summary>
		/// <remarks>
		/// <para>The method blocks the execution of the calling thread until the GPU is idle.</para>
		/// <para>The method does not flush immediate contexts, so it will only wait for commands that have been previously submitted for execution. An application should explicitly flush the contexts using IDeviceContext::Flush()
		/// if it needs to make sure all recorded commands are complete when the method returns.</para>
		/// </remarks>
		void IdleGPU();

	}
}
