using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Contain attached files.</summary>
	public sealed partial class Attachments
	{
		/// <summary>An attached file.</summary>
		public readonly AttachedFile[] attachedFile;

		internal Attachments( Stream stream )
		{
			List<AttachedFile> attachedFilelist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.AttachedFile:
						if( null == attachedFilelist ) attachedFilelist = new List<AttachedFile>();
						attachedFilelist.Add( new AttachedFile( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( attachedFilelist != null ) attachedFile = attachedFilelist.ToArray();
		}
	}
}