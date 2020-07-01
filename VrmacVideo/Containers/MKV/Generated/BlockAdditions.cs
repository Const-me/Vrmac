using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contain additional blocks to complete the main one. An EBML parser that has no knowledge of the Block structure could still see and use/skip these data.</summary>
	public sealed partial class BlockAdditions
	{
		/// <summary>Contain the BlockAdditional and some parameters.</summary>
		public readonly BlockMore[] blockMore;

		internal BlockAdditions( Stream stream )
		{
			List<BlockMore> blockMorelist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.BlockMore:
						if( null == blockMorelist ) blockMorelist = new List<BlockMore>();
						blockMorelist.Add( new BlockMore( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( blockMorelist != null ) blockMore = blockMorelist.ToArray();
		}
	}
}