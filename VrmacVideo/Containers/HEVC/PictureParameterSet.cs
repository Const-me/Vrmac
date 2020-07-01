using System;
using System.Diagnostics;

namespace VrmacVideo.Containers.HEVC
{
	struct PictureParameterSet
	{
		readonly uint pps_pic_parameter_set_id, pps_seq_parameter_set_id;
		readonly bool dependent_slice_segments_enabled_flag, output_flag_present_flag;
		readonly byte num_extra_slice_header_bits;
		readonly bool sign_data_hiding_enabled_flag, cabac_init_present_flag;
		readonly uint num_ref_idx_l0_default_active, num_ref_idx_l1_default_active;
		readonly int init_qp;
		readonly bool constrained_intra_pred_flag, transform_skip_enabled_flag, cu_qp_delta_enabled_flag;
		readonly uint diff_cu_qp_delta_depth;
		readonly int pps_cb_qp_offset, pps_cr_qp_offset;
		readonly bool pps_slice_chroma_qp_offsets_present_flag, weighted_pred_flag, weighted_bipred_flag, transquant_bypass_enabled_flag, tiles_enabled_flag, entropy_coding_sync_enabled_flag;

		readonly uint num_tile_columns, num_tile_rows;
		readonly uint[] column_width, row_height;
		readonly bool loop_filter_across_tiles_enabled_flag;

		readonly bool pps_loop_filter_across_slices_enabled_flag, deblocking_filter_control_present_flag, deblocking_filter_override_enabled_flag, pps_deblocking_filter_disabled_flag;
		readonly int pps_beta_offset_div2, pps_tc_offset_div2;

		public readonly bool pps_scaling_list_data_present_flag;
		public readonly ScalingList scalingList;

		public PictureParameterSet( ReadOnlySpan<byte> span )
		{
			BitReader reader = new BitReader( span );

			// 7.3.2.3.1 General picture parameter set RBSP syntax, page 38
			pps_pic_parameter_set_id = reader.unsignedGolomb();
			pps_seq_parameter_set_id = reader.unsignedGolomb();
			dependent_slice_segments_enabled_flag = reader.readBit();
			output_flag_present_flag = reader.readBit();
			num_extra_slice_header_bits = reader.readByte( 3 );
			sign_data_hiding_enabled_flag = reader.readBit();
			cabac_init_present_flag = reader.readBit();
			num_ref_idx_l0_default_active = reader.unsignedGolomb() + 1;
			num_ref_idx_l1_default_active = reader.unsignedGolomb() + 1;
			init_qp = reader.signedGolomb() + 26;

			constrained_intra_pred_flag = reader.readBit();
			transform_skip_enabled_flag = reader.readBit();
			cu_qp_delta_enabled_flag = reader.readBit();
			if( cu_qp_delta_enabled_flag )
				diff_cu_qp_delta_depth = reader.unsignedGolomb();
			else
				diff_cu_qp_delta_depth = 0; // When not present, the value of diff_cu_qp_delta_depth is inferred to be equal to 0.

			pps_cb_qp_offset = reader.signedGolomb();
			pps_cr_qp_offset = reader.signedGolomb();
			pps_slice_chroma_qp_offsets_present_flag = reader.readBit();
			weighted_pred_flag = reader.readBit();
			weighted_bipred_flag = reader.readBit();
			transquant_bypass_enabled_flag = reader.readBit();
			tiles_enabled_flag = reader.readBit();
			entropy_coding_sync_enabled_flag = reader.readBit();

			num_tile_columns = num_tile_rows = 0;
			column_width = row_height = null;
			loop_filter_across_tiles_enabled_flag = false;
			if( tiles_enabled_flag )
			{
				num_tile_columns = reader.unsignedGolomb() + 1;
				num_tile_rows = reader.unsignedGolomb() + 1;
				bool uniform_spacing_flag = reader.readBit();
				if( !uniform_spacing_flag )
				{
					column_width = new uint[ num_tile_columns - 1 ];
					for( uint i = 0; i < num_tile_columns - 1; i++ )
						column_width[ i ] = reader.unsignedGolomb();

					row_height = new uint[ num_tile_rows - 1 ];
					for( uint i = 0; i < num_tile_rows - 1; i++ )
						row_height[ i ] = reader.unsignedGolomb();
				}
				loop_filter_across_tiles_enabled_flag = reader.readBit();
			}

			pps_loop_filter_across_slices_enabled_flag = reader.readBit();
			deblocking_filter_control_present_flag = reader.readBit();
			deblocking_filter_override_enabled_flag = false;
			pps_deblocking_filter_disabled_flag = false;
			pps_beta_offset_div2 = pps_tc_offset_div2 = 0;
			if( deblocking_filter_control_present_flag )
			{
				deblocking_filter_override_enabled_flag = reader.readBit();
				pps_deblocking_filter_disabled_flag = reader.readBit();
				if( !pps_deblocking_filter_disabled_flag )
				{
					pps_beta_offset_div2 = reader.signedGolomb();
					pps_tc_offset_div2 = reader.signedGolomb();
				}
			}

			pps_scaling_list_data_present_flag = reader.readBit();
			if( pps_scaling_list_data_present_flag )
			{
				scalingList = new ScalingList();
				scalingList.read( ref reader );
			}
			else
				scalingList = null;
		}

		/// <summary>Make a data for PPS register of the hardware decoder.</summary>
		/// <remarks>Apparently, the 2 integers should be taken from slice header, see 7.3.6.1 General slice segment header syntax on page 46.</remarks>
		public uint produceDecoderData( ref SequenceParameterSet sps, int slice_cb_qp_offset, int slice_cr_qp_offset )
		{
			int tmp = (int)sps.log2_ctb_size - (int)diff_cu_qp_delta_depth;
			Debug.Assert( tmp >= 0 && tmp < 16 );

			uint res = (uint)tmp;
			if( cu_qp_delta_enabled_flag )
				res |= 1u << 4;
			if( transquant_bypass_enabled_flag )
				res |= 1u << 5;
			if( transform_skip_enabled_flag )
				res |= 1u << 6;
			if( sign_data_hiding_enabled_flag )
				res |= 1u << 7;

			uint uu = (uint)( ( pps_cb_qp_offset + slice_cb_qp_offset ) & 0xFF );
			res |= uu << 8;
			uu = (uint)( ( pps_cr_qp_offset + slice_cr_qp_offset ) & 0xFF );
			res |= uu << 16;

			if( constrained_intra_pred_flag )
				res |= 1u << 24;

			return res;
		}
	}
}