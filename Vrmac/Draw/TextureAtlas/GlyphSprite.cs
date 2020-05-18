namespace Vrmac.Draw
{
	/// <summary>Sprite size and position within the atlas</summary>
	public struct GlyphSprite
	{
		/// <summary>Sprite size in pixels</summary>
		/// <remarks>If the sprite was added with padding, the 1px padding doesn’t count.</remarks>
		public readonly CSize size;

		/// <summary>Texture coordinates in pixels, all 4 of them in 1.15 fixed point format.</summary>
		/// <remarks>If the sprite was added with padding, the 1px padding doesn’t count.</remarks>
		public readonly ulong uv;

		/// <summary>Layer of the texture array</summary>
		public readonly int layer;

		internal GlyphSprite( ref CSize size, ulong rect, int layer )
		{
			this.size = size;
			uv = rect;
			this.layer = layer;
		}
	}
}