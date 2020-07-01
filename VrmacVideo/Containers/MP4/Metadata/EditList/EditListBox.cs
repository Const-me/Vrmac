using System;
using System.Buffers.Binary;
using System.Diagnostics;

namespace VrmacVideo.Containers.MP4.EditList
{
	sealed class EditListBox
	{
		public readonly Array entries;
		public readonly int mediaRate;
		const double dmlMul = 1.0 / 0x10000;
		public double mediaRateDbl => mediaRate * dmlMul;

		EditListBox( Array entries, int mediaRate )
		{
			this.entries = entries;
			this.mediaRate = mediaRate;
		}

		internal static EditListBox load( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.edts );
			foreach( eBoxType boxType in reader.readChildren() )
			{
				switch( boxType )
				{
					case eBoxType.elst:
						{
							var h = reader.readStructure<BoxHeader>();
							int length = h.entriesCount;
							if( length <= 0 )
								return null;
							switch( h.version )
							{
								case 0:
									return load32( reader, length );
								case 0x1000000:
									return load64( reader, length );
							}
						}
						break;
					default:
						reader.skipCurrentBox();
						break;
				}
			}
			return null;
		}

		static EditListBox load32( Mp4Reader reader, int length )
		{
			Entry32[] entries = new Entry32[ length ];
			var bytes = entries.AsSpan().asBytes();
			reader.read( bytes );

			Span<uint> integers = bytes.cast<uint>();
			for( int i = 0; i < integers.Length; i++ )
				integers[ i ] = BinaryPrimitives.ReverseEndianness( integers[ i ] );

			int rate = reader.readStructure<int>().endian();
			return new EditListBox( entries, rate );
		}

		static EditListBox load64( Mp4Reader reader, int length )
		{
			var entries = new Entry64[ length ];
			var bytes = entries.AsSpan().asBytes();
			reader.read( bytes );
			
			Span<ulong> integers = bytes.cast<ulong>();
			for( int i = 0; i < integers.Length; i++ )
				integers[ i ] = BinaryPrimitives.ReverseEndianness( integers[ i ] );

			int rate = reader.readStructure<int>().endian();
			return new EditListBox( entries, rate );
		}
	}
}