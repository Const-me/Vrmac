using ComLight;
using Diligent.Graphics;
using System;

namespace Vrmac
{
	/// <summary>GPU context for rendering. Might be a window, or a physical display.</summary>
	[ComInterface( "bb58cf9e-697d-490b-9da9-321334fb3923", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iRenderingContext: IDisposable
	{
		/// <summary>Get render device interface</summary>
		void getDevice( out IRenderDevice device );
		/// <summary>Render device</summary>
		IRenderDevice device { get; }

		/// <summary>Get the device context</summary>
		void getContext( out IDeviceContext context );
		/// <summary>Device context</summary>
		IDeviceContext context { get; }

		/// <summary>Get the swap chain</summary>
		void getSwapChain( out ISwapChain swapChain );
		/// <summary>Swap chain</summary>
		ISwapChain swapChain { get; }

		/// <summary>Render and present a frame.</summary>
		/// <remarks>If you have a window subsystem and use <see cref="iDispatcher" />, you don't ever need to call this, instead pass the context to <see cref="iDispatcher.run(iRenderingContext[], int)" /></remarks>
		bool renderFrame();

		/// <summary>Wait for next vertical blank event</summary>
		void waitForVBlank();
	}
}