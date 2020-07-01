using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using VrmacVideo.Containers.MP4;
using VrmacVideo.Containers.MP4.ElementaryStream;
using VrmacVideo.IO;

namespace VrmacVideo.Containers.MKV
{
	sealed class VideoReader: ReaderBase, iVideoTrackReader
	{
		readonly EncodedQueue queue;

		public VideoReader( MkvMediaFile file, TrackEntry track, VideoParams videoParams, EncodedQueue queue ) :
			base( file, track )
		{
			this.queue = queue;
#if DEBUG
			if( null == queue )
				return;
#endif
			// Enqueue SPS and PPS NALUs
			videoParams.enqueueParameters( queue );
		}

		ReaderState readerState = default;

		eNaluAction iVideoTrackReader.writeNextNalu( EncodedBuffer dest )
		{
			if( EOF )
				return eNaluAction.EOF;

			int naluLength;
			Span<byte> naluPayload;
			lock( clusters.syncRoot )
			{
				// Seek to the blob
				seek( ref readerState );

				// Read 4-bytes NALU length. Unlike mpeg4, MKV header never says it's 4 bytes, i.e. just guessing here.
				Span<byte> naluLengthSpan = stackalloc byte[ 4 ];
				read( ref readerState, naluLengthSpan );
				naluLength = BinaryPrimitives.ReadInt32BigEndian( naluLengthSpan );
				Debug.Assert( readerState.bytesLeft >= naluLength );

				// Write NALU start code to mapped memory
				EmulationPrevention.writeStartCode4( dest.span, 0 );

				// Write the payload
				naluPayload = dest.span.Slice( 4, naluLength );
				read( ref readerState, naluPayload );
			}

			dest.setLength( naluLength + 4 );

			// Parse the payload. The shared memory mapped by the driver is both readable and writeable.
			eNaluAction act = setBufferMetadata( dest, naluPayload );

			if( readerState.bytesLeft <= 0 )
				advance();
			return act;
		}

		eNaluAction setBufferMetadata( EncodedBuffer destBuffer, ReadOnlySpan<byte> data )
		{
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
					}
					destBuffer.setTimestamp( timestamp );
					return eNaluAction.Decode;

				case eNaluType.SEI:
					// Logger.logVerbose( "Ignoring SEI NALU" );
					return eNaluAction.Ignore;

				default:
					// Logger.logVerbose( "{0} NALU", naluType );
					// destBuffer.setTimestamp( sampleReader.timestamp );
					return eNaluAction.Decode;
			}
		}

		StreamPosition iTrackReader.findStreamPosition( TimeSpan ts ) => findPosition( ts );

		sealed class SeekPointComparer: IComparer<SeekPoint>
		{
			int IComparer<SeekPoint>.Compare( SeekPoint x, SeekPoint y ) => x.cluster.CompareTo( y.cluster );
		}
		static readonly SeekPointComparer seekPointComparer = new SeekPointComparer();

		MkvSeekPosition clusterStartPosition( int idx )
		{
			var cph = clusters.file.segment.cluster[ idx ];
			TimeSpan ts = clusters.timeScaler.convert( (long)cph.timestamp );
			return new MkvSeekPosition( ts, idx, 0 );
		}

		StreamPosition findKeyFrame( MkvSeekPosition pos )
		{
			SeekPoint search = new SeekPoint( pos.cluster, default );
			int idx = Array.BinarySearch( seekIndex, search, seekPointComparer );
			if( idx >= 0 )
				return clusterStartPosition( pos.cluster );

			idx = ~idx - 1;
			if( idx >= 0 )
				return clusterStartPosition( seekIndex[ idx ].cluster );

			return clusterStartPosition( 0 );
		}

		StreamPosition iTrackReader.findKeyFrame( StreamPosition seekFrame )
		{
			var sp = findKeyFrame( (MkvSeekPosition)seekFrame );
			Logger.logDebug( "findKeyFrame: {0} -> {1}", seekFrame, sp );
			return sp;
		}

		void iTrackReader.seekToSample( StreamPosition index ) => seekMedia( index );
	}
}