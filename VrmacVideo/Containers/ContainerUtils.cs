using System;
using System.Buffers.Binary;

namespace VrmacVideo.Containers
{
	static class ContainerUtils
	{
		public static byte[][] copyBlobs( int count, ReadOnlySpan<byte> span, ref int readOffset )
		{
			if( count <= 0 )
				return null;
			var result = new byte[ count ][];
			for( int i = 0; i < count; i++ )
			{
				ushort len = BinaryPrimitives.ReadUInt16BigEndian( span.Slice( readOffset ) );
				readOffset += 2;

				result[ i ] = span.Slice( readOffset, len ).ToArray();
				readOffset += len;
			}
			return result;
		}
	}
}