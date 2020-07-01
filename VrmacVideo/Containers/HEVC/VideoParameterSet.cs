using System;
using System.Collections;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.HEVC
{
	public struct VideoParameterSet
	{
		public readonly byte vps_video_parameter_set_id;
		public readonly bool vps_base_layer_internal_flag, vps_base_layer_available_flag;
		public readonly byte vps_max_layers, vps_max_sub_layers;
		public readonly bool vps_temporal_id_nesting_flag, vps_sub_layer_ordering_info_present_flag;

		public struct SubLayer
		{
			public readonly uint vps_max_dec_pic_buffering, vps_max_num_reorder_pics;
			public readonly int vps_max_latency_increase;

			internal SubLayer( ref BitReader reader )
			{
				vps_max_dec_pic_buffering = reader.unsignedGolomb() + 1;    // vps_max_dec_pic_buffering_minus1
				vps_max_num_reorder_pics = reader.unsignedGolomb();
				vps_max_latency_increase = (int)reader.unsignedGolomb() - 1;    // vps_max_latency_increase_plus1
			}
		}
		public readonly SubLayer[] subLayers;
		public readonly byte vps_max_layer_id;
		public readonly uint vps_num_layer_sets;
		public readonly BitArray layer_id_included_flags;

		public readonly bool vps_timing_info_present_flag;
		public readonly uint vps_num_units_in_tick, vps_time_scale;
		public readonly bool vps_poc_proportional_to_timing_flag;
		public readonly uint vps_num_ticks_poc_diff_one;
		public readonly uint vps_num_hrd_parameters;
		public readonly VpsExtension? ext;

		public VideoParameterSet( ReadOnlySpan<byte> span )
		{
			// File.WriteAllBytes( @"C:\Temp\2remove\mkv\vps.bin", span.ToArray() );
			BitReader reader = new BitReader( span );

			// F.7.3.2.1 Video parameter set RBSP
			vps_video_parameter_set_id = reader.readByte( 4 );
			vps_base_layer_internal_flag = reader.readBit();
			vps_base_layer_available_flag = reader.readBit();
			vps_max_layers = (byte)( reader.readInt( 6 ) + 1 ); // vps_max_layers_minus1
			vps_max_sub_layers = (byte)( reader.readInt( 3 ) + 1 ); // vps_max_sub_layers_minus1
			vps_temporal_id_nesting_flag = reader.readBit();
			reader.skipBits( 16 );  // vps_reserved_0xffff_16bits
			vps_sub_layer_ordering_info_present_flag = reader.readBit();

			int iStart = vps_sub_layer_ordering_info_present_flag ? 0 : vps_max_sub_layers - 1;
			if( iStart < vps_max_sub_layers )
			{
				subLayers = new SubLayer[ vps_max_sub_layers - iStart ];
				for( int i = iStart; i < vps_max_sub_layers; i++ )
					subLayers[ i - iStart ] = new SubLayer( ref reader );
			}
			else
				subLayers = null;

			vps_max_layer_id = reader.readByte( 6 );
			vps_num_layer_sets = reader.unsignedGolomb() + 1;
			if( vps_num_layer_sets > 0 )
			{
				layer_id_included_flags = new BitArray( (int)vps_num_layer_sets * ( vps_max_layer_id + 1 ) );
				int k = 0;
				for( int i = 0; i < vps_num_layer_sets; i++ )
					for( int j = 0; j <= vps_max_layer_id; j++, k++ )
						layer_id_included_flags[ k ] = reader.readBit();
			}
			else
				layer_id_included_flags = null;

			vps_timing_info_present_flag = reader.readBit();
			if( vps_timing_info_present_flag )
			{
				vps_num_units_in_tick = unchecked((uint)reader.readInt( 32 ));
				vps_time_scale = unchecked((uint)reader.readInt( 32 ));
				vps_poc_proportional_to_timing_flag = reader.readBit();
				if( vps_poc_proportional_to_timing_flag )
					vps_num_ticks_poc_diff_one = reader.unsignedGolomb() + 1; // vps_num_ticks_poc_diff_one_minus1
				else
					vps_num_ticks_poc_diff_one = 0;
				vps_num_hrd_parameters = reader.unsignedGolomb();
				for( int i = 0; i < vps_num_hrd_parameters; i++ )
				{
					uint hrd_layer_set_idx = reader.unsignedGolomb();
					if( i > 0 )
					{
						reader.skipBits( 1 );   // cprms_present_flag
					}
					var hrd = new HrdParam( ref reader, (int)vps_num_hrd_parameters );
				}
			}
			else
			{
				vps_num_units_in_tick = 0;
				vps_time_scale = 0;
				vps_poc_proportional_to_timing_flag = false;
				vps_num_ticks_poc_diff_one = 0;
				vps_num_hrd_parameters = 0;
			}

			bool vps_extension_flag = reader.readBit();
			if( vps_extension_flag )
			{
				reader.byteAlign();
				ext = new VpsExtension( ref reader, vps_max_layers, vps_base_layer_internal_flag, vps_max_sub_layers );
			}
			else
				ext = null;
		}
	}
}