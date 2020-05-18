using System;
using System.Diagnostics;

namespace Vrmac.Draw.SwapChain
{
	/// <summary>It's not a chain if it only has 1 element.</summary>
	/// <remarks>On Linux, contexts only have 1 back buffer. No need to waste time on that array.</remarks>
	sealed class SingleTarget: SwapChainBase
	{
		RenderTarget renderTarget;

		public SingleTarget( Context context, byte sampleCount ) : base( context, sampleCount )
		{
			Debug.Assert( context.swapChainBuffersCount <= 1 );
		}

		bool rendering = false;

		protected override RenderTarget begin()
		{
			if( rendering )
				throw new ApplicationException();
			if( null == renderTarget )
				renderTarget = new RenderTarget( context, size, context.swapChainFormats.color, sampleCount, 0 );
			rendering = true;
			return renderTarget;
		}

		protected override RenderTarget end()
		{
			if( !rendering )
				throw new ApplicationException();
			rendering = false;
			return renderTarget;
		}

		public override void destroyTargets()
		{
			ComUtils.clear( ref renderTarget );
		}
		protected override void resized()
		{
			Debug.Assert( renderTarget == null );
		}
	}
}