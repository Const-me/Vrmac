using System;
using System.Collections.Generic;
using System.Diagnostics;
using Vrmac;

namespace VrmacVideo.Containers.HEVC
{
	public struct SequenceParameterSet
	{
		public readonly byte vpsId;
		public readonly byte maxSubLayers;
		public readonly bool sps_temporal_id_nesting_flag;
		public readonly uint sps_seq_parameter_set_id;
		public readonly eChromaFormat chromaFormat;
		public readonly bool separate_colour_plane_flag;

		public readonly sDecodedVideoSize decodedVideoSize;
		public readonly byte bit_depth_luma, bit_depth_chroma;

		public readonly byte log2_max_pic_order_cnt_lsb;

		readonly byte log2_min_cb_size, log2_diff_max_min_luma_coding_block_size, log2_min_luma_transform_block_size, log2_diff_max_min_luma_transform_block_size;
		readonly uint max_transform_hierarchy_depth_inter, max_transform_hierarchy_depth_intra;

		public readonly bool scaling_list_enabled_flag;
		// 4 groups of 8 bits
		// readonly uint scaling_list_pred_mode_flag;
		// readonly uint[,] scaling_list_pred_matrix_id_delta;
		// readonly int[,] scaling_list_dc_coef;
		// readonly byte[,,] ScalingList;
		readonly ScalingList scalingList;

		readonly bool amp_enabled_flag, pcm_enabled_flag;
		readonly byte pcm_sample_bit_depth_luma, pcm_sample_bit_depth_chroma;
		readonly uint log2_min_pcm_luma_coding_block_size, log2_diff_max_min_pcm_luma_coding_block_size;
		readonly bool pcm_loop_filter_disabled_flag;
		readonly ShortTermRefPictureSet[] shortTermRefPictureSets;

		readonly bool sps_temporal_mvp_enabled_flag, strong_intra_smoothing_enabled_flag, vui_parameters_present_flag;

