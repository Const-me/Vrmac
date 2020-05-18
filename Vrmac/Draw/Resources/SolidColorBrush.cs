using Diligent.Graphics;
using System;

namespace Vrmac.Draw
{
	sealed class SolidColorBrush: iBrush
	{
		public readonly SolidColorData data;

		public SolidColorBrush( PaletteTexture texture, ref Vector4 color )
		{
			data = texture.colorData( ref color );
		}
		public SolidColorBrush( ConsoleColor console )
		{
			data = new SolidColorData( (int)console, eBrushType.Opaque );
		}
		public SolidColorBrush(SolidColorData data)
		{
			this.data = data;
		}
		void IDisposable.Dispose() { }
	}
}