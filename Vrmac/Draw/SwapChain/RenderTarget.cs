using Diligent.Graphics;
using System;

namespace Vrmac.Draw.SwapChain
{
	/// <summary>A single render target of the <see cref="MsaaSwapChain" />.</summary>
	class RenderTarget: IDisposable
	{
		readonly ITexture texture;
		readonly ITextureView targetView;
		readonly CSize size;
		IShaderResourceBinding blenderBinding = null;

		public RenderTarget( Context context, CSize size, TextureFormat format, int sampleCount, string name )
		{
			this.size = size;

			TextureDesc desc = new TextureDesc( false );
			desc.Type = ResourceDimension.Tex2d;
			desc.Size = size;
			desc.Format = format;
			desc.SampleCount = (uint)sampleCount;
			desc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;

			using( var device = context.renderContext.device )
				texture = device.CreateTexture( ref desc, name );

			targetView = texture.GetDefaultView( TextureViewType.RenderTarget );
		}

		public RenderTarget( Context context, CSize size, TextureFormat format, int sampleCount, int index ) :
			this( context, size, format, sampleCount, $"2D MSAA target { index }" )
		{ }

		public void Dispose()
		{
			blenderBinding?.Dispose();
			targetView?.Dispose();
			texture?.Dispose();
		}

		public ITextureView bind( IDeviceContext context )
		{
			context.SetRenderTarget( targetView, null );
			return targetView;
		}

		void resolve( IDeviceContext ic, ITexture destination, ITexture source )
		{
			ResolveTextureSubresourceAttribs rtsra = new ResolveTextureSubresourceAttribs( false );
			rtsra.SrcTextureTransitionMode = ResourceStateTransitionMode.Transition;
			rtsra.DstTextureTransitionMode = ResourceStateTransitionMode.Transition;
			ic.ResolveTextureSubresource( source, destination, ref rtsra );
		}

		public void replace( Context context, ITextureView destRgb, ITextureView destDepth )
		{
			var ic = context.context;
			ic.SetRenderTarget( destRgb, destDepth );
			using( var rtt = destRgb.GetTexture() )
				resolve( ic, rtt, texture );
		}

		public void blend( Context context, ITextureView destRgb, ITextureView destDepth, Blender blender )
		{
			// BTW, the ability to read from multi-sampled textures appeared in GLES 3.1, just in time:
			// https://www.khronos.org/registry/OpenGL-Refpages/es3.1/html/texelFetch.xhtml

			if( null == blenderBinding )
			{
				using( var resourceView = texture.GetDefaultView( TextureViewType.ShaderResource ) )
					blenderBinding = blender.bindSource( resourceView );
			}

			var ic = context.context;
			ic.SetRenderTarget( destRgb, destDepth );
			blender.blend( ic, blenderBinding );
		}
	}
}