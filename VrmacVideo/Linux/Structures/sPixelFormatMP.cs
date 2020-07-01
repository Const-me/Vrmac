using Vrmac;
using System;
using System.Text;

namespace VrmacVideo.Linux
{
	/// <summary>C++ type is v4l2_plane_pix_format structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/pixfmt-v4l2-mplane.html" />
	public unsafe struct sPlanePixelFormat
	{
		/// <summary>Maximum size in bytes required for image data in this plane</summary>
		public int sizeImage;
		/// <summary>Distance in bytes between the leftmost pixels in two adjacent lines</summary>
		public int bytesPerLine;

		/// <summary>Reserved for future extensions</summary>
		fixed ushort reserved[ 6 ];

		public override string ToString() => $"sizeImage { sizeImage }, bytesPerLine { bytesPerLine }";
	}

	/// <summary>C++ type is v4l2_pix_format_mplane	structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/pixfmt-v4l2-mplane.html" />
	public unsafe struct sPixelFormatMP
	{
		public const int MAX_PLANES = 8;

		public CSize size;
		public ePixelFormat pixelFormat;
		public eField field;
		public eColorSpace colorSpace;
		fixed int plane_fmt[ MAX_PLANES * 5 ];
		public byte numPlanes;
		public ePixelFormatFlags flags;
		// Either eYCbCrEncoding or eHsvEncoding
		public byte encoding;
		public eQuantization quantization;
		public eTransferFunction transferFunction;
		/// <summary>Reserved for future extensions</summary>
		fixed byte reserved[ 7 ];

		public sPlanePixelFormat getPlaneFormat( int idx )
		{
			if( idx < 0 || idx >= numPlanes )
				throw new ArgumentOutOfRangeException();
			int baseIndex = idx * 5;
			sPlanePixelFormat res = new sPlanePixelFormat();
			res.sizeImage = plane_fmt[ baseIndex ];
			res.bytesPerLine = plane_fmt[ baseIndex + 1 ];
			return res;
		}

		public void setPlaneFormat( int idx, sPlanePixelFormat ppf )
		{
			if( idx < 0 || idx >= MAX_PLANES )
				throw new ArgumentOutOfRangeException();
			int baseIndex = idx * 5;
			plane_fmt[ baseIndex ] = ppf.sizeImage;
			plane_fmt[ baseIndex + 1 ] = ppf.bytesPerLine;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( "size = {0}", size );
			sb.AppendFormat( ", pixelFormat = {0}", pixelFormat );
			sb.AppendFormat( ", colorSpace = {0}", colorSpace );
			sb.AppendFormat( ", numPlanes = {0}", numPlanes );
			sb.AppendFormat( ", encoding = {0}", encoding );
			sb.AppendFormat( ", quantization = {0}", quantization );
			sb.AppendFormat( ", transferFunction = {0}", transferFunction );
			for( int i = 0; i < numPlanes; i++ )
				sb.AppendFormat( "\nPlane #{0}: {1}", i, getPlaneFormat( i ) );
			return sb.ToString();
		}
	}
}