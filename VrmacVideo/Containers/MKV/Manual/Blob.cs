using System;
using System.Buffers.Binary;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Block Header Flags</summary>
	/// <seealso href="https://www.matroska.org/technical/basics.html#block-structure" />
	[Flags]
	public enum eBlockFlags: byte
	{
		/// <summary>The block contains only key frames</summary>
		Keyframe = 0x80,
		/// <summary>Invisible, the codec should decode this frame but not display it</summary>
		Invisible = 0b1000,

		/// <summary>Bitmask to get lacing value from the flags</summary>
		LacingMask = 0b110,
		NoLacing = 0,
		Xiph = 0b010,
		FixedSize = 0b100,
		EBML = 0b110,

		/// <summary>The frames of the Block can be discarded during playing if needed</summary>
		Discardable = 1,
	}

	public struct Blob
	{
		// Offset from start of the file to start of the payload; the header is excluded.
		public readonly long position;
		// Size of the payload of the blob in bytes; the header is excluded but it may include lacing header
		public readonly int length;

		/// <summary>Track Number (Track Entry)</summary>
		/// <remarks>THe MKV spec says "up to 8 bytes" however in practice these are very small numbers. Using 1 byte instead, this way this complete structure packs in 16 bytes.</remarks>
		public readonly byte trackNumber;
		/// <summary>Block Header Flags</summary>
		public readonly eBlockFlags flags;
		/// <summary>Timestamp relative to Cluster timestamp</summary>
		public readonly short timestamp;

		Blob( long len, Stream stream )
		{
			if( len < 0 )
				throw new ArgumentOutOfRangeException();
			long startPosition = stream.Position;

			trackNumber = checked((byte)stream.readUint4());
			Span<byte> buffer = stackalloc byte[ 3 ];
			stream.read( buffer );
			timestamp = BinaryPrimitives.ReadInt16BigEndian( buffer );
			flags = (eBlockFlags)buffer[ 2 ];

			long currentPosition = stream.Position;
			position = currentPosition;
			length = checked((int)( len - ( currentPosition - startPosition ) ));
		}

		public void seek( Stream stream )
		{
			stream.Seek( position, SeekOrigin.Begin );
		}

		internal static Blob read( ElementReader reader )
		{
			long len = checked((long)reader.stream.readUint8());
			Blob blob = new Blob( len, reader.stream );
			reader.stream.Seek( blob.position + blob.length, SeekOrigin.Begin );
			return blob;
		}

		public eBlockFlags lacing => flags & eBlockFlags.LacingMask;

		public override string ToString() => $"position { position }, length { length }, trackNumber { trackNumber }, timestamp { timestamp }, flags { flags & ~eBlockFlags.LacingMask }, lacing { flags & eBlockFlags.LacingMask }";
	}
}