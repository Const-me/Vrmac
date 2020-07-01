using Vrmac;

namespace VrmacVideo
{
	public struct sDecodedVideoSize
	{
		/// <summary>Size of the video including cropping borders</summary>
		public readonly CSize size;

		/// <summary>Crop rectangle; h264 works on macroblocks, for most sizes of the video it needs to be cropped.
		/// Cropping can only occur between the chroma samples, i.e. for 4:2:0 chroma subsampling, crop granularity is 2x2 pixels.</summary>
		public readonly CRect cropRect;

		/// <summary>The only one supported so far is 4:2:0</summary>
		public readonly eChromaFormat chromaFormat;

		internal sDecodedVideoSize( CSize size, CRect cropRect, eChromaFormat chromaFormat )
		{
			this.size = size;
			this.cropRect = cropRect;
			this.chromaFormat = chromaFormat;
		}

		/// <summary>Create uncropped</summary>
		public sDecodedVideoSize( CSize size )
		{
			this.size = size;
			cropRect = new CRect( default, size );
			chromaFormat = eChromaFormat.Unknown;
		}

		public override string ToString() => $"size { size }, cropRect { cropRect }, chromaFormat { chromaFormat }";
	}
}