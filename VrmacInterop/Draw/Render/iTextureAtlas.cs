using ComLight;
using Diligent.Graphics;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>Pixel format of the texture atlas</summary>
	public enum eTextureAtlasFormat: byte
	{
		/// <summary>4 bytes / pixel, alpha is replaced with 0xFF while adding sprites</summary>
		RGB8 = 0,
		/// <summary>4 bytes / pixel with pre-multiplied alpha</summary>
		RGBA8 = 1,
		/// <summary>1 byte / pixel, can be used for grayscale images, opacity masks, signed distance fields, etc.</summary>
		R8 = 2,
	}

	/// <summary>Flags for <see cref="iTextureAtlas.addImage(IntPtr, CSize, int, eCopyImageFlags)" /> method</summary>
	[Flags]
	public enum eCopyImageFlags: byte
	{
		/// <summary>None of the below</summary>
		None = 0,
		/// <summary>Add 1px padding around the sprite</summary>
		AddPadding = 1,
		/// <summary>Flip BGR or BGRA source data into RGB or RGBA</summary>
		FlipBgr = 2,
		/// <summary>The source data has straight alpha, convert into premultiplied alpha in the atlas.</summary>
		PremultiplyAlpha = 4,
	}

	/// <summary>Flags for <see cref="iTextureAtlas.addGlyph(IntPtr, CSize, int, eCopyGlyphFlags)" /> method</summary>
	public enum eCopyGlyphFlags: byte
	{
		/// <summary>If the destination atlas is RGB, broadcast source bytes into all 4 channels of the atlas</summary>
		Grayscale = 1,
		/// <summary>Gather horizontal ClearType subpixels into RGB channels, compute alpha = the maximum of the 3.</summary>
		ClearTypeHorizontal = 2,
		/// <summary>Gather vertical ClearType subpixels into RGB channels, compute alpha = the maximum of the 3.</summary>
		ClearTypeVertical = 3,
	}

	/// <summary>Manages a texture atlas in GPU memory.</summary>
	/// <remarks>
	/// <para>The GPU object being managed is Texture2DArray.</para>
	/// <para>The only method accessing the GPU is <see cref="update(IRenderDevice, IDeviceContext)" />, the rest of them may be called from any thread.
	/// Method like getSprite which don’t modify the atlas may be called concurrently from multiple threads in parallel.</para>
	/// <para>The native methods which add sprites contain sizable amount of SIMD code. On PC, the app gonna crash with a runtime error if the CPU doesn’t support SSSE3 or SSE 4.1.</para>
	/// </remarks>
	[ComInterface( "{be90b5b5-3519-483a-b21c-099d6afa127a}", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iTextureAtlas: IDisposable
	{
		/// <summary>Pixel format of the atlas</summary>
		void getPixelFormat( out eTextureAtlasFormat format );
		/// <summary>Pixel format of the atlas</summary>
		eTextureAtlasFormat pixelFormat { get; }

		/// <summary>Size of the texture atlas</summary>
		void getSize( out CSize size );
		/// <summary>Size of the texture atlas</summary>
		CSize size { get; }
		/// <summary>Count of layers in the atlas</summary>
		void getLayersCount( out int count );
		/// <summary>Count of layers in the atlas</summary>
		int layersCount { get; }
		/// <summary>Size of 1 texel in fixed point 1.15</summary>
		void getPixelSizeInUvUnits( out uint result );
		/// <summary>Size of 1 texel in fixed point 1.15</summary>
		uint pixelSizeInUvUnits { get; }

		/// <summary>Add sprite from uncompressed data in system memory. Returns new sprite index on success, HRESULT code if failed.</summary>
		int addImage( IntPtr pointer, CSize size, int sourceStride, eCopyImageFlags flags = eCopyImageFlags.None );
		/// <summary>Add a glyph from system memory. Returns new sprite index on success, HRESULT code if failed.</summary>
		/// <remarks>Very similar to addImage, but supports ClearType-specific image transforms.</remarks>
		int addGlyph( IntPtr pointer, CSize size, int sourceStride, eCopyGlyphFlags flags );
		/// <summary>Add sprite by decoding compressed image from the stream. Returns new sprite index on success, HRESULT code if failed.</summary>
		int loadImage( [ReadStream] Stream source, eImageFileFormat format, [MarshalAs( UnmanagedType.U1 )] bool addPadding = true );

		/// <summary>Get sprite rectangle and layer</summary>
		void getSprite( int spriteIndex, out ulong uv, out int layer );
		/// <summary>Same as above, also returns sprite size in pixels</summary>
		void getSprite( int spriteIndex, out ulong uv, out CSize sizePixels, out int layer );

		/// <summary>Texture view for the shaders</summary>
		void getNativeTexture( out ITextureView textureView );
		/// <summary>Texture view for the shaders</summary>
		ITextureView nativeTexture { get; }

		/// <summary>True if addImage, addGlyph or loadImage methods were called since the last update() call</summary>
		bool needsUpdate();
		/// <summary>Apply all pending updates to the texture. Returns true if the native texture was re-created due to expansion of the atlas, or false if the old one is still good.</summary>
		bool update( IRenderDevice device, IDeviceContext context );
	}
}