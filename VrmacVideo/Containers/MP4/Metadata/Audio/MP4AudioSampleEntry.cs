using System;
using VrmacVideo.Containers.MP4.ElementaryStream;
using VrmacVideo.IO;

namespace VrmacVideo.Containers.MP4
{
	sealed class MP4AudioSampleEntry: AudioSampleEntry
	{
		public readonly ushort id;
		public readonly eDescriptorFlags flags;

		public readonly byte priority;

		/// <summary>ID of another elementary stream on which this elementary stream depends</summary>
		public readonly ushort? dependsOn;

		/// <summary>URL that shall point to the location of an SyncLayer-packetized stream by name.</summary>
		public readonly string url;

		/// <summary>Elementary stream from which the time base for this elementary stream is derived.</summary>
		public readonly ushort? esId;

		public readonly DecoderConfiguration? decoderConfig;
		public readonly SyncLayerConfiguration? syncLayerConfig;
		public readonly byte? profileLevelIndicationindex;

		public override byte[] audioSpecificConfig => decoderConfig?.audioSpecificConfig;

		public MP4AudioSampleEntry( Mp4Reader mp4, int bytesLeft ) :
			base( mp4, ref bytesLeft )
		{
			Span<byte> bytes = stackalloc byte[ bytesLeft ];
			mp4.read( bytes );

			int atomSize = BitConverter.ToInt32( bytes ).endian();
			uint atomTag = BitConverter.ToUInt32( bytes.Slice( 4 ) );

			switch( atomTag )
			{
				case (uint)eAudioBoxType.esds:
					break;
				default:
					throw new NotImplementedException();
			}

			// Skip header, version and flags
			bytes = bytes.Slice( 12 );

			Reader streamReader = new Reader( bytes );
			if( streamReader.EOF )
				throw new ArgumentException( $"The `esds` atom is empty" );

			// Read tag and size
			eDescriptorTag tag = streamReader.readTag();
			if( eDescriptorTag.ElementaryStream != tag )
				throw new ArgumentException( $"The `esds` atom is expected to contain an elementary stream descriptor, got { tag } instead" );
			int size = streamReader.readSize();

			// Create a reader for the content
			Reader esdReader = streamReader.readSubStream( size );

			// Read ElementaryStreamDescriptor
			ElementaryStreamDescriptor esd = esdReader.readStructure<ElementaryStreamDescriptor>();
			id = esd.id;
			flags = esd.flags;
			priority = esd.priority;

			if( esd.flags.HasFlag( eDescriptorFlags.DependentStream ) )
				dependsOn = esdReader.readStructure<ushort>().endian();

			if( esd.flags.HasFlag( eDescriptorFlags.URL ) )
			{
				byte urlLength = esdReader.readByte();
				Span<byte> urlBuffer = stackalloc byte[ urlLength ];
				esdReader.readBytes( urlBuffer );
				unsafe
				{
					fixed ( byte* ptr = urlBuffer )
						url = StringMarshal.copy( ptr, urlLength );
				}
			}

			if( esd.flags.HasFlag( eDescriptorFlags.OCRstream ) )
				esId = esdReader.readStructure<ushort>().endian();

			while( !esdReader.EOF )
			{
				tag = esdReader.readTag();
				size = esdReader.readSize();
				Reader ss = esdReader.readSubStream( size );
				switch( tag )
				{
					case eDescriptorTag.DecoderConfiguration:
						decoderConfig = new DecoderConfiguration( ref ss );
						break;
					case eDescriptorTag.SyncLayerConfiguration:
						syncLayerConfig = new SyncLayerConfiguration( ref ss );
						break;
					case eDescriptorTag.ProfileLevelIndicationIndex:
						profileLevelIndicationindex = ss.readByte();
						break;
					case eDescriptorTag.IPIdentificationPointer:
					case eDescriptorTag.IPMPPointer:
					case eDescriptorTag.Language:
					case eDescriptorTag.QoS:
					case eDescriptorTag.Registration:
						// Because of the way we implemented readSubStream, this break will skip them gracefully.
						// TODO: support at least language, likely to be seen in the wild
						break;
					default:
						throw new NotSupportedException();
				}
			}

			// TODO: the esdReader has EOF, but the outer one, streamReader, might have not. It might contain moar stuff.
			// You can test for `if( !streamReader.EOF )` here, and parse even moar garbage carefully designed by these ISO/IEC committees and documented in many thousands of pages of these PDFs they sell.
		}
	}
}