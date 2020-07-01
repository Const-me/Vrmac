using Vrmac;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.HEVC
{
	// F.7.3.2.1.2 Representation format syntax, rep_format()
	struct RepresentationFormat
	{
		public readonly sDecodedVideoSize decodedVideoSize;
		public readonly byte bit_depth_vps_luma, bit_depth_vps_chroma;

		internal RepresentationFormat( ref BitReader reader, eChromaFormat chromaFormat )
		{
			CSize size = default;
			size.cx = reader.readInt( 16 ); // pic_width_vps_in_luma_samples
			size.cy = reader.readInt( 16 ); // pic_height_vps_in_luma_samples

			bool chroma_and_bit_depth_vps_present_flag = reader.readBit();
			if( chroma_and_bit_depth_vps_present_flag )
			{
				int chroma_format_vps_idc = reader.readInt( 2 );
				if( chroma_format_vps_idc == 3 )
				{
					bool separate_colour_plane_vps_flag = reader.readBit();
				}
				bit_depth_vps_luma = (byte)( reader.readInt( 4 ) + 8 );
				bit_depth_vps_chroma = (byte)( reader.readInt( 4 ) + 8 );
			}
			else
			{
				bit_depth_vps_luma = bit_depth_vps_chroma = 8;
			}

			bool conformance_window_vps_flag = reader.readBit();
			if( conformance_window_vps_flag )
			{
				CRect rc = default;
				rc.left = (int)reader.unsignedGolomb(); // conf_win_vps_left_offset 
				rc.right = size.cx - (int)reader.unsignedGolomb();   // conf_win_vps_right_offset
				rc.top = (int)reader.unsignedGolomb();   // conf_win_vps_top_offset 
				rc.bottom = size.cy - (int)reader.unsignedGolomb();  // conf_win_vps_bottom_offset 
				decodedVideoSize = new sDecodedVideoSize( size, rc, chromaFormat );
			}
			else
			{
				decodedVideoSize = new sDecodedVideoSize( size, new CRect( default, size ), chromaFormat );
			}
		}
	}
}