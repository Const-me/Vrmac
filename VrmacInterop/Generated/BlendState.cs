// This source file was automatically generated from "BlendState.h" C++ header by a custom tool.
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
	/// <para>[D3D11_BLEND]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476086(v=vs.85).aspx</para>
	/// <para>[D3D12_BLEND]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770338(v=vs.85).aspx</para>
	/// <para>[glBlendFuncSeparate]: https://www.opengl.org/wiki/GLAPI/glBlendFuncSeparate</para>
	/// <para>This enumeration defines blend factors for alpha-blending.</para>
	/// <para>It generatlly mirrors [D3D11_BLEND][] and [D3D12_BLEND][] enumerations and is used by RenderTargetBlendDesc structure</para>
	/// <para>to define source and destination blend factors for color and alpha channels.</para>
	/// </summary>
	/// <remarks>[D3D11_BLEND on MSDN][D3D11_BLEND], [D3D12_BLEND on MSDN][D3D12_BLEND], [glBlendFuncSeparate on OpenGL.org][glBlendFuncSeparate]</remarks>
	public enum BlendFactor : sbyte
	{
		/// <summary>Undefined blend factor</summary>
		Undefined = 0,
		/// <summary>
		/// <para>The blend factor is zero.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_ZERO/D3D12_BLEND_ZERO. OpenGL counterpart: GL_ZERO.</para>
		/// </summary>
		Zero = 1,
		/// <summary>
		/// <para>The blend factor is one.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_ONE/D3D12_BLEND_ONE. OpenGL counterpart: GL_ONE.</para>
		/// </summary>
		One = 2,
		/// <summary>
		/// <para>The blend factor is RGB data from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_SRC_COLOR/D3D12_BLEND_SRC_COLOR. OpenGL counterpart: GL_SRC_COLOR.</para>
		/// </summary>
		SrcColor = 3,
		/// <summary>
		/// <para>The blend factor is 1-RGB, where RGB is the data from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_INV_SRC_COLOR/D3D12_BLEND_INV_SRC_COLOR. OpenGL counterpart: GL_ONE_MINUS_SRC_COLOR.</para>
		/// </summary>
		InvSrcColor = 4,
		/// <summary>
		/// <para>The blend factor is alpha (A) data from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_SRC_ALPHA/D3D12_BLEND_SRC_ALPHA. OpenGL counterpart: GL_SRC_ALPHA.</para>
		/// </summary>
		SrcAlpha = 5,
		/// <summary>
		/// <para>The blend factor is 1-A, where A is alpha data from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_INV_SRC_ALPHA/D3D12_BLEND_INV_SRC_ALPHA. OpenGL counterpart: GL_ONE_MINUS_SRC_ALPHA.</para>
		/// </summary>
		InvSrcAlpha = 6,
		/// <summary>
		/// <para>The blend factor is alpha (A) data from a render target.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_DEST_ALPHA/D3D12_BLEND_DEST_ALPHA. OpenGL counterpart: GL_DST_ALPHA.</para>
		/// </summary>
		DestAlpha = 7,
		/// <summary>
		/// <para>The blend factor is 1-A, where A is alpha data from a render target.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_INV_DEST_ALPHA/D3D12_BLEND_INV_DEST_ALPHA. OpenGL counterpart: GL_ONE_MINUS_DST_ALPHA.</para>
		/// </summary>
		InvDestAlpha = 8,
		/// <summary>
		/// <para>The blend factor is RGB data from a render target.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_DEST_COLOR/D3D12_BLEND_DEST_COLOR. OpenGL counterpart: GL_DST_COLOR.</para>
		/// </summary>
		DestColor = 9,
		/// <summary>
		/// <para>The blend factor is 1-RGB, where RGB is the data from a render target.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_INV_DEST_COLOR/D3D12_BLEND_INV_DEST_COLOR. OpenGL counterpart: GL_ONE_MINUS_DST_COLOR.</para>
		/// </summary>
		InvDestColor = 10,
		/// <summary>
		/// <para>The blend factor is (f,f,f,1), where f = min(As, 1-Ad),</para>
		/// <para>As is alpha data from a pixel shader, and Ad is alpha data from a render target.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_SRC_ALPHA_SAT/D3D12_BLEND_SRC_ALPHA_SAT. OpenGL counterpart: GL_SRC_ALPHA_SATURATE.</para>
		/// </summary>
		SrcAlphaSat = 11,
		/// <summary>
		/// <para>The blend factor is the constant blend factor set with IDeviceContext::SetBlendFactors().</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_BLEND_FACTOR/D3D12_BLEND_BLEND_FACTOR. OpenGL counterpart: GL_CONSTANT_COLOR.</para>
		/// </summary>
		BlendFactor = 12,
		/// <summary>
		/// <para>The blend factor is one minus constant blend factor set with IDeviceContext::SetBlendFactors().</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_INV_BLEND_FACTOR/D3D12_BLEND_INV_BLEND_FACTOR. OpenGL counterpart: GL_ONE_MINUS_CONSTANT_COLOR.</para>
		/// </summary>
		InvBlendFactor = 13,
		/// <summary>
		/// <para>The blend factor is the second RGB data output from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_SRC1_COLOR/D3D12_BLEND_SRC1_COLOR. OpenGL counterpart: GL_SRC1_COLOR.</para>
		/// </summary>
		Src1Color = 14,
		/// <summary>
		/// <para>The blend factor is 1-RGB, where RGB is the second RGB data output from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_INV_SRC1_COLOR/D3D12_BLEND_INV_SRC1_COLOR. OpenGL counterpart: GL_ONE_MINUS_SRC1_COLOR.</para>
		/// </summary>
		InvSrc1Color = 15,
		/// <summary>
		/// <para>The blend factor is the second alpha (A) data output from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_SRC1_ALPHA/D3D12_BLEND_SRC1_ALPHA. OpenGL counterpart: GL_SRC1_ALPHA.</para>
		/// </summary>
		Src1Alpha = 16,
		/// <summary>
		/// <para>The blend factor is 1-A, where A is the second alpha data output from a pixel shader.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_INV_SRC1_ALPHA/D3D12_BLEND_INV_SRC1_ALPHA. OpenGL counterpart: GL_ONE_MINUS_SRC1_ALPHA.</para>
		/// </summary>
		InvSrc1Alpha = 17,
		/// <summary>Helper value that stores the total number of blend factors in the enumeration.</summary>
		NumFactors = 18
	}

	/// <summary>
	/// <para>[D3D11_BLEND_OP]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476088(v=vs.85).aspx</para>
	/// <para>[D3D12_BLEND_OP]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770340(v=vs.85).aspx</para>
	/// <para>[glBlendEquationSeparate]: https://www.opengl.org/wiki/GLAPI/glBlendEquationSeparate</para>
	/// <para>This enumeration describes blending operation for RGB or Alpha channels and generally mirrors</para>
	/// <para>[D3D11_BLEND_OP][] and [D3D12_BLEND_OP][] enums. It is used by RenderTargetBlendDesc structure to define RGB and Alpha</para>
	/// <para>blending operations</para>
	/// </summary>
	/// <remarks>[D3D11_BLEND_OP on MSDN][D3D11_BLEND_OP], [D3D12_BLEND_OP on MSDN][D3D12_BLEND_OP], [glBlendEquationSeparate on OpenGL.org][glBlendEquationSeparate]</remarks>
	public enum BlendOperation : sbyte
	{
		/// <summary>Undefined blend operation</summary>
		Undefined = 0,
		/// <summary>
		/// <para>Add source and destination color components.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_OP_ADD/D3D12_BLEND_OP_ADD. OpenGL counterpart: GL_FUNC_ADD.</para>
		/// </summary>
		Add = 1,
		/// <summary>
		/// <para>Subtract destination color components from source color components.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_OP_SUBTRACT/D3D12_BLEND_OP_SUBTRACT. OpenGL counterpart: GL_FUNC_SUBTRACT.</para>
		/// </summary>
		Subtract = 2,
		/// <summary>
		/// <para>Subtract source color components from destination color components.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_OP_REV_SUBTRACT/D3D12_BLEND_OP_REV_SUBTRACT. OpenGL counterpart: GL_FUNC_REVERSE_SUBTRACT.</para>
		/// </summary>
		RevSubtract = 3,
		/// <summary>
		/// <para>Compute the minimum of source and destination color components.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_OP_MIN/D3D12_BLEND_OP_MIN. OpenGL counterpart: GL_MIN.</para>
		/// </summary>
		Min = 4,
		/// <summary>
		/// <para>Compute the maximum of source and destination color components.</para>
		/// <para>Direct3D counterpart: D3D11_BLEND_OP_MAX/D3D12_BLEND_OP_MAX. OpenGL counterpart: GL_MAX.</para>
		/// </summary>
		Max = 5,
		/// <summary>Helper value that stores the total number of blend operations in the enumeration.</summary>
		NumOperations = 6
	}

	/// <summary>
	/// <para>These flags are used by RenderTargetBlendDesc structure to define</para>
	/// <para>writable components of the render target</para>
	/// </summary>
	public enum ColorMask : sbyte
	{
		/// <summary>Allow data to be stored in the red component.</summary>
		Red = 1,
		/// <summary>Allow data to be stored in the green component.</summary>
		Green = 2,
		/// <summary>Allow data to be stored in the blue component.</summary>
		Blue = 4,
		/// <summary>Allow data to be stored in the alpha component.</summary>
		Alpha = 8,
		/// <summary>Allow data to be stored in all components.</summary>
		All = 15
	}

	/// <summary>
	/// <para>[D3D12_LOGIC_OP]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770379(v=vs.85).aspx</para>
	/// <para>This enumeration describes logic operation and generally mirrors [D3D12_LOGIC_OP][] enum.</para>
	/// <para>It is used by RenderTargetBlendDesc structure to define logic operation.</para>
	/// <para>Only available on D3D12 engine</para>
	/// </summary>
	/// <remarks>[D3D12_LOGIC_OP on MSDN][D3D12_LOGIC_OP]</remarks>
	public enum LogicOperation : sbyte
	{
		/// <summary>
		/// <para>Clear the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_CLEAR.</para>
		/// </summary>
		Clear = 0,
		/// <summary>
		/// <para>Set the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_SET.</para>
		/// </summary>
		Set = 1,
		/// <summary>
		/// <para>Copy the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_COPY.</para>
		/// </summary>
		Copy = 2,
		/// <summary>
		/// <para>Perform an inverted-copy of the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_COPY_INVERTED.</para>
		/// </summary>
		CopyInverted = 3,
		/// <summary>
		/// <para>No operation is performed on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_NOOP.</para>
		/// </summary>
		Noop = 4,
		/// <summary>
		/// <para>Invert the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_INVERT.</para>
		/// </summary>
		Invert = 5,
		/// <summary>
		/// <para>Perform a logical AND operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_AND.</para>
		/// </summary>
		And = 6,
		/// <summary>
		/// <para>Perform a logical NAND operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_NAND.</para>
		/// </summary>
		Nand = 7,
		/// <summary>
		/// <para>Perform a logical OR operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_OR.</para>
		/// </summary>
		Or = 8,
		/// <summary>
		/// <para>Perform a logical NOR operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_NOR.</para>
		/// </summary>
		Nor = 9,
		/// <summary>
		/// <para>Perform a logical XOR operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_XOR.</para>
		/// </summary>
		Xor = 10,
		/// <summary>
		/// <para>Perform a logical equal operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_EQUIV.</para>
		/// </summary>
		Equiv = 11,
		/// <summary>
		/// <para>Perform a logical AND and reverse operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_AND_REVERSE.</para>
		/// </summary>
		AndReverse = 12,
		/// <summary>
		/// <para>Perform a logical AND and invert operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_AND_INVERTED.</para>
		/// </summary>
		AndInverted = 13,
		/// <summary>
		/// <para>Perform a logical OR and reverse operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_OR_REVERSE.</para>
		/// </summary>
		OrReverse = 14,
		/// <summary>
		/// <para>Perform a logical OR and invert operation on the render target.</para>
		/// <para>Direct3D12 counterpart: D3D12_LOGIC_OP_OR_INVERTED.</para>
		/// </summary>
		OrInverted = 15,
		/// <summary>Helper value that stores the total number of logical operations in the enumeration.</summary>
		NumOperations = 16
	}

	/// <summary>
	/// <para>This structure is used by BlendStateDesc to describe</para>
	/// <para>blend states for render targets</para>
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct RenderTargetBlendDesc
	{
		byte m_BlendEnable;
		/// <summary>Enable or disable blending for this render target. Default value: False.</summary>
		public bool BlendEnable
		{
			get => ( 0 != m_BlendEnable );
			set => m_BlendEnable = MiscUtils.byteFromBool( value );
		}

		byte m_LogicOperationEnable;
		/// <summary>Enable or disable a logical operation for this render target. Default value: False.</summary>
		public bool LogicOperationEnable
		{
			get => ( 0 != m_LogicOperationEnable );
			set => m_LogicOperationEnable = MiscUtils.byteFromBool( value );
		}

		/// <summary>
		/// <para>Specifies the blend factor to apply to the RGB value output from the pixel shader</para>
		/// <para>Default value: Diligent::BLEND_FACTOR_ONE.</para>
		/// </summary>
		public BlendFactor SrcBlend;

		/// <summary>
		/// <para>Specifies the blend factor to apply to the RGB value in the render target</para>
		/// <para>Default value: Diligent::BLEND_FACTOR_ZERO.</para>
		/// </summary>
		public BlendFactor DestBlend;

		/// <summary>
		/// <para>Defines how to combine the source and destination RGB values</para>
		/// <para>after applying the SrcBlend and DestBlend factors.</para>
		/// <para>Default value: Diligent::BLEND_OPERATION_ADD.</para>
		/// </summary>
		public BlendOperation BlendOp;

		/// <summary>
		/// <para>Specifies the blend factor to apply to the alpha value output from the pixel shader.</para>
		/// <para>Blend factors that end in _COLOR are not allowed.</para>
		/// <para>Default value: Diligent::BLEND_FACTOR_ONE.</para>
		/// </summary>
		public BlendFactor SrcBlendAlpha;

		/// <summary>
		/// <para>Specifies the blend factor to apply to the alpha value in the render target.</para>
		/// <para>Blend factors that end in _COLOR are not allowed.</para>
		/// <para>Default value: Diligent::BLEND_FACTOR_ZERO.</para>
		/// </summary>
		public BlendFactor DestBlendAlpha;

		/// <summary>
		/// <para>Defines how to combine the source and destination alpha values</para>
		/// <para>after applying the SrcBlendAlpha and DestBlendAlpha factors.</para>
		/// <para>Default value: Diligent::BLEND_OPERATION_ADD.</para>
		/// </summary>
		public BlendOperation BlendOpAlpha;

		/// <summary>
		/// <para>Defines logical operation for the render target.</para>
		/// <para>Default value: Diligent::LOGIC_OP_NOOP.</para>
		/// </summary>
		public LogicOperation LogicOp;

		/// <summary>
		/// <para>Render target write mask.</para>
		/// <para>Default value: Diligent::COLOR_MASK_ALL.</para>
		/// </summary>
		public ColorMask RenderTargetWriteMask;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public RenderTargetBlendDesc( bool unused )
		{
			m_BlendEnable = 0;
			m_LogicOperationEnable = 0;
			SrcBlend = BlendFactor.One;
			DestBlend = BlendFactor.Zero;
			BlendOp = BlendOperation.Add;
			SrcBlendAlpha = BlendFactor.One;
			DestBlendAlpha = BlendFactor.Zero;
			BlendOpAlpha = BlendOperation.Add;
			LogicOp = LogicOperation.Noop;
			RenderTargetWriteMask = ColorMask.All;
		}
	}
}
