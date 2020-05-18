namespace Vrmac.Draw
{
	/// <summary>Sprite position within the atlas</summary>
	public struct Sprite
	{
		/// <summary>Texture coordinates in pixels, all 4 of them in 1.15 fixed point format.</summary>
		/// <remarks>If the sprite was added with padding, the 1px padding doesn’t count.</remarks>
		public readonly ulong uv;

		/// <summary>Layer of the texture array</summary>
		public readonly int layer;

		internal Sprite( ulong rect, int layer )
		{
			uv = rect;
			this.layer = layer;
		}
	}
}