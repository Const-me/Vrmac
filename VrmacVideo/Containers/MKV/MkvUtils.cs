using System;
using System.Collections.Generic;
using System.Linq;

namespace VrmacVideo.Containers.MKV
{
	static class MkvUtils
	{
		static IEnumerable<Blob> allBlobs( this Cluster cluster )
		{
			if( null != cluster.simpleBlock )
				foreach( var b in cluster.simpleBlock )
					yield return b;
			if( null != cluster.blockGroup )
				foreach( var b in cluster.blockGroup )
					yield return b.block;
		}

		static IEnumerable<T> enumerate<T>( this IEnumerable<T> e )
		{
			return ( null == e ) ? Enumerable.Empty<T>() : e;
		}

		static int extraBytesCount( this TrackEntry track )
		{
			int res = 0;
			foreach( var e in ( track?.contentEncodings?.contentEncoding ).enumerate() )
			{
				var comp = e.contentCompression;
				if( null == comp )
					continue;
				if( comp.contentCompAlgo == eContentCompAlgo.HeaderStripping )
					res += comp.contentCompSettings.Length;
			}
			return res;
		}

		public static byte[] strippedHeaderBytes( this TrackEntry track )
		{
			foreach( var e in ( track?.contentEncodings?.contentEncoding ).enumerate() )
			{
				var comp = e.contentCompression;
				if( null == comp )
					continue;
				if( comp.contentCompAlgo == eContentCompAlgo.HeaderStripping )
					return comp.contentCompSettings;
			}
			return null;
		}
	}
}