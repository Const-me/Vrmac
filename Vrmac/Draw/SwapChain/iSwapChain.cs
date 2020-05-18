using Diligent.Graphics;
using System;

namespace Vrmac.Draw.SwapChain
{
	interface iSwapChain: IDisposable
	{
		/// <summary>Start rendering 2D content. Returns the render target which is now current.</summary>
		ITextureView begin( ITextureView rtv, ITextureView dsv, Vector4 clearColor );

		void end( bool replaceContent );

		SwapChainFormats swapChainFormats { get; }

		void destroyTargets();
	}
}