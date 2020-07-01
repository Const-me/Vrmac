using System.Collections.Generic;
using VrmacVideo.Containers.MP4;
using VrmacVideo.Linux;

namespace VrmacVideo.Containers
{
	static class MapColorSpace
	{
		static V? lookup<K, V>( this Dictionary<K, V> dict, K k ) where V : struct
		{
			if( dict.TryGetValue( k, out var v ) )
				return v;
			return null;
		}

		static readonly Dictionary<eTransferFunc, eTransferFunction> s_transferFunctions = new Dictionary<eTransferFunc, eTransferFunction>()
		{
			{ eTransferFunc.BT709, eTransferFunction.BT_709 },
			{ eTransferFunc.Unspecified, eTransferFunction.BT_709 },
			{ eTransferFunc.BT470M, eTransferFunction.BT_709 },
			{ eTransferFunc.BT470BG, eTransferFunction.BT_709 },
			{ eTransferFunc.SMPTE170, eTransferFunction.BT_709 },
			{ eTransferFunc.SMPTE240, eTransferFunction.SMPTE240M },
			{ eTransferFunc.Linear, eTransferFunction.None },
		};

		public static eTransferFunction? map( this eTransferFunc tf ) => s_transferFunctions.lookup( tf );

		static readonly Dictionary<eMatrixCoefficients, eYCbCrEncoding> s_encoding = new Dictionary<eMatrixCoefficients, eYCbCrEncoding>()
		{
			{ eMatrixCoefficients.BT709, eYCbCrEncoding.BT709 },
			{ eMatrixCoefficients.Unspecified, eYCbCrEncoding.BT709 },
			{ eMatrixCoefficients.FCC, eYCbCrEncoding.BT601 },	// The FCC says "KR = 0.30; KB = 0.11", 601 says "0.299, 0.114", let's hope it's close enough
			{ eMatrixCoefficients.BT470, eYCbCrEncoding.BT601 },
			{ eMatrixCoefficients.SMPTE170M, eYCbCrEncoding.BT601 },
			{ eMatrixCoefficients.SMPTE240M, eYCbCrEncoding.SMPTE240M },
		};

		public static eYCbCrEncoding? map( this eMatrixCoefficients mc ) => s_encoding.lookup( mc );

		static readonly Dictionary<eColorPrimaries, eColorSpace> s_colors = new Dictionary<eColorPrimaries, eColorSpace>()
		{
			{ eColorPrimaries.BT709, eColorSpace.BT709 },
			{ eColorPrimaries.Unspecified, eColorSpace.BT709 },
			{ eColorPrimaries.BT470M, eColorSpace.NTSC_470_SYSTEM_M },
			{ eColorPrimaries.BT470BG, eColorSpace.PAL_470_SYSTEM_BG },
			{ eColorPrimaries.SMPTE170, eColorSpace.SMPTE170M },
			{ eColorPrimaries.SMPTE240, eColorSpace.SMPTE240M },
			// eColorPrimaries.Film: Generic film, color filters using Illuminant C doesn't have a matching value in V4L2
		};

		public static eColorSpace? map( this eColorPrimaries cp ) => s_colors.lookup( cp );
	}
}