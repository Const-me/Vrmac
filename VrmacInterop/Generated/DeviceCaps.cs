// This source file was automatically generated from "DeviceCaps.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>Device type</summary>
	public enum RenderDeviceType
	{
		/// <summary>Undefined device</summary>
		Undefined = 0,
		/// <summary>D3D11 device</summary>
		D3D11 = 1,
		/// <summary>D3D12 device</summary>
		D3D12 = 2,
		/// <summary>OpenGL device</summary>
		GL = 3,
		/// <summary>OpenGLES device</summary>
		GLES = 4,
		/// <summary>Vulkan device</summary>
		Vulkan = 5,
		/// <summary>Metal device (not yet implemented)</summary>
		Metal = 6
	}

	/// <summary>Texture sampler capabilities</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct SamplerCaps
	{
		byte m_BorderSamplingModeSupported;
		/// <summary>Indicates if device supports border texture addressing mode</summary>
		public bool BorderSamplingModeSupported
		{
			get => ( 0 != m_BorderSamplingModeSupported );
			set => m_BorderSamplingModeSupported = MiscUtils.byteFromBool( value );
		}

		byte m_AnisotropicFilteringSupported;
		/// <summary>Indicates if device supports anisotrpoic filtering</summary>
		public bool AnisotropicFilteringSupported
		{
			get => ( 0 != m_AnisotropicFilteringSupported );
			set => m_AnisotropicFilteringSupported = MiscUtils.byteFromBool( value );
		}

		byte m_LODBiasSupported;
		/// <summary>Indicates if device supports MIP load bias</summary>
		public bool LODBiasSupported
		{
			get => ( 0 != m_LODBiasSupported );
			set => m_LODBiasSupported = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public SamplerCaps( bool unused )
		{
			m_BorderSamplingModeSupported = 0;
			m_AnisotropicFilteringSupported = 0;
			m_LODBiasSupported = 0;
		}
	}

	/// <summary>Texture capabilities</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct TextureCaps
	{
		/// <summary>Maximum dimension (width) of a 1D texture, or 0 if 1D textures are not supported.</summary>
		public int MaxTexture1DDimension;

		/// <summary>Maximum number of slices in a 1D texture array, or 0 if 1D texture arrays are not supported.</summary>
		public int MaxTexture1DArraySlices;

		/// <summary>Maximum dimension (width or height) of a 2D texture.</summary>
		public int MaxTexture2DDimension;

		/// <summary>Maximum number of slices in a 2D texture array, or 0 if 2D texture arrays are not supported.</summary>
		public int MaxTexture2DArraySlices;

		/// <summary>Maximum dimension (width, height, or depth) of a 3D texture, or 0 if 3D textures are not supported.</summary>
		public int MaxTexture3DDimension;

		/// <summary>Maximum dimension (width or height) of a cubemap face, or 0 if cubemap textures are not supported.</summary>
		public int MaxTextureCubeDimension;

		byte m_Texture2DMSSupported;
		/// <summary>Indicates if device supports 2D multisampled textures</summary>
		public bool Texture2DMSSupported
		{
			get => ( 0 != m_Texture2DMSSupported );
			set => m_Texture2DMSSupported = MiscUtils.byteFromBool( value );
		}

		byte m_Texture2DMSArraySupported;
		/// <summary>Indicates if device supports 2D multisampled texture arrays</summary>
		public bool Texture2DMSArraySupported
		{
			get => ( 0 != m_Texture2DMSArraySupported );
			set => m_Texture2DMSArraySupported = MiscUtils.byteFromBool( value );
		}

		byte m_TextureViewSupported;
		/// <summary>Indicates if device supports texture views</summary>
		public bool TextureViewSupported
		{
			get => ( 0 != m_TextureViewSupported );
			set => m_TextureViewSupported = MiscUtils.byteFromBool( value );
		}

		byte m_CubemapArraysSupported;
		/// <summary>Indicates if device supports cubemap arrays</summary>
		public bool CubemapArraysSupported
		{
			get => ( 0 != m_CubemapArraysSupported );
			set => m_CubemapArraysSupported = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public TextureCaps( bool unused )
		{
			MaxTexture1DDimension = 0;
			MaxTexture1DArraySlices = 0;
			MaxTexture2DDimension = 0;
			MaxTexture2DArraySlices = 0;
			MaxTexture3DDimension = 0;
			MaxTextureCubeDimension = 0;
			m_Texture2DMSSupported = 0;
			m_Texture2DMSArraySupported = 0;
			m_TextureViewSupported = 0;
			m_CubemapArraysSupported = 0;
		}
	}

	/// <summary>Describes supported device features</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DeviceFeatures
	{
		byte m_SeparablePrograms;
		/// <summary>Indicates if device supports separable programs</summary>
		public bool SeparablePrograms
		{
			get => ( 0 != m_SeparablePrograms );
			set => m_SeparablePrograms = MiscUtils.byteFromBool( value );
		}

		byte m_IndirectRendering;
		/// <summary>Indicates if device supports indirect draw commands</summary>
		public bool IndirectRendering
		{
			get => ( 0 != m_IndirectRendering );
			set => m_IndirectRendering = MiscUtils.byteFromBool( value );
		}

		byte m_WireframeFill;
		/// <summary>Indicates if device supports wireframe fill mode</summary>
		public bool WireframeFill
		{
			get => ( 0 != m_WireframeFill );
			set => m_WireframeFill = MiscUtils.byteFromBool( value );
		}

		byte m_MultithreadedResourceCreation;
		/// <summary>Indicates if device supports multithreaded resource creation</summary>
		public bool MultithreadedResourceCreation
		{
			get => ( 0 != m_MultithreadedResourceCreation );
			set => m_MultithreadedResourceCreation = MiscUtils.byteFromBool( value );
		}

		byte m_ComputeShaders;
		/// <summary>Indicates if device supports compute shaders</summary>
		public bool ComputeShaders
		{
			get => ( 0 != m_ComputeShaders );
			set => m_ComputeShaders = MiscUtils.byteFromBool( value );
		}

		byte m_GeometryShaders;
		/// <summary>Indicates if device supports geometry shaders</summary>
		public bool GeometryShaders
		{
			get => ( 0 != m_GeometryShaders );
			set => m_GeometryShaders = MiscUtils.byteFromBool( value );
		}

		byte m_Tessellation;
		/// <summary>Indicates if device supports tessellation</summary>
		public bool Tessellation
		{
			get => ( 0 != m_Tessellation );
			set => m_Tessellation = MiscUtils.byteFromBool( value );
		}

		byte m_BindlessResources;
		/// <summary>Indicates if device supports bindless resources</summary>
		public bool BindlessResources
		{
			get => ( 0 != m_BindlessResources );
			set => m_BindlessResources = MiscUtils.byteFromBool( value );
		}

		byte m_OcclusionQueries;
		/// <summary>Indicates if device supports occlusion queries (see Diligent::QUERY_TYPE_OCCLUSION).</summary>
		public bool OcclusionQueries
		{
			get => ( 0 != m_OcclusionQueries );
			set => m_OcclusionQueries = MiscUtils.byteFromBool( value );
		}

		byte m_BinaryOcclusionQueries;
		/// <summary>Indicates if device supports binary occlusion queries (see Diligent::QUERY_TYPE_BINARY_OCCLUSION).</summary>
		public bool BinaryOcclusionQueries
		{
			get => ( 0 != m_BinaryOcclusionQueries );
			set => m_BinaryOcclusionQueries = MiscUtils.byteFromBool( value );
		}

		byte m_TimestampQueries;
		/// <summary>Indicates if device supports timestamp queries (see Diligent::QUERY_TYPE_TIMESTAMP).</summary>
		public bool TimestampQueries
		{
			get => ( 0 != m_TimestampQueries );
			set => m_TimestampQueries = MiscUtils.byteFromBool( value );
		}

		byte m_PipelineStatisticsQueries;
		/// <summary>Indicates if device supports pipeline statistics queries (see Diligent::QUERY_TYPE_PIPELINE_STATISTICS).</summary>
		public bool PipelineStatisticsQueries
		{
			get => ( 0 != m_PipelineStatisticsQueries );
			set => m_PipelineStatisticsQueries = MiscUtils.byteFromBool( value );
		}

		byte m_DepthBiasClamp;
		/// <summary>Indicates if device supports depth bias clamping</summary>
		public bool DepthBiasClamp
		{
			get => ( 0 != m_DepthBiasClamp );
			set => m_DepthBiasClamp = MiscUtils.byteFromBool( value );
		}

		byte m_DepthClamp;
		/// <summary>Indicates if device supports depth clamping</summary>
		public bool DepthClamp
		{
			get => ( 0 != m_DepthClamp );
			set => m_DepthClamp = MiscUtils.byteFromBool( value );
		}

		byte m_IndependentBlend;
		/// <summary>Indicates if device supports depth clamping</summary>
		public bool IndependentBlend
		{
			get => ( 0 != m_IndependentBlend );
			set => m_IndependentBlend = MiscUtils.byteFromBool( value );
		}

		byte m_DualSourceBlend;
		/// <summary>Indicates if device supports dual-source blend</summary>
		public bool DualSourceBlend
		{
			get => ( 0 != m_DualSourceBlend );
			set => m_DualSourceBlend = MiscUtils.byteFromBool( value );
		}

		byte m_MultiViewport;
		/// <summary>Indicates if device supports multiviewport</summary>
		public bool MultiViewport
		{
			get => ( 0 != m_MultiViewport );
			set => m_MultiViewport = MiscUtils.byteFromBool( value );
		}

		byte m_TextureCompressionBC;
		/// <summary>Indicates if device supports all BC-compressed formats</summary>
		public bool TextureCompressionBC
		{
			get => ( 0 != m_TextureCompressionBC );
			set => m_TextureCompressionBC = MiscUtils.byteFromBool( value );
		}

		byte m_VertexPipelineUAVWritesAndAtomics;
		/// <summary>
		/// <para>Indicates if device supports writes to UAVs as well as atomic operations in vertex,</para>
		/// <para>tessellation, and geometry shader stages.</para>
		/// </summary>
		public bool VertexPipelineUAVWritesAndAtomics
		{
			get => ( 0 != m_VertexPipelineUAVWritesAndAtomics );
			set => m_VertexPipelineUAVWritesAndAtomics = MiscUtils.byteFromBool( value );
		}

		byte m_PixelUAVWritesAndAtomics;
		/// <summary>
		/// <para>Indicates if device supports writes to UAVs as well as atomic operations in pixel</para>
		/// <para>shader stage.</para>
		/// </summary>
		public bool PixelUAVWritesAndAtomics
		{
			get => ( 0 != m_PixelUAVWritesAndAtomics );
			set => m_PixelUAVWritesAndAtomics = MiscUtils.byteFromBool( value );
		}

		byte m_TextureUAVExtendedFormats;
		/// <summary>Specifies whether all the extended UAV texture formats are available in shader code.</summary>
		public bool TextureUAVExtendedFormats
		{
			get => ( 0 != m_TextureUAVExtendedFormats );
			set => m_TextureUAVExtendedFormats = MiscUtils.byteFromBool( value );
		}

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DeviceFeatures( bool unused )
		{
			m_SeparablePrograms = 0;
			m_IndirectRendering = 0;
			m_WireframeFill = 0;
			m_MultithreadedResourceCreation = 0;
			m_ComputeShaders = 0;
			m_GeometryShaders = 0;
			m_Tessellation = 0;
			m_BindlessResources = 0;
			m_OcclusionQueries = 0;
			m_BinaryOcclusionQueries = 0;
			m_TimestampQueries = 0;
			m_PipelineStatisticsQueries = 0;
			m_DepthBiasClamp = 0;
			m_DepthClamp = 0;
			m_IndependentBlend = 0;
			m_DualSourceBlend = 0;
			m_MultiViewport = 0;
			m_TextureCompressionBC = 0;
			m_VertexPipelineUAVWritesAndAtomics = 0;
			m_PixelUAVWritesAndAtomics = 0;
			m_TextureUAVExtendedFormats = 0;
		}
	}

	/// <summary>Device capabilities</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DeviceCaps
	{
		/// <summary>Device type. See Diligent::DeviceType.</summary>
		public RenderDeviceType DevType;

		/// <summary>
		/// <para>Major revision of the graphics API supported by the graphics adapter.</para>
		/// <para>Note that this value indicates the maximum supported feature level, so,</para>
		/// <para>for example, if the device type is D3D11, this value will be 10 when</para>
		/// <para>the maximum supported Direct3D feature level of the graphics adapter is 10.0.</para>
		/// </summary>
		public int MajorVersion;

		/// <summary>
		/// <para>Minor revision of the graphics API supported by the graphics adapter.</para>
		/// <para>Similar to MajorVersion, this value indicates the maximum supported feature level.</para>
		/// </summary>
		public int MinorVersion;

		/// <summary>Adapter type. See Diligent::ADAPTER_TYPE.</summary>
		public AdapterType AdaterType;

		/// <summary>Texture sampling capabilities. See Diligent::SamplerCaps.</summary>
		public SamplerCaps SamCaps;

		/// <summary>Texture capabilities. See Diligent::TextureCaps.</summary>
		public TextureCaps TexCaps;

		/// <summary>Device features. See Diligent::DeviceFeatures.</summary>
		public DeviceFeatures Features;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DeviceCaps( bool unused )
		{
			DevType = RenderDeviceType.Undefined;
			MajorVersion = 0;
			MinorVersion = 0;
			AdaterType = AdapterType.Unknown;
			SamCaps = new SamplerCaps( true );
			TexCaps = new TextureCaps( true );
			Features = new DeviceFeatures( true );
		}
	}
}
