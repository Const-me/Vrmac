using ComLight;
using System.Runtime.InteropServices;
using Vrmac;
using Vrmac.Utils;

namespace Diligent.Graphics
{
	/// <summary>Memory mapped plane buffer exported from "video for Linux 2" API.</summary>
	/// <remarks>Doing that DMA export to avoid copies, even with NV12 copies through userspace memory would take too much CPU time and memory bandwidth.</remarks>
	/// <seealso href="https://elinux.org/images/5/53/Zero-copy_video_streaming.pdf" />
	public struct sDmaBuffer
	{
		/// <summary>DMA buffer file descriptor</summary>
		public int fd;

		/// <summary>Offset from the start of the DMA buffer</summary>
		public int offset;

		/// <summary>Distance in bytes between lines of the image</summary>
		public int stride;

		/// <summary>Size in bytes of the complete image</summary>
		public int imageSize;

		/// <summary>Texture size in pixels</summary>
		public CSize sizePixels;
		/// <summary>Index of the buffer, for texture names and log messages</summary>
		public int bufferIndex;

		/// <summary>A string for debugging</summary>
		public override string ToString() => $"bufferIndex { bufferIndex }, sizePixels { sizePixels }, imageSize { imageSize }, stride { stride }, offset { offset }, fd { fd }";
	}

	/// <summary>Linux native library supports this extra COM interface of IRenderDevice object.</summary>
	/// <remarks>Initially created for video playback, to integrate with V4L2 decoder without making copies.</remarks>
	[ComInterface( "e709c72c-5475-4cca-bd0c-51725816d8fd" ), CustomConventions( typeof( NativeErrorMessages ) )]
	public interface iGlesRenderDevice
	{
		/// <summary>Import luma texture from DMA buffer into GLES, and create shader resource view.</summary>
		/// <remarks>Luma textures are single channel, the imported GLES texture will be of type GL_RED</remarks>
		[RetValIndex] ITexture importLumaTexture( [In] ref sDmaBuffer buffer );

		/// <summary>Import chroma texture from DMA buffer into GLES, and create shader resource view.</summary>
		/// <remarks>Chroma textures use 2 channels for Cb and Cr, the imported GL texture gonna be of type GL_RG, with red and green channels containing Cb and Cr, respectively.</remarks>
		[RetValIndex] ITexture importChromaTexture( [In] ref sDmaBuffer buffer );

		/// <summary>Import the complete NV12 texture</summary>
		[RetValIndex] ITexture importNv12Texture( [In] ref sDmaBuffer buffer, [In] ref Video.ColorFormat format );
	}
}