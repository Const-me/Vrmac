using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>This structure describes the graphics pipeline state and is part of the PipelineStateDesc structure.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public unsafe struct GraphicsPipelineDesc
	{
		/// <summary>Vertex shader to be used with the pipeline</summary>
		public IntPtr pVS;

		/// <summary>Pixel shader to be used with the pipeline</summary>
		public IntPtr pPS;

		/// <summary>Domain shader to be used with the pipeline</summary>
		public IntPtr pDS;

		/// <summary>Hull shader to be used with the pipeline</summary>
		public IntPtr pHS;

		/// <summary>Geometry shader to be used with the pipeline</summary>
		public IntPtr pGS;

		/// <summary>Blend state description</summary>
		public BlendStateDesc BlendDesc;

		/// <summary>
		/// <para>32-bit sample mask that determines which samples get updated</para>
		/// <para>in all the active render targets. A sample mask is always applied;</para>
		/// <para>it is independent of whether multisampling is enabled, and does not</para>
		/// <para>depend on whether an application uses multisample render targets.</para>
		/// </summary>
		public uint SampleMask;

		/// <summary>Rasterizer state description</summary>
		public RasterizerStateDesc RasterizerDesc;

		/// <summary>Depth-stencil state description</summary>
		public DepthStencilStateDesc DepthStencilDesc;

		/// <summary>Input layout</summary>
		public InputLayoutDesc InputLayout;

		/// <summary>Primitive topology type</summary>
		public PrimitiveTopology PrimitiveTopology;

		/// <summary>Number of viewports used by this pipeline</summary>
		public byte NumViewports;

		/// <summary>Number of render targets in the RTVFormats member</summary>
		public byte NumRenderTargets;

		/// <summary>Render target formats</summary>
		fixed ushort RTVFormats[ 8 ];

		/// <summary>Depth-stencil format</summary>
		public TextureFormat DSVFormat;

		/// <summary>Multisampling parameters</summary>
		public SampleDesc SmplDesc;

		/// <summary>Node mask.</summary>
		public uint NodeMask;

		/// <summary>Create and set fields to their defaults</summary>
		public GraphicsPipelineDesc( bool unused )
		{
			pVS = IntPtr.Zero;
			pPS = IntPtr.Zero;
			pDS = IntPtr.Zero;
			pHS = IntPtr.Zero;
			pGS = IntPtr.Zero;
			BlendDesc = new BlendStateDesc( true );
			SampleMask = 0xFFFFFFFF;
			RasterizerDesc = new RasterizerStateDesc( true );
			DepthStencilDesc = new DepthStencilStateDesc( true );
			InputLayout = new InputLayoutDesc( true );
			PrimitiveTopology = PrimitiveTopology.TriangleList;
			NumViewports = 1;
			NumRenderTargets = 0;
			DSVFormat = TextureFormat.Unknown;
			SmplDesc = new SampleDesc( true );
			NodeMask = 0;
		}

		/// <summary>Set render target format of the specified render target</summary>
		public void setRTVFormat( int idxRenderTarget, TextureFormat format )
		{
			if( idxRenderTarget < 0 || idxRenderTarget >= 8 )
				throw new ArgumentOutOfRangeException();
			RTVFormats[ idxRenderTarget ] = (ushort)format;
		}
	}
}