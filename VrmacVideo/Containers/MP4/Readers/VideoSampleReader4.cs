using System;
using VrmacVideo.Containers.MP4;
using VrmacVideo.IO;

namespace VrmacVideo.Utils.Readers
{
	/// <summary>Writes samples with 3 bytes in NALU length, it replaces NALU lengths in the file with `00 00 00 01` start codes in shared memory.</summary>
	sealed class VideoSampleReader4: VideoSampleReader
	{
		public VideoSampleReader4( Mp4File mp4 ) : base( mp4 ) { }

		/* protected override int writeFrame( EncodedBuffer destBuffer, int cb )
		{
			// Read the complete sample
			Span<byte> dest = destBuffer.span;
			stream.Read( dest.Slice( 0, cb ) );

			// Produce NALU start codes. MP4 files contain AVCC bitstream. Hardware encoders want "Annex B" bitstream.
			// BTW same equally applies to Windows, media foundation decoders also only wants Annex B samples.
			// Moar info: https://stackoverflow.com/a/24890903/126995

			// Fortunately, AVCC in mp4 files already includes emulation prevention bytes, we don't need to emit them here:
			// https://stackoverflow.com/a/30241657/126995
			int naluCount = 0;
			for( int offset = 0; offset < cb; naluCount++ )
			{
				var naluLengthSpan = dest.Slice( offset, 4 ).cast<int>();
				int naluLength = naluLengthSpan[ 0 ].endian();
				naluLengthSpan[ 0 ] = 0x1000000;
				// The length field includes the size of both the one byte NAL header and the EBSP payload but does not include the length field itself.
				offset += ( naluLength + 4 );
			}

			Logger.logVerbose( "Wrote a sample into buffer #{0}; took {1} bytes including start codes, {2} NALUs", destBuffer.index, cb, naluCount );
			return cb;
		} */

		protected override int writeNalu( EncodedBuffer destBuffer, out eNaluAction result, out eNaluType type )
		{
			// Read NALU length from the file into array on the stack
			Span<byte> naluLength = stackalloc byte[ 4 ];
			stream.read( naluLength );
			int cbNalu = BitConverter.ToInt32( naluLength ).endian();

			Span<byte> dest = destBuffer.span;
			// Write NALU start code to mapped memory
			EmulationPrevention.writeStartCode4( dest, 0 );
			// Read NALU payload from file into mapped memory
			Span<byte> naluPayload = dest.Slice( 4, cbNalu );
			stream.read( naluPayload );
			destBuffer.setLength( cbNalu + 4 );
			type = setBufferMetadata( destBuffer, naluPayload, out result );
			return cbNalu + 4;
		}
	}
}