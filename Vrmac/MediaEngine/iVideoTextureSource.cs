using Diligent.Graphics;
using VrmacVideo;

namespace Vrmac.MediaEngine
{
	/// <summary>Interface for Linux NV12 video renderer to interact with the owner, the media engine</summary>
	interface iVideoTextureSource
	{
		/// <summary>Query video size and crop rectangle</summary>
		sDecodedVideoSize videoSize { get; }

		/// <summary>Dequeue decoded buffer, return GLES texture view of that</summary>
		ITextureView dequeueTexture();
		ITextureView getLastFrameTexture();

		/// <summary>You must call this exactly once, for each dequeueTexture call</summary>
		void releaseTexture();
	}
}