using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.HEVC
{
	struct HrdParam
	{
		public readonly uint vps_num_add_hrd_params;

		public HrdParam( ref BitReader reader, int vps_num_hrd_parameters )
		{
			vps_num_add_hrd_params = reader.unsignedGolomb();
			for( int i = vps_num_hrd_parameters; i < vps_num_hrd_parameters + vps_num_add_hrd_params; i++ )
			{
				bool cprms_add_present_flag = false;
				if( i > 0 )
					cprms_add_present_flag = reader.readBit();
				uint num_sub_layer_hrd_minus1 = reader.unsignedGolomb();
			}
		}
	}
}