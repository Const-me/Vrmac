// This source file was automatically generated from "DepthStencilState.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>
	/// <para>[D3D11_STENCIL_OP]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476219(v=vs.85).aspx</para>
	/// <para>[D3D12_STENCIL_OP]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770409(v=vs.85).aspx</para>
	/// <para>This enumeration describes the stencil operation and generally mirrors</para>
	/// <para>[D3D11_STENCIL_OP][]/[D3D12_STENCIL_OP][] enumeration.</para>
	/// <para>It is used by Diligent::StencilOpDesc structure to describe the stencil fail, depth fail</para>
	/// <para>and stencil pass operations</para>
	/// </summary>
	public enum StencilOp : sbyte
	{
		/// <summary>Undefined operation.</summary>
		Undefined = 0,
		/// <summary>
		/// <para>Keep the existing stencil data.</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_KEEP/D3D12_STENCIL_OP_KEEP. OpenGL counterpart: GL_KEEP.</para>
		/// </summary>
		Keep = 1,
		/// <summary>
		/// <para>Set the stencil data to 0.</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_ZERO/D3D12_STENCIL_OP_ZERO. OpenGL counterpart: GL_ZERO.</para>
		/// </summary>
		Zero = 2,
		/// <summary>
		/// <para>Set the stencil data to the reference value set by calling IDeviceContext::SetStencilRef().</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_REPLACE/D3D12_STENCIL_OP_REPLACE. OpenGL counterpart: GL_REPLACE.</para>
		/// </summary>
		Replace = 3,
		/// <summary>
		/// <para>Increment the current stencil value, and clamp to the maximum representable unsigned value.</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_INCR_SAT/D3D12_STENCIL_OP_INCR_SAT. OpenGL counterpart: GL_INCR.</para>
		/// </summary>
		IncrSat = 4,
		/// <summary>
		/// <para>Decrement the current stencil value, and clamp to 0.</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_DECR_SAT/D3D12_STENCIL_OP_DECR_SAT. OpenGL counterpart: GL_DECR.</para>
		/// </summary>
		DecrSat = 5,
		/// <summary>
		/// <para>Bitwise invert the current stencil buffer value.</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_INVERT/D3D12_STENCIL_OP_INVERT. OpenGL counterpart: GL_INVERT.</para>
		/// </summary>
		Invert = 6,
		/// <summary>
		/// <para>Increment the current stencil value, and wrap the value to zero when incrementing</para>
		/// <para>the maximum representable unsigned value.</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_INCR/D3D12_STENCIL_OP_INCR. OpenGL counterpart: GL_INCR_WRAP.</para>
		/// </summary>
		IncrWrap = 7,
		/// <summary>
		/// <para>Decrement the current stencil value, and wrap the value to the maximum representable</para>
		/// <para>unsigned value when decrementing a value of zero.</para>
		/// <para>Direct3D counterpart: D3D11_STENCIL_OP_DECR/D3D12_STENCIL_OP_DECR. OpenGL counterpart: GL_DECR_WRAP.</para>
		/// </summary>
		DecrWrap = 8,
		/// <summary>Helper value that stores the total number of stencil operations in the enumeration.</summary>
		NumOps = 9
	}

	/// <summary>
	/// <para>[D3D11_DEPTH_STENCILOP_DESC]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476109(v=vs.85).aspx</para>
	/// <para>[D3D12_DEPTH_STENCILOP_DESC]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770355(v=vs.85).aspx</para>
	/// <para>The structure generally mirrors [D3D11_DEPTH_STENCILOP_DESC][]/[D3D12_DEPTH_STENCILOP_DESC][] structure.</para>
	/// <para>It is used by Diligent::DepthStencilStateDesc structure to describe the stencil</para>
	/// <para>operations for the front and back facing polygons.</para>
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct StencilOpDesc
	{
		/// <summary>
		/// <para>The stencil operation to perform when stencil testing fails.</para>
		/// <para>Default value: Diligent::STENCIL_OP_KEEP.</para>
		/// </summary>
		public StencilOp StencilFailOp;

		/// <summary>
		/// <para>The stencil operation to perform when stencil testing passes and depth testing fails.</para>
		/// <para>Default value: Diligent::STENCIL_OP_KEEP.</para>
		/// </summary>
		public StencilOp StencilDepthFailOp;

		/// <summary>
		/// <para>The stencil operation to perform when stencil testing and depth testing both pass.</para>
		/// <para>Default value: Diligent::STENCIL_OP_KEEP.</para>
		/// </summary>
		public StencilOp StencilPassOp;

		/// <summary>
		/// <para>A function that compares stencil data against existing stencil data.</para>
		/// <para>Default value: Diligent::COMPARISON_FUNC_ALWAYS. See Diligent::COMPARISON_FUNCTION.</para>
		/// </summary>
		public ComparisonFunction StencilFunc;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public StencilOpDesc( bool unused )
		{
			StencilFailOp = StencilOp.Keep;
			StencilDepthFailOp = StencilOp.Keep;
			StencilPassOp = StencilOp.Keep;
			StencilFunc = ComparisonFunction.Always;
		}
	}

	/// <summary>
	/// <para>[D3D11_DEPTH_STENCIL_DESC]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476110(v=vs.85).aspx</para>
	/// <para>[D3D12_DEPTH_STENCIL_DESC]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770356(v=vs.85).aspx</para>
	/// <para>This structure describes the depth stencil state and is part of the GraphicsPipelineDesc.</para>
	/// <para>The structure generally mirrors [D3D11_DEPTH_STENCIL_DESC][]/[D3D12_DEPTH_STENCIL_DESC][]</para>
	/// <para>structure.</para>
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct DepthStencilStateDesc
	{
		byte m_DepthEnable;
		/// <summary>
		/// <para>Enable depth-stencil operations. When it is set to False,</para>
		/// <para>depth test always passes, depth writes are disabled,</para>
		/// <para>and no stencil operations are performed. Default value: True.</para>
		/// </summary>
		public bool DepthEnable
		{
			get => ( 0 != m_DepthEnable );
			set => m_DepthEnable = MiscUtils.byteFromBool( value );
		}

		byte m_DepthWriteEnable;
		/// <summary>Enable or disable writes to a depth buffer. Default value: True.</summary>
		public bool DepthWriteEnable
		{
			get => ( 0 != m_DepthWriteEnable );
			set => m_DepthWriteEnable = MiscUtils.byteFromBool( value );
		}

		/// <summary>
		/// <para>A function that compares depth data against existing depth data.</para>
		/// <para>See Diligent::COMPARISON_FUNCTION for details.</para>
		/// <para>Default value: Diligent::COMPARISON_FUNC_LESS.</para>
		/// </summary>
		public ComparisonFunction DepthFunc;

		byte m_StencilEnable;
		/// <summary>Enable stencil opertaions. Default value: False.</summary>
		public bool StencilEnable
		{
			get => ( 0 != m_StencilEnable );
			set => m_StencilEnable = MiscUtils.byteFromBool( value );
		}

		/// <summary>
		/// <para>Identify which bits of the depth-stencil buffer are accessed when reading stencil data.</para>
		/// <para>Default value: 0xFF.</para>
		/// </summary>
		public byte StencilReadMask;

		/// <summary>
		/// <para>Identify which bits of the depth-stencil buffer are accessed when writing stencil data.</para>
		/// <para>Default value: 0xFF.</para>
		/// </summary>
		public byte StencilWriteMask;

		/// <summary>Identify stencil operations for the front-facing triangles, see Diligent::StencilOpDesc.</summary>
		public StencilOpDesc FrontFace;

		/// <summary>Identify stencil operations for the back-facing triangles, see Diligent::StencilOpDesc.</summary>
		public StencilOpDesc BackFace;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public DepthStencilStateDesc( bool unused )
		{
			m_DepthEnable = 1;
			m_DepthWriteEnable = 1;
			DepthFunc = ComparisonFunction.Less;
			m_StencilEnable = 0;
			StencilReadMask = 0xFF;
			StencilWriteMask = 0xFF;
			FrontFace = new StencilOpDesc( true );
			BackFace = new StencilOpDesc( true );
		}
	}
}
