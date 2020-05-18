using Diligent.Graphics;
using System;

namespace Vrmac.Utils
{
	/// <summary>Mouse cursor texture in VRAM</summary>
	public abstract class CursorTexture
	{
		/// <summary>The texture</summary>
		public readonly ITextureView texture;
		/// <summary>Size of the texture in pixels</summary>
		public readonly CSize size;
		/// <summary>Hotspot position within the texture</summary>
		public readonly CPoint hotspot;

		internal CursorTexture( ITextureView texture, CSize size, CPoint hotspot )
		{
			this.texture = texture;
			this.size = size;
			this.hotspot = hotspot;
		}
	}

	/// <summary>Alpha-blended RGBA cursor texture</summary>
	public sealed class StaticCursorTexture: CursorTexture
	{
		internal StaticCursorTexture( ITextureView texture, CSize size, CPoint hotspot ) :
			base( texture, size, hotspot )
		{ }
	}

	/// <summary>Monochrome cursor texture with 4 colors: black, white, transparent, and inverted</summary>
	/// <remarks>Uploaded to VRAM as R8_UNORM texture, the 4 values are 0 for black, 0x55 for white, 0xAA for transparent, and 0xFF for inverted.</remarks>
	public sealed class MonochromeCursorTexture: CursorTexture
	{
		internal MonochromeCursorTexture( ITextureView texture, CSize size, CPoint hotspot ) :
			base( texture, size, hotspot )
		{ }
	}

	/// <summary>Texture array with alpha-blended RGBA animated cursor</summary>
	public sealed class AnimatedCursorTexture: CursorTexture
	{
		/// <summary>Count of animation frames</summary>
		public readonly int animationFrames;
		/// <summary>Frame duration</summary>
		public readonly TimeSpan frameDuration;

		internal AnimatedCursorTexture( ITextureView texture, CSize size, CPoint hotspot, int frames, TimeSpan duration ):
			base( texture, size, hotspot )
		{
			animationFrames = frames;
			frameDuration = duration;
		}
	}
}