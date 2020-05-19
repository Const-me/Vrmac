// This source file was automatically generated from "PipelineState.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>This structure is used by GraphicsPipelineDesc to describe multisampling parameters</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct SampleDesc
	{
		/// <summary>Sample count</summary>
		public byte Count;

		/// <summary>Quality</summary>
		public byte Quality;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public SampleDesc( bool unused )
		{
			Count = 1;
			Quality = 0;
		}
	}

	/// <summary>Describes shader variable</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ShaderResourceVariableDesc
	{
		/// <summary>Shader stages this resources variable applies to. More than one shader stage can be specified.</summary>
		public ShaderType ShaderStages;

		/// <summary>Shader variable name</summary>
		public IntPtr Name;

		/// <summary>Shader variable type. See Diligent::SHADER_RESOURCE_VARIABLE_TYPE for a list of allowed types</summary>
		public ShaderResourceVariableType Type;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ShaderResourceVariableDesc( bool unused )
		{
			ShaderStages = ShaderType.Unknown;
			Name = IntPtr.Zero;
			Type = ShaderResourceVariableType.Static;
		}
	}

	/// <summary>Static sampler description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct StaticSamplerDesc
	{
		/// <summary>Shader stages that this static sampler applies to. More than one shader stage can be specified.</summary>
		public ShaderType ShaderStages;

		/// <summary>
		/// <para>The name of the sampler itself or the name of the texture variable that</para>
		/// <para>this static sampler is assigned to if combined texture samplers are used.</para>
		/// </summary>
		public IntPtr SamplerOrTextureName;

		/// <summary>Sampler description</summary>
		public SamplerDesc Desc;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public StaticSamplerDesc( bool unused )
		{
			ShaderStages = ShaderType.Unknown;
			SamplerOrTextureName = IntPtr.Zero;
			Desc = new SamplerDesc( true );
		}
	}

	/// <summary>Pipeline layout description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct PipelineResourceLayoutDesc
	{
		/// <summary>
		/// <para>Default shader resource variable type. This type will be used if shader</para>
		/// <para>variable description is not found in the Variables array</para>
		/// <para>or if Variables == nullptr</para>
		/// </summary>
		public ShaderResourceVariableType DefaultVariableType;

		/// <summary>Number of elements in Variables array</summary>
		public int NumVariables;

		/// <summary>Array of shader resource variable descriptions</summary>
		public IntPtr Variables;

		/// <summary>Number of static samplers in StaticSamplers array</summary>
		public int NumStaticSamplers;

		/// <summary>Array of static sampler descriptions</summary>
		public IntPtr StaticSamplers;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public PipelineResourceLayoutDesc( bool unused )
		{
			DefaultVariableType = ShaderResourceVariableType.Static;
			NumVariables = 0;
			Variables = IntPtr.Zero;
			NumStaticSamplers = 0;
			StaticSamplers = IntPtr.Zero;
		}
	}

	/// <summary>This structure describes the compute pipeline state and is part of the PipelineStateDesc structure.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ComputePipelineDesc
	{
		/// <summary>Compute shader to be used with the pipeline</summary>
		public IntPtr pCS;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public ComputePipelineDesc( bool unused )
		{
			pCS = IntPtr.Zero;
		}
	}

	/// <summary>Pipeline state description</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct PipelineStateDesc
	{
		/// <summary>Structures in C# can’t inherit from other structures. Using encapsulation instead.</summary>
		public DeviceObjectAttribs baseStruct;

		byte m_IsComputePipeline;
		/// <summary>Flag indicating if pipeline state is a compute pipeline state</summary>
		public bool IsComputePipeline
		{
			get => ( 0 != m_IsComputePipeline );
			set => m_IsComputePipeline = MiscUtils.byteFromBool( value );
		}

		/// <summary>
		/// <para>This member defines allocation granularity for internal resources required by the shader resource</para>
		/// <para>binding object instances.</para>
		/// </summary>
		public int SRBAllocationGranularity;

		/// <summary>Defines which command queues this pipeline state can be used with</summary>
		public ulong CommandQueueMask;

		/// <summary>Pipeline layout description</summary>
		public PipelineResourceLayoutDesc ResourceLayout;

		/// <summary>Graphics pipeline state description. This memeber is ignored if IsComputePipeline == True</summary>
		public GraphicsPipelineDesc GraphicsPipeline;

		/// <summary>Compute pipeline state description. This memeber is ignored if IsComputePipeline == False</summary>
		public ComputePipelineDesc ComputePipeline;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public PipelineStateDesc( bool unused )
		{
			baseStruct = new DeviceObjectAttribs( true );
			m_IsComputePipeline = 0;
			SRBAllocationGranularity = 1;
			CommandQueueMask = 1;
			ResourceLayout = new PipelineResourceLayoutDesc( true );
			GraphicsPipeline = new GraphicsPipelineDesc( true );
			ComputePipeline = new ComputePipelineDesc( true );
		}
	}

	/// <summary>Pipeline state interface</summary>
	[ComInterface( "06084ae5-6a71-4fe8-84b9-395dd489a28c", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface IPipelineState: IDeviceObject
	{
		/// <summary>Get interface ID of the top-level IDeviceObject-based interface implemented by the object.</summary>
		new void getIid( out Guid iid );
		/// <summary>Returns unique identifier assigned to an object</summary>
		new int GetUniqueID();
		/// <summary>Cast interface pointer from interop to native COM interface</summary>
		new IntPtr nativeCast();
		[RetValIndex] PipelineStateDesc GetDesc();


		/// <summary>Binds resources for all shaders in the pipeline state</summary>
		/// <param name="ShaderFlags">
		/// <para>Flags that specify shader stages, for which resources will be bound.</para>
		/// <para>Any combination of <see cref="ShaderType" /> may be used.</para>
		/// </param>
		/// <param name="pResourceMapping">Pointer to the resource mapping interface.</param>
		/// <param name="Flags">Additional flags. See <see cref="BindShaderResourcesFlags" /> .</param>
		void BindStaticResources( int ShaderFlags, IResourceMapping pResourceMapping, int Flags );

		/// <summary>Returns the number of static shader resource variables.</summary>
		/// <param name="ShaderType">Type of the shader.</param>
		/// <remarks>
		/// <para>Only static variables (that can be accessed directly through the PSO) are counted.</para>
		/// <para>Mutable and dynamic variables are accessed through Shader Resource Binding object.</para>
		/// </remarks>
		[RetValIndex( 1 )] int GetStaticVariableCount( ShaderType ShaderType );

		/// <summary>Returns static shader resource variable. If the variable is not found, throws an exception.</summary>
		[RetValIndex] IShaderResourceVariable GetStaticVariableByName( ShaderType shaderType, [In, MarshalAs( UnmanagedType.LPUTF8Str )] string name );

		/// <summary>Returns static shader resource variable by its index.</summary>
		[RetValIndex] IShaderResourceVariable GetStaticVariableByIndex( ShaderType shaderType, int index );

		/// <summary>Creates a shader resource binding object</summary>
		/// <param name="InitStaticResources">if set to true, the method will initialize static resources in the created object, which has the exact same effect as calling IShaderResourceBinding::InitializeStaticResources().</param>
		/// <returns>memory location where pointer to the new shader resource binding object is written.</returns>
		[RetValIndex] IShaderResourceBinding CreateShaderResourceBinding( [MarshalAs( UnmanagedType.U1 )] bool InitStaticResources );

		/// <summary>Checks if this pipeline state object is compatible with another PSO</summary>
		/// <param name="pPSO">Pointer to the pipeline state object to check compatibility with</param>
		/// <remarks>
		/// <para>If two pipeline state objects are compatible, they can use shader resource binding objects interchangebly, i.e. SRBs created by one PSO can be committed when another PSO is bound.</para>
		/// <para>The function only checks that shader resource layouts are compatible, but does not check if resource types match. For instance, if a pixel shader in one PSO uses a texture at slot 0, and a pixel shader in another
		/// PSO uses texture array at slot 0, the pipelines will be compatible. However, if you try to use SRB object from the first pipeline to commit resources for the second pipeline, a runtime error will occur. The function only
		/// checks compatibility of shader resource layouts. It does not take into account vertex shader input layout, number of outputs, etc.</para>
		/// </remarks>
		bool IsCompatibleWith( IPipelineState pPSO );
	}
}
