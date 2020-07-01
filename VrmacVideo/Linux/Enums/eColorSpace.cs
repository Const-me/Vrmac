using Diligent.Graphics.Video;
using System;

namespace VrmacVideo.Linux
{
	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
	public sealed class DefaultTransferFunctionAttribute: Attribute
	{
		public readonly eTransferFunction transferFunction;

		public DefaultTransferFunctionAttribute( eTransferFunction func )
		{
			transferFunction = func;
		}
	}

	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
	public sealed class DefaultEncodingAttribute: Attribute
	{
		public readonly eYCbCrEncoding encoding;

		public DefaultEncodingAttribute( eYCbCrEncoding enc )
		{
			encoding = enc;
		}
	}

	public enum eColorSpace: uint
	{
		/// <summary>Default colorspace, i.e. let the driver figure it out. Can only be used with video capture.</summary>
		Default = 0,

		/// <summary>SMPTE 170M: used for broadcast NTSC/PAL SDTV</summary>
		[DefaultTransferFunction( eTransferFunction.BT_709 ), DefaultEncoding( eYCbCrEncoding.BT601 )]
		SMPTE170M = 1,

		/// <summary>Obsolete pre-1998 SMPTE 240M HDTV standard, superseded by Rec 709</summary>
		[DefaultTransferFunction( eTransferFunction.SMPTE240M ), DefaultEncoding( eYCbCrEncoding.SMPTE240M )]
		SMPTE240M = 2,

		/// <summary>Rec.709: used for HDTV</summary>
		[DefaultTransferFunction( eTransferFunction.BT_709 ), DefaultEncoding( eYCbCrEncoding.BT709 )]
		BT709 = 3,

		/// <summary>NTSC 1953 colorspace. This only makes sense when dealing with really, really old NTSC recordings. Superseded by SMPTE 170M.</summary>
		[DefaultTransferFunction( eTransferFunction.BT_709 ), DefaultEncoding( eYCbCrEncoding.BT601 )]
		NTSC_470_SYSTEM_M = 5,

		/// <summary>EBU Tech 3213 PAL/SECAM colorspace. This only makes sense when dealing with really old PAL/SECAM recordings. Superseded by SMPTE 170M.</summary>
		[DefaultTransferFunction( eTransferFunction.BT_709 ), DefaultEncoding( eYCbCrEncoding.BT601 )]
		PAL_470_SYSTEM_BG = 6,

		/// <summary>Effectively shorthand for V4L2_COLORSPACE_SRGB, V4L2_YCBCR_ENC_601 * and V4L2_QUANTIZATION_FULL_RANGE. To be used for (Motion-)JPEG.</summary>
		[DefaultTransferFunction( eTransferFunction.SRGB ), DefaultEncoding( eYCbCrEncoding.BT601 )]
		JPEG = 7,

		/// <summary>For RGB colorspaces such as produces by most webcams.</summary>
		[DefaultTransferFunction( eTransferFunction.SRGB ), DefaultEncoding( eYCbCrEncoding.BT601 )]
		SRGB = 8,

		/// <summary>AdobeRGB colorspace</summary>
		[DefaultTransferFunction( eTransferFunction.AdobeRGB ), DefaultEncoding( eYCbCrEncoding.BT601 )]
		AdobeRGB = 9,

		/// <summary>BT.2020 colorspace, used for UHDTV.</summary>
		[DefaultTransferFunction( eTransferFunction.BT_709 ), DefaultEncoding( eYCbCrEncoding.BT2020 )]
		BT2020 = 10,

		/// <summary>Raw colorspace: for RAW unprocessed images</summary>
		[DefaultTransferFunction( eTransferFunction.None )]
		RAW = 11,

		/// <summary>DCI-P3 colorspace, used by cinema projectors</summary>
		[DefaultTransferFunction( eTransferFunction.DCI_P3 ), DefaultEncoding( eYCbCrEncoding.BT709 )]
		DCI_P3 = 12,
	}
}