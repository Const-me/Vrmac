using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vrmac;

namespace VrmacVideo.Containers.MKV
{
	static class MaxEncodedSize
	{
#if false
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

		public static int find( MkvMediaFile file, TrackEntry track, CSize size )
		{
			ulong trackNum = track.trackNumber;

			int maxFound = 0;
			for( int i = 0; i < 166; i++ )
			{
				Cluster firstCluster = file.segment.cluster[ i ].loadCluster( file.stream );

				int extraBytes = track.extraBytesCount();

				int clusterMax = 0;
				foreach( var b in firstCluster.allBlobs() )
				{
					if( b.trackNumber != trackNum )
						continue;

					// Don't bother decoding NALUs. We don't need precise number, just a reasonable upper estimate.
					clusterMax = Math.Max( clusterMax, (int)b.length + extraBytes );
				}
				Console.WriteLine( "{0}\t{1}", i, clusterMax );
				maxFound = Math.Max( maxFound, clusterMax );
			}

			return maxFound;
		}
#else
		/* The above code ain't reliable enough. Example from a random video:
		cluster   max video frame
		0         45859
		1         46951
		2         60244
		16        106926
		63        196535
		147       209187
		Parsing the complete video on startup would be slow, just too many GB of I/O bandwidth.
		That's why cheating with the "max bits/pixel" heuristic
		*/

		// This results in ~674kb for 1920x816. We mmap 2 encoded video buffers, the overhead ain't that bad compared to the total amount of physical RAM on the Pi, which is measured in gigabytes.
		// Let's just hope Linux doesn't do anything stupid with these memory mapped buffers,
		// like pinning the whole buffer into caches, or committing physical RAM pages for the complete buffer despite we only use a smaller portion at the start of them.
		const int maxBitsPerPixel = 3;

		public static int find( MkvMediaFile file, TrackEntry track, CSize size )
		{
			int pixels = size.cx * size.cy;
			return pixels * maxBitsPerPixel / 8;
		}
#endif
	}
}