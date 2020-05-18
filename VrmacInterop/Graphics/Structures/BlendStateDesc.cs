using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Diligent.Graphics
{
	/// <summary>This structure describes the blend state and is part of the GraphicsPipelineDesc.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public unsafe struct BlendStateDesc
	{
		byte m_AlphaToCoverageEnable;
		/// <summary>Specifies whether to use alpha-to-coverage as a multisampling technique when setting a pixel to a render target. Default value: False.</summary>
		public bool AlphaToCoverageEnable
		{
			get => ( 0 != m_AlphaToCoverageEnable );
			set => m_AlphaToCoverageEnable = MiscUtils.byteFromBool( value );
		}

		byte m_IndependentBlendEnable;
		/// <summary>Specifies whether to enable independent blending in simultaneous render targets.</summary>
		/// <remarks>If set to False, only RenderTargets[0] is used. Default value: False.</remarks>
		public bool IndependentBlendEnable
		{
			get => ( 0 != m_IndependentBlendEnable );
			set => m_IndependentBlendEnable = MiscUtils.byteFromBool( value );
		}

		/// <summary>An array of RenderTargetBlendDesc structures that describe the blend states for render targets</summary>
		fixed byte RenderTargets[ 80 ];    // RenderTargetBlendDesc[ 8 ]

		/// <summary>Create and set fields to their defaults</summary>
		public BlendStateDesc( bool unused )
		{
			m_AlphaToCoverageEnable = 0;
			m_IndependentBlendEnable = 0;

			// Set RenderTargetBlendDesc to default values. By default, C# initializes structures with all zeros, and when RenderTargetBlendDesc is all 0 it doesn't write anything at all because one of the fields is the write bitmask.
			RenderTargetBlendDesc bd = new RenderTargetBlendDesc( true );
			Debug.Assert( Marshal.SizeOf<RenderTargetBlendDesc>() == 10 );
			unsafe
			{
				fixed ( BlendStateDesc* pThis = &this )
				{
					RenderTargetBlendDesc* pDest = (RenderTargetBlendDesc*)( pThis->RenderTargets );
					for( int i = 0; i < 8; i++ )
						pDest[ i ] = bd;
				}
			}
		}

		/// <summary>Set blending state of the specified render target</summary>
		public void setRenderTarget( int idx, RenderTargetBlendDesc blendDesc )
		{
			Debug.Assert( idx >= 0 && idx < 8 );
			unsafe
			{
				fixed ( BlendStateDesc* pThis = &this )
					( (RenderTargetBlendDesc*)( pThis->RenderTargets ) )[ idx ] = blendDesc;
			}
		}

		/// <summary>Set blending state of the first render target</summary>
		public void setRenderTarget( RenderTargetBlendDesc blendDesc )
		{
			setRenderTarget( 0, blendDesc );
		}
	}
}