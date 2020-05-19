using Diligent.Graphics;
using System;
using System.Numerics;

namespace Vrmac.Draw.SwapChain
{
	abstract class SwapChainBase: iSwapChain
	{
		protected readonly Context context;
		protected readonly byte sampleCount;
		protected CSize size { get; private set; }
		Blender blender;

		ITextureView rtv, dsv;

		public SwapChainBase( Context context, byte sampleCount )
		{
			this.context = context;
			this.sampleCount = sampleCount;
			size = context.swapChainSize;
		}

		SwapChainFormats iSwapChain.swapChainFormats =>
			new SwapChainFormats( context.swapChainFormats.color, TextureFormat.Unknown, sampleCount );

		protected abstract RenderTarget begin();

		ITextureView iSwapChain.begin( ITextureView rtv, ITextureView dsv, Vector4 clearColor )
		{
			// ConsoleLogger.logDebug( "iSwapChain.begin 0" );
			if( context.swapChainSize != size )
			{
				size = context.swapChainSize;
				resized();
			}
			// ConsoleLogger.logDebug( "iSwapChain.begin 1" );
			RenderTarget rt = begin();
			// ConsoleLogger.logDebug( "iSwapChain.begin 2" );
			ITextureView target = rt.bind( context.context );
			context.context.ClearRenderTarget( target, clearColor );
			// ConsoleLogger.logDebug( "iSwapChain.begin 3" );
			this.rtv = rtv;
			this.dsv = dsv;
			// ConsoleLogger.logDebug( "iSwapChain.begin 4" );
			return target;
		}

		void IDisposable.Dispose()
		{
			destroyTargets();
			blender?.Dispose();
			blender = null;
		}

		protected abstract RenderTarget end();

		void iSwapChain.end( bool replaceContent )
		{
			RenderTarget rt = end();

			if( replaceContent )
			{
				rt.replace( context, rtv, dsv );
				// ConsoleLogger.logDebug( "SwapChainBase.end replaced render target content" );
			}
			else
			{
				if( null != blender && blender.samplesCount != 0 && blender.samplesCount != sampleCount )
					ComUtils.clear( ref blender );

				if( null == blender )
				{
					using( var dev = context.renderContext.device )
						blender = new Blender( context, dev, sampleCount );
				}
				rt.blend( context, rtv, dsv, blender );
			}
			rtv = dsv = null;
		}

		public abstract void destroyTargets();

		protected abstract void resized();
	}
}