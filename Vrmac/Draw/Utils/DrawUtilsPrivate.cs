using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	static class DrawUtilsPrivate
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static eBuildFilledMesh fillFlagsFromColor( eBrushType brushType, bool hasStroke )
		{
			eBuildFilledMesh vaaBit = hasStroke ? eBuildFilledMesh.None : eBuildFilledMesh.VAA;

			switch( brushType )
			{
				case eBrushType.Null:
					return eBuildFilledMesh.None;
				case eBrushType.Transparent:
					return eBuildFilledMesh.FilledMesh | eBuildFilledMesh.BrushHasTransparency | vaaBit;
				case eBrushType.Opaque:
					return eBuildFilledMesh.FilledMesh | vaaBit;
			}
			throw new ArgumentException();
		}

		public static uint bits( this float f )
		{
			int i = BitConverter.SingleToInt32Bits( f );
			return unchecked((uint)i);
		}

		public static SolidColorData data( this iBrush brush )
		{
			SolidColorBrush b = (SolidColorBrush)brush;
			return b.data;
		}
	}
}