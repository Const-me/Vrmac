using System.Runtime.InteropServices;

namespace VrmacVideo.Linux
{
	/// <summary>C++ type is v4l2_format structure</summary>
	/// <seealso href="https://www.kernel.org/doc/html/v4.19/media/uapi/v4l/vidioc-g-fmt.html" />
	[StructLayout( LayoutKind.Explicit, Size = 204 )]
	struct sStreamDataFormat
	{
		[FieldOffset( 0 )] public eBufferType bufferType;
		[FieldOffset( 4 )] public sPixelFormat pix;
		[FieldOffset( 4 )] public sPixelFormatMP pix_mp;
		// There're 4 more values in the union, but we don't need them

		public override string ToString()
		{
			if( bufferType.isMultiPlaneBufferType() )
				return pix_mp.ToString();
			else
				return pix.ToString();
		}
	}
}