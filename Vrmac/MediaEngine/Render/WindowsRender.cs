using Diligent.Graphics;
using System.Collections.Generic;
using System.Numerics;
using VrmacVideo;

namespace Vrmac.MediaEngine.Render
{
	/// <summary>Windows doesn’t quite need this code. It doesn't eliminate extra RGB copy.
	/// The optimization is irrelevant on PCs anyway, my desktop GPU has VRAM bandwidth to copy FullHD across textures at 2.9 kHz.
	/// Only implementing this to have identical media engine API across Windows and Linux.</summary>
	class WindowsRender: RenderBase
	{
		readonly iMediaEngine mediaEngine;
		readonly ITextureView videoTexture;

		public WindowsRender( IRenderDevice device, CSize renderTargetSize, SwapChainFormats formats, Vector4 borderColor, iMediaEngine mediaEngine ) :
			base( device, renderTargetSize, formats, borderColor, new sDecodedVideoSize( mediaEngine.nativeVideoSize ) )
		{
			this.mediaEngine = mediaEngine;
			videoTexture = mediaEngine.createFrameTexture( device, formats.color );
		}

		public override void render( IDeviceContext ic )
		{
			mediaEngine.transferVideoFrame();
			drawTriangle( ic, videoTexture );
		}

		public override void renderLastFrame( IDeviceContext ic )
		{
			drawTriangle( ic, videoTexture );
		}

		static IEnumerable<(string, string)> makeMacros( string uvMin, string uvMax, string colorString )
		{
			yield return ("UV_MIN", uvMin);
			yield return ("UV_MAX", uvMax);
			yield return ("BORDER_COLOR", colorString);
		}

		protected override IShader compilePixelShader( iShaderFactory compiler, iStorageFolder assets, string uvMin, string uvMax, string colorString )
		{
			ShaderSourceInfo ssi = new ShaderSourceInfo( ShaderType.Pixel, ShaderSourceLanguage.Hlsl )
			{
				combinedTextureSamplers = true
			};
			return compiler.compileFromFile( assets, "VideoPS.hlsl", ssi, "RenderVideoPS", makeMacros( uvMin, uvMax, colorString ) );
		}
	}
}