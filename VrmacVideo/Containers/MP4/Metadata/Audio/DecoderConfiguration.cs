using System;
using System.IO;
using VrmacVideo.Containers.MP4.ElementaryStream;

namespace VrmacVideo.Containers.MP4
{
	struct DecoderConfiguration
	{
		public readonly eObjectType objectType;
		public readonly eStreamType streamType;
		public bool upStream;
		public readonly int decodingBufferSize;
		public readonly int maximumBitrate, averageBitrate;
		public readonly byte[] audioSpecificConfig;

		internal DecoderConfiguration( ref Reader reader )
		{
			var dcd = reader.readStructure<DecoderConfigDescriptor>();

			objectType = (eObjectType)dcd.objectTypeIndication;
			streamType = (eStreamType)dcd.streamType;
			upStream = dcd.upStream;
			decodingBufferSize = dcd.bufferSizeDB;
			maximumBitrate = dcd.maxBitrate;
			averageBitrate = dcd.avgBitrate;

			// The reader gotta be positioned at the start of AudioSpecificConfig tag.
			// See ISO/IEC 14496-1 annex "E" for more info on tags and sizes.
			eDescriptorTag tag = (eDescriptorTag)reader.readByte();
			if( tag != eDescriptorTag.DecoderSpecificInfo )
				throw new ArgumentException( $"Expected eDescriptorTag.DecoderSpecificInfo, got { tag } instead" );
			int cb = reader.readSize();
			if( reader.bytesLeft < cb )
				throw new EndOfStreamException( $"DecoderSpecificInfo has size { cb } in the header, yet the stream only has { reader.bytesLeft } unread bytes left" );
			audioSpecificConfig = new byte[ cb ];
			reader.readBytes( audioSpecificConfig.AsSpan() );
		}
	}
}