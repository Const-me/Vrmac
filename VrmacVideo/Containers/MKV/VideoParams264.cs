#pragma warning disable CS0649, CS0169
using System;
using System.IO;
using System.Runtime.InteropServices;
using VrmacVideo.Containers.MP4;
using VrmacVideo.Containers.MP4.ElementaryStream;
using VrmacVideo.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Parameters for h264 video in MKV container.</summary>
	public sealed class VideoParams264: VideoParams
	{
		struct NativeStruct
		{
			byte configurationVersion; // = 1

			public eAvcProfile profileCode;
			public byte profileCompatibility;
			/// <summary>Level multiplied by 10, e.g. 41 = 0x29 for the level 4.1</summary>
			public byte levelCode;

			byte m_lengthSizeMinusOne;
			public byte lengthSizeMinusOne => (byte)( m_lengthSizeMinusOne & 3 );

			byte m_numOfSequenceParameterSets;
			public byte numOfSequenceParameterSets => (byte)( m_numOfSequenceParameterSets & 0x1F );
		}

		public readonly eAvcProfile profile;
		public readonly byte profileCompatibility;
		/// <summary>Level multiplied by 10, e.g. 41 = 0x29 for the level 4.1</summary>
		public readonly byte levelCode;

		public readonly byte[][] sps, pps, ppsExt;

		public override eChromaFormat chromaFormat { get; }
		public readonly byte bitDepthLuma, bitDepthChroma;

		public readonly SequenceParameterSet parsedSps;
		readonly sDecodedVideoSize m_decodedSize;

		internal VideoParams264( TrackEntry videoTrack )
		{
			// File.WriteAllBytes( @"C:\Temp\2remove\mkv\videoPrivateData.bin", videoTrack.codecPrivate );
			ReadOnlySpan<byte> codecPrivate = videoTrack.codecPrivate.AsSpan();
			int cbHeader = Marshal.SizeOf<NativeStruct>();
			NativeStruct ns = codecPrivate.Slice( 0, cbHeader ).cast<NativeStruct>()[ 0 ];

			profile = ns.profileCode;
			profileCompatibility = ns.profileCompatibility;
			levelCode = ns.levelCode;

			int offset = cbHeader;
			sps = ContainerUtils.copyBlobs( ns.numOfSequenceParameterSets, codecPrivate, ref offset );

			// File.WriteAllBytes( @"C:\Temp\2remove\mkv\sps.bin", sps[ 0 ] );

			int ppsCount = codecPrivate[ offset++ ];
			pps = ContainerUtils.copyBlobs( ppsCount, codecPrivate, ref offset );

			ReadOnlySpan<byte> spsBlob = sps[ 0 ].AsSpan();
			if( MiscUtils.getNaluType( spsBlob[ 0 ] ) != eNaluType.SPS )
				throw new ApplicationException( "The SPS is invalid, wrong NALU type" );
			spsBlob = spsBlob.Slice( 1 );

			BitReader spsReader = new BitReader( spsBlob );
			parsedSps = new SequenceParameterSet( ref spsReader );

			chromaFormat = parsedSps.chromaFormat;
			bitDepthLuma = parsedSps.bitDepthLuma;
			bitDepthChroma = parsedSps.bitDepthChroma;
			m_decodedSize = new sDecodedVideoSize( parsedSps.decodedSize, parsedSps.cropRectangle, chromaFormat );
		}

		public override sDecodedVideoSize decodedSize => m_decodedSize;

		/// <summary>Enqueue SPS and PPS NALUs</summary>
		internal override void enqueueParameters( EncodedQueue queue )
		{
			var b = queue.nextEnqueue;
			b.writeSps( sps[ 0 ] );
			queue.enqueue( b );

			b = queue.nextEnqueue;
			b.writePps( pps[ 0 ] );
			queue.enqueue( b );
		}

		internal override void setColorAttributes( ref Linux.sPixelFormatMP pixFormat )
		{
			parsedSps.setColorAttributes( ref pixFormat );
		}
	}
}