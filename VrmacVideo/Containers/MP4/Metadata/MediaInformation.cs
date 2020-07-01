using System;
using System.Buffers.Binary;
using System.Diagnostics;

namespace VrmacVideo.Containers.MP4
{
	public abstract class MediaInformation
	{
		public SampleTable sampleTable { get; protected set; }
	}

	public sealed class VideoInformation: MediaInformation
	{
		internal VideoInformation( Mp4Reader reader )
		{
			foreach( eBoxType boxType in reader.readChildren() )
			{
				switch( boxType )
				{
					default:
						// dinf is useless but mandatory in the spec.
						// vmhd is useless, as well.
						reader.skipCurrentBox();
						break;
					case eBoxType.stbl:
						sampleTable = new SampleTable( reader );
						break;
				}
			}
		}
	}

	public sealed class AudioInformation: MediaInformation
	{
		public readonly float balance;

		internal AudioInformation( Mp4Reader reader )
		{
			foreach( eBoxType boxType in reader.readChildren() )
			{
				switch( boxType )
				{
					case eBoxType.smhd:
						balance = readHeader( reader );
						break;
					default:
						// dinf is useless but mandatory in the spec
						reader.skipCurrentBox();
						break;
					case eBoxType.stbl:
						sampleTable = new SampleTable( reader );
						break;
				}
			}
		}

		static float readHeader( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.smhd );
			Span<byte> bytes = stackalloc byte[ 8 ];
			reader.read( bytes );
			short balance = BitConverter.ToInt16( bytes.Slice( 4 ) );
			balance = BinaryPrimitives.ReverseEndianness( balance );
			return balance * ( 1.0f / 0x100 );
		}
	}
}