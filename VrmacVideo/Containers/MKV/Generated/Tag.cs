using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>A single metadata descriptor.</summary>
	public sealed partial class Tag
	{
		/// <summary>Specifies which other elements the metadata represented by the Tag applies to. If empty or not present, then the Tag describes everything in the Segment.</summary>
		public readonly Targets targets;
		/// <summary>Contains general information about the target.</summary>
		public readonly SimpleTag[] simpleTag;

		internal Tag( Stream stream )
		{
			List<SimpleTag> simpleTaglist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.Targets:
						targets = new Targets( stream );
						break;
					case eElement.SimpleTag:
						if( null == simpleTaglist ) simpleTaglist = new List<SimpleTag>();
						simpleTaglist.Add( new SimpleTag( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( simpleTaglist != null ) simpleTag = simpleTaglist.ToArray();
		}
	}
}