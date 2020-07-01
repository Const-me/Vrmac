using System.Runtime.InteropServices;
using Vrmac;

namespace VrmacVideo.Linux
{
	/// <summary>C++ type is v4l2_pix_format structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.12/media/uapi/v4l/pixfmt-002.html?highlight=v4l2_pix_format" />
	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	public struct sPixelFormat
	{
		public CSize size;
		/// <summary>Pixel format or type of compression, set by the application</summary>
		public ePixelFormat pixelFormat;
		public eField field;
		/// <summary>for padding, zero if unused</summary>
		public int bytesPerLine;
		/// <summary>Size in bytes of the buffer to hold a complete image. When the image consists of variable length compressed data, the maximum number of bytes required to hold an image.</summary>
		public int sizeImage;
		public eColorSpace colorSpace;

		uint privateDataMarker;

		const uint V4L2_PIX_FMT_PRIV_MAGIC = 0xfeedcafe;
		/// <summary>When false, the subsequent fields are invalid and must be interpreted as all zeros.</summary>
		public bool extendedFieldsValid
		{
			get => privateDataMarker == V4L2_PIX_FMT_PRIV_MAGIC;
			set => privateDataMarker = value ? V4L2_PIX_FMT_PRIV_MAGIC : 0;
		}

		public ePixelFormatFlags pixelFormatFlags;
		// Either eYCbCrEncoding or eHsvEncoding
		public uint encoding;

		int m_quantization, m_transferFunction;
		public eQuantization quantization
		{
			get => (eQuantization)m_quantization;
			set => m_quantization = (int)value;
		}

		public eTransferFunction transferFunction
		{
			get => (eTransferFunction)m_transferFunction;
			set => m_transferFunction = (int)value;
		}
	}
}