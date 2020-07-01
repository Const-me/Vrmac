using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Settings for several content encoding mechanisms like compression or encryption.</summary>
	public sealed partial class ContentEncodings
	{
		/// <summary>Settings for one content encoding like compression or encryption.</summary>
		public readonly ContentEncoding[] contentEncoding;

		internal ContentEncodings( Stream stream )
		{
			List<ContentEncoding> contentEncodinglist = null;
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.ContentEncoding:
						if( null == contentEncodinglist ) contentEncodinglist = new List<ContentEncoding>();
						contentEncodinglist.Add( new ContentEncoding( stream ) );
						break;
					default:
						reader.skipElement();
						break;
				}
			}
			if( contentEncodinglist != null ) contentEncoding = contentEncodinglist.ToArray();
		}
	}
}