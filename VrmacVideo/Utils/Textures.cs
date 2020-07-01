using Diligent.Graphics;
using System;
using Vrmac;

namespace VrmacVideo
{
	/// <summary>Description for 1 frame of video texture</summary>
	/// <remarks>Linux implementation of the media engine maintains 2 queues to pass frames to/from the decoder.
	/// This structure describes a frame the media engine is willing to write frames to.</remarks>
	public struct VideoTextureDesc
	{
		public readonly TextureFormat lumaFormat, chromaFormat;
		public readonly CSize lumaSize, chromaSize;

		internal VideoTextureDesc( TextureFormat lumaFormat, TextureFormat chromaFormat, CSize lumaSize, CSize chromaSize )
		{
			this.lumaFormat = lumaFormat;
			this.chromaFormat = chromaFormat;
			this.lumaSize = lumaSize;
			this.chromaSize = chromaSize;
		}
	}

	/// <summary>Textures for a single frame of video, split into 2 planes</summary>
	struct VideoTextures
	{
		readonly ITexture m_luma, m_chroma;

		public VideoTextures( ITexture luma, ITexture chroma )
		{
			m_luma = luma;
			m_chroma = chroma;
			this.luma = luma.GetDefaultView( TextureViewType.ShaderResource );
			this.chroma = chroma.GetDefaultView( TextureViewType.ShaderResource );
		}

		public readonly ITextureView luma, chroma;
	}

	/// <summary>Single NV12 texture for the complete frame of video.</summary>
	/// <remarks>Requires some weird EGL extensions to import to GLES and consume in pixel shaders.
	/// Still, it works in practice, and after couple optimizations is fast enough on my Pi4.</remarks>
	public struct Nv12Texture
	{
		readonly ITexture texture;
		public readonly ITextureView view;

		public Nv12Texture( ITexture texture )
		{
			this.texture = texture;
			view = texture.GetDefaultView( TextureViewType.ShaderResource );
			if( null == view )
				throw new ApplicationException( "The NV12 texture doesn't have a shader resource view" );
		}
		public void finalize()
		{
			view?.Dispose();
			texture?.Dispose();
		}
	}
}