using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Vrmac;

namespace VrmacVideo.Containers.MP4
{
	/// <summary>Parses structure of the MP4 container, as defined in ISO/IEC 14496-12. The parsing is forward-only.</summary>
	public sealed class Mp4Reader: IDisposable
	{
		internal readonly Stream stream;

		public long currentOffset { get; private set; }
		public long currentBoxEnd { get; private set; }
		public long remainingBytes => currentBoxEnd - currentOffset;

		public Mp4Reader( Stream stm )
		{
			stream = stm;
		}

		void readBytes( Span<byte> span )
		{
			int cb = span.Length;
			if( cb != stream.Read( span ) )
				throw new EndOfStreamException();
			currentOffset += cb;
		}
		bool tryReadBytes( Span<byte> span )
		{
			int cb = span.Length;
			if( cb != stream.Read( span ) )
				return false;
			currentOffset += cb;
			return true;
		}

		void skipBytes( int cb )
		{
			stream.skip( cb );
			currentOffset += cb;
		}
		struct sCurrentBox
		{
			public readonly eBoxType type;
			public readonly long endOffset;

			public sCurrentBox( eBoxType type, long endOffset )
			{
				this.type = type;
				this.endOffset = endOffset;
			}
		}

		readonly Stack<sCurrentBox> stack = new Stack<sCurrentBox>();

		public string currentBoxNames => string.Join( " / ", stack.Select( cb => cb.type.print() ).Reverse() );

		bool skipToNextBox()
		{
			long end = stack.Peek().endOffset;
			if( currentOffset + 8 <= end )
			{
				// Current box has enough space. Assuming user doesn't want to skip anything, wants to read from the current position
				return true;
			}
			long nextBoxStart = end;
			skipBytes( checked((int)( nextBoxStart - currentOffset )) );
			stack.Pop();
			return false;
		}

		public eBoxType readBox()
		{
			dbgVerifyOffset();

			if( stack.Count > 0 && !stack.Peek().type.isContainer() )
				skipCurrentBox();

			if( stack.Count > 0 )
			{
				Debug.Assert( stack.Peek().type.isContainer() );
				if( !skipToNextBox() )
					return eBoxType.ChildContainerEnd;
			}

			long startOffset = currentOffset;

			Span<byte> first8bytes = stackalloc byte[ 8 ];
			if( !tryReadBytes( first8bytes ) )
				return eBoxType.Empty;

			ReadOnlySpan<uint> header = first8bytes.cast<uint>();

			// Damn byte order. Pretty much all big endian platforms died decades ago, yet we still suffer from that BS in 2020.
			uint size = BinaryPrimitives.ReverseEndianness( header[ 0 ] );
			eBoxType bt = (eBoxType)header[ 1 ];
			if( size == 0 )
				throw new NotImplementedException( "Boxes without size are valid according to the spec, but not currently supported by this library." );
			if( size != 1 )
				currentBoxEnd = startOffset + size;
			else
			{
				// 64-bit size version
				Span<byte> sizeExt = stackalloc byte[ 8 ];
				readBytes( sizeExt );

				ulong val = sizeExt.cast<ulong>()[ 0 ];
				val = BinaryPrimitives.ReverseEndianness( val );
				currentBoxEnd = startOffset + (long)val;
			}
			stack.Push( new sCurrentBox( bt, currentBoxEnd ) );
			return bt;
		}

		const int gigabyte = ( 1 << 30 );

		public void skipCurrentBox()
		{
			long nextBoxStart = stack.Peek().endOffset;
			if( stream.CanSeek )
			{
				stream.Seek( nextBoxStart, SeekOrigin.Begin );
				currentOffset = nextBoxStart;
				stack.Pop();
				return;
			}

			long cb = nextBoxStart - currentOffset;
			while( cb > gigabyte )
			{
				skipBytes( gigabyte );
				cb -= gigabyte;
			}
			skipBytes( (int)cb );
			stack.Pop();
		}

		public void read( Span<byte> span )
		{
			if( span.Length > remainingBytes )
				throw new EndOfStreamException();
			readBytes( span );
		}

		public void Dispose()
		{
			stream?.Dispose();
		}

		void dbgVerifyOffset()
		{
#if DEBUG
			if( stream.CanSeek )
				Debug.Assert( stream.Position == currentOffset );
#endif
		}

		public eBoxType currentBox => stack.Count > 0 ? stack.Peek().type : eBoxType.Empty;

		public int level => stack.Count;
	}
}