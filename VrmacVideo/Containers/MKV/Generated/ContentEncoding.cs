using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Settings for one content encoding like compression or encryption.</summary>
	public sealed partial class ContentEncoding
	{
		/// <summary>Tells when this modification was used during encoding/muxing starting with 0 and counting upwards. The decoder/demuxer has to start with the highest order number it finds and work its way down. This value has to be
		/// unique over all ContentEncodingOrder Elements in the TrackEntry that contains this ContentEncodingOrder element.</summary>
		public readonly ulong contentEncodingOrder = 0;
		/// <summary>A bit field that describes which Elements have been modified in this way. Values (big endian) can be OR'ed.</summary>
		public readonly eContentEncodingScope contentEncodingScope = eContentEncodingScope.FrameContent;
		/// <summary>A value describing what kind of transformation is applied.</summary>
		public readonly eContentEncodingType contentEncodingType = eContentEncodingType.Compression;
		/// <summary>Settings describing the compression used. This Element MUST be present if the value of ContentEncodingType is 0 and absent otherwise. Each block MUST be decompressable even if no previous block is available in order not
		/// to prevent seeking.</summary>
		public readonly ContentCompression contentCompression;
		/// <summary>Settings describing the encryption used. This Element MUST be present if the value of `ContentEncodingType` is 1 (encryption) and MUST be ignored otherwise.</summary>
		public readonly ContentEncryption contentEncryption;

		internal ContentEncoding( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ContentEncodingOrder:
						contentEncodingOrder = reader.readUlong( 0 );
						break;
					case eElement.ContentEncodingScope:
						contentEncodingScope = (eContentEncodingScope)reader.readByte( 1 );
						break;
					case eElement.ContentEncodingType:
						contentEncodingType = (eContentEncodingType)reader.readByte( 0 );
						break;
					case eElement.ContentCompression:
						contentCompression = new ContentCompression( stream );
						break;
					case eElement.ContentEncryption:
						contentEncryption = new ContentEncryption( stream );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}