using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contains elements that describe each value of <a href="https://www.matroska.org/technical/elements.html#BlockAddID">BlockAddID</a> found in the Track.</summary>
	public sealed partial class BlockAdditionMapping
	{
		/// <summary>The <a href="https://www.matroska.org/technical/elements.html#BlockAddID">BlockAddID</a> value being described. To keep MaxBlockAdditionID as low as possible, small values SHOULD be used.</summary>
		public readonly ulong blockAddIDValue;
		/// <summary>A human-friendly name describing the type of BlockAdditional data as defined by the associated Block Additional Mapping.</summary>
		public readonly string blockAddIDName;
		/// <summary>Stores the registered identifer of the Block Additional Mapping to define how the BlockAdditional data should be handled.</summary>
		public readonly ulong blockAddIDType = 0;
		/// <summary>Extra binary data that the BlockAddIDType can use to interpret the BlockAdditional data. The intepretation of the binary data depends on the BlockAddIDType value and the corresponding Block Additional Mapping.</summary>
		public readonly Blob blockAddIDExtraData;

		internal BlockAdditionMapping( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.BlockAddIDValue:
						blockAddIDValue = reader.readUlong();
						break;
					case eElement.BlockAddIDName:
						blockAddIDName = reader.readAscii();
						break;
					case eElement.BlockAddIDType:
						blockAddIDType = reader.readUlong( 0 );
						break;
					case eElement.BlockAddIDExtraData:
						blockAddIDExtraData = Blob.read( reader );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}