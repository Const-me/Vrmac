// This source file was automatically generated from "DeviceContext.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Draw command flags</summary>
	[Flags]
	public enum DrawFlags : byte
	{
		/// <summary>No flags.</summary>
		None = 0x0,
		/// <summary>
		/// <para>Verify the sate of index and vertex buffers (if any) used by the draw</para>
		/// <para>command. State validation is only performed in debug and development builds</para>
		/// <para>and the flag has no effect in release build.</para>
		/// </summary>
		VerifyStates = 0x1,
		/// <summary>Verify correctness of parameters passed to the draw command.</summary>
		VerifyDrawAttribs = 0x2,
		/// <summary>Verify that render targets bound to the context are consistent with the pipeline state.</summary>
		VerifyRenderTargets = 0x4,
		/// <summary>Perform all state validation checks</summary>
		VerifyAll = 7,
		/// <summary>
		/// <para>Indicates that none of the dynamic resource buffers used by the draw command</para>
		/// <para>have been modified by the CPU since the last command.</para>
		/// </summary>
		/// <remarks>
		/// <para>This flag should be used to improve performance when an application issues a</para>
		/// <para>series of draw commands that use the same pipeline state and shader resources and</para>
		/// <para>no dynamic buffers (constant or bound as shader resources) are updated between the</para>
		/// <para>commands.</para>
		/// <para>The flag has no effect on dynamic vertex and index buffers.</para>
		/// <para>Details</para>
		/// <para>D3D12 and Vulkan back-ends have to perform some work to make data in buffers</para>
		/// <para>available to draw commands. When a dynamic buffer is mapped, the engine allocates</para>
		/// <para>new memory and assigns a new GPU address to this buffer. When a draw command is issued,</para>
		/// <para>this GPU address needs to be used. By default the engine assumes that the CPU may</para>
		/// <para>map the buffer before any command (to write new transformation matrices for example)</para>
		/// <para>and that all GPU addresses need to always be refreshed. This is not always the case,</para>
		/// <para>and the application may use the flag to inform the engine that the data in the buffer</para>
		/// <para>stay intact to avoid extra work.</para>
		/// <para>Note that after a new PSO is bound or an SRB is committed, the engine will always set all</para>
		/// <para>required buffer addresses/offsets regardless of the flag. The flag will only take effect</para>
		/// <para>on the second and susbequent draw calls that use the same PSO and SRB.</para>
		/// <para>The flag has no effect in D3D11 and OpenGL backends.</para>
		/// <para>Implementation details</para>
		/// <para>Vulkan backend allocates VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC descriptors for all uniform (constant),</para>
		/// <para>buffers and VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC descriptors for storage buffers.</para>
		/// <para>Note that HLSL structured buffers are mapped to read-only storage buffers in SPIRV and RW buffers</para>
		/// <para>are mapped to RW-storage buffers.</para>
		/// <para>By default, all dynamic descriptor sets that have dynamic buffers bound are updated every time a draw command is</para>
		/// <para>issued (see PipelineStateVkImpl::BindDescriptorSetsWithDynamicOffsets). When DRAW_FLAG_DYNAMIC_RESOURCE_BUFFERS_INTACT</para>
		/// <para>is specified, dynamic descriptor sets are only bound by the first draw command that uses the PSO and the SRB.</para>
		/// <para>The flag avoids binding descriptors with the same offsets if none of the dynamic offsets have changed.</para>
		/// <para>Direct3D12 backend binds constant buffers to root views. By default the engine assumes that virtual GPU addresses</para>
		/// <para>of all dynamic buffers may change between the draw commands and always binds dynamic buffers to root views</para>
		/// <para>(see RootSignature::CommitRootViews). When DRAW_FLAG_DYNAMIC_RESOURCE_BUFFERS_INTACT is set, root views are only bound</para>
		/// <para>by the first draw command that uses the PSO + SRB pair. The flag avoids setting the same GPU virtual addresses when</para>
		/// <para>they stay unchanged.</para>
		/// </remarks>
		DynamicResourceBuffersIntact = 0x8
	}

	/// <summary>
	/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation</para>
	/// <para>of resource state management in Diligent Engine.</para>
	/// </summary>
	public enum ResourceStateTransitionMode : byte
	{
		/// <summary>
		/// <para>Perform no state transitions and no state validation.</para>
		/// <para>Resource states are not accessed (either read or written) by the command.</para>
		/// </summary>
		None = 0,
		/// <summary>
		/// <para>Transition resources to the states required by the specific command.</para>
		/// <para>Resources in unknown state are ignored.</para>
		/// </summary>
		/// <remarks>
		/// <para>Any method that uses this mode may alter the state of the resources it works with.</para>
		/// <para>As automatic state management is not thread-safe, no other thread is allowed to read</para>
		/// <para>or write the state of the resources being transitioned.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to</para>
		/// <para>explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation</para>
		/// <para>of resource state management in Diligent Engine.</para>
		/// </remarks>
		Transition = 1,
		/// <summary>
		/// <para>Do not transition, but verify that states are correct.</para>
		/// <para>No validation is performed if the state is unknown to the engine.</para>
		/// <para>This mode only has effect in debug and development builds. No validation</para>
		/// <para>is performed in release build.</para>
		/// </summary>
		/// <remarks>
		/// <para>Any method that uses this mode will read the state of resources it works with.</para>
		/// <para>As automatic state management is not thread-safe, no other thread is allowed to alter</para>
		/// <para>the state of resources being used by the command. It is safe to read these states.</para>
		/// </remarks>
		Verify = 2
	}

	/// <summary>These flags are used by IDeviceContext::ClearDepthStencil().</summary>
	[Flags]
	public enum ClearDepthStencilFlags : uint
	{
		/// <summary>Perform no clear.</summary>
		DepthFlagNone = 0x0,
		/// <summary>Clear depth part of the buffer.</summary>
		DepthFlag = 0x1,
		/// <summary>Clear stencil part of the buffer.</summary>
		StencilFlag = 0x2
	}

	/// <summary>Defines allowed flags for IDeviceContext::SetVertexBuffers() function.</summary>
	[Flags]
	public enum SetVertexBuffersFlags : byte
	{
		/// <summary>No extra operations.</summary>
		None = 0x0,
		/// <summary>
		/// <para>Reset the vertex buffers to only the buffers specified in this</para>
		/// <para>call. All buffers previously bound to the pipeline will be unbound.</para>
		/// </summary>
		Reset = 0x1
	}

	/// <summary>This structure is used by IDeviceContext::Draw().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DrawAttribs
	{
		/// <summary>The number of vertices to draw.</summary>
		public int NumVertices;

		/// <summary>Additional flags, see Diligent::DRAW_FLAGS.</summary>
		public DrawFlags Flags;

		/// <summary>
		/// <para>The number of instances to draw. If more than one instance is specified,</para>
		/// <para>instanced draw call will be performed.</para>
		/// </summary>
		public int NumInstances;

		/// <summary>
		/// <para>LOCATION (or INDEX, but NOT the byte offset) of the first vertex in the</para>
		/// <para>vertex buffer to start reading vertices from.</para>
		/// </summary>
		public int StartVertexLocation;

		/// <summary>
		/// <para>LOCATION (or INDEX, but NOT the byte offset) in the vertex buffer to start</para>
		/// <para>reading instance data from.</para>
		/// </summary>
		public int FirstInstanceLocation;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DrawAttribs( bool unused )
		{
			NumVertices = 0;
			Flags = DrawFlags.None;
			NumInstances = 1;
			StartVertexLocation = 0;
			FirstInstanceLocation = 0;
		}
	}

	/// <summary>This structure is used by IDeviceContext::DrawIndexed().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DrawIndexedAttribs
	{
		/// <summary>The number of indices to draw.</summary>
		public int NumIndices;

		/// <summary>
		/// <para>The type of elements in the index buffer.</para>
		/// <para>Allowed values: VT_UINT16 and VT_UINT32.</para>
		/// </summary>
		public GpuValueType IndexType;

		/// <summary>Additional flags, see Diligent::DRAW_FLAGS.</summary>
		public DrawFlags Flags;

		/// <summary>
		/// <para>Number of instances to draw. If more than one instance is specified,</para>
		/// <para>instanced draw call will be performed.</para>
		/// </summary>
		public int NumInstances;

		/// <summary>
		/// <para>LOCATION (NOT the byte offset) of the first index in</para>
		/// <para>the index buffer to start reading indices from.</para>
		/// </summary>
		public int FirstIndexLocation;

		/// <summary>A constant which is added to each index before accessing the vertex buffer.</summary>
		public int BaseVertex;

		/// <summary>
		/// <para>LOCATION (or INDEX, but NOT the byte offset) in the vertex</para>
		/// <para>buffer to start reading instance data from.</para>
		/// </summary>
		public int FirstInstanceLocation;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DrawIndexedAttribs( bool unused )
		{
			NumIndices = 0;
			IndexType = GpuValueType.Undefined;
			Flags = DrawFlags.None;
			NumInstances = 1;
			FirstIndexLocation = 0;
			BaseVertex = 0;
			FirstInstanceLocation = 0;
		}
	}

	/// <summary>This structure is used by IDeviceContext::DrawIndirect().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DrawIndirectAttribs
	{
		/// <summary>Additional flags, see Diligent::DRAW_FLAGS.</summary>
		public DrawFlags Flags;

		/// <summary>State transition mode for indirect draw arguments buffer.</summary>
		public ResourceStateTransitionMode IndirectAttribsBufferStateTransitionMode;

		/// <summary>Offset from the beginning of the buffer to the location of draw command attributes.</summary>
		public int IndirectDrawArgsOffset;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DrawIndirectAttribs( bool unused )
		{
			Flags = DrawFlags.None;
			IndirectAttribsBufferStateTransitionMode = ResourceStateTransitionMode.None;
			IndirectDrawArgsOffset = 0;
		}
	}

	/// <summary>This structure is used by IDeviceContext::DrawIndexedIndirect().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DrawIndexedIndirectAttribs
	{
		/// <summary>
		/// <para>The type of the elements in the index buffer.</para>
		/// <para>Allowed values: VT_UINT16 and VT_UINT32.</para>
		/// </summary>
		public GpuValueType IndexType;

		/// <summary>Additional flags, see Diligent::DRAW_FLAGS.</summary>
		public DrawFlags Flags;

		/// <summary>State transition mode for indirect draw arguments buffer.</summary>
		public ResourceStateTransitionMode IndirectAttribsBufferStateTransitionMode;

		/// <summary>Offset from the beginning of the buffer to the location of draw command attributes.</summary>
		public int IndirectDrawArgsOffset;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DrawIndexedIndirectAttribs( bool unused )
		{
			IndexType = GpuValueType.Undefined;
			Flags = DrawFlags.None;
			IndirectAttribsBufferStateTransitionMode = ResourceStateTransitionMode.None;
			IndirectDrawArgsOffset = 0;
		}
	}

	/// <summary>This structure is used by IDeviceContext::DispatchCompute().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DispatchComputeAttribs
	{
		/// <summary>Number of groups dispatched in X direction.</summary>
		public int ThreadGroupCountX;

		/// <summary>Number of groups dispatched in Y direction.</summary>
		public int ThreadGroupCountY;

		/// <summary>Number of groups dispatched in Z direction.</summary>
		public int ThreadGroupCountZ;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DispatchComputeAttribs( bool unused )
		{
			ThreadGroupCountX = 1;
			ThreadGroupCountY = 1;
			ThreadGroupCountZ = 1;
		}
	}

	/// <summary>This structure is used by IDeviceContext::DispatchComputeIndirect().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DispatchComputeIndirectAttribs
	{
		/// <summary>State transition mode for indirect dispatch attributes buffer.</summary>
		public ResourceStateTransitionMode IndirectAttribsBufferStateTransitionMode;

		/// <summary>The offset from the beginning of the buffer to the dispatch command arguments.</summary>
		public int DispatchArgsByteOffset;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DispatchComputeIndirectAttribs( bool unused )
		{
			IndirectAttribsBufferStateTransitionMode = ResourceStateTransitionMode.None;
			DispatchArgsByteOffset = 0;
		}
	}

	/// <summary>This structure is used by IDeviceContext::ResolveTextureSubresource().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ResolveTextureSubresourceAttribs
	{
		/// <summary>Mip level of the source multi-sampled texture to resolve.</summary>
		public int SrcMipLevel;

		/// <summary>Array slice of the source multi-sampled texture to resolve.</summary>
		public int SrcSlice;

		/// <summary>Source texture state transition mode, see Diligent::RESOURCE_STATE_TRANSITION_MODE.</summary>
		public ResourceStateTransitionMode SrcTextureTransitionMode;

		/// <summary>Mip level of the destination non-multi-sampled texture.</summary>
		public int DstMipLevel;

		/// <summary>Array slice of the destination non-multi-sampled texture.</summary>
		public int DstSlice;

		/// <summary>Destination texture state transition mode, see Diligent::RESOURCE_STATE_TRANSITION_MODE.</summary>
		public ResourceStateTransitionMode DstTextureTransitionMode;

		/// <summary>
		/// <para>If one or both textures are typeless, specifies the type of the typeless texture.</para>
		/// <para>If both texture formats are not typeless, in which case they must be identical, this member must be</para>
		/// <para>either TEX_FORMAT_UNKNOWN, or match this format.</para>
		/// </summary>
		public TextureFormat Format;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ResolveTextureSubresourceAttribs( bool unused )
		{
			SrcMipLevel = 0;
			SrcSlice = 0;
			SrcTextureTransitionMode = ResourceStateTransitionMode.None;
			DstMipLevel = 0;
			DstSlice = 0;
			DstTextureTransitionMode = ResourceStateTransitionMode.None;
			Format = TextureFormat.Unknown;
		}
	}

	/// <summary>This structure is used by IDeviceContext::SetViewports().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Viewport
	{
		/// <summary>X coordinate of the left boundary of the viewport.</summary>
		public float TopLeftX;

		/// <summary>
		/// <para>Y coordinate of the top boundary of the viewport.</para>
		/// <para>When defining a viewport, DirectX convention is used:</para>
		/// <para>window coordinate systems originates in the LEFT TOP corner</para>
		/// <para>of the screen with Y axis pointing down.</para>
		/// </summary>
		public float TopLeftY;

		/// <summary>Viewport width.</summary>
		public float Width;

		/// <summary>Viewport Height.</summary>
		public float Height;

		/// <summary>Minimum depth of the viewport. Ranges between 0 and 1.</summary>
		public float MinDepth;

		/// <summary>Maximum depth of the viewport. Ranges between 0 and 1.</summary>
		public float MaxDepth;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public Viewport( bool unused )
		{
			TopLeftX = 0.0f;
			TopLeftY = 0.0f;
			Width = 0.0f;
			Height = 0.0f;
			MinDepth = 0.0f;
			MaxDepth = 1.0f;
		}
	}

	/// <summary>This structure is used by IDeviceContext::CopyTexture().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct CopyTextureAttribs
	{
		/// <summary>Source texture to copy data from.</summary>
		public IntPtr pSrcTexture;

		/// <summary>Mip level of the source texture to copy data from.</summary>
		public int SrcMipLevel;

		/// <summary>Array slice of the source texture to copy data from. Must be 0 for non-array textures.</summary>
		public int SrcSlice;

		/// <summary>Source region to copy. Use nullptr to copy the entire subresource.</summary>
		public IntPtr pSrcBox;

		/// <summary>Source texture state transition mode (see Diligent::RESOURCE_STATE_TRANSITION_MODE).</summary>
		public ResourceStateTransitionMode SrcTextureTransitionMode;

		/// <summary>Destination texture.</summary>
		public IntPtr pDstTexture;

		/// <summary>Destination mip level.</summary>
		public int DstMipLevel;

		/// <summary>Destination array slice. Must be 0 for non-array textures.</summary>
		public int DstSlice;

		/// <summary>X offset on the destination subresource.</summary>
		public int DstX;

		/// <summary>Y offset on the destination subresource.</summary>
		public int DstY;

		/// <summary>Z offset on the destination subresource</summary>
		public int DstZ;

		/// <summary>Destination texture state transition mode (see Diligent::RESOURCE_STATE_TRANSITION_MODE).</summary>
		public ResourceStateTransitionMode DstTextureTransitionMode;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public CopyTextureAttribs( bool unused )
		{
			pSrcTexture = IntPtr.Zero;
			SrcMipLevel = 0;
			SrcSlice = 0;
			pSrcBox = IntPtr.Zero;
			SrcTextureTransitionMode = ResourceStateTransitionMode.None;
			pDstTexture = IntPtr.Zero;
			DstMipLevel = 0;
			DstSlice = 0;
			DstX = 0;
			DstY = 0;
			DstZ = 0;
			DstTextureTransitionMode = ResourceStateTransitionMode.None;
		}
	}

	/// <remarks>
	/// <para>Device context keeps strong references to all objects currently bound to</para>
	/// <para>the pipeline: buffers, states, samplers, shaders, etc.</para>
	/// <para>The context also keeps strong reference to the device and</para>
	/// <para>the swap chain.</para>
	/// </remarks>
	[ComInterface( "dc92711b-a1be-4319-b2bd-c662d1cc19e4", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IDeviceContext: IDisposable
	{
		/// <summary>Sets the pipeline state.</summary>
		/// <param name="pPipelineState">Pointer to IPipelineState interface to bind to the context.</param>
		void SetPipelineState( IPipelineState pPipelineState );

		/// <summary>Transitions shader resources to the states required by Draw or Dispatch command.</summary>
		/// <param name="pPipelineState">Pipeline state object that was used to create the shader resource binding.</param>
		/// <param name="pShaderResourceBinding">Shader resource binding whose resources will be transitioned.</param>
		/// <remarks>
		/// <para>This method explicitly transitiones all resources except ones in unknown state to the states required by Draw or Dispatch command.</para>
		/// <para>If this method was called, there is no need to use <see cref="ResourceStateTransitionMode.Transition" /> when calling IDeviceContext::CommitShaderResources()</para>
		/// <para>Resource state transitioning is not thread safe. As the method may alter the states of resources referenced by the shader resource binding, no other thread is allowed to read or write these states.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation of resource state management in Diligent Engine.</para>
		/// </remarks>
		void TransitionShaderResources( IPipelineState pPipelineState, IShaderResourceBinding pShaderResourceBinding );

		/// <summary>Commits shader resources to the device context.</summary>
		/// <param name="pShaderResourceBinding">
		/// <para>Shader resource binding whose resources will be committed.</para>
		/// <para>If pipeline state contains no shader resources, this parameter can be null.</para>
		/// </param>
		/// <param name="StateTransitionMode">State transition mode (see <see cref="ResourceStateTransitionMode" /> ).</param>
		/// <remarks>
		/// <para>Pipeline state object that was used to create the shader resource binding must be bound to the pipeline when CommitShaderResources() is called. If no pipeline state object is bound or the pipeline state object does
		/// not match the shader resource binding, the method will fail. If <see cref="ResourceStateTransitionMode.Transition" /> mode is used, the engine will also transition all shader resources to required states. If the flag is
		/// not set, it is assumed that all resources are already in correct states. Resources can be explicitly transitioned to required states by calling IDeviceContext::TransitionShaderResources() or
		/// IDeviceContext::TransitionResourceStates().</para>
		/// <para>Automatic resource state transitioning is not thread-safe.</para>
		/// <para>- If <see cref="ResourceStateTransitionMode.Transition" /> mode is used, the method may alter the states of resources referenced by the shader resource binding and no other thread is allowed to read or write these
		/// states.</para>
		/// <para>- If <see cref="ResourceStateTransitionMode.Verify" /> mode is used, the method will read the states, so no other thread should alter the states by calling any of the methods that use
		/// <see cref="ResourceStateTransitionMode.Transition" /> mode.</para>
		/// <para>It is safe for other threads to read the states.</para>
		/// <para>- If <see cref="ResourceStateTransitionMode.None" /> mode is used, the method does not access the states of resources.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it should manage the states manually by setting the state to <see cref="ResourceState.Unknown" /> (which will disable automatic
		/// state management) using IBuffer::SetState() or ITexture::SetState() and explicitly transitioning the states with IDeviceContext::TransitionResourceStates().</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation of resource state management in Diligent Engine.</para>
		/// <para>If an application calls any method that changes the state of any resource after it has been committed, the application is responsible for transitioning the resource back to correct state using one of the available
		/// methods before issuing the next draw or dispatch command.</para>
		/// </remarks>
		void CommitShaderResources( IShaderResourceBinding pShaderResourceBinding, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Sets the stencil reference value.</summary>
		/// <param name="StencilRef">Stencil reference value.</param>
		void SetStencilRef( int StencilRef );

		/// <summary>\param [in] pBlendFactors - Array of four blend factors, one for each RGBA component.</summary>
		/// <remarks>Theses factors are used if the blend state uses one of the <see cref="BlendFactor.BlendFactor" /> or <see cref="BlendFactor.InvBlendFactor" /> blend factors. If nullptr is provided, default blend factors array
		/// {1,1,1,1} will be used.</remarks>
		void SetBlendFactors( [In] ref Vector4 blendFactors );

		/// <summary>Binds vertex buffers to the pipeline.</summary>
		/// <param name="StartSlot">The first input slot for binding. The first vertex buffer is explicitly bound to the start slot; each additional vertex buffer in the array is implicitly bound to each subsequent input slot.
		/// </param>
		/// <param name="NumBuffersSet">The number of vertex buffers in the array.</param>
		/// <param name="buffers">
		/// <para>A pointer to an array of vertex buffers.</para>
		/// <para>The buffers must have been created with the <see cref="BindFlags.VertexBuffer" /> flag.</para>
		/// </param>
		/// <param name="offsets">Pointer to an array of offset values; one offset value for each buffer in the vertex-buffer array. Each offset is the number of bytes between the first element of a vertex buffer and the first
		/// element that will be used. If this parameter is nullptr, zero offsets for all buffers will be used.</param>
		/// <param name="StateTransitionMode">State transition mode for buffers being set (see <see cref="ResourceStateTransitionMode" /> ).</param>
		/// <param name="Flags">Additional flags. See <see cref="SetVertexBuffersFlags" /> for a list of allowed values.</param>
		/// <remarks>
		/// <para>The device context keeps strong references to all bound vertex buffers.</para>
		/// <para>Thus a buffer cannot be released until it is unbound from the context. It is suggested to specify <see cref="SetVertexBuffersFlags.Reset" /> flag whenever possible. This will assure that no buffers from previous
		/// draw calls are bound to the pipeline.</para>
		/// <para>When StateTransitionMode is <see cref="ResourceStateTransitionMode.Transition" /> , the method will transition all buffers in known states to <see cref="ResourceState.VertexBuffer" /> . Resource state transitioning
		/// is not thread safe, so no other thread is allowed to read or write the states of these buffers.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation of resource state management in Diligent Engine.</para>
		/// </remarks>
		void SetVertexBuffers( int StartSlot, int NumBuffersSet, IBuffer[] buffers, [In] ref int offsets, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition, SetVertexBuffersFlags Flags = SetVertexBuffersFlags.Reset );

		void SetVertexBuffer( uint StartSlot, IBuffer buffer, uint offset, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition, SetVertexBuffersFlags Flags = SetVertexBuffersFlags.Reset );

		/// <summary>Invalidates the cached context state.</summary>
		/// <remarks>This method should be called by an application to invalidate internal cached states.</remarks>
		void InvalidateState();

		/// <summary>Binds an index buffer to the pipeline.</summary>
		/// <param name="pIndexBuffer">Pointer to the index buffer. The buffer must have been created with the <see cref="BindFlags.IndexBuffer" /> flag.</param>
		/// <param name="ByteOffset">Offset from the beginning of the buffer to the start of index data.</param>
		/// <param name="StateTransitionMode">State transiton mode for the index buffer to bind (see <see cref="ResourceStateTransitionMode" /> ).</param>
		/// <remarks>
		/// <para>The device context keeps strong reference to the index buffer.</para>
		/// <para>Thus an index buffer object cannot be released until it is unbound from the context.</para>
		/// <para>When StateTransitionMode is <see cref="ResourceStateTransitionMode.Transition" /> , the method will transition the buffer to <see cref="ResourceState.IndexBuffer" /> (if its state is not unknown). Resource state
		/// transitioning is not thread safe, so no other thread is allowed to read or write the state of the buffer.</para>
		/// <para>If the application intends to use the same resource in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation of resource state management in Diligent Engine.</para>
		/// </remarks>
		void SetIndexBuffer( IBuffer pIndexBuffer, int ByteOffset, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Sets an array of viewports.</summary>
		/// <param name="NumViewports">Number of viewports to set.</param>
		/// <param name="viewports">An array of <see cref="Viewport" /> structures describing the viewports to bind.</param>
		/// <param name="RTWidth">Render target width. If 0 is provided, width of the currently bound render target will be used.</param>
		/// <param name="RTHeight">Render target height. If 0 is provided, height of the currently bound render target will be used.</param>
		/// <remarks>
		/// <para>DirectX and OpenGL use different window coordinate systems. In DirectX, the coordinate system origin is in the left top corner of the screen with Y axis pointing down. In OpenGL, the origin is in the left bottom
		/// corener of the screen with Y axis pointing up. Render target size is required to convert viewport from DirectX to OpenGL coordinate system if OpenGL device is used.</para>
		/// <para>All viewports must be set atomically as one operation. Any viewports not defined by the call are disabled.</para>
		/// <para>You can set the viewport size to match the currently bound render target using the following call:</para>
		/// <para>pContext-&gt;SetViewports(1, nullptr, 0, 0);</para>
		/// </remarks>
		void SetViewports( int NumViewports, [In] ref Viewport viewports, int RTWidth, int RTHeight );

		/// <summary>Sets active scissor rects.</summary>
		/// <param name="NumRects">Number of scissor rectangles to set.</param>
		/// <param name="rectangles">An array of <see cref="CRect" /> structures describing the scissor rectangles to bind.</param>
		/// <param name="RTWidth">Render target width. If 0 is provided, width of the currently bound render target will be used.</param>
		/// <param name="RTHeight">Render target height. If 0 is provided, height of the currently bound render target will be used.</param>
		/// <remarks>
		/// <para>DirectX and OpenGL use different window coordinate systems. In DirectX, the coordinate system origin is in the left top corner of the screen with Y axis pointing down. In OpenGL, the origin is in the left bottom
		/// corener of the screen with Y axis pointing up. Render target size is required to convert viewport from DirectX to OpenGL coordinate system if OpenGL device is used.</para>
		/// <para>All scissor rects must be set atomically as one operation. Any rects not defined by the call are disabled.</para>
		/// </remarks>
		void SetScissorRects( int NumRects, [In] ref CRect rectangles, int RTWidth, int RTHeight );

		/// <summary>Binds one or more render targets and the depth-stencil buffer to the context. It also sets the viewport to match the first non-null render target or depth-stencil buffer.</summary>
		/// <param name="NumRenderTargets">Number of render targets to bind.</param>
		/// <param name="ppRenderTargets">Array of pointers to ITextureView that represent the render targets to bind to the device. The type of each view in the array must be <see cref="TextureViewType.RenderTarget" /> .</param>
		/// <param name="pDepthStencil">Pointer to the ITextureView that represents the depth stencil to bind to the device. The view type must be <see cref="TextureViewType.DepthStencil" /> .</param>
		/// <param name="StateTransitionMode">State transition mode of the render targets and depth stencil buffer being set (see <see cref="ResourceStateTransitionMode" /> ).</param>
		/// <remarks>
		/// <para>The device context will keep strong references to all bound render target and depth-stencil views. Thus these views (and consequently referenced textures) cannot be released until they are unbound from the context.
		/// Any render targets not defined by this call are set to nullptr.</para>
		/// <para>When StateTransitionMode is <see cref="ResourceStateTransitionMode.Transition" /> , the method will transition all render targets in known states to Diligent::RESOURCE_STATE_REDER_TARGET, and the depth-stencil
		/// buffer to <see cref="ResourceState.DepthWrite" /> state.</para>
		/// <para>Resource state transitioning is not thread safe, so no other thread is allowed to read or write the states of resources used by the command.</para>
		/// <para>If the application intends to use the same resource in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation of resource state management in Diligent Engine.</para>
		/// </remarks>
		void SetRenderTargets( int NumRenderTargets, ITextureView[] ppRenderTargets, ITextureView pDepthStencil, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition );

		void SetRenderTarget( ITextureView renderTarget, ITextureView depthStencil, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Executes a draw command.</summary>
		/// <param name="Attribs">Draw command attributes, see <see cref="DrawAttribs" /> for details.</param>
		/// <remarks>
		/// <para>If <see cref="DrawFlags.VerifyStates" /> flag is set, the method reads the state of vertex buffers, so no other threads are allowed to alter the states of the same resources.</para>
		/// <para>It is OK to read these states.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// </remarks>
		void Draw( [In] ref DrawAttribs Attribs );

		/// <summary>Executes an indexed draw command.</summary>
		/// <param name="Attribs">Draw command attributes, see <see cref="DrawIndexedAttribs" /> for details.</param>
		/// <remarks>
		/// <para>If <see cref="DrawFlags.VerifyStates" /> flag is set, the method reads the state of vertex/index buffers, so no other threads are allowed to alter the states of the same resources.</para>
		/// <para>It is OK to read these states.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// </remarks>
		void DrawIndexed( [In] ref DrawIndexedAttribs Attribs );

		/// <summary>Executes an indirect draw command.</summary>
		/// <param name="Attribs">Structure describing the command attributes, see <see cref="DrawIndirectAttribs" /> for details.</param>
		/// <param name="pAttribsBuffer">Pointer to the buffer, from which indirect draw attributes will be read.</param>
		/// <remarks>
		/// <para>If IndirectAttribsBufferStateTransitionMode member is <see cref="ResourceStateTransitionMode.Transition" /> , the method may transition the state of the indirect draw arguments buffer. This is not a thread safe
		/// operation, so no other thread is allowed to read or write the state of the buffer.</para>
		/// <para>If <see cref="DrawFlags.VerifyStates" /> flag is set, the method reads the state of vertex/index buffers, so no other threads are allowed to alter the states of the same resources.</para>
		/// <para>It is OK to read these states.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// </remarks>
		void DrawIndirect( [In] ref DrawIndirectAttribs Attribs, IBuffer pAttribsBuffer );

		/// <summary>Executes an indexed indirect draw command.</summary>
		/// <param name="Attribs">Structure describing the command attributes, see <see cref="DrawIndexedIndirectAttribs" /> for details.</param>
		/// <param name="pAttribsBuffer">Pointer to the buffer, from which indirect draw attributes will be read.</param>
		/// <remarks>
		/// <para>If IndirectAttribsBufferStateTransitionMode member is <see cref="ResourceStateTransitionMode.Transition" /> , the method may transition the state of the indirect draw arguments buffer. This is not a thread safe
		/// operation, so no other thread is allowed to read or write the state of the buffer.</para>
		/// <para>If <see cref="DrawFlags.VerifyStates" /> flag is set, the method reads the state of vertex/index buffers, so no other threads are allowed to alter the states of the same resources.</para>
		/// <para>It is OK to read these states.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// </remarks>
		void DrawIndexedIndirect( [In] ref DrawIndexedIndirectAttribs Attribs, IBuffer pAttribsBuffer );

		/// <summary>Executes a dispatch compute command.</summary>
		/// <param name="Attribs">Dispatch command attributes, see <see cref="DispatchComputeAttribs" /> for details.</param>
		void DispatchCompute( [In] ref DispatchComputeAttribs Attribs );

		/// <summary>Executes an indirect dispatch compute command.</summary>
		/// <param name="Attribs">The command attributes, see <see cref="DispatchComputeIndirectAttribs" /> for details.</param>
		/// <param name="pAttribsBuffer">Pointer to the buffer containing indirect dispatch arguments.</param>
		/// <remarks>
		/// <para>If IndirectAttribsBufferStateTransitionMode member is <see cref="ResourceStateTransitionMode.Transition" /> , the method may transition the state of indirect dispatch arguments buffer. This is not a thread safe
		/// operation, so no other thread is allowed to read or write the state of the same resource.</para>
		/// <para>If the application intends to use the same resources in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// </remarks>
		void DispatchComputeIndirect( [In] ref DispatchComputeIndirectAttribs Attribs, IBuffer pAttribsBuffer );

		/// <summary>Clears a depth-stencil view.</summary>
		/// <param name="pView">Pointer to ITextureView interface to clear. The view type must be <see cref="TextureViewType.DepthStencil" /> .</param>
		/// <param name="StateTransitionMode">state transition mode of the depth-stencil buffer to clear.</param>
		/// <param name="ClearFlags">Idicates which parts of the buffer to clear, see <see cref="ClearDepthStencilFlags" /> .</param>
		/// <param name="fDepth">Value to clear depth part of the view with.</param>
		/// <param name="Stencil">Value to clear stencil part of the view with.</param>
		/// <remarks>
		/// <para>The full extent of the view is always cleared. <see cref="Viewport" /> and scissor settings are not applied.</para>
		/// <para>The depth-stencil view must be bound to the pipeline for clear operation to be performed.</para>
		/// <para>When StateTransitionMode is <see cref="ResourceStateTransitionMode.Transition" /> , the method will transition the state of the texture to the state required by clear operation.</para>
		/// <para>In Direct3D12, this satate is always <see cref="ResourceState.DepthWrite" /> , however in Vulkan the state depends on whether the depth buffer is bound to the pipeline.</para>
		/// <para>Resource state transitioning is not thread safe, so no other thread is allowed to read or write the state of resources used by the command.</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation of resource state management in Diligent Engine.</para>
		/// </remarks>
		void ClearDepthStencil( ITextureView pView, ClearDepthStencilFlags ClearFlags, float fDepth, byte Stencil, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Clears a render target view</summary>
		/// <param name="pView">Pointer to ITextureView interface to clear. The view type must be <see cref="TextureViewType.RenderTarget" /> .</param>
		/// <param name="RGBA">
		/// <para>A 4-component array that represents the color to fill the render target with.</para>
		/// <para>If nullptr is provided, the default array {0,0,0,0} will be used.</para>
		/// </param>
		/// <param name="StateTransitionMode">Defines required state transitions (see <see cref="ResourceStateTransitionMode" /> )</param>
		/// <remarks>
		/// <para>The full extent of the view is always cleared. <see cref="Viewport" /> and scissor settings are not applied.</para>
		/// <para>The render target view must be bound to the pipeline for clear operation to be performed in OpenGL backend.</para>
		/// <para>When StateTransitionMode is <see cref="ResourceStateTransitionMode.Transition" /> , the method will transition the texture to the state required by the command. Resource state transitioning is not thread safe, so
		/// no other thread is allowed to read or write the states of the same textures.</para>
		/// <para>If the application intends to use the same resource in other threads simultaneously, it needs to explicitly manage the states using IDeviceContext::TransitionResourceStates() method.</para>
		/// <para>In D3D12 backend clearing render targets requires textures to always be transitioned to <see cref="ResourceState.RenderTarget" /> state. In Vulkan backend however this depends on whether a render pass has been
		/// started. To clear render target outside of a render pass, the texture must be transitioned to <see cref="ResourceState.CopyDest" /> state. Inside a render pass it must be in <see cref="ResourceState.RenderTarget" />
		/// state. When using Diligent::RESOURCE_STATE_TRANSITION_TRANSITION mode, the engine takes care of proper resource state transition, otherwise it is the responsibility of the application.</para>
		/// </remarks>
		void ClearRenderTarget( ITextureView pView, [In] ref Vector4 RGBA, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Finishes recording commands and generates a command list.</summary>
		/// <returns>Memory location where pointer to the recorded command list will be written.</returns>
		[RetValIndex] ICommandList FinishCommandList();

		/// <summary>Executes recorded commands in a command list.</summary>
		/// <param name="pCommandList">Pointer to the command list to executre.</param>
		/// <remarks>After command list is executed, it is no longer valid and should be released.</remarks>
		void ExecuteCommandList( ICommandList pCommandList );

		/// <summary>Tells the GPU to set a fence to a specified value after all previous work has completed.</summary>
		/// <param name="pFence">The fence to signal</param>
		/// <param name="Value">The value to set the fence to. This value must be greater than the previously signaled value on the same fence.</param>
		/// <remarks>
		/// <para>The method does not flush the context (an application can do this explcitly if needed) and the fence will be signaled only when the command context is flushed next time.</para>
		/// <para>If an application needs to wait for the fence in a loop, it must flush the context after signalling the fence.</para>
		/// </remarks>
		void SignalFence( IFence pFence, ulong Value );

		/// <summary>Waits until the specified fence reaches or exceeds the specified value, on the host.</summary>
		/// <param name="pFence">The fence to wait.</param>
		/// <param name="Value">The value that the context is waiting for the fence to reach.</param>
		/// <param name="FlushContext">Whether to flush the commands in the context before initiating the wait.</param>
		/// <remarks>
		/// <para>The method blocks the execution of the calling thread until the wait is complete.</para>
		/// <para>Wait is only allowed for immediate contexts. When FlushContext is true, the method flushes the context before initiating the wait (see IDeviceContext::Flush()), so an application must explicitly reset the PSO and
		/// bind all required shader resources after waiting for the fence. If FlushContext is false, the commands preceding the fence (including signaling the fence itself) may not have been submitted to the GPU and the method may
		/// never return. If an application does not explicitly flush the context, it should typically set FlushContext to true. If the value the context is waiting for has never been signaled, the method may never return. The fence
		/// can only be waited for from the same context it has previously been signaled.</para>
		/// </remarks>
		void WaitForFence( IFence pFence, ulong Value, [MarshalAs( UnmanagedType.U1 )] bool FlushContext );

		/// <summary>Submits all outstanding commands for execution to the GPU and waits until they are complete.</summary>
		/// <remarks>
		/// <para>The method blocks the execution of the calling thread until the wait is complete.</para>
		/// <para>Only immediate contexts can be idled. The methods implicitly flushes the context (see IDeviceContext::Flush()), so an application must explicitly reset the PSO and bind all required shader resources after idling
		/// the context.</para>
		/// </remarks>
		void WaitForIdle();

		/// <summary>Marks the beginning of a query.</summary>
		/// <param name="pQuery">A pointer to a query object.</param>
		/// <remarks>
		/// <para>Only immediate contexts can begin a query.</para>
		/// <para>Vulkan requires that a query must either begin and end inside the same subpass of a render pass instance, or must both begin and end outside of a render pass instance. This means that an application must either
		/// begin and end a query while preserving render targets, or begin it when no render targets are bound to the context. In the latter case the engine will automaticaly end the render pass, if needed, when the query is ended.
		/// </para>
		/// <para>Also note that resource transitions must be performed outside of a render pass, and may thus require ending current render pass.</para>
		/// <para>To explicitly end current render pass, call SetRenderTargets(0, nullptr, nullptr, <see cref="ResourceStateTransitionMode.None" /></para>
		/// <para>OpenGL and Vulkan do not support nested queries of the same type.</para>
		/// </remarks>
		void BeginQuery( IQuery pQuery );

		/// <summary>Marks the end of a query.</summary>
		/// <param name="pQuery">A pointer to a query object.</param>
		/// <remarks>
		/// <para>A query must be ended by the same context that began it.</para>
		/// <para>In Direct3D12 and Vulkan, queries (except for timestamp queries) cannot span command list boundaries, so the engine will never flush the context even if the number of commands exceeds the user-specified limit when
		/// there is an active query.</para>
		/// <para>It is an error to explicitly flush the context while a query is active.</para>
		/// <para>All queries must be ended when IDeviceContext::FinishFrame() is called.</para>
		/// </remarks>
		void EndQuery( IQuery pQuery );

		/// <summary>Submits all pending commands in the context for execution to the command queue.</summary>
		/// <remarks>
		/// <para>Only immediate contexts can be flushed. Internally the method resets the state of the current command list/buffer.</para>
		/// <para>When the next draw command is issued, the engine will restore all states (rebind render targets and depth-stencil buffer as well as index and vertex buffers, restore viewports and scissor rects, etc.) except for
		/// the pipeline state and shader resource bindings. An application must explicitly reset the PSO and bind all required shader resources after flushing the context.</para>
		/// </remarks>
		void Flush();

		/// <summary>Updates the data in the buffer.</summary>
		/// <param name="pBuffer">Pointer to the buffer to updates.</param>
		/// <param name="Offset">Offset in bytes from the beginning of the buffer to the update region.</param>
		/// <param name="Size">Size in bytes of the data region to update.</param>
		/// <param name="pData">Pointer to the data to write to the buffer.</param>
		/// <param name="StateTransitionMode">Buffer state transition mode (see <see cref="ResourceStateTransitionMode" /> )</param>
		void UpdateBuffer( IBuffer pBuffer, int Offset, int Size, IntPtr pData, ResourceStateTransitionMode StateTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Copies the data from one buffer to another.</summary>
		/// <param name="pSrcBuffer">Source buffer to copy data from.</param>
		/// <param name="SrcOffset">Offset in bytes from the beginning of the source buffer to the beginning of data to copy.</param>
		/// <param name="SrcBufferTransitionMode">State transition mode of the source buffer (see <see cref="ResourceStateTransitionMode" /> ).</param>
		/// <param name="pDstBuffer">Destination buffer to copy data to.</param>
		/// <param name="DstOffset">Offset in bytes from the beginning of the destination buffer to the beginning of the destination region.</param>
		/// <param name="Size">Size in bytes of data to copy.</param>
		/// <param name="DstBufferTransitionMode">State transition mode of the destination buffer (see <see cref="ResourceStateTransitionMode" /> ).</param>
		void CopyBuffer( IBuffer pSrcBuffer, int SrcOffset, ResourceStateTransitionMode SrcBufferTransitionMode, IBuffer pDstBuffer, int DstOffset, int Size, ResourceStateTransitionMode DstBufferTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Maps the buffer.</summary>
		/// <param name="pBuffer">Pointer to the buffer to map.</param>
		/// <param name="MapType">Type of the map operation. See <see cref="MapType" /> .</param>
		/// <param name="MapFlags">Special map flags. See <see cref="MapFlags" /> .</param>
		[RetValIndex( 3 )] IntPtr MapBuffer( IBuffer pBuffer, MapType MapType, MapFlags MapFlags );

		/// <summary>Map the buffer with MapType.Write access and MapFlags.Discard flags, copy data from the source pointer, and unmap the buffer.</summary>
		void MapBufferWriteDiscard( IBuffer buffer, IntPtr sourcePointer, int sizeBytes );

		/// <summary>Unmaps the previously mapped buffer.</summary>
		/// <param name="pBuffer">Pointer to the buffer to unmap.</param>
		/// <param name="MapType">Type of the map operation. This parameter must match the type that was provided to the Map() method.</param>
		void UnmapBuffer( IBuffer pBuffer, MapType MapType );

		/// <summary>Updates the data in the texture.</summary>
		/// <param name="pTexture">Pointer to the device context interface to be used to perform the operation.</param>
		/// <param name="MipLevel">Mip level of the texture subresource to update.</param>
		/// <param name="Slice">Array slice. Should be 0 for non-array textures.</param>
		/// <param name="DstBox">Destination region on the texture to update.</param>
		/// <param name="SubresData">Source data to copy to the texture.</param>
		/// <param name="SrcBufferTransitionMode">
		/// <para>If pSrcBuffer member of <see cref="TextureSubResData" /> structure is not null, this parameter defines state transition mode of the source buffer.</para>
		/// <para>If pSrcBuffer is null, this parameter is ignored.</para>
		/// </param>
		/// <param name="TextureTransitionMode">Texture state transition mode (see <see cref="ResourceStateTransitionMode" /> )</param>
		void UpdateTexture( ITexture pTexture, int MipLevel, int Slice, [In] ref Box DstBox, [In] ref TextureSubResData SubresData, ResourceStateTransitionMode SrcBufferTransitionMode = ResourceStateTransitionMode.Transition, ResourceStateTransitionMode TextureTransitionMode = ResourceStateTransitionMode.Transition );

		/// <summary>Copies data from one texture to another.</summary>
		/// <param name="CopyAttribs">Structure describing copy command attributes, see <see cref="CopyTextureAttribs" /> for details.</param>
		void CopyTexture( [In] ref CopyTextureAttribs CopyAttribs );

		/// <summary>Maps the texture subresource.</summary>
		/// <param name="pTexture">Pointer to the texture to map.</param>
		/// <param name="MipLevel">Mip level to map.</param>
		/// <param name="ArraySlice">Array slice to map. This parameter must be 0 for non-array textures.</param>
		/// <param name="MapType">Type of the map operation. See <see cref="MapType" /> .</param>
		/// <param name="MapFlags">Special map flags. See <see cref="MapFlags" /> .</param>
		/// <param name="pMapRegion">Texture region to map. If this parameter is null, the entire subresource is mapped.</param>
		/// <returns>Mapped texture region data</returns>
		/// <remarks>
		/// <para>This method is supported in D3D11, D3D12 and Vulkan backends. In D3D11 backend, only the entire subresource can be mapped, so pMapRegion must either be null, or cover the entire subresource.</para>
		/// <para>In D3D11 and Vulkan backends, dynamic textures are no different from non-dynamic textures, and mapping with <see cref="MapFlags.Discard" /> has exactly the same behavior.</para>
		/// </remarks>
		[RetValIndex( 6 )] MappedTextureSubresource MapTextureSubresource( ITexture pTexture, int MipLevel, int ArraySlice, MapType MapType, MapFlags MapFlags, [In] ref Box pMapRegion );

		/// <summary>Unmaps the texture subresource.</summary>
		void UnmapTextureSubresource( ITexture pTexture, int MipLevel, int ArraySlice );

		/// <summary>Generates a mipmap chain.</summary>
		/// <param name="pTextureView">Texture view to generate mip maps for.</param>
		/// <remarks>
		/// <para>This function can only be called for a shader resource view.</para>
		/// <para>The texture must be created with <see cref="MiscTextureFlags.GenerateMips" /> flag.</para>
		/// </remarks>
		void GenerateMips( ITextureView pTextureView );

		/// <summary>Finishes the current frame and releases dynamic resources allocated by the context.</summary>
		/// <remarks>
		/// <para>For immediate context, this method is called automatically by ISwapChain::Present() of the primary swap chain, but can also be called explicitly. For deferred contexts, the method must be called by the application
		/// to release dynamic resources. The method has some overhead, so it is better to call it once per frame, though it can be called with different frequency. Note that unless the GPU is idled, the resources may actually be
		/// released several frames after the one they were used in last time.</para>
		/// <para>After the call all dynamic resources become invalid and must be written again before the next use.</para>
		/// <para>Also, all committed resources become invalid. For deferred contexts, this method must be called after all command lists referencing dynamic resources have been executed through immediate context. The method does
		/// not Flush() the context.</para>
		/// </remarks>
		void FinishFrame();

		/// <summary>Transitions resource states.</summary>
		/// <param name="BarrierCount">Number of barriers in pResourceBarriers array</param>
		/// <param name="resourceBarriers">Pointer to the array of resource barriers</param>
		/// <remarks>
		/// <para>When both old and new states are <see cref="ResourceState.UnorderedAccess" /> the engine executes UAV barrier on the resource. The barrier makes sure that all UAV accesses (reads or writes) are complete before any
		/// future UAV accesses (read or write) can begin. There are two main usage scenarios for this method:</para>
		/// <para>
		/// <list type="number">
		/// <item>An application knows specifics of resource state transitions not available to the engine. For example, only single mip level needs to be transitioned.</item>
		/// <item>An application manages resource states in multiple threads in parallel.</item>
		/// </list></para>
		/// <para>The method always reads the states of all resources to transition. If the state of a resource is managed by multiple threads in parallel, the resource must first be transitioned to unknown state (
		/// <see cref="ResourceState.Unknown" /> ) to disable automatic state management in the engine.</para>
		/// <para>When StateTransitionDesc::UpdateResourceState is set to true, the method may update the state of the corresponding resource which is not thread safe. No other threads should read or write the sate of that resource.
		/// </para>
		/// <para>Any method that uses <see cref="ResourceStateTransitionMode.Transition" /> mode may alter the state of resources it works with. <see cref="ResourceStateTransitionMode.Verify" /> mode makes the method read the
		/// states, but not write them. When <see cref="ResourceStateTransitionMode.None" /> is used, the method assumes the states are guaranteed to be correct and does not read or write them.</para>
		/// <para>It is the responsibility of the application to make sure this is indeed true.</para>
		/// <para>Refer to http://diligentgraphics.com/2018/12/09/resource-state-management/ for detailed explanation of resource state management in Diligent Engine.</para>
		/// </remarks>
		void TransitionResourceStates( int BarrierCount, [In] ref StateTransitionDesc resourceBarriers );

		/// <summary>Resolves a multi-sampled texture subresource into a non-multi-sampled texture subresource.</summary>
		/// <param name="pSrcTexture">Source multi-sampled texture.</param>
		/// <param name="pDstTexture">Destination non-multi-sampled texture.</param>
		/// <param name="ResolveAttribs">Resolve command attributes, see <see cref="ResolveTextureSubresourceAttribs" /> for details.</param>
		void ResolveTextureSubresource( ITexture pSrcTexture, ITexture pDstTexture, [In] ref ResolveTextureSubresourceAttribs ResolveAttribs );
	}
}
