using System;
using Vrmac;

namespace Diligent.Graphics
{
	/// <summary>Extension methods for IDeviceContext</summary>
	public static class DeviceContextExt
	{
		/// <summary>Clears a render target view</summary>
		public static void ClearRenderTarget( this IDeviceContext context, ITextureView textureView, Vector4 color )
		{
			context.ClearRenderTarget( textureView, ref color, ResourceStateTransitionMode.Transition );
		}

		/// <summary>Set a single viewport</summary>
		public static void setViewport( this IDeviceContext context, CSize size )
		{
			Viewport vp = new Viewport( false );
			vp.Width = size.cx;
			vp.Height = size.cy;
			context.SetViewports( 1, ref vp, size.cx, size.cy );
		}

		/// <summary>Set a single viewport</summary>
		public static void setViewport( this IDeviceContext context, CSize size, Viewport vp )
		{
			context.SetViewports( 1, ref vp, size.cx, size.cy );
		}

		/// <summary>Sets an array of viewports</summary>
		public static void setViewports( this IDeviceContext context, CSize size, Span<Viewport> viewports )
		{
			context.SetViewports( viewports.Length, ref viewports.GetPinnableReference(), size.cx, size.cy );
		}

		/// <summary>Bind single VB</summary>
		public static void setVertexBuffer( this IDeviceContext context, IBuffer buffer )
		{
			context.SetVertexBuffer( 0, buffer, 0 );
		}
	}
}