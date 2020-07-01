using Diligent.Graphics;
using System.IO;
using System.Numerics;
using System.Text;

namespace Vrmac.MediaEngine.Render
{
	/// <summary>Implements a performance optimization for Pi4 by skipping intermediate RGB texture, and directly rendering NV12 frames from V4L2 into the destination.</summary>
	/// <remarks>The original version took ~50ms to render, not even enough for 24 FPS.</remarks>
	sealed class LinuxRender: RenderBase
	{
		readonly iVideoTextureSource source;

		public LinuxRender( IRenderDevice device, CSize renderTargetSize, SwapChainFormats formats, Vector4 borderColor, iVideoTextureSource source ) :
			base( device, renderTargetSize, formats, borderColor, source.videoSize )
		{
			this.source = source;
		}

		static string readSource( iStorageFolder assets )
		{
			assets.openRead( "VideoPS.glsl", out var stm );
			using( var reader = new StreamReader( stm ) )
				return reader.ReadToEnd();
		}

		protected override IShader compilePixelShader( iShaderFactory compiler, iStorageFolder assets,
			string uvMin, string uvMax, string colorString )
		{
			StringBuilder sb = new StringBuilder( readSource( assets ) );
			sb.Replace( "$( UV_MIN )", uvMin );
			sb.Replace( "$( UV_MAX )", uvMax );
			sb.Replace( "$( BORDER_COLOR )", colorString );
			string glsl = sb.ToString();

			ShaderSourceInfo sourceInfo = new ShaderSourceInfo( ShaderType.Pixel, ShaderSourceLanguage.Glsl );
			sourceInfo.combinedTextureSamplers = true;
			return compiler.compileFromSource( glsl, sourceInfo, "RenderVideoPS" );
		}

		public override void render( IDeviceContext ic )
		{
			ITextureView nv12 = source.dequeueTexture();
			try
			{
				drawTriangle( ic, nv12 );
			}
			finally
			{
				source.releaseTexture();
			}
		}

		public override void renderLastFrame( IDeviceContext ic )
		{
			ITextureView nv12 = source.getLastFrameTexture();
			drawTriangle( ic, nv12 );
		}
	}
} 