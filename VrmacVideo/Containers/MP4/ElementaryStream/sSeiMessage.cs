using System;
using System.Runtime.InteropServices;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.MP4
{
	/// <summary>ISO/IEC 14496-10, Annex D "SEI payload syntax"</summary>
	enum eSeiType: byte
	{
		BufferingPeriod = 0,
		PicTiming = 1,
		PanScanRect = 2,
		FillerPayload = 3,
		UserDataRegisteredItuTT35 = 4,
		UserDataUnregistered = 5,
		RecoveryPoint = 6,
		DecRefPicMarkingRepetition = 7,
		SparePic = 8,
		SceneInfo = 9,
		SubSeqInfo = 10,
		SubSeqLayerCharacteristics = 11,
		SubSeqCharacteristics = 12,
		FullFrameFreeze = 13,
		FullFrameFreezeRelease = 14,
		FullFrameSnapshot = 15,
		ProgressiveRefinementSegmentStart = 16,
		ProgressiveRefinementSegmentEnd = 17,
		MotionConstrainedSliceGroupSet = 18,
		Other = 19
	}

	[Flags]
	enum eTimingFlags: byte
	{
		CpbDpbDelays = 1,

	}

	/// <summary>ISO/IEC 14496-10, Annex D section 1.2 "Picture timing SEI message syntax"</summary>
	unsafe struct SeiTiming
	{
		/// <summary>Table D-1 "Interpretation of pic_struct"</summary>
		static readonly byte[] timestampsCountTable = new byte[ 9 ]
		{
			1, 1, 1, 2, 2, 3, 3, 2, 3
		};

		static long parseTimestamp( ref BitReader reader, ref TimingFormat timingFormat )
		{
			bool clock_timestamp_flag = reader.readBit();
			if( !clock_timestamp_flag )
				return -1;
			int ct_type = reader.readInt( 2 );
			int nuit_field_based_flag = reader.readInt( 1 );
			int counting_type = reader.readInt( 5 );
			bool full_timestamp_flag = reader.readBit();
			bool discontinuity_flag = reader.readBit();
			bool cnt_dropped_flag = reader.readBit();
			int n_frames = reader.readInt( 8 );
			byte seconds = 0, minutes = 0, hours = 0;
			if( full_timestamp_flag )
			{
				seconds = (byte)reader.readInt( 6 );
				minutes = (byte)reader.readInt( 6 );
				hours = (byte)reader.readInt( 5 );
			}
			else
			{
				if( reader.readBit() )
				{
					// seconds_flag
					seconds = (byte)reader.readInt( 6 );
					if( reader.readBit() )
					{
						// minutes_flag
						minutes = (byte)reader.readInt( 6 );
						if( reader.readBit() )
						{
							// hours_flag
							hours = (byte)reader.readInt( 5 );
						}
					}
				}
			}
			int time_offset = 0;
			if( timingFormat.timeOffsetLength > 0 )
			{
				int tol = timingFormat.timeOffsetLength;
				time_offset = reader.readInt( tol );
				// Make it signed
				if( 0 != ( time_offset & ( 1 << ( tol - 1 ) ) ) )
					time_offset -= ( 1 << tol );
			}
			// D.2.2 Picture timing SEI message semantics
			// clockTimestamp = ( ( hH * 60 + mM ) * 60 + sS ) * time_scale + nFrames * ( num_units_in_tick * ( 1 + nuit_field_based_flag ) ) + tOffset
			int totalSeconds = seconds + minutes * 60 + hours * 3600;
			long result = (long)totalSeconds * timingFormat.timeScale;
			result += n_frames * ( timingFormat.numUnitsInTick * ( nuit_field_based_flag + 1 ) );
			result += time_offset;
			return result;
		}

		internal SeiTiming( ref BitReader reader, ref TimingFormat timingFormat )
		{
			int cpb_removal_delay = 0, dpb_output_delay = 0;
			if( timingFormat.cpbDpbDelaysPresent )
			{
				cpb_removal_delay = reader.readInt( timingFormat.cpbRemovalDelayLength );
				dpb_output_delay = reader.readInt( timingFormat.dpbOutputDelayLength );
			}
			if( timingFormat.picStructPresent )
			{
				int picStruct = reader.readInt( 4 );
				if( picStruct >= timestampsCountTable.Length )
					throw new ArgumentException( $"picStruct value { picStruct } is out of range, should be in [ 0 ..8 ] interval" );
				this.count = timestampsCountTable[ picStruct ];
				int count = this.count;
				for( int i = 0; i < count; i++ )
					timestamps[ i ] = parseTimestamp( ref reader, ref timingFormat );
			}
			else
				count = 0;
		}

		public readonly byte count;
		public fixed long timestamps[ 3 ];

		public override string ToString()
		{
			switch( count )
			{
				case 0:
					return "No timestamps";
				case 1:
					return $"1 timestamp: { timestamps[ 0 ] }";
				case 2:
					return $"2 timestamps: { timestamps[ 0 ] }, { timestamps[ 1 ] }";
				case 3:
					return $"3 timestamps: { timestamps[ 0 ] }, { timestamps[ 1 ] }, { timestamps[ 2 ] }";
			}
			throw new ApplicationException();
		}
	}

	struct sSeiMessage
	{
		readonly uint m_type;

		static eSeiType getType( uint t ) => ( t <= 18 ) ? (eSeiType)t : eSeiType.Other;
		public eSeiType type => getType( m_type );

		public readonly int payloadSize;

		[StructLayout( LayoutKind.Explicit )]
		public struct Union
		{
			[FieldOffset( 0 )] public SeiTiming timing;
		}
		Union u;

		// Another incompatible version of variable-length integers encodings. I wonder how many versions are in the complete h264 standard.
		static int readInt( ref BitReader reader )
		{
			int res = 0;
			while( true )
			{
				byte b = (byte)reader.readInt( 8 );
				res += b;
				if( b != 0xFF )
					return res;
			}
		}

		internal sSeiMessage( ref BitReader reader, ref TimingFormat timingFormat )
		{
			uint tp = (uint)readInt( ref reader );
			m_type = tp;
			u = default;

			payloadSize = readInt( ref reader );
			if( getType( tp ) == eSeiType.PicTiming )
				u.timing = new SeiTiming( ref reader, ref timingFormat );
		}

		public override string ToString()
		{
			switch( type )
			{
				case eSeiType.PicTiming:
					return $"{ type }, { u.timing }";
				default:
					return $"{ type }, { payloadSize } bytes payload";
			}
		}
	}
}