using VrmacVideo.Linux;

namespace VrmacVideo.Containers.MKV
{
	public abstract class VideoParams
	{
		public abstract eChromaFormat chromaFormat { get; }
		public abstract sDecodedVideoSize decodedSize { get; }

		internal abstract void enqueueParameters( EncodedQueue queue );

		internal abstract void setColorAttributes( ref sPixelFormatMP pixFormat );
	}
}