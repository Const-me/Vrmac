using System;
using System.Collections.Generic;
using Vrmac;
using VrmacVideo.Containers.MP4.ElementaryStream;
using VrmacVideo.Linux;

namespace VrmacVideo.Containers.MP4
{
	public struct SequenceParameterSet
	{
		public readonly eAvcProfile profile;
		/// <summary>Level * 10, e.g. 41 for level 4.1</summary>
		public readonly byte levelIndex;
		public readonly byte parameterSetId;
		public readonly eChromaFormat chromaFormat;
		public readonly bool separateColourPlaneFlag;
		public readonly byte bitDepthLuma, bitDepthChroma;
		public readonly byte frameIndexBits;

		public readonly CSize decodedSize;
		public readonly CRect cropRectangle;
		public readonly VideoUsabilityInfo vui;

		static readonly HashSet<byte> extraBs = new HashSet<byte>()
		{
			// 7.3.2.1.1 Sequence parameter set data syntax
			44, 83, 86, 100, 110, 118, 122, 128, 138, 134, 135, 139, 244
		};

		internal SequenceParameterSet( ref BitReader reader )
		{
			// Ported from C++ in Chromium, \chromium-master\media\video\h264_parser.cc

			profile = (eAvcProfile)reader.readInt( 8 );
			reader.skipBits( 8 );   // constraint_set0_flag x6 + 2 reserved bits
			levelIndex = (byte)reader.readInt( 8 );
			parameterSetId = checked((byte)reader.unsignedGolomb());    // seq_parameter_set_id
			if( parameterSetId >= 32 )
				throw new ArgumentException();

			if( extraBs.Contains( (byte)profile ) )
			{
				uint chromaFormatIndex = reader.unsignedGolomb();   // uint chroma_format_idc
				if( chromaFormatIndex >= 4 )
					throw new ArgumentException();
				chromaFormat = (eChromaFormat)chromaFormatIndex;
				if( 3 == chromaFormatIndex )
					separateColourPlaneFlag = reader.readBit();
				else
					separateColourPlaneFlag = false;

				uint bit_depth_luma_minus8 = reader.unsignedGolomb();
				if( bit_depth_luma_minus8 >= 7 )
					throw new ArgumentException();
				bitDepthLuma = (byte)( bit_depth_luma_minus8 + 8 );

				uint bit_depth_chroma_minus8 = reader.unsignedGolomb();
				if( bit_depth_chroma_minus8 >= 7 )
					throw new ArgumentException();
				bitDepthChroma = (byte)( bit_depth_chroma_minus8 + 8 );

				bool qpprime_y_zero_transform_bypass_flag = reader.readBit();
				bool seq_scaling_matrix_present_flag = reader.readBit();
				if( seq_scaling_matrix_present_flag )
					throw new NotImplementedException();
			}
			else
			{
				chromaFormat = eChromaFormat.c420;
				separateColourPlaneFlag = false;
				bitDepthLuma = bitDepthChroma = 8;
			}

			uint log2_max_frame_num_minus4 = reader.unsignedGolomb();
			frameIndexBits = checked((byte)( log2_max_frame_num_minus4 + 4 ));

			uint pic_order_cnt_type = reader.unsignedGolomb();
			switch( pic_order_cnt_type )
			{
				case 0:
					reader.skipGolomb();    // log2_max_pic_order_cnt_lsb_minus4
					break;
				case 1:
					reader.skipBits( 1 ); // delta_pic_order_always_zero_flag
					reader.skipGolomb();  // int offset_for_non_ref_pic
					reader.skipGolomb();  // int offset_for_top_to_bottom_field
					uint num_ref_frames_in_pic_order_cnt_cycle = reader.unsignedGolomb();
					if( num_ref_frames_in_pic_order_cnt_cycle >= 0xFF )
						throw new ArgumentException();
					for( int i = 0; i < num_ref_frames_in_pic_order_cnt_cycle; i++ )
						reader.skipGolomb();    // int offset_for_ref_frame
					break;
			}
			reader.skipGolomb();// uint max_num_ref_frames
			reader.skipBits( 1 );   // bool gaps_in_frame_num_value_allowed_flag

			// Decoded size
			int pic_width_in_mbs_minus1 = (int)reader.unsignedGolomb();
			int pic_height_in_map_units_minus1 = (int)reader.unsignedGolomb();
			bool frame_mbs_only_flag = reader.readBit();
			decodedSize = default;
			decodedSize.cx = ( pic_width_in_mbs_minus1 + 1 ) * 16;
			if( false == frame_mbs_only_flag )
				reader.skipBits( 1 );   // bool mb_adaptive_frame_field_flag
			int map_unit = frame_mbs_only_flag ? 16 : 32;
			decodedSize.cy = map_unit * ( pic_height_in_map_units_minus1 + 1 );

			reader.skipBits( 1 ); // direct_8x8_inference_flag

			// Frame cropping
			bool frame_cropping_flag = reader.readBit();
			if( frame_cropping_flag )
			{
				int frame_crop_left_offset = (int)reader.unsignedGolomb();
				int frame_crop_right_offset = (int)reader.unsignedGolomb();
				int frame_crop_top_offset = (int)reader.unsignedGolomb();
				int frame_crop_bottom_offset = (int)reader.unsignedGolomb();

				CSize cropUnit;
				switch( chromaFormat )
				{
					case eChromaFormat.c420:
						cropUnit = new CSize( 2, 2 );
						break;
					case eChromaFormat.c422:
						cropUnit = new CSize( 2, 1 );
						break;
					case eChromaFormat.c444:
						cropUnit = new CSize( 1, 1 );
						break;
					default:
						throw new ArgumentException();
				}

				cropRectangle = default;
				cropRectangle.left = frame_crop_left_offset * cropUnit.cx;
				cropRectangle.top = frame_crop_top_offset * cropUnit.cy;
				cropRectangle.right = decodedSize.cx - frame_crop_right_offset * cropUnit.cx;
				cropRectangle.bottom = decodedSize.cy - frame_crop_bottom_offset * cropUnit.cy;
			}
			else
				cropRectangle = new CRect( default, decodedSize );

			vui = new VideoUsabilityInfo( ref reader );
		}

