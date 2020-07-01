using System;
using System.Diagnostics;
using System.IO;
using VrmacVideo.Containers.MP4;
using VrmacVideo.IO;
using VrmacVideo.Utils.Readers;

namespace VrmacVideo
{
	public enum eNaluAction: byte
	{
		Decode,
		Ignore,
		EOF,
	}

	/// <summary>Reads NALUs from mp4, writes them to V4L2 buffers</summary>
	abstract class VideoSampleReader: iVideoTrackReader
	{
		protected readonly Stream stream;
		readonly SampleReader sampleReader;

		public VideoSampleReader( Mp4File mp4 )
		{
			stream = mp4.reader.stream;
			sampleReader = new SampleReader( mp4.videoTrack );
		}

		public static VideoSampleReader create( Mp4File mp4 )
		{
			TrackMetadata track = mp4.videoTrack;
			if( null == track )
				throw new ApplicationException( "The MP4 doesn’t have a video track" );

			VideoSampleEntry vse = mp4.videoTrackSample;
			switch( vse.naluLengthSize )
			{
				case 4:
					return new VideoSampleReader4( mp4 );
				case 3:
					return new VideoSampleReader3( mp4 );
			}
			// The specs also defines 1 and 2 bytes versions.
			// Slightly harder to handle as we need to expand the samples, 3 bytes is the minimum length of NALU start code in Annex B bitstream.
			throw new NotImplementedException();
		}

		/* bool writeNextFrame( EncodedBuffer dest )
		{
			Debug.Assert( dest.state == eBufferState.User );
			if( sampleReader.EOF )
				return false;

			// Write the sample
			int cb = sampleReader.seek( stream );
			cb = writeFrame( dest, cb );
			dest.setLength( cb );

			// Logger.logVerbose( "Wrote a frame into buffer #{0}; took {1} bytes including start codes", dest.index, cb );

			// Advance the state
			sampleReader.advance();
			return true;
		} */

		struct ReaderState
		{
			public int sampleSize;
			public int currentOffset;
		};
		ReaderState state;

		eNaluAction iVideoTrackReader.writeNextNalu( EncodedBuffer dest )
		{
			Debug.Assert( dest.state == eBufferState.User );
			if( sampleReader.EOF )
				return eNaluAction.EOF;

			eNaluAction result;
			if( state.currentOffset >= state.sampleSize )
			{
				// First NALU in the sample
				state.sampleSize = sampleReader.seek( stream );
				state.currentOffset = writeNalu( dest, out result, out var type );
				// Logger.logVerbose( "Wrote a first NALU of a sample into buffer #{0}; sample size {1}, NALU size {2}, type {3}", dest.index, state.sampleSize, state.currentOffset, type );
			}
			else
			{
				// Some other NALU from the same sample
				sampleReader.seekToNalu( stream, state.currentOffset );
				int cb = writeNalu( dest, out result, out var type );
				state.currentOffset += cb;
				// Logger.logVerbose( "Wrote a NALU of a sample into buffer #{0}; NALU size {1}, type {2}", dest.index, cb, type );
			}

			if( state.currentOffset >= state.sampleSize )
			{
				// No more NALUs left in the sample, advance sample reader to the next one. This may set EOF flag, when the music is over.
				sampleReader.advance();
				state.sampleSize = 0;
			}
			return result;
		}

		/// <summary>Write complete sample to the buffer.</summary>
		/// <remarks>This requires <see cref="Linux.eImageFormatDescriptionFlags.ContinuousByteStream" /> capability flag for the h264 format.
		/// Needless to say, Pi4 hardware decoder ain’t supporting that flag.</remarks>
		// protected abstract int writeFrame( EncodedBuffer dest, int cb );

		/// <summary>Write a single NALU to the buffer, including start code. Returns count of bytes read from the file.</summary>
		protected abstract int writeNalu( EncodedBuffer dest, out eNaluAction result, out eNaluType type );

		protected bool separateColourPlaneFlag { get; private set; }
		protected byte frameIndexBits { get; private set; }

		TimingFormat timingFormat;

		public void setSps( ref SequenceParameterSet sps )
		{
			separateColourPlaneFlag = sps.separateColourPlaneFlag;
			frameIndexBits = sps.frameIndexBits;

			timingFormat = new TimingFormat( ref sps );

			Logger.logInfo( "VideoSampleReader.setSps: separateColourPlaneFlag = {0}, frameIndexBits = {1}",
				separateColourPlaneFlag, frameIndexBits );
		}

		protected eNaluType setBufferMetadata( EncodedBuffer destBuffer, ReadOnlySpan<byte> data, out eNaluAction result )
		{
			// NALU header
			eNaluType naluType = (eNaluType)( data[ 0 ] & 0x1f );
			BitReader reader = new BitReader( data );
			reader.skipBits( 8 );

			switch( naluType )
			{
				case eNaluType.IDR:
				case eNaluType.NonIDR:
					{
						uint first_mb_in_slice = reader.unsignedGolomb();   // address of the first macroblock in the slice
						uint slice_type = reader.unsignedGolomb();
						if( slice_type > 9 )
							throw new ArgumentException( "Malformed h264 stream, slice_type must be in [ 0 .. 9 ] interval" );
						destBuffer.setSliceTypeFlag( slice_type );

						uint pic_parameter_set_id = reader.unsignedGolomb();
						if( separateColourPlaneFlag )
							reader.skipBits( 2 );   // uint colour_plane_id
						int frameIndex = reader.readInt( frameIndexBits );
						// Logger.logVerbose( "{0} NALU: slice type {1}, frame index {2}", naluType, (eSliceType)slice_type, frameIndex );
						// if( naluType != eNaluType.NonIDR )
						//	Logger.logVerbose( "{0} NALU: slice type {1}, frame index {2}", naluType, (eSliceType)slice_type, frameIndex );
					}
					destBuffer.setTimestamp( sampleReader.timestamp );
					result = eNaluAction.Decode;
					return naluType;
				case eNaluType.SEI:
					{
						sSeiMessage sei = new sSeiMessage( ref reader, ref timingFormat );
						if( sei.type == eSeiType.PicTiming )
						{
							// ISO/IEC 14496-15 section 5.2.2:
							// All timing information is external to stream.
							// Timing provided within the AVC stream in this file format should be ignored as it may contradict the timing provided by the file format and may not be correct or consistent within itself.
							result = eNaluAction.Ignore;
						}
						else
						{
							// Logger.logVerbose( "{0} NALU: {1}", naluType, sei );
							// destBuffer.setTimestamp( sampleReader.timestamp );
							result = eNaluAction.Decode;
						}
						return naluType;
					}
				default:
					// Logger.logVerbose( "{0} NALU", naluType );
					// destBuffer.setTimestamp( sampleReader.timestamp );
					result = eNaluAction.Decode;
					return naluType;
			}
		}

		StreamPosition iTrackReader.findStreamPosition( TimeSpan ts ) => sampleReader.findStreamPosition( ts );
		StreamPosition iTrackReader.findKeyFrame( StreamPosition seekFrame ) => sampleReader.findKeyFrame( (Mp4StreamPosition)seekFrame );
		void iTrackReader.seekToSample( StreamPosition index )
		{
			Mp4StreamPosition pos = (Mp4StreamPosition)index;
			sampleReader.seekToSample( pos.sample );
			state = default;
		}
	}
}