using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Settings describing the compression used. This Element MUST be present if the value of ContentEncodingType is 0 and absent otherwise. Each block MUST be decompressable even if no previous block is available in order not
	/// to prevent seeking.</summary>
	public sealed partial class ContentCompression
	{
		/// <summary>The compression algorithm used.</summary>
		public readonly eContentCompAlgo contentCompAlgo = eContentCompAlgo.Zlib;
		/// <summary>Settings that might be needed by the decompressor. For Header Stripping (`ContentCompAlgo`=3), the bytes that were removed from the beginning of each frames of the track.</summary>
		public readonly byte[] contentCompSettings;

		internal ContentCompression( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ContentCompAlgo:
						contentCompAlgo = (eContentCompAlgo)reader.readByte( 0 );
						break;
					case eElement.ContentCompSettings:
						contentCompSettings = reader.readByteArray();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}