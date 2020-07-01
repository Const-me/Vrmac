using System;
using VrmacVideo.Containers.MP4;
using VrmacVideo.Linux;

namespace VrmacVideo.IO
{
	static class EncodedBufferExt
	{
		static readonly eBufferFlags[] sliceTypeFlags = new eBufferFlags[ 10 ]
		{
			eBufferFlags.PFrame,
			eBufferFlags.BFrame,
			eBufferFlags.KeyFrame,
			0,
			0,
			eBufferFlags.PFrame,
			eBufferFlags.BFrame,
			eBufferFlags.KeyFrame,
			0,
			0,
		};

		public static void setSliceTypeFlag( this EncodedBuffer destBuffer, uint slice_type )
		{
			eBufferFlags flags = sliceTypeFlags[ slice_type ];
			destBuffer.setFlags( flags );
		}

		static void writeParameters( this EncodedBuffer buffer, byte[] source, string what )
		{
			var span = buffer.span;
			int pos = EmulationPrevention.writeStartCode4( span, 0 );
			// pos = EmulationPrevention.writeBytes( span, source.AsSpan(), pos );
			source.AsSpan().CopyTo( span.Slice( pos ) );
			pos += source.Length;
			buffer.setLength( pos );
			Logger.logVerbose( "Wrote {0} into buffer #{1}; took {2} bytes including start code", what, buffer.index, pos );
		}

		public static void writeSps( this EncodedBuffer buffer, byte[] source )
		{
			if( MiscUtils.getNaluType( source[ 0 ] ) != eNaluType.SPS )
				throw new ApplicationException( "The SPS is invalid, wrong NALU type" );
			buffer.writeParameters( source, "SPS" );
		}

		public static void writePps( this EncodedBuffer buffer, byte[] source )
		{
			if( MiscUtils.getNaluType( source[ 0 ] ) != eNaluType.PPS )
				throw new ApplicationException( "The PPS is invalid, wrong NALU type" );
			buffer.writeParameters( source, "PPS" );
		}
	}
}