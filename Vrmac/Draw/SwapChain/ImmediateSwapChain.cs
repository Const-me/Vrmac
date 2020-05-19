using Diligent.Graphics;
using System;
using System.Numerics;

namespace Vrmac.Draw.SwapChain
{
	/// <summary>Dummy 2D swap chain that doesn’t do anything, renders directly to the 3D swap chain.</summary>
	/// <remarks>In the current version of Vrmac graphics, it’s the only one that is ever used.</remarks>
	sealed class ImmediateSwapChain: iSwapChain
	{
		readonly Context context;

		public ImmediateSwapChain( Context context )
		{
			this.context = context;
		}

		ITextureView iSwapChain.begin( ITextureView rtv, ITextureView dsv, Vector4 clearColor )
		{
			var ic = context.context;
			ic.SetRenderTarget( rtv, dsv );
			if( clearColor.isNotTransparent() )
				ic.ClearRenderTarget( rtv, clearColor );
			ic.ClearDepthStencil( dsv, ClearDepthStencilFlags.DepthFlag, 1, 0 );
			return rtv;
		}

		void iSwapChain.end( bool replaceContent ) { }

		void IDisposable.Dispose() { }

		SwapChainFormats iSwapChain.swapChainFormats => context.swapChainFormats;

		void iSwapChain.destroyTargets() { }
	}
}