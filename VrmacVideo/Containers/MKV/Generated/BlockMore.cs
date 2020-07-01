using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contain the BlockAdditional and some parameters.</summary>
	public sealed partial class BlockMore
	{
		/// <summary>An ID to identify the BlockAdditional level. A value of 1 means the BlockAdditional data is interpreted as additional data passed to the codec with the Block data.</summary>
		public readonly ulong blockAddID = 1;
		/// <summary>Interpreted by the codec as it wishes (using the BlockAddID).</summary>
		public readonly Blob blockAdditional;

		internal BlockMore( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.BlockAddID:
						blockAddID = reader.readUlong( 1 );
						break;
					case eElement.BlockAdditional:
						blockAdditional = Blob.read( reader );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}