		/// <summary>True if the video usability info, VUI, has color information of the video signal</summary>
		bool hasVideoColorDesc
		{
			get
			{
				eVuiFlags flags = eVuiFlags.VUI | eVuiFlags.VideoSignal | eVuiFlags.VideoSignalColorDesc;
				return ( vui.flags & flags ) == flags;
			}
		}

		eQuantization quantization
		{
			get
			{
				if( vui.flags.HasFlag( eVuiFlags.VideoSignalFullRange ) )
					return eQuantization.FullRange;
				return eQuantization.LimitedRange;
			}
		}

		eTransferFunction transferFunction
		{
			get
			{
				if( !hasVideoColorDesc )
					return eTransferFunction.BT_709;
				var v = vui.transferCharacteristics.map();
				if( v.HasValue )
					return v.Value;
				Logger.logWarning( "Unknown transfer function {0}, defaulting to BT.709", vui.transferCharacteristics );
				return eTransferFunction.BT_709;
			}
		}

		eYCbCrEncoding encoding
		{
			get
			{
				if( !hasVideoColorDesc )
					return eYCbCrEncoding.BT709;
				var v = vui.matrixCoefficients.map();
				if( v.HasValue )
					return v.Value;
				Logger.logWarning( "Unknown matrix coefficients {0}, defaulting to BT.709", vui.matrixCoefficients );
				return eYCbCrEncoding.BT709;
			}
		}

		eColorSpace colorSpace
		{
			get
			{
				if( !hasVideoColorDesc )
					return eColorSpace.BT709;
				var v = vui.colorPrimaries.map();
				if( v.HasValue )
					return v.Value;
				Logger.logWarning( "Unknown color space {0}, defaulting to BT.709", vui.colorPrimaries );
				return eColorSpace.BT709;
			}
		}

		public void setColorAttributes( ref sPixelFormatMP res )
		{
			res.colorSpace = colorSpace;
			res.encoding = (byte)encoding;
			res.quantization = quantization;
			res.transferFunction = transferFunction;
		}
	}
}