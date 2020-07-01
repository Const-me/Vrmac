using System;
using System.Buffers.Binary;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.HEVC
{
	public struct VpsExtension
	{
		// internal readonly ProfileTierLevel? tierLevel;


		static int NumberOfSetBits( int i )
		{
			i = i - ( ( i >> 1 ) & 0x55555555 );
			i = ( i & 0x33333333 ) + ( ( i >> 2 ) & 0x33333333 );
			return ( ( ( i + ( i >> 4 ) ) & 0x0F0F0F0F ) * 0x01010101 ) >> 24;
		}

		internal VpsExtension( ref BitReader reader, byte vps_max_layers, bool vps_base_layer_internal_flag, byte vps_max_sub_layers )
		{
			// F.7.3.2.1.1 Video parameter set extension syntax, page 444
			// F.7.4.3.1.1 Video parameter set extension semantics, page 464

			if( vps_max_layers > 1 && vps_base_layer_internal_flag )
			{
				ProfileTierLevel.skip( ref reader, false, vps_max_sub_layers );
			}
			bool splitting_flag = reader.readBit();

			int NumScalabilityTypes = NumberOfSetBits( reader.readInt( 16 ) );  // scalability_mask_flag[ i ]
			Span<byte> dimension_id_len = stackalloc byte[ NumScalabilityTypes ];
			for( int j = 0; j < ( NumScalabilityTypes - ( ( splitting_flag ) ? 1 : 0 ) ); j++ )
			{
				dimension_id_len[ j ] = (byte)( reader.readInt( 3 ) + 1 );	// dimension_id_len_minus1[ j ]
			}

			bool vps_nuh_layer_id_present_flag = reader.readBit();
			for( int i = 1; i < vps_max_layers; i++ )
			{
				if( vps_nuh_layer_id_present_flag )
					reader.skipBits( 6 );   // layer_id_in_nuh[ i ]
				if( !splitting_flag )
				{
					for( int j = 0; j < NumScalabilityTypes; j++ )
						reader.skipBits( dimension_id_len[ j ] );   // dimension_id[ i ][ j ]
				}
			}
			byte view_id_len = reader.readByte( 4 );

			/* int NumViews = 1;
			for( int i = 1; i < vps_max_layers; i++ )
			{
				bool newViewFlag = true;
				for( int j = 0; j < i; j++ )
				{
					if( ViewOrderIdx[ lId ] = ViewOrderIdx[ layer_id_in_nuh[ j ] ] ) newViewFlag = 0
				}
			}

			if( view_id_len > 0 )
			{
				for( int i = 0; i < NumViews; i++ )
				{

				}
			} */
		}
	}
}