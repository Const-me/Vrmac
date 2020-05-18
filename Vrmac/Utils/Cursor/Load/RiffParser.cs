using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac.Utils.Cursor.Load
{
	/// <summary>Interface to consume chunks from RIFF files.</summary>
	/// <remarks>RIFF and LIST chunks are consumed internally by the parser, you won't see them in this interface. You'll only get the payload data here.</remarks>
	public interface iRiffChunk
	{
		/// <summary>ID of the current chunk</summary>
		uint tag { get; }
		/// <summary>ID of the parent one</summary>
		uint parentTag { get; }

		/// <summary>Offset from the start of the file to the start of the current chunk’s payload</summary>
		int chunkOffset { get; }
		/// <summary>Length of the current chunk’s payload</summary>
		int length { get; }
		/// <summary>Index of this chunk within the parent container</summary>
		int index { get; }

		/// <summary>Current read offset from the start of the RIFF, increases as you read data</summary>
		int currentOffset { get; }
		/// <summary>Count of bytes still available in the current chunk, decreases as you read data</summary>
		int bytesLeft { get; }

		/// <summary>Read bytes from the current chunk</summary>
		void read( Span<byte> dest );

		/// <summary>Skip bytes within current chunk</summary>
		void skip( int cb );
	}

	/// <summary>Parser for RIFF files.</summary>
	/// <remarks>Quite generic one, will probably work for *.wave files and the rest of them, as long as they’re less than 2GB.</remarks>
	/// <seealso href="https://en.wikipedia.org/wiki/Resource_Interchange_File_Format" />
	static class RiffParser
	{
		[StructLayout( LayoutKind.Sequential )]
		struct ListChunk
		{
			public uint tag;
			public int size;
			public uint contentTag;
		}

		[StructLayout( LayoutKind.Sequential )]
		struct Chunk
		{
			public uint tag;
			public int size;
		}

		const uint RIFF = 0x46464952;
		const uint LIST = 0x5453494C;

		public static string decodeTag( uint tag )
		{
			byte[] ascii = BitConverter.GetBytes( tag );
			return System.Text.Encoding.ASCII.GetString( ascii );
		}

		class RiffContext: iRiffChunk
		{
			readonly Stream stream;
			readonly Action<iRiffChunk> consume;

			public RiffContext( Stream stream, Action<iRiffChunk> consume, ListChunk root )
			{
				this.stream = stream;
				this.consume = consume;

				currentOffset = 12;
				pushList( root.contentTag, root.size );
			}

			uint iRiffChunk.tag => current.id;
			public uint parentTag { get; private set; }
			public int chunkOffset => current.offset;
			public int length => current.length;
			public int index => current.index;
			public int currentOffset { get; private set; }
			public int bytesLeft => current.offset + current.length - currentOffset;

			struct State
			{
				public uint id;
				public int index;
				public int offset, length;
			}

			readonly Stack<State> stack = new Stack<State>();
			State current;

			void push( uint newId, int newLength )
			{
				parentTag = current.id;
				stack.Push( current );
				current.id = newId;
				current.offset = currentOffset;
				current.length = newLength;
			}

			void pushList( uint newId, int newLength )
			{
				// Apparently, the list tag field counts as payload.
				push( newId, newLength - 4 );
				current.index = 0;
			}

			bool pop()
			{
				// Read current chunk to the end. User may or may not do that, and even if they did, there's padding there.
				int lengthPadded = ( current.length + 1 ) & ~1;
				int cbSkip = current.offset + lengthPadded - currentOffset;
				stream.skip( cbSkip );
				currentOffset += cbSkip;

				// Pop from the stack
				current = stack.Pop();
				current.index++;

				if( stack.TryPeek( out State parent ) )
				{
					parentTag = parent.id;
					return false;
				}

				// Popped the root RIFF chunk, this is the end of the RIFF.
				return true;
			}

			public void parse()
			{
				while( true )
				{
#if DEBUG
					if( stream.CanSeek )
						Debug.Assert( stream.Position == currentOffset );
#endif
					if( bytesLeft <= 0 )
					{
						if( pop() )
							return;
						continue;
					}

					var chunk = stream.read<Chunk>();
					currentOffset += 8;

					if( chunk.tag == LIST )
					{
						uint listId = stream.read<uint>();
						currentOffset += 4;
						pushList( listId, chunk.size );
					}
					else
					{
						push( chunk.tag, chunk.size );
						consume( this );
						pop();
					}
				}
			}

			void iRiffChunk.read( Span<byte> span )
			{
				if( span.Length > bytesLeft )
					throw new EndOfStreamException();
				if( span.Length != stream.Read( span ) )
					throw new EndOfStreamException();
				currentOffset += span.Length;
			}

			void iRiffChunk.skip( int cb )
			{
				if( cb > bytesLeft )
					throw new EndOfStreamException();
				stream.skip( cb );
				currentOffset += cb;
			}

			public override string ToString()
			{
				string pt = decodeTag( parentTag );
				string ct = decodeTag( current.id );
				string begin = chunkOffset.ToString( "x" );
				string end = ( chunkOffset + length ).ToString( "x" );
				return $"{ pt } / { ct } [ { index } ]: { length } bytes, 0x{ begin } - 0x{ end }";
			}
		}

		/// <summary>Parse a RIFF file</summary>
		public static void parse( Stream stream, Action<iRiffChunk> consumeChunk )
		{
			ListChunk list = stream.read<ListChunk>();
			if( list.tag != RIFF )
				throw new ArgumentException( "The stream is not a RIFF file" );
			if( list.size < 0 )
				throw new NotImplementedException( "RIFF parser only works for files less than 2GB" );
			if( list.size == 0 )
				throw new ArgumentException( "The RIFF has no data" );

			RiffContext context = new RiffContext( stream, consumeChunk, list );
			context.parse();
		}

		/// <summary>Read a structure from RIFF chunk</summary>
		public static T read<T>( this iRiffChunk chunk ) where T : unmanaged
		{
			T result = new T();
			chunk.read( MiscUtils.asSpan( ref result ) );
			return result;
		}

		/// <summary>Read an array of structures from RIFF chunk</summary>
		public static void read<T>( this iRiffChunk chunk, T[] arr ) where T : unmanaged
		{
			var span = MemoryMarshal.Cast<T, byte>( arr.AsSpan() );
			chunk.read( span );
		}
	}
}