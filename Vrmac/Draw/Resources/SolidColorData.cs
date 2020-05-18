using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	enum eBrushType: byte
	{
		Null,
		Opaque,
		Transparent,
	};

	struct SolidColorData
	{
		public readonly int paletteIndex;
		public readonly eBrushType brushType;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public SolidColorData( int paletteIndex, eBrushType brushType )
		{
			this.paletteIndex = paletteIndex;
			this.brushType = brushType;
		}
	};
}