using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;
using Vrmac;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo.Containers.MP4
{
	sealed class AVC1SampleEntry: VideoSampleEntry
	{
		public readonly eAvcProfile profile;
		public readonly byte profileCompatibility;
		/// <summary>Level multiplied by 10, e.g. 41 = 0x29 for the level 4.1</summary>
		public readonly byte levelCode;

		public readonly byte[][] sps, pps, ppsExt;

		public readonly eChromaFormat chromaFormat;
		public readonly byte bitDepthLuma, bitDepthChroma;
		public readonly MPEG4BitRateBox bitRate;

		static readonly int decoderConfigSizeof = Marshal.SizeOf<Structures.AVCDecoderConfigurationRecord>();

		readonly int m_maxBytesInFrame;
		public override int maxBytesInFrame => m_maxBytesInFrame;

		public AVC1SampleEntry( Mp4Reader reader, int bytesLeft ) :
			base( reader, ref bytesLeft )
		{
			var avcc = reader.readStructure<Structures.AVCDecoderConfigurationRecord>();
			if( avcc.boxType != eAVC1BoxType.avcC )
				throw new NotImplementedException();
			bytesLeft -= decoderConfigSizeof;

			profile = avcc.profileCode;
			profileCompatibility = avcc.profileCompatibility;
			levelCode = avcc.levelCode;
			naluLengthSize = checked((byte)( avcc.lengthSizeMinusOne + 1 ));

			Span<byte> remainingStuff = stackalloc byte[ bytesLeft ];
			reader.read( remainingStuff );

			int readOffset = 0;
			sps = ContainerUtils.copyBlobs( avcc.numOfSequenceParameterSets, remainingStuff, ref readOffset );

			if( null == sps )
				throw new ArgumentException( "The file doesn't have an SPS" );
			// SpsData spsData = new SpsData( sps[ 0 ] );
			// File.WriteAllBytes( @"C:\Temp\2remove\h264\sps.bin", sps[ 0 ] );

			int ppsCount = remainingStuff[ readOffset++ ];
			pps = ContainerUtils.copyBlobs( ppsCount, remainingStuff, ref readOffset );

			if( null == sps || null == pps )
				throw new NotImplementedException( "Vrmac Video only supports mp4 files with out-of-band SPS and PPS blobs, in the `avcC` atom of the file." );
			if( sps.Length > 1 || pps.Length > 1 )
				throw new NotImplementedException( "Vrmac Video only supports mp4 files with a single out-of-band SPS and PPS for the complete video." );   // The video payload may include other PPS-es, these are fine.

			if( readOffset >= remainingStuff.Length )
				return;

			remainingStuff = remainingStuff.Slice( readOffset );

			if( readOffset + decoderConfigSizeof < avcc.length )
			{
				// The spec I have says the files with profile IDs 100, 110, 122, 144 have this.
				// The mp4 file I use to test this code has 100, but misses this data.
				chromaFormat = (eChromaFormat)( remainingStuff[ 0 ] & 3 );
				bitDepthLuma = (byte)( ( remainingStuff[ 1 ] & 7 ) + 8 );
				bitDepthChroma = (byte)( ( remainingStuff[ 2 ] & 7 ) + 8 );
				int numPpsEx = remainingStuff[ 3 ];
				readOffset = 4; // Resetting because sliced the span
				ppsExt = ContainerUtils.copyBlobs( numPpsEx, remainingStuff, ref readOffset );

				remainingStuff = remainingStuff.Slice( readOffset );
			}
			else
			{
				// https://en.wikipedia.org/wiki/Advanced_Video_Coding#Feature_support_in_particular_profiles
				chromaFormat = eChromaFormat.c420;
				bitDepthLuma = 8;
				bitDepthChroma = 8;
			}

			while( !remainingStuff.IsEmpty )
			{
				int size = BitConverter.ToInt32( remainingStuff ).endian();
				eAVC1BoxType code = (eAVC1BoxType)BitConverter.ToUInt32( remainingStuff.Slice( 4 ) );
				switch( code )
				{
					case eAVC1BoxType.btrt:
						bitRate = new MPEG4BitRateBox( remainingStuff );
						m_maxBytesInFrame = bitRate.decodingBufferSize;
						break;
				}
				remainingStuff = remainingStuff.Slice( size );
			}
		}

		// static CSize RoundUp2x2( CSize sz ) => new CSize( ( sz.cx + 1 ) & ( ~1 ), ( sz.cy + 1 ) & ( ~1 ) );
		public override VideoTextureDesc getTextureDesc()
		{
			switch( chromaFormat )
			{
				case eChromaFormat.c420:
					{
						CSize sizeChroma = new CSize( sizePixels.cx / 2, sizePixels.cy / 2 );
						if( 8 == bitDepthLuma && 8 == bitDepthChroma )
							return new VideoTextureDesc( TextureFormat.R8Unorm, TextureFormat.Rg8Unorm, sizePixels, sizeChroma );

						throw new NotImplementedException( "Vrmac Video only supports 8-bit color depth" );
					}
			}
			throw new NotImplementedException( "4:2:2 and 4:4:4 chroma profiles are not implemented" );
		}

		public override sPixelFormatMP getEncodedFormat()
		{
			sPixelFormatMP res = new sPixelFormatMP();
			res.size = sizePixels;
			res.pixelFormat = ePixelFormat.H264;
			res.field = eField.Progressive;
			// res.sizeImage = bitRate.decodingBufferSize;
			res.colorSpace = eColorSpace.BT709;
			//if( !extended )
			// 	return res;

			res.encoding = (byte)eYCbCrEncoding.BT709;
			res.quantization = eQuantization.LimitedRange;
			res.transferFunction = eTransferFunction.BT_709;
			return res;
		}

		internal override void writeParameters( EncodedBuffer buffer, eParameterSet which )
		{
			switch( which )
			{
				case eParameterSet.SPS:
					buffer.writeSps( sps[ 0 ] );
					return;
				case eParameterSet.PPS:
					buffer.writePps( pps[ 0 ] );
					return;
				case eParameterSet.VPS:
					throw new NotImplementedException( "VPS is only for h.265 video which is not yet implemented" );
				default:
					throw new ArgumentException( $"Unexpected eParameterSet set value { which }" );
			}
		}

		public override sPixelFormatMP getDecodedFormat()
		{
			sPixelFormatMP res = new sPixelFormatMP();
			res.size = sizePixels;
			res.pixelFormat = ePixelFormat.NV12;
			res.field = eField.Progressive;
			res.colorSpace = eColorSpace.BT709;
			res.encoding = (byte)eYCbCrEncoding.BT709;
			res.quantization = eQuantization.FullRange;
			res.transferFunction = eTransferFunction.BT_709;
			return res;
		}

		public override byte[] getEncodedSps()
		{
			if( null == sps || sps.Length < 1 )
				throw new ApplicationException( "No SPS" );
			if( sps.Length > 1 )
				throw new NotImplementedException( "Multiple SPS-es" );
			return sps[ 0 ];
		}

		public override SequenceParameterSet parseSps()
		{
			byte[] src = getEncodedSps();
			eNaluType nt = (eNaluType)( src[ 0 ] & 0x1F );
			if( nt != eNaluType.SPS )
				throw new ArgumentException();
			BitReader reader = new BitReader( src.AsSpan().Slice( 1 ) );
			return new SequenceParameterSet( ref reader );
		}

		internal override sDecodedVideoSize getDecodedSize()
		{
			SequenceParameterSet sps = parseSps();
			if( sps.bitDepthLuma != 8 || sps.bitDepthChroma != 8 )
				throw new NotImplementedException( "So far the library only supports 8-bit color depth of the video" );

			return new sDecodedVideoSize( sps.decodedSize, sps.cropRectangle, sps.chromaFormat );
		}
	}
}