		public SequenceParameterSet( ReadOnlySpan<byte> span, Dictionary<byte, VideoParameterSet> vpsDict, byte nuh_layer_id = 0 )
		{
			// System.IO.File.WriteAllBytes( @"C:\Temp\2remove\mkv\sps.bin", span.ToArray() );
			// MiscUtils.writeBinaryText( @"C:\Temp\2remove\mkv\sps.txt", span );
			BitReader reader = new BitReader( span );

			checked
			{
				// F.7.3.2.2.1 General sequence parameter set RBSP syntax, page 450
				vpsId = reader.readByte( 4 );  // sps_video_parameter_set_id
				VideoParameterSet vps = vpsDict[ vpsId ];
				maxSubLayers = (byte)( reader.readInt( 3 ) + 1 );   // sps_max_sub_layers_minus1 or sps_ext_or_max_sub_layers_minus1
				sps_temporal_id_nesting_flag = reader.readBit();
				// const bool MultiLayerExtSpsFlag = false;  // nuh_layer_id != 0 && sps_ext_or_max_sub_layers_minus1 = = 7
				ProfileTierLevel.skip( ref reader, true, maxSubLayers );

				sps_seq_parameter_set_id = reader.unsignedGolomb();
				chromaFormat = (eChromaFormat)reader.unsignedGolomb();
				if( chromaFormat == eChromaFormat.c444 )
					separate_colour_plane_flag = reader.readBit();
				else
					separate_colour_plane_flag = false;

				CSize size = default;
				size.cx = (int)reader.unsignedGolomb();
				size.cy = (int)reader.unsignedGolomb();

				CRect rc;
				if( reader.readBit() )    // conformance_window_flag
				{
					rc = default;
					int scalingX = ( chromaFormat < eChromaFormat.c444 ) ? 2 : 1;
					rc.left = scalingX * (int)reader.unsignedGolomb(); // conf_win_vps_left_offset 
					rc.right = size.cx - scalingX * (int)reader.unsignedGolomb();   // conf_win_vps_right_offset

					int scalingY = ( chromaFormat < eChromaFormat.c422 ) ? 2 : 1;
					rc.top = scalingY * (int)reader.unsignedGolomb();   // conf_win_vps_top_offset 
					rc.bottom = size.cy - scalingY * (int)reader.unsignedGolomb();  // conf_win_vps_bottom_offset 
				}
				else
					rc = new CRect( default, size );
				decodedVideoSize = new sDecodedVideoSize( size, rc, chromaFormat );

				bit_depth_luma = (byte)( reader.unsignedGolomb() + 8 );
				bit_depth_chroma = (byte)( reader.unsignedGolomb() + 8 );

				// There's probably a bug below this line, the code throws an exception trying to read past the end of the span.
				log2_max_pic_order_cnt_lsb = (byte)( reader.unsignedGolomb() + 4 );
				if( log2_max_pic_order_cnt_lsb > 16 )
					throw new ArgumentOutOfRangeException( "log2_max_pic_order_cnt_lsb is out of range" );

				bool sps_sub_layer_ordering_info_present_flag = reader.readBit();
				int iStart;
				if( sps_sub_layer_ordering_info_present_flag )
					iStart = 0;
				else
					iStart = maxSubLayers - 1;
				for( int i = iStart; i < maxSubLayers; i++ )
				{
					uint sps_max_dec_pic_buffering = reader.unsignedGolomb() + 1;
					uint sps_max_num_reorder_pics = reader.unsignedGolomb();
					int sps_max_latency_increase_plus1 = (int)reader.unsignedGolomb() - 1;
				}

				log2_min_cb_size = (byte)( reader.unsignedGolomb() + 3 );
				int MinCbLog2SizeY = log2_min_cb_size;
				int MinCbSizeY = 1 << MinCbLog2SizeY;

				log2_diff_max_min_luma_coding_block_size = (byte)( reader.unsignedGolomb() );
				int CtbLog2SizeY = MinCbLog2SizeY + log2_diff_max_min_luma_coding_block_size;
				int CtbSizeY = 1 << CtbLog2SizeY;

				log2_min_luma_transform_block_size = (byte)( reader.unsignedGolomb() + 2 );

				log2_diff_max_min_luma_transform_block_size = (byte)( reader.unsignedGolomb() );

				max_transform_hierarchy_depth_inter = reader.unsignedGolomb();
				max_transform_hierarchy_depth_intra = reader.unsignedGolomb();
				// The value of max_transform_hierarchy_depth_intra shall be in the range of 0 to CtbLog2SizeY − MinTbLog2SizeY, inclusive.

				scaling_list_enabled_flag = reader.readBit();
				// scaling_list_pred_mode_flag = 0;
				// scaling_list_dc_coef = null;
				// scaling_list_pred_matrix_id_delta = null;
				// ScalingList = null;
				scalingList = null;
				if( scaling_list_enabled_flag )
				{
					bool sps_scaling_list_data_present_flag = reader.readBit();
					if( sps_scaling_list_data_present_flag )
					{
						scalingList = new ScalingList();
						scalingList.setDefault();
						scalingList.read( ref reader );
					}
				}

				// Scroll to page 452..
				amp_enabled_flag = reader.readBit();
				bool sample_adaptive_offset_enabled_flag = reader.readBit();
				pcm_enabled_flag = reader.readBit();
				if( pcm_enabled_flag )
				{
					pcm_sample_bit_depth_luma = (byte)( reader.readInt( 4 ) + 1 );  // pcm_sample_bit_depth_luma_minus1
					pcm_sample_bit_depth_chroma = (byte)( reader.readInt( 4 ) + 1 );    // pcm_sample_bit_depth_chroma_minus1
					log2_min_pcm_luma_coding_block_size = reader.unsignedGolomb() + 3;  // log2_min_pcm_luma_coding_block_size_minus3
					log2_diff_max_min_pcm_luma_coding_block_size = reader.unsignedGolomb();
					pcm_loop_filter_disabled_flag = reader.readBit();
				}
				else
				{
					pcm_sample_bit_depth_luma = bit_depth_luma;
					pcm_sample_bit_depth_chroma = bit_depth_chroma;
					log2_min_pcm_luma_coding_block_size = log2_diff_max_min_pcm_luma_coding_block_size = 0;
					pcm_loop_filter_disabled_flag = false;
				}

				uint num_short_term_ref_pic_sets = reader.unsignedGolomb();
				if( num_short_term_ref_pic_sets > 64 )
					throw new ArgumentException( "The value of num_short_term_ref_pic_sets shall be in the range of 0 to 64, inclusive." );

				if( num_short_term_ref_pic_sets > 0 )
				{
					shortTermRefPictureSets = new ShortTermRefPictureSet[ num_short_term_ref_pic_sets ];
					uint[] NumDeltaPocs = new uint[ (int)num_short_term_ref_pic_sets ];
					for( uint i = 0; i < num_short_term_ref_pic_sets; i++ )
						shortTermRefPictureSets[ i ] = new ShortTermRefPictureSet( ref reader, num_short_term_ref_pic_sets, i, NumDeltaPocs );
				}
				else
					shortTermRefPictureSets = null;

				bool long_term_ref_pics_present_flag = reader.readBit();
				if( long_term_ref_pics_present_flag )
				{
					uint num_long_term_ref_pics_sps = reader.unsignedGolomb();
					if( num_long_term_ref_pics_sps > 0 )
					{
						reader.skipBits( log2_max_pic_order_cnt_lsb );// lt_ref_pic_poc_lsb_sps[ i ]
						reader.skipBits( 1 ); // used_by_curr_pic_lt_sps_flag[ i ]
					}
				}

				sps_temporal_mvp_enabled_flag = reader.readBit();
				strong_intra_smoothing_enabled_flag = reader.readBit();
				vui_parameters_present_flag = reader.readBit();
			}
		}

