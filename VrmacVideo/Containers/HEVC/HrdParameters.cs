using System;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.HEVC
{
	struct HrdParameters
	{
		public HrdParameters( ref BitReader reader, bool commonInfPresentFlag, uint maxNumSubLayers )
		{
			bool nal_hrd_parameters_present_flag = false;
			bool vcl_hrd_parameters_present_flag = false;
			if( commonInfPresentFlag )
			{
				nal_hrd_parameters_present_flag = reader.readBit();
				vcl_hrd_parameters_present_flag = reader.readBit();
				if( nal_hrd_parameters_present_flag || vcl_hrd_parameters_present_flag )
				{
					bool sub_pic_hrd_params_present_flag = reader.readBit();
					if( sub_pic_hrd_params_present_flag )
					{
						ushort tick_divisor = (ushort)( reader.readInt( 8 ) + 2 );
						byte du_cpb_removal_delay_increment_length = (byte)( reader.readInt( 5 ) + 1 );
						bool sub_pic_cpb_params_in_pic_timing_sei_flag = reader.readBit();
						byte dpb_output_delay_du_length = (byte)( reader.readInt( 5 ) + 1 );
					}
					byte bit_rate_scale = reader.readByte( 4 );
					byte cpb_size_scale = reader.readByte( 4 );
					if( sub_pic_hrd_params_present_flag )
					{
						byte cpb_size_du_scale = reader.readByte( 4 );
					}
					byte initial_cpb_removal_delay_length = (byte)( reader.readInt( 5 ) + 1 );
					byte au_cpb_removal_delay_length = (byte)( reader.readInt( 5 ) + 1 );
					byte dpb_output_delay_length = (byte)( reader.readInt( 5 ) + 1 );
				}
			}
			for( uint i = 0; i < maxNumSubLayers; i++ )
			{
				bool fixed_pic_rate_general_flag = reader.readBit();
				bool fixed_pic_rate_within_cvs_flag = false;
				bool low_delay_hrd_flag = false;
				if( fixed_pic_rate_within_cvs_flag )
				{
					uint elemental_duration_in_tc = reader.unsignedGolomb() + 1;
				}
				else
					low_delay_hrd_flag = reader.readBit();
				if( !low_delay_hrd_flag )
				{
					uint cpb_cnt = reader.unsignedGolomb() + 1;
				}
				if( nal_hrd_parameters_present_flag )
					throw new NotImplementedException();
				if( vcl_hrd_parameters_present_flag )
					throw new NotImplementedException();
			}
		}
	}
}