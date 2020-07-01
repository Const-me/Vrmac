using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	struct LacedFrame
	{
		/// <summary>Offset in the buffer, or if the payload was left in the file, offset from the start of the blob.</summary>
		public readonly int position;
		/// <summary>Length in bytes</summary>
		public readonly int length;

		public LacedFrame( int position, int length )
		{
			this.position = position;
			this.length = length;
		}

		public override string ToString() => $"position { position }, length { length }";
	}

	enum eLacingResult: byte
	{
		/// <summary>The blob uses no lacing</summary>
		NoLacing,

		/// <summary>The blob uses lacing, and the combined size of all frames in the blob was small enough to buffer the complete blob in memory</summary>
		/// <remarks>Happens for DTS audio. Even with 5.1 audio, a frame only takes 1006 bytes (a blob has 8 of them).
		/// 512 samples = 10.66ms = frames are played at 93.75Hz. That's too often to spam I/O kernel calls at that frequency, 8kb reads are way more reasonable.</remarks>
		Buffered,

		/// <summary>The blob uses lacing, but the combined size of all frames in the blob was too large to buffer the blob.
		/// Lacing.unpackFrames has only read the header from the file, the blob payload is left in the file.</summary>
		Large,
	}

	struct LacedFrames
	{
		public eLacingResult lacing;
		int index;

		List<LacedFrame> frames;
		public byte[] buffer;

		public bool advance()
		{
			index++;
			return index < frames.Count;
		}

		public void rewind() => index = 0;

		public LacedFrame currentFrame => frames[ index ];

		public void initialize( int count )
		{
			index = 0;
			if( null != frames )
				frames.Clear();
			else
				frames = new List<LacedFrame>( count );
		}

		public void add( int position, int length ) =>
			frames.Add( new LacedFrame( position, length ) );
	}

	static class Lacing
	{
		const int maxBuffer = 1024 * 64;

		static readonly int[] rangeShiftConstants = new int[ 4 ]
		{
			0x3F,   // 1 byte only, the offset is 2^6-1
			0x1FFF,   // 2 bytes, the offset is 2^13-1
			0xFFFFF,   // 3 bytes, the offset is 2^20-1
			0x7FFFFFF   // 4 bytes, the offset is 2^27-1
		};

		static int rangeShiftSignedInt( int difference, int cb )
		{
			if( cb > 0 && cb < 5 )
				return difference - rangeShiftConstants[ cb - 1 ];
			throw new ArgumentOutOfRangeException();
		}

		static void unpackEbmlFromBuffer( int lacedBlocksCount, ReadOnlySpan<byte> buffer, ref LacedFrames laced )
		{
			// The count is stored in 1 byte, the absolute max.limit is 256, totally fine for the stack.
			Span<int> sizes = stackalloc int[ lacedBlocksCount ];
			int lacingHeaderBytes = 1; // The 1 byte is lacedBlocksCount we have already consumed from that buffer
			int cb;
			int prevSize = checked((int)buffer.parseUint4( lacingHeaderBytes, out cb ));
			lacingHeaderBytes += cb;
			sizes[ 0 ] = prevSize;
			int combinedSize = prevSize;
			for( int i = 1; i < lacedBlocksCount - 1; i++ )
			{
				int difference = checked((int)buffer.parseUint4( lacingHeaderBytes, out cb ));
				lacingHeaderBytes += cb;
				prevSize += rangeShiftSignedInt( difference, cb );

				sizes[ i ] = prevSize;
				combinedSize += prevSize;
			}
			// The size of the last frame is deduced from the total size of the Block.
			sizes[ lacedBlocksCount - 1 ] = buffer.Length - lacingHeaderBytes - combinedSize;

			int pos = lacingHeaderBytes;
			for( int i = 0; i < lacedBlocksCount; i++ )
			{
				laced.add( pos, sizes[ i ] );
				pos += sizes[ i ];
			}
		}

		static void unpackEbmlFromStream( int lacedBlocksCount, Stream stream, int blobSize, ref LacedFrames laced )
		{
			Span<int> sizes = stackalloc int[ lacedBlocksCount ];
			int lacingHeaderBytes = 1; // The 1 byte is lacedBlocksCount we have already read
			int cb;
			int prevSize = checked((int)stream.readUint4( out cb ));
			lacingHeaderBytes += cb;
			sizes[ 0 ] = prevSize;
			int combinedSize = prevSize;
			for( int i = 1; i < lacedBlocksCount - 1; i++ )
			{
				int difference = checked((int)stream.readUint4( out cb ));
				lacingHeaderBytes += cb;
				prevSize += rangeShiftSignedInt( difference, cb );

				sizes[ i ] = prevSize;
				combinedSize += prevSize;
			}
			// The size of the last frame is deduced from the total size of the Block.
			sizes[ lacedBlocksCount - 1 ] = blobSize - lacingHeaderBytes - combinedSize;

			int pos = lacingHeaderBytes;
			for( int i = 0; i < lacedBlocksCount; i++ )
			{
				laced.add( pos, sizes[ i ] );
				pos += sizes[ i ];
			}
		}

		public static void unpackFrames( this Stream stream, ref Blob blob, ref LacedFrames laced )
		{
			eBlockFlags lacing = blob.lacing;

			if( lacing == eBlockFlags.NoLacing )
			{
				// laced.frames?.Clear();
				laced.lacing = eLacingResult.NoLacing;
				return;
			}

			stream.Seek( blob.position, SeekOrigin.Begin );

			int lacingHeader;
			if( blob.length <= maxBuffer )
			{
				if( null != laced.buffer )
				{
					if( laced.buffer.Length < blob.length )
						laced.buffer = new byte[ blob.length ];
				}
				else
					laced.buffer = new byte[ blob.length ];

				stream.read( laced.buffer.AsSpan().Slice( 0, blob.length ) );
				lacingHeader = laced.buffer[ 0 ];
				laced.lacing = eLacingResult.Buffered;
			}
			else
			{
				lacingHeader = stream.ReadByte();
				if( lacingHeader < 0 )
					throw new EndOfStreamException();
				laced.lacing = eLacingResult.Large;
			}

			int lacedBlocksCount = lacingHeader + 1;
			laced.initialize( lacedBlocksCount );

			switch( lacing )
			{
				case eBlockFlags.FixedSize:
					int cbPayload = blob.length - 1;
					if( 0 != cbPayload % lacedBlocksCount )
						throw new ArgumentException( $"Error in the MKV file, blob header specifies fixed size lacing with { lacedBlocksCount } laced frames, yet the block payload size { cbPayload } ain’t divisible" );
					int cbFrame = cbPayload / lacedBlocksCount;
					for( int i = 0; i < lacedBlocksCount; i++ )
						laced.add( 1 + cbFrame * i, cbFrame );
					return;
				case eBlockFlags.EBML:
					if( laced.lacing == eLacingResult.Buffered )
						unpackEbmlFromBuffer( lacedBlocksCount, laced.buffer.AsSpan().Slice( 0, blob.length ), ref laced );
					else
						unpackEbmlFromStream( lacedBlocksCount, stream, blob.length, ref laced );
					break;
				default:
					throw new NotImplementedException( $"{ lacing } lacing is not implemented" );
			}
		}
	}
}