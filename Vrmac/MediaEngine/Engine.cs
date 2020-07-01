using Diligent.Graphics;
using System.Numerics;
using VrmacVideo;
using VrmacVideo.Containers.MP4;

namespace Vrmac.MediaEngine
{
	/// <summary>Implements missing pieces of LinuxEngine, i.e. GLES integration.</summary>
	sealed class Engine: LinuxEngine, iVideoTextureSource
	{
		readonly Context context;
		readonly TextureFormat destTextureFormat;

		public Engine( Context context, TextureFormat destTextureFormat ) :
			base( GraphicsUtils.simdUtils, context.displayRefresh )
		{
			this.context = context;
			this.destTextureFormat = destTextureFormat;
		}

		TextureFormat rgbFormat;
		ITexture frameTexture;
		ITextureView frameRtv;
		CSize outputSize;
		// SavePngFrames savePng;

		protected override ITextureView createOutputTexture( IRenderDevice device, TextureFormat format, CSize size )
		{
			outputSize = size;

			TextureDesc desc = new TextureDesc( false )
			{
				Type = ResourceDimension.Tex2d,
				BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
				Format = format,
				Size = size
			};

			frameTexture = device.CreateTexture( ref desc, $"Video frame RGB" );
			frameRtv = frameTexture.GetDefaultView( TextureViewType.RenderTarget );
			rgbFormat = format;
			// savePng = new SavePngFrames();

			return frameTexture.GetDefaultView( TextureViewType.ShaderResource );
		}

		protected override IRenderDevice getRenderDevice() => context.device;

		Nv12State state;

		protected override void initRendering( IRenderDevice device, Nv12Texture[] textures, ref sDecodedVideoSize videoSize )
		{
			state = new Nv12State( device, rgbFormat, textures, ref videoSize );
		}

		protected override void renderFrame( int bufferIndex )
		{
			var ic = context.context;
			// MicroProfiler.methodBegin();
			state.render( ic, frameRtv, outputSize, bufferIndex );
			// MicroProfiler.methodEnd();
			/* if( null != savePng )
			{
				var device = context.device;
				savePng.saveFrame( device, ic, frameTexture );
			} */
		}

		public override iVideoRenderState createRenderer( IRenderDevice device, CSize renderTargetSize, SwapChainFormats formats, Vector4 borderColor )
		{
			setNv12PresentMode();
			return new Render.LinuxRender( device, renderTargetSize, formats, borderColor, this );
		}

		// ==== iVideoTextureSource ====
		sDecodedVideoSize iVideoTextureSource.videoSize => decodedSize;
		ITextureView iVideoTextureSource.dequeueTexture() => dequeueTexture();
		ITextureView iVideoTextureSource.getLastFrameTexture() => getLastFrameTexture();
		void iVideoTextureSource.releaseTexture() => releaseTexture();
	}
}