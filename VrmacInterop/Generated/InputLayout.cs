// This source file was automatically generated from "InputLayout.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>Input frequency</summary>
	public enum InputElementFrequency
	{
		/// <summary>Frequency is undefined.</summary>
		Undefined = 0,
		/// <summary>Input data is per-vertex data.</summary>
		PerVertex = 1,
		/// <summary>Input data is per-instance data.</summary>
		PerInstance = 2,
		/// <summary>Helper value that stores the total number of frequencies in the enumeration.</summary>
		NumFrequencies = 3
	}

	/// <summary>Description of a single element of the input layout</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct LayoutElement
	{
		/// <summary>
		/// <para>HLSL semantic. Default value (&quot;ATTRIB&quot;) allows HLSL shaders to be converted</para>
		/// <para>to GLSL and used in OpenGL backend as well as compiled to SPIRV and used</para>
		/// <para>in Vulkan backend.</para>
		/// <para>Any value other than default will only work in Direct3D11 and Direct3D12 backends.</para>
		/// </summary>
		public IntPtr HLSLSemantic;

		/// <summary>
		/// <para>Input index of the element that is specified in the vertex shader.</para>
		/// <para>In Direct3D11 and Direct3D12 backends this is the semantic index.</para>
		/// </summary>
		public int InputIndex;

		/// <summary>Buffer slot index that this element is read from.</summary>
		public int BufferSlot;

		/// <summary>Number of components in the element. Allowed values are 1, 2, 3, and 4.</summary>
		public int NumComponents;

		/// <summary>Type of the element components, see Diligent::VALUE_TYPE for details.</summary>
		public GpuValueType ValueType;

		byte m_IsNormalized;
		/// <summary>
		/// <para>For signed and unsigned integer value types</para>
		/// <para>(VT_INT8, VT_INT16, VT_INT32, VT_UINT8, VT_UINT16, VT_UINT32)</para>
		/// <para>indicates if the value should be normalized to [-1,+1] or</para>
		/// <para>[0, 1] range respectively. For floating point types</para>
		/// <para>(VT_FLOAT16 and VT_FLOAT32), this member is ignored.</para>
		/// </summary>
		public bool IsNormalized
		{
			get => ( 0 != m_IsNormalized );
			set => m_IsNormalized = MiscUtils.byteFromBool( value );
		}

		/// <summary>
		/// <para>Relative offset, in bytes, to the element bits.</para>
		/// <para>If this value is set to LAYOUT_ELEMENT_AUTO_OFFSET (default value), the offset will</para>
		/// <para>be computed automatically by placing the element right after the previous one.</para>
		/// </summary>
		public int RelativeOffset;

		/// <summary>
		/// <para>Stride, in bytes, between two elements, for this buffer slot.</para>
		/// <para>If this value is set to LAYOUT_ELEMENT_AUTO_STRIDE, the stride will be</para>
		/// <para>computed automatically assuming that all elements in the same buffer slot are</para>
		/// <para>packed one after another. If the buffer slot contains multiple layout elements,</para>
		/// <para>they all must specify the same stride or use LAYOUT_ELEMENT_AUTO_STRIDE value.</para>
		/// </summary>
		public int Stride;

		public InputElementFrequency Frequency;

		/// <summary>
		/// <para>The number of instances to draw using the same per-instance data before advancing</para>
		/// <para>in the buffer by one element.</para>
		/// </summary>
		public int InstanceDataStepRate;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public LayoutElement( bool unused )
		{
			HLSLSemantic = IntPtr.Zero;
			InputIndex = 0;
			BufferSlot = 0;
			NumComponents = 0;
			ValueType = GpuValueType.Float32;
			m_IsNormalized = 1;
			RelativeOffset = InputLayout.LayoutElementAutoOffset;
			Stride = InputLayout.LayoutElementAutoStride;
			Frequency = InputElementFrequency.PerVertex;
			InstanceDataStepRate = 1;
		}
	}

	/// <summary>This structure is used by IRenderDevice::CreatePipelineState().</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct InputLayoutDesc
	{
		/// <summary>Array of layout elements</summary>
		public IntPtr LayoutElements;

		/// <summary>Number of layout elements</summary>
		public int NumElements;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public InputLayoutDesc( bool unused )
		{
			LayoutElements = IntPtr.Zero;
			NumElements = 0;
		}
	}
}