		public uint log2_ctb_size => (uint)log2_min_cb_size + log2_diff_max_min_luma_coding_block_size;

		/// <summary>Make two 32-bit values to configure SPS in the hardware decoder</summary>
		public void produceDecoderData( Span<uint> sps )
		{
			Debug.Assert( sps.Length == 2 );

			// Make first half of the SPS
			Debug.Assert( log2_min_cb_size < 16 );
			uint v = log2_min_cb_size;

			uint log2_ctb_size = this.log2_ctb_size;
			Debug.Assert( log2_ctb_size < 16 );
			v |= ( log2_ctb_size << 4 );

			Debug.Assert( log2_min_luma_transform_block_size < 16 );
			v |= ( (uint)log2_min_luma_transform_block_size << 8 );

			uint log2_max_trafo_size = (uint)log2_diff_max_min_luma_coding_block_size + log2_min_luma_transform_block_size;
			Debug.Assert( log2_max_trafo_size < 16 );
			v |= ( (uint)log2_min_luma_transform_block_size << 12 );

			Debug.Assert( bit_depth_luma < 16 && bit_depth_chroma < 16 );
			v |= ( (uint)bit_depth_luma << 16 );
			v |= ( (uint)bit_depth_chroma << 20 );

			Debug.Assert( max_transform_hierarchy_depth_inter < 16 && max_transform_hierarchy_depth_intra < 16 );
			v |= ( max_transform_hierarchy_depth_inter << 24 );
			v |= ( max_transform_hierarchy_depth_inter << 28 );
			sps[ 0 ] = v;

			// Make second half of the SPS
			v = 0;
			Debug.Assert( pcm_sample_bit_depth_luma < 16 && pcm_sample_bit_depth_chroma < 16 );
			v = pcm_sample_bit_depth_luma;
			v |= (uint)pcm_sample_bit_depth_chroma << 4;

			Debug.Assert( log2_min_pcm_luma_coding_block_size < 16 && log2_diff_max_min_pcm_luma_coding_block_size < 16 );
			v |= log2_min_pcm_luma_coding_block_size << 8;
			v |= log2_diff_max_min_pcm_luma_coding_block_size << 12;

			if( !separate_colour_plane_flag )
				v |= (uint)chromaFormat << 16;

			if( amp_enabled_flag )
				v |= 1u << 18;
			if( pcm_enabled_flag )
				v |= 1u << 19;
			if( scaling_list_enabled_flag )
				v |= 1u << 20;
			if( strong_intra_smoothing_enabled_flag )
				v |= 1u << 22;

			sps[ 1 ] = v;
		}
	}
}