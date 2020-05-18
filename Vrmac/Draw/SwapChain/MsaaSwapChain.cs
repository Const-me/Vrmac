using System;
using System.Diagnostics;

namespace Vrmac.Draw.SwapChain
{
	/// <summary>Implements 2D swap chain which renders 2D content into separate MSAA-enabled render targets.</summary>
	/// <remarks>Not used anymore because Pi4 is way too slow with MSAA. Implemented faster and higher-quality VAA instead.</remarks>
	sealed class MsaaSwapChain: SwapChainBase
	{
		readonly RenderTarget[] renderTargets;
		int? currendIndex = null;

		public MsaaSwapChain( Context context, byte sampleCount, int buffersCount ) :
			base( context, sampleCount )
		{
			renderTargets = new RenderTarget[ buffersCount ];
		}

		protected override RenderTarget begin()
		{
			if( currendIndex.HasValue )
				throw new ApplicationException();

			int index = context.getCurrentBackBufferIndex();
			RenderTarget rt = renderTargets[ index ];
			if( null == rt )
			{
				rt = new RenderTarget( context, size, context.swapChainFormats.color, sampleCount, index );
				renderTargets[ index ] = rt;
			}
			currendIndex = index;
			return rt;
		}

		protected override RenderTarget end()
		{
			if( !currendIndex.HasValue )
				throw new ApplicationException();
			int index = currendIndex.Value;
			currendIndex = null;
			RenderTarget rt = renderTargets[ index ];
			if( null == rt )
				throw new ApplicationException();
			return rt;
		}

		public override void destroyTargets()
		{
			for( int i = 0; i < renderTargets.Length; i++ )
			{
				if( null == renderTargets[ i ] )
					continue;
				renderTargets[ i ].Dispose();
				renderTargets[ i ] = null;
			}
		}

		protected override void resized()
		{
			foreach( var rt in renderTargets )
				Debug.Assert( rt == null );
		}
	}
}