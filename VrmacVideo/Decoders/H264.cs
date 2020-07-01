using System;
using System.Collections.Generic;
using VrmacVideo.Linux;

namespace VrmacVideo.Decoders
{
	class H264: IDisposable
	{
		public readonly VideoDevice device;

		// If you wanna know WTF input formats are called "output", ask Linux kernel developers; I don't want to fix that on my side because documentation.

		// If you wanna know WTF h264 format, which has no planes at all being a flat sequence of NALUs, is multiplane - ask Raspberry Pi4 developers why it's the only one supported by their encoder driver.
		// The documentation there https://www.kernel.org/doc/html/latest/media/uapi/v4l/dev-decoder.html section 4.5.1.1.5 says following:
		// "Single-planar API (see Single- and multi-planar APIs) and applicable structures may be used interchangeably with multi-planar API, unless specified otherwise"
		// Despite Pi4 only officially supports 1 specific version of Debian Linux, they failed to implement a good enough driver support.
		// The requirement to use multi-planar API for encoded video complicated quite a few things here.
		const eBufferType inputBufferType = eBufferType.VideoOutputMPlane;

		const eBufferType outputBufferType = eBufferType.VideoCaptureMPlane;

		const ePixelFormat inputPixelFormat = ePixelFormat.H264;
		// At least on Windows, NV12 is the most compatible of them, in my experience.
		// It's also the most efficient: keeping both chroma channels together makes better use of caches, and saves 1/3 = 33% of texture loads while rendering the video.
		const ePixelFormat outputPixelFormat = ePixelFormat.NV12;

		readonly sImageFormat inputFormat, outputFormat;
		public readonly SizeSupported inputSize, outputSize;

		public H264( VideoDevice device )
		{
			this.device = device;

			// Pi4 only supports two:
			// "H.264", H264
			// "Motion-JPEG", MJPEG
			sImageFormatDescription formatDesc = device.enumerateFormats( inputBufferType )
				.first( i => i.pixelFormat == inputPixelFormat, "h.264 decoder requires a hardware capable of decoding h.264. The provided video device can’t do that." );
			// Logger.logVerbose( "Compressed format: {0}", formatDesc );
			inputFormat = new sImageFormat( ref formatDesc );

			// We gonna be using NV12, but generally speaking Pi4 supports following:
			// "Planar YUV 4:2:0", YUV420
			// "Planar YVU 4:2:0", YVU420
			// "Y/CbCr 4:2:0", NV12
			// "Y/CrCb 4:2:0", NV21
			// "16-bit RGB 5-6-5", RGB565
			formatDesc = device.enumerateFormats( outputBufferType )
				.first( i => i.pixelFormat == outputPixelFormat, "h.264 decoder requires a hardware capable of decoding h.264 into NV12. The provided video device can’t do that." );
			outputFormat = new sImageFormat( ref formatDesc );

			inputSize = SizeSupported.query( device, inputPixelFormat );
			outputSize = SizeSupported.query( device, outputPixelFormat );
		}

		IEnumerable<string> details()
		{
			yield return $"Input format: \"{ inputFormat.description }\"";
			yield return $"Input size supported: { inputSize }";

			yield return $"Output format: \"{ outputFormat.description }\"";
			yield return $"Output size supported: { outputSize }";
		}

		public override string ToString() => details().makeLines();

		public void Dispose()
		{
			device?.Dispose();
		}
	}
}