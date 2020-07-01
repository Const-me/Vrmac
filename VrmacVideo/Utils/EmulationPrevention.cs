using System;

namespace VrmacVideo
{
	static class EmulationPrevention
	{
		/// <summary>Search the stream for forbidden sequences which need an emulation prevention byte</summary>
		struct SearchForbidden
		{
			uint val;
			public SearchForbidden( bool unused )
			{
				val = uint.MaxValue;
			}

			// Add a byte, return true if the last 4 bytes, including the new one, form a sequence 00 00 00 00, 00 00 00 01, 00 00 00 02 or 00 00 00 03
			// https://yumichan.net/video-processing/video-compression/introduction-to-h264-nal-unit/
			public bool addByte( byte b )
			{
				val = val << 8;
				val |= b;
				return val <= 3;
			}
		}

		/// <summary>Write 3-bytes start code</summary>
		public static int writeStartCode( Span<byte> dest, int idx )
		{
			dest[ idx ] = 0;
			dest[ idx + 1 ] = 0;
			dest[ idx + 2 ] = 1;
			return idx + 3;
		}

		/// <summary>Write 4-bytes start code</summary>
		public static int writeStartCode4( Span<byte> dest, int idx )
		{
			dest[ idx ] = 0;
			dest[ idx + 1 ] = 0;
			dest[ idx + 2 ] = 0;
			dest[ idx + 3 ] = 1;
			return idx + 4;
		}

		/// <summary>Write bytes, inserting emulation prevention bytes as needed</summary>
		public static int writeBytes( Span<byte> dest, ReadOnlySpan<byte> src, int index )
		{
			SearchForbidden sf = new SearchForbidden( false );
			foreach( byte b in src )
			{
				if( !sf.addByte( b ) )
					dest[ index++ ] = b;
				else
				{
					dest[ index++ ] = 3;
					dest[ index++ ] = b;
				}
			}
			return index;
		}
	}
}