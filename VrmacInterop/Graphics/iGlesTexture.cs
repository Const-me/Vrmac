#pragma warning disable CS1591	// Missing XML comment
using ComLight;
using Vrmac.Utils;

namespace Diligent.Graphics
{
	/// <summary>The consortium neglected to document that extension. I can only guess what the fields mean.</summary>
	public struct sDmaExportedImage
	{
		public readonly uint fourcc;
		public readonly int num_planes;
		public readonly ulong modifiers;
		public readonly int dmaFileDescriptor;
		public readonly int stride;
		public readonly int offset;
	};

	/// <summary>Linux-specific interface implemented by ITexture objects, 2D only.</summary>
	[ComInterface( "feadc5f5-596b-43ed-9b19-7fb4228936bd" ), CustomConventions( typeof( NativeErrorMessages ) )]
	public interface iGlesTexture
	{
		/// <summary>Export the texture out of the GLES for use in other graphics APIs.</summary>
		/// <remarks>
		/// <para>The functionality is similar to DXGI surface sharing introduced in Vista in 2007, and universally supported on Windows since then.</para>
		/// <para>On Linux the API is much less usable, and the support is limited, that EGL extension was only written in 2015.</para>
		/// </remarks>
		/// <seealso href="https://www.khronos.org/registry/EGL/extensions/MESA/EGL_MESA_image_dma_buf_export.txt" />
		void exportDmaBuffer( out sDmaExportedImage image );
	}
}