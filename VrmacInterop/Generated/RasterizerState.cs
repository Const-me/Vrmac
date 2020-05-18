// This source file was automatically generated from "RasterizerState.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent.Graphics
{
	/// <summary>
	/// <para>[D3D11_FILL_MODE]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476131(v=vs.85).aspx</para>
	/// <para>[D3D12_FILL_MODE]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770366(v=vs.85).aspx</para>
	/// <para>This enumeration determines the fill mode to use when rendering triangles and mirrors the</para>
	/// <para>[D3D11_FILL_MODE][]/[D3D12_FILL_MODE][] enum. It is used by RasterizerStateDesc structure to define the fill mode.</para>
	/// </summary>
	public enum FillMode : sbyte
	{
		/// <summary>Undefined fill mode.</summary>
		Undefined = 0,
		/// <summary>
		/// <para>Rasterize triangles using wireframe fill.</para>
		/// <para>Direct3D counterpart: D3D11_FILL_WIREFRAME/D3D12_FILL_MODE_WIREFRAME. OpenGL counterpart: GL_LINE.</para>
		/// </summary>
		Wireframe = 1,
		/// <summary>
		/// <para>Rasterize triangles using solid fill.</para>
		/// <para>Direct3D counterpart: D3D11_FILL_SOLID/D3D12_FILL_MODE_SOLID. OpenGL counterpart: GL_FILL.</para>
		/// </summary>
		Solid = 2,
		/// <summary>Helper value that stores the total number of fill modes in the enumeration.</summary>
		NumModes = 3
	}

	/// <summary>
	/// <para>[D3D11_CULL_MODE]: https://msdn.microsoft.com/en-us/library/windows/desktop/ff476108(v=vs.85).aspx</para>
	/// <para>[D3D12_CULL_MODE]: https://msdn.microsoft.com/en-us/library/windows/desktop/dn770354(v=vs.85).aspx</para>
	/// <para>This enumeration defines which triangles are not drawn during the rasterization and mirrors</para>
	/// <para>[D3D11_CULL_MODE][]/[D3D12_CULL_MODE][] enum. It is used by RasterizerStateDesc structure to define the polygon cull mode.</para>
	/// </summary>
	public enum CullMode : sbyte
	{
		/// <summary>Undefined cull mode.</summary>
		Undefined = 0,
		/// <summary>
		/// <para>Draw all triangles.</para>
		/// <para>Direct3D counterpart: D3D11_CULL_NONE/D3D12_CULL_MODE_NONE. OpenGL counterpart: glDisable( GL_CULL_FACE ).</para>
		/// </summary>
		None = 1,
		/// <summary>
		/// <para>Do not draw triangles that are front-facing. Front- and back-facing triangles are determined</para>
		/// <para>by the RasterizerStateDesc::FrontCounterClockwise member.</para>
		/// <para>Direct3D counterpart: D3D11_CULL_FRONT/D3D12_CULL_MODE_FRONT. OpenGL counterpart: GL_FRONT.</para>
		/// </summary>
		Front = 2,
		/// <summary>
		/// <para>Do not draw triangles that are back-facing. Front- and back-facing triangles are determined</para>
		/// <para>by the RasterizerStateDesc::FrontCounterClockwise member.</para>
		/// <para>Direct3D counterpart: D3D11_CULL_BACK/D3D12_CULL_MODE_BACK. OpenGL counterpart: GL_BACK.</para>
		/// </summary>
		Back = 3,
		/// <summary>Helper value that stores the total number of cull modes in the enumeration.</summary>
		NumModes = 4
	}

	/// <summary>This structure describes the rasterizer state and is part of the GraphicsPipelineDesc.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct RasterizerStateDesc
	{
		/// <summary>
		/// <para>Determines traingle fill mode, see Diligent::FILL_MODE for details.</para>
		/// <para>Default value: Diligent::FILL_MODE_SOLID.</para>
		/// </summary>
		public FillMode FillMode;

		/// <summary>
		/// <para>Determines traingle cull mode, see Diligent::CULL_MODE for details.</para>
		/// <para>Default value: Diligent::CULL_MODE_BACK.</para>
		/// </summary>
		public CullMode CullMode;

		byte m_FrontCounterClockwise;
		/// <summary>
		/// <para>Determines if a triangle is front- or back-facing. If this parameter is True,</para>
		/// <para>a triangle will be considered front-facing if its vertices are counter-clockwise</para>
		/// <para>on the render target and considered back-facing if they are clockwise.</para>
		/// <para>If this parameter is False, the opposite is true.</para>
		/// <para>Default value: False.</para>
		/// </summary>
		public bool FrontCounterClockwise
		{
			get => ( 0 != m_FrontCounterClockwise );
			set => m_FrontCounterClockwise = MiscUtils.byteFromBool( value );
		}

		byte m_DepthClipEnable;
		/// <summary>
		/// <para>Enable clipping against near and far clip planes.</para>
		/// <para>Default value: True.</para>
		/// </summary>
		public bool DepthClipEnable
		{
			get => ( 0 != m_DepthClipEnable );
			set => m_DepthClipEnable = MiscUtils.byteFromBool( value );
		}

		byte m_ScissorEnable;
		/// <summary>
		/// <para>Enable scissor-rectangle culling. All pixels outside an active scissor rectangle are culled.</para>
		/// <para>Default value: False.</para>
		/// </summary>
		public bool ScissorEnable
		{
			get => ( 0 != m_ScissorEnable );
			set => m_ScissorEnable = MiscUtils.byteFromBool( value );
		}

		byte m_AntialiasedLineEnable;
		/// <summary>
		/// <para>Specifies whether to enable line antialiasing.</para>
		/// <para>Default value: False.</para>
		/// </summary>
		public bool AntialiasedLineEnable
		{
			get => ( 0 != m_AntialiasedLineEnable );
			set => m_AntialiasedLineEnable = MiscUtils.byteFromBool( value );
		}

		/// <summary>
		/// <para>Constant value added to the depth of a given pixel.</para>
		/// <para>Default value: 0.</para>
		/// </summary>
		public int DepthBias;

		/// <summary>Maximum depth bias of a pixel.</summary>
		/// <remarks>
		/// <para>Depth bias clamp is not available in OpenGL</para>
		/// <para>Default value: 0.</para>
		/// </remarks>
		public float DepthBiasClamp;

		/// <summary>
		/// <para>Scalar that scales the given pixel's slope before adding to the pixel's depth.</para>
		/// <para>Default value: 0.</para>
		/// </summary>
		public float SlopeScaledDepthBias;

		/// <summary>To initialize this structure with default value, use this constructor. The argument value is ignored.</summary>
		/// <remarks>This is only here because C# doesn’t support parameterless constructors for structures.</remarks>
		public RasterizerStateDesc( bool unused )
		{
			FillMode = FillMode.Solid;
			CullMode = CullMode.Back;
			m_FrontCounterClockwise = 0;
			m_DepthClipEnable = 1;
			m_ScissorEnable = 0;
			m_AntialiasedLineEnable = 0;
			DepthBias = 0;
			DepthBiasClamp = 0.0f;
			SlopeScaledDepthBias = 0.0f;
		}
	}
}
