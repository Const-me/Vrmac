using Diligent.Graphics;
using System.Threading.Tasks;

namespace Vrmac
{
	/// <summary>Implement this interface to render something.</summary>
	public interface iScene
	{
		/// <summary>Create GPU resources essential for rendering even very first frame</summary>
		void createResources( Context context, IRenderDevice device );

		/// <summary>Render a frame</summary>
		void render( Context context, ITextureView swapChainRgb, ITextureView swapChainDepthStencil );
	}

	/// <summary>Implement this interface on the same object which implements iScene to create moar GPU resources.</summary>
	public interface iSceneAsyncInit
	{
		/// <summary>Optional asynchronous initialization. The library will start calling iScene.render before this method is complete.</summary>
		/// <remarks>Animation timers only start running after this method completes successfully.</remarks>
		Task createResourcesAsync( Context context, IRenderDevice device );
	}
}