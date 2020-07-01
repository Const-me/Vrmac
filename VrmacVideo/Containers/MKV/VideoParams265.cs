using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Vrmac;
using VrmacVideo.Containers.HEVC;
using VrmacVideo.Linux;

namespace VrmacVideo.Containers.MKV
{
	class VideoParams265: VideoParams
	{
		// https://stackoverflow.com/a/43617477/
		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		unsafe struct NativeStruct
		{
			byte configurationVersion; // 1

			// general_profile_space, general_tier_flag, general_profile_idc
			byte m_profileInfo;
			public byte general_profile_space => (byte)( m_profileInfo >> 6 );
			public bool general_tier_flag => 0 != ( m_profileInfo & 20 );
			public byte general_profile_idc => (byte)( m_profileInfo & 0x1F );

			fixed byte general_profile_compatibility_flags[ 4 ];
			fixed byte general_constraint_indicator_flags[ 6 ];
			public byte general_level_idc;
			ushort m_min_spatial_segmentation_idc;
			public ushort min_spatial_segmentation_idc => (ushort)( BinaryPrimitives.ReverseEndianness( m_min_spatial_segmentation_idc ) & 0xFFF );

			byte m_parallelismType;
			public byte parallelismType => (byte)( m_parallelismType & 3 );

			byte chroma_format_idc;
			public byte chromaFormatIndex => (byte)( chroma_format_idc & 3 );

			byte bit_depth_luma_minus8;
			public byte bitDepthLuma => (byte)( ( bit_depth_luma_minus8 & 7 ) + 8 );

			byte bit_depth_chroma_minus8;
			public byte bitDepthChroma => (byte)( ( bit_depth_chroma_minus8 & 7 ) + 8 );

			ushort avgFrameRate;
			public ushort averageFrameRate => BinaryPrimitives.ReverseEndianness( avgFrameRate );

			byte m_random;
			public byte constantFrameRate => (byte)( m_random >> 6 );
			public byte numTemporalLayers => (byte)( ( m_random >> 3 ) & 7 );
			public bool temporalIdNested => 0 != ( m_random & 4 );
			public byte lengthSize => (byte)( ( m_random & 3 ) + 1 );
			public byte numOfArrays;
		}

		struct ConfigArray
		{
			public readonly bool arrayCompleteness;
			public readonly eNaluType naluType;
			public readonly byte[][] nalus;

			public ConfigArray( ref ReadOnlySpan<byte> src )
			{
				byte b1 = src[ 0 ];
				arrayCompleteness = 0 != ( b1 & 0x80 );
				naluType = (eNaluType)( b1 & 0x3F );
				int numNalus = BinaryPrimitives.ReadUInt16BigEndian( src.Slice( 1 ) );
				nalus = new byte[ numNalus ][];
				src = src.Slice( 3 );

				for( int i = 0; i < numNalus; i++ )
				{
					int nalUnitLength = BinaryPrimitives.ReadUInt16BigEndian( src );
					nalus[ i ] = src.Slice( 2, nalUnitLength ).ToArray();
					src = src.Slice( 2 + nalUnitLength );
				}
			}

			public override string ToString() => $"{ naluType }, { nalus.Length.pluralString( "blob" ) }, arrayCompleteness = { arrayCompleteness }";
		}

		readonly ConfigArray[] arrays;

		IEnumerable<byte[]> blobs( eNaluType nalu ) =>
			arrays.Where( a => a.naluType == nalu ).SelectMany( a => a.nalus );

		readonly Dictionary<byte, VideoParameterSet> parsedVps;
		readonly SequenceParameterSet[] parsedSps;
		readonly PictureParameterSet[] parsedPps;

		internal VideoParams265( TrackEntry videoTrack )
		{
			// File.WriteAllBytes( @"C:\Temp\2remove\mkv\videoPrivateData.bin", videoTrack.codecPrivate );

			ReadOnlySpan<byte> codecPrivate = videoTrack.codecPrivate.AsSpan();
			int cbHeader = Marshal.SizeOf<NativeStruct>();
			NativeStruct ns = codecPrivate.Slice( 0, cbHeader ).cast<NativeStruct>()[ 0 ];
			codecPrivate = codecPrivate.Slice( cbHeader );
			chromaFormat = (eChromaFormat)ns.chromaFormatIndex;

			arrays = new ConfigArray[ ns.numOfArrays ];
			for( int i = 0; i < ns.numOfArrays; i++ )
			{
				var ca = new ConfigArray( ref codecPrivate );
				arrays[ i ] = ca;
			}

			parsedVps = blobs( eNaluType.VPS )
				.Select( arr => new VideoParameterSet( arr.AsSpan() ) )
				.ToDictionary( v => v.vps_video_parameter_set_id );

			parsedSps = blobs( eNaluType.SPS )
				.Select( arr => new SequenceParameterSet( arr.AsSpan(), parsedVps ) )
				.ToArray();

			parsedPps = blobs( eNaluType.PPS )
				.Select( arr => new PictureParameterSet( arr.AsSpan() ) )
				.ToArray();

			if( chromaFormat == eChromaFormat.Unknown )
				chromaFormat = parsedSps[ 0 ].chromaFormat;
			else if( parsedSps[ 0 ].chromaFormat != eChromaFormat.Unknown && parsedSps[ 0 ].chromaFormat != chromaFormat )
				throw new ApplicationException( "Different parameter sets disagree on the chroma format" );
		}

		public override eChromaFormat chromaFormat { get; }

		public override sDecodedVideoSize decodedSize => parsedSps[ 0 ].decodedVideoSize;

		internal override void enqueueParameters( EncodedQueue queue )
		{
			throw new NotImplementedException();
		}

		internal override void setColorAttributes( ref sPixelFormatMP pixFormat )
		{
			throw new NotImplementedException();
		}


	}
}