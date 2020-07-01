using System;
using VrmacVideo.Containers.MP4;
using VrmacVideo.IO;

namespace VrmacVideo.Utils.Readers
{
	/// <summary>Writes samples with 3 bytes in NALU length</summary>
	/// <remarks>Untested because I don’t think I have such videos, but might work.</remarks>
	sealed class VideoSampleReader3: VideoSampleReader
	{
		public VideoSampleReader3( Mp4File mp4 ) : base( mp4 ) { }

		/* protected override int writeFrame( EncodedBuffer destBuffer, int cb )
		{
			// Read the complete sample
			Span<byte> dest = destBuffer.span;
			stream.Read( dest.Slice( 0, cb ) );

			// Produce NALU start codes.
			for( int offset = 0; offset < cb; )
			{
				int naluLength = ( dest[ offset ] << 16 ) | ( dest[ offset + 1 ] << 8 ) | dest[ offset + 2 ];

				dest[ offset ] = 0;
				dest[ offset + 1 ] = 0;
				dest[ offset + 2 ] = 1;

				offset += ( naluLength + 3 );
			}
			return cb;
		} */

		protected override int writeNalu( EncodedBuffer destBuffer, out eNaluAction result, out eNaluType type )
		{
			Span<byte> naluLength = stackalloc byte[ 4 ];
			naluLength[ 0 ] = 0;
			stream.read( naluLength.Slice( 1 ) );
			int cbNalu = BitConverter.ToInt32( naluLength ).endian();

			Span<byte> dest = destBuffer.span;
			// Write NALU start code to mapped memory
			EmulationPrevention.writeStartCode4( dest, 0 );
			// Read NALU payload from file into mapped memory
			Span<byte> naluPayload = dest.Slice( 4, cbNalu );
			stream.read( naluPayload );
			destBuffer.setLength( cbNalu + 4 );
			type = setBufferMetadata( destBuffer, naluPayload, out result );
			return cbNalu + 3;
		}
	}
}