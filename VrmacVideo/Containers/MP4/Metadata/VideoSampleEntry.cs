using System.Numerics;
using System.Runtime.InteropServices;
using Vrmac;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo.Containers.MP4
{
	public enum eParameterSet: byte
	{
		SPS,
		PPS,
		VPS
	}

	public abstract class VideoSampleEntry: SampleEntry
	{
		/// <summary>Video size in pixels</summary>
		public readonly CSize sizePixels;
		/// <summary>Nominal resolution in pixels per inch.</summary>
		/// <remarks>No idea why it’s there, people ain’t gonna print their videos.</remarks>
		public readonly Vector2 pixelsPerInch;
		/// <summary>How many frames of compressed video are stored in each sample. Normally 1 meaning one frame per sample. Might be &gt; 1 for multiple frames per sample</summary>
		public readonly ushort framesPerSample;
		/// <summary>Optional metadata in the video, can be null</summary>
		public readonly string compressorName;

		public byte naluLengthSize { get; protected set; }

		internal VideoSampleEntry( Mp4Reader reader, ref int bytesLeft )
		{
			var ss = reader.readStructure<Structures.VisualSampleEntry>();
			bytesLeft -= Marshal.SizeOf<Structures.VisualSampleEntry>();

			sizePixels = ss.size;
			pixelsPerInch = ss.resolution;
			framesPerSample = ss.frameCount;
			unsafe
			{
				byte* comp = ss.compressorname;
				compressorName = StringMarshal.copy( comp, 32 );
			}
		}

		public abstract VideoTextureDesc getTextureDesc();

		public abstract int maxBytesInFrame { get; }
		public abstract sPixelFormatMP getEncodedFormat();
		public abstract sPixelFormatMP getDecodedFormat();

		internal abstract void writeParameters( EncodedBuffer buffer, eParameterSet which );

		internal abstract sDecodedVideoSize getDecodedSize();

		public abstract byte[] getEncodedSps();
		public abstract SequenceParameterSet parseSps();
	}
}