using Diligent.Graphics.Video;
using System;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	static class ColorFormatExt
	{
		public static ColorFormat colorFormat( this ref sPixelFormatMP pmp )
		{
			eRange range;
			switch( pmp.quantization )
			{
				default:
					throw new ArgumentException( $"Unexpected quantization value { pmp.quantization }" );
				case eQuantization.FullRange:
					range = eRange.Full;
					break;
				case eQuantization.LimitedRange:
					range = eRange.Narrow;
					break;
			}

			// TODO: support more of them
			eVideoColorSpace cs;
			switch( pmp.colorSpace )
			{
				case eColorSpace.BT709:
					cs = eVideoColorSpace.REC709;
					break;
				case eColorSpace.BT2020:
					cs = eVideoColorSpace.REC2020;
					break;
				case eColorSpace.SMPTE170M:
					cs = eVideoColorSpace.REC601;
					break;
				default:
					throw new NotSupportedException( $"Unsupported color space { pmp.colorSpace }" );
			}

			// TODO: verify the siting values are correct
			return new ColorFormat( cs, range, eChromaSiting.Zero, eChromaSiting.PointFive );
		}
	}
}