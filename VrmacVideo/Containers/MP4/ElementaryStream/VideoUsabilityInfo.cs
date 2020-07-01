using System;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.MP4
{
	/// <summary>Not from the spec, a collection of the bits in VUI parameters</summary>
	[Flags]
	public enum eVuiFlags: ushort
	{
		/// <summary>When 0, the SPS doesn't have a VUI</summary>
		VUI = 1,
		/// <summary>aspect_ratio_info_present_flag</summary>
		AspectRatio = 2,
		/// <summary>overscan_info_present_flag</summary>
		OverscanPresent = 4,
		/// <summary>For entertainment television programming, or for a live view of people in a videoconference, should be set;
		/// for computer screen capture or security camera content should be cleared.</summary>
		OverscanAppropriate = 8,

		/// <summary>video_signal_type_present_flag</summary>
		VideoSignal = 0x10,
		VideoSignalFullRange = 0x20,
		VideoSignalColorDesc = 0x40,

		/// <summary>chroma_loc_info_present_flag</summary>
		ChromaLocation = 0x80,
		/// <summary>timing_info_present_flag</summary>
		Timing = 0x100,
		/// <summary>fixed_frame_rate_flag</summary>
		TimingFixedRate = 0x200,
		/// <summary>nal_hrd_parameters_present_flag</summary>
		NalHrd = 0x400,
		/// <summary>vcl_hrd_parameters_present_flag</summary>
		VclHrd = 0x800,
		/// <summary>low_delay_hrd_flag</summary>
		LowDelay = 0x1000,
		/// <summary>pic_struct_present_flag</summary>
		PicStruct = 0x2000,
	}

	/// <summary></summary>
	public struct sSampleAspectRatio
	{
		public readonly ushort width, height;

		internal sSampleAspectRatio( ushort w, ushort h )
		{
			width = w;
			height = h;
		}

		// Table E-1 – Meaning of sample aspect ratio indicator
		public static readonly sSampleAspectRatio[] predefined = new sSampleAspectRatio[]
		{
			new sSampleAspectRatio( 0, 0 ),	// Unspecified
			new sSampleAspectRatio( 1, 1 ),	// Square
			new sSampleAspectRatio( 12, 11 ),
			new sSampleAspectRatio( 10, 11 ),
			new sSampleAspectRatio( 16, 11 ),
			new sSampleAspectRatio( 40, 33 ),
			new sSampleAspectRatio( 24, 11 ),
			new sSampleAspectRatio( 20, 11 ),
			new sSampleAspectRatio( 32, 11 ),
			new sSampleAspectRatio( 80, 33 ),
			new sSampleAspectRatio( 18, 11 ),
			new sSampleAspectRatio( 15, 11 ),
			new sSampleAspectRatio( 64, 33 ),
			new sSampleAspectRatio( 160, 99 ),
		};
	}

	public enum eVideoFormat: byte
	{
		Component = 0,
		PAL = 1,
		NTSC = 2,
		SECAM = 3,
		MAC = 4,
		Unspecified = 5,
	}

	/// <summary>Annex E section 1.2 "HRD parameters syntax"</summary>
	/// <remarks>
	/// The spec says it means "hypothetical reference decoder" which makes zero sense.
	/// A hypothetical decoder model that specifies constraints on the variability of conforming NAL unit streams or conforming byte streams that an encoding process may produce.
	/// </remarks>
	public struct HRD
	{
		public readonly byte bitRateScale;
		public readonly byte cpbSizeScale;

		public struct Entry
		{
			public readonly int BitRate;
			public readonly int CpbSize;
			public readonly bool CbrFlag;
			internal Entry( ref BitReader reader )
			{
				BitRate = (int)reader.unsignedGolomb() + 1; // bit_rate_value_minus1
				CpbSize = (int)reader.unsignedGolomb() + 1; // cpb_size_value_minus1
				CbrFlag = reader.readBit(); // cbr_flag
			}
		}
		public readonly Entry[] entries;

		public readonly byte InitialCpbRemovalDelayLength;
		public readonly byte CpbRemovalDelayLength;
		public readonly byte DpbOutputDelayLength;
		public readonly byte TimeOffsetLength;

		internal HRD( ref BitReader reader )
		{
			int cpbCount = (int)reader.unsignedGolomb() + 1;    // cpb_cnt_minus1
			bitRateScale = (byte)reader.readInt( 4 );   // bit_rate_scale
			cpbSizeScale = (byte)reader.readInt( 4 );   // cpb_size_scale

			entries = new Entry[ cpbCount ];
			for( int i = 0; i < cpbCount; i++ )
				entries[ i ] = new Entry( ref reader );

			InitialCpbRemovalDelayLength = (byte)( reader.readInt( 5 ) + 1 );   // initial_cpb_removal_delay_length_minus1
			CpbRemovalDelayLength = (byte)( reader.readInt( 5 ) + 1 );  // cpb_removal_delay_length_minus1
			DpbOutputDelayLength = (byte)( reader.readInt( 5 ) + 1 );   // dpb_output_delay_length_minus1
			TimeOffsetLength = (byte)( reader.readInt( 5 ) );   // time_offset_length
		}
	}

	// Table E-5 – Matrix coefficients
	public enum eMatrixCoefficients: byte
	{
		/// <summary>ITU-R Recommendation BT.709: KR = 0.2126; KB = 0.0722 </summary>
		BT709 = 1,
		/// <summary></summary>
		Unspecified = 2,
		/// <summary>Federal Communications Commission: KR = 0.30; KB = 0.11 </summary>
		FCC = 4,
		/// <summary>ITU-R Recommendation BT.470-2 System B, G: KR = 0.299; KB = 0.114 </summary>
		BT470 = 5,
		/// <summary>Society of Motion Picture and Television Engineers 170M: KR = 0.299; KB = 0.114 </summary>
		SMPTE170M = 6,
		/// <summary>Society of Motion Picture and Television Engineers 240M (1987): KR = 0.212; KB = 0.087 </summary>
		SMPTE240M = 7,
	}

	/// <summary>ISO/IEC 14496-10, annex E "Video usability information"</summary>
	public struct VideoUsabilityInfo
	{
		public readonly eVuiFlags flags;
		public readonly sSampleAspectRatio sampleAspectRatio;
		public readonly eVideoFormat videoFormat;
		public readonly eColorPrimaries colorPrimaries;
		public readonly eTransferFunc transferCharacteristics;
		public readonly eMatrixCoefficients matrixCoefficients;

		public readonly byte chromaLocationTop, chromaLocationBottom;

		/// <summary>num_units_in_tick</summary>
		public readonly int numUnitsInTick;
		/// <summary>time_scale</summary>
		public readonly int timeScale;

		public readonly HRD hrd;

		internal VideoUsabilityInfo( ref BitReader reader )
		{
			flags = default;
			sampleAspectRatio = default;
			// When the video_format syntax element is not present, video_format value shall be inferred to be equal to 5.
			videoFormat = eVideoFormat.Unspecified;
			colorPrimaries = eColorPrimaries.Unspecified;
			transferCharacteristics = eTransferFunc.Unspecified;
			matrixCoefficients = eMatrixCoefficients.Unspecified;
			numUnitsInTick = timeScale = 0;
			chromaLocationTop = chromaLocationBottom = 0;
			hrd = default;

			if( !reader.readBit() )
				return;

			flags = eVuiFlags.VUI;
			if( reader.readBit() )
			{
				// aspect_ratio_info_present_flag
				flags |= eVuiFlags.AspectRatio;
				byte index = (byte)reader.readInt( 8 );
				if( index < sSampleAspectRatio.predefined.Length )
					sampleAspectRatio = sSampleAspectRatio.predefined[ index ];
				else if( index == 0xFF )
				{
					checked
					{
						ushort w = (ushort)reader.readInt( 16 );
						ushort h = (ushort)reader.readInt( 16 );
						sampleAspectRatio = new sSampleAspectRatio( w, h );
					}
				}
				else
					throw new ArgumentException( $"Unexpected sample aspect ratio index { index }; it must be < 14, or 0xFF for \"Extended_SAR\"" );
			}

			if( reader.readBit() )
			{
				// overscan_info_present_flag
				flags |= eVuiFlags.OverscanPresent;
				if( reader.readBit() )
					flags |= eVuiFlags.OverscanAppropriate;
			}

			if( reader.readBit() )
			{
				// video_signal_type_present_flag
				flags |= eVuiFlags.VideoSignal;
				videoFormat = (eVideoFormat)reader.readInt( 3 );
				if( reader.readBit() )
					flags |= eVuiFlags.VideoSignalFullRange;
				if( reader.readBit() )  // colour_description_present_flag
				{
					flags |= eVuiFlags.VideoSignalColorDesc;
					colorPrimaries = (eColorPrimaries)reader.readInt( 8 );
					transferCharacteristics = (eTransferFunc)reader.readInt( 8 );
					matrixCoefficients = (eMatrixCoefficients)(byte)reader.readInt( 8 );
				}
			}

			if( reader.readBit() )
			{
				// chroma_loc_info_present_flag
				flags |= eVuiFlags.ChromaLocation;
				checked
				{
					chromaLocationTop = (byte)reader.unsignedGolomb();
					chromaLocationBottom = (byte)reader.unsignedGolomb();
					if( chromaLocationTop > 5 || chromaLocationBottom > 5 )
						throw new ArgumentException( "Invalid chroma location values, must be in [ 0 .. 5 ] interval" );
				}
			}

			if( reader.readBit() )
			{
				flags |= eVuiFlags.Timing;
				numUnitsInTick = reader.readInt( 32 );  // 
				timeScale = reader.readInt( 32 );
				if( reader.readBit() )
					flags |= eVuiFlags.TimingFixedRate;
			}

			if( reader.readBit() )
			{
				flags |= eVuiFlags.NalHrd;
				hrd = new HRD( ref reader );
			}
			if( reader.readBit() )
			{
				flags |= eVuiFlags.VclHrd;
				hrd = new HRD( ref reader );
			}
			if( 0 != ( flags & ( eVuiFlags.NalHrd | eVuiFlags.VclHrd ) ) )
				if( reader.readBit() )
					flags |= eVuiFlags.LowDelay;

			if( reader.readBit() )
				flags |= eVuiFlags.PicStruct;
		}

		public bool CpbDpbDelaysPresent => 0 != ( flags & ( eVuiFlags.NalHrd | eVuiFlags.VclHrd ) );
	}
}