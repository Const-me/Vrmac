using Diligent.Graphics;
using System.Numerics;
using System.Threading.Tasks;

namespace Vrmac.MediaEngine
{
	/// <summary>Linux implementation of media engine interface supports this interface, in addition to <see cref="iMediaEngine" />.</summary>
	public interface iLinuxMediaEngine
	{
		/// <summary>Sets the URL of a media resource, and load the media</summary>
		Task loadMedia( string url );

		/// <summary>Create utility object to render video frames to the specified swap chain.</summary>
		/// <remarks>This method is a performance optimization.
		/// Apparently, Pi4 GPU doesn’t have the GPU bandwidth for 2 passes of video rendering, first from NV12 into RGB, another one from RGB into frame buffer.</remarks>
		iVideoRenderState createRenderer( IRenderDevice device, CSize renderTargetSize, SwapChainFormats formats, Vector4 borderColor );
	}
}