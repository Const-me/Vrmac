using Diligent.Graphics;
using System;

namespace Vrmac.MediaEngine
{
	/// <summary>Utility object to render video frames</summary>
	public interface iVideoRenderState: IDisposable
	{
		/// <summary>Render the current frame of the video.</summary>
		/// <remarks>
		/// This fills the complete viewport.
		/// It's assumed the caller has already setup the render target, the only thing this method does — renders a full-screen triangle with the video + border.
		/// </remarks>
		void render( IDeviceContext ic );

		/// <summary>Repeat rendering of the last frame of the video.</summary>
		void renderLastFrame( IDeviceContext ic );

		/// <summary>Handle resize of the destination render target</summary>
		void resize( IRenderDevice device, CSize newSize );
	}
}