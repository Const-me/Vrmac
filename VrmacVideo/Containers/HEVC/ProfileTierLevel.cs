using System;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.HEVC
{
	static class ProfileTierLevel
	{
		[Flags]
		enum eSubLayerFlags: byte
		{
			None = 0,
			ProfilePresent = 1,
			LevelPresent = 2,
		}

		public static void skip( ref BitReader reader, bool profilePresentFlag, int maxNumSubLayers )
		{
			// 7.3.3 Profile, tier and level syntax, page 42
			if( profilePresentFlag )
			{
				byte general_profile_space = reader.readByte( 2 );
				bool general_tier_flag = reader.readBit();
				byte general_profile_idc = reader.readByte( 5 );
				uint general_profile_compatibility_flag = unchecked((uint)reader.readInt( 32 ));
				bool general_progressive_source_flag = reader.readBit();
				bool general_interlaced_source_flag = reader.readBit();
				bool general_non_packed_constraint_flag = reader.readBit();
				bool general_frame_only_constraint_flag = reader.readBit();

				// Lots of stuff, depends on levels
				reader.skipBits( 43 );

				reader.skipBits( 1 );   // general_reserved_zero_bit or general_inbld_flag
			}

			byte general_level_idc = reader.readByte( 8 );

			Span<eSubLayerFlags> subFlags = stackalloc eSubLayerFlags[ maxNumSubLayers - 1 ];
			for( int i = 0; i < maxNumSubLayers - 1; i++ )
			{
				eSubLayerFlags flags = eSubLayerFlags.None;
				if( reader.readBit() )
					flags |= eSubLayerFlags.ProfilePresent;
				if( reader.readBit() )
					flags |= eSubLayerFlags.LevelPresent;
				subFlags[ i ] = flags;
			}
			if( maxNumSubLayers > 1 )
				for( int i = maxNumSubLayers - 1; i < 8; i++ )
					reader.skipBits( 2 );   // reserved_zero_2bits

			for( int i = 0; i < maxNumSubLayers - 1; i++ )
			{
				if( subFlags[ i ].HasFlag( eSubLayerFlags.ProfilePresent ) )
				{
					reader.skipBits( 8 );   // sub_layer_profile_space, sub_layer_tier_flag[ i ], sub_layer_profile_idc[ i ]
					reader.skipBits( 32 );  // sub_layer_profile_compatibility_flag[ i ][ j ]
					reader.skipBits( 4 );  // sub_layer_progressive_source_flag[ i ], sub_layer_interlaced_source_flag[ i ], sub_layer_non_packed_constraint_flag[ i ], sub_layer_frame_only_constraint_flag[ i ]

					// Lots of stuff
					reader.skipBits( 43 );

					reader.skipBits( 1 );   // sub_layer_inbld_flag[ i ] or sub_layer_reserved_zero_bit[ i ]
				}
				if( subFlags[ i ].HasFlag( eSubLayerFlags.LevelPresent ) )
					reader.skipBits( 8 );
			}
		}
	}
}