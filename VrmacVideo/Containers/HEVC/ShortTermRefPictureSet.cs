using System;

namespace VrmacVideo.Containers.HEVC
{
	struct ShortTermRefPictureSet
	{
		readonly bool inter_ref_pic_set_prediction_flag;
		readonly uint delta_idx;

		internal ShortTermRefPictureSet( ref BitReader reader, uint num_short_term_ref_pic_sets, uint stRpsIdx, uint[] NumDeltaPocs )
		{
			// 7.3.7 Short-term reference picture set syntax, page 50

			// 7.4.8 Short - term reference picture set semantics page 102
			// When inter_ref_pic_set_prediction_flag is not present, it is inferred to be equal to 0.
			inter_ref_pic_set_prediction_flag = false;

			if( 0 != stRpsIdx )
				inter_ref_pic_set_prediction_flag = reader.readBit();

			// When delta_idx_minus1 is not present, it is inferred to be equal to 0.
			delta_idx = 1;

			if( inter_ref_pic_set_prediction_flag )
			{
				if( stRpsIdx == num_short_term_ref_pic_sets )
					delta_idx = reader.unsignedGolomb() + 1;

				int delta_rps_sign = reader.readBit() ? 1 : -1;
				uint abs_delta_rps = reader.unsignedGolomb();
				int RefRpsIdx = (int)stRpsIdx - (int)delta_idx;
				for( uint j = 0; j <= NumDeltaPocs[ RefRpsIdx ]; j++ )
				{
					bool used_by_curr_pic_flag = reader.readBit();
					if( !used_by_curr_pic_flag )
					{
						bool use_delta_flag = reader.readBit();
					}
				}
			}
			else
			{
				uint num_negative_pics = reader.unsignedGolomb();
				uint num_positive_pics = reader.unsignedGolomb();
				for( uint i = 0; i < num_negative_pics; i++ )
				{
					uint delta_poc_s0 = reader.unsignedGolomb() + 1;
					bool used_by_curr_pic_s0 = reader.readBit();
				}
				for( uint i = 0; i < num_positive_pics; i++ )
				{
					uint delta_poc_s1 = reader.unsignedGolomb() + 1;
					bool used_by_curr_pic_s1 = reader.readBit();
				}
				NumDeltaPocs[ (int)stRpsIdx ] = num_negative_pics + num_positive_pics;
			}
		}
	}
}