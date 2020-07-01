using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	struct FileSlice
	{
		/// <summary>Offset in the file</summary>
		public readonly long position;
		/// <summary>Length in bytes</summary>
		public readonly int length;

		public FileSlice( long position, int length )
		{
			this.position = position;
			this.length = length;
		}
	}

	abstract class ReaderBase
	{
		protected readonly ClustersCache clusters;
		protected readonly ulong trackNumber;
		protected readonly TrackEntry track;
		readonly byte[] strippedHeaders;
		readonly int strippedHeaderBytes;
		protected readonly SeekPoint[] seekIndex;

		public ReaderBase( MkvMediaFile file, TrackEntry track )
		{
			clusters = file.clusters;
			this.track = track;
			trackNumber = track.trackNumber;
			strippedHeaders = track.strippedHeaderBytes();
			strippedHeaderBytes = strippedHeaders?.Length ?? 0;
			seekIndex = file.segment.buildSeekIndex( trackNumber );
			stream = file.stream;

			loadCluster( 0 );
		}

		int clusterIndex = -1;
		long clusterTimestamp;
		// The list is filtered, only contains entries for the current track
		readonly List<Blob> blobs = new List<Blob>();

		bool loadCluster( int i, bool unpackFrames = true )
		{
			// Stopwatch sw = Stopwatch.StartNew();
			ReusableCluster cluster = clusters.load( i );
			clusterTimestamp = (long)cluster.timestamp;
			clusterIndex = i;
			blobs.Clear();
			if( null != cluster.simpleBlock )
			{
				foreach( var b in cluster.simpleBlock )
				{
					if( b.trackNumber != trackNumber )
						continue;
					blobs.Add( b );
				}
			}
			if( null != cluster.blockGroup )
			{
				foreach( var b in cluster.blockGroup )
				{
					if( b.block.trackNumber != trackNumber )
						continue;
					blobs.Add( b.block );
				}
			}

			if( blobs.Count > 0 )
			{
				if( !unpackFrames )
					return true;

				// Initialize the first block of the now current cluster
				var firstBlock = blobs[ 0 ];
				lock( clusters.syncRoot )
					stream.unpackFrames( ref firstBlock, ref laced );
				updateTimestamp( firstBlock.timestamp );

				// Logger.logVerbose( "{0}.loadCluster #{1}, {2}", this.GetType().Name, i, timestamp );
				// Logger.logVerbose( "{0}.loadCluster #{1}, {2}; took {3} ms", this.GetType().Name, i, timestamp, sw.Elapsed.TotalMilliseconds );
				return true;
			}

			// Sometimes a cluster has a single blob of only 1 stream.
			// Probably caused by the fact that unlike Mpeg4, for some reason MKV developers decided to use the same time scale for all tracks.
			// Despite they're completely unrelated, audio comes at 48 KHz, video at 24 or 29.97 or 59.94 FPS.
			return false;
		}

		void updateTimestamp( short rel )
		{
			// https://www.matroska.org/technical/basics.html // Block Header:
			// Timestamp (relative to Cluster timestamp, signed int16)
			timestamp = clusters.timeScaler.convert( clusterTimestamp + rel );
			// Logger.logVerbose( "updateTimestamp: relative={0}", rel );
		}

		int blobIndex = 0;

		public bool EOF => clusterIndex >= clusters.clustersCount;

		public Stream stream { get; }

		/// <summary>Advance to the next frame, loading new cluster if needed.</summary>
		protected bool advance()
		{
			if( laced.lacing != eLacingResult.NoLacing )
			{
				if( laced.advance() )
				{
					dbgPrintPosition();
					return true;
				}
			}

			blobIndex++;
			if( blobIndex < blobs.Count )
			{
				var b = blobs[ blobIndex ];
				lock( clusters.syncRoot )
					stream.unpackFrames( ref b, ref laced );
				updateTimestamp( b.timestamp );
				dbgPrintPosition();
				return true;
			}

			blobIndex = 0;
			while( true )
			{
				clusterIndex++;
				if( clusterIndex < clusters.clustersCount )
				{
					if( loadCluster( clusterIndex ) )
					{
						dbgPrintPosition();
						return true;
					}
					continue;
				}
				break;
			}
			// End of the media
			// Logger.logInfo( "{0}.advance: end of media", typeName );
			return false;
		}

		string typeName => ( this is VideoReader ) ? "\tVideo" : "Audio";

		void dbgPrintPosition()
		{
#if false
			if( currentBlockHasLacing )
				Logger.logInfo( "{0}: cluster {1}, block {2}, frame {3}, timestamp {4}",
					typeName, clusterIndex, blobIndex, lacedSubBlockIndex, timestamp.print() );
			else
				Logger.logInfo( "{0}: cluster {1}, block {2}, timestamp {3}",
					typeName, clusterIndex, blobIndex, timestamp.print() );
#endif
		}

		protected TimeSpan timestamp { get; private set; }

		LacedFrames laced;

		protected int getFrameSize()
		{
			int cb;
			if( laced.lacing != eLacingResult.NoLacing )
				cb = laced.currentFrame.length;
			else
				cb = blobs[ blobIndex ].length;
			return cb + strippedHeaderBytes;
		}

		/// <summary>Read complete frame into a span</summary>
		/// <remarks>Used by audio reader. That span points to an unmanaged buffer owned by <see cref="Audio.Queues" /> class.</remarks>
		protected int readCurrentFrame( Span<byte> span )
		{
			if( laced.lacing != eLacingResult.NoLacing )
			{
				// Lacing is used
				LacedFrame slice = laced.currentFrame;
				int cb = strippedHeaderBytes + slice.length;
				Debug.Assert( span.Length <= cb );
				if( null != strippedHeaders )
					strippedHeaders.CopyTo( span );
				span = span.Slice( strippedHeaderBytes, slice.length );
				if( laced.lacing == eLacingResult.Buffered )
					laced.buffer.AsSpan().Slice( slice.position, slice.length ).CopyTo( span );
				else
				{
					Blob b = blobs[ blobIndex ];
					stream.Seek( b.position + slice.position, SeekOrigin.Begin );
					stream.read( span );
				}
				return cb;
			}
			else
			{
				Blob b = blobs[ blobIndex ];
				Debug.Assert( b.lacing == eBlockFlags.NoLacing );

				int cb = strippedHeaderBytes + b.length;
				Debug.Assert( span.Length <= cb );

				if( null != strippedHeaders )
					strippedHeaders.CopyTo( span );

				span = span.Slice( strippedHeaderBytes, b.length );
				stream.Seek( b.position, SeekOrigin.Begin );
				stream.read( span );
				return cb;
			}
		}

		#region Video Reader Support
		protected struct ReaderState
		{
			// Negative means we have to copy the header. Otherwise it's offset within the frame.
			public int offset;
			// Count of bytes left in the frame
			public int bytesLeft;
		}

		int seekToFrame( int offset = 0 )
		{
			Blob blob;
			int len;
			switch( laced.lacing )
			{
				case eLacingResult.NoLacing:
					blob = blobs[ blobIndex ];
					Debug.Assert( blob.lacing == eBlockFlags.NoLacing );
					stream.Seek( blob.position + offset, SeekOrigin.Begin );
					len = blob.length;
					break;
				case eLacingResult.Buffered:
					len = laced.currentFrame.length;
					break;
				case eLacingResult.Large:
					var clf = laced.currentFrame;
					blob = blobs[ blobIndex ];
					stream.Seek( blob.position + clf.position + offset, SeekOrigin.Begin );
					len = clf.length;
					break;
				default:
					throw new ApplicationException( "Unexpected eLacingResult value" );
			}
			return len - offset;
		}

		protected void seek( ref ReaderState rs )
		{
			if( rs.bytesLeft <= 0 )
			{
				int len = seekToFrame();
				rs.bytesLeft = len + strippedHeaderBytes;
				rs.offset = -strippedHeaderBytes;
				return;
			}

			if( rs.offset >= 0 )
			{
				int cb = seekToFrame( rs.offset );
				Debug.Assert( cb == rs.bytesLeft );
			}
			else
			{
				int cb = seekToFrame();
				Debug.Assert( cb - rs.offset == rs.bytesLeft );
			}
		}

		protected void read( ref ReaderState rs, Span<byte> span )
		{
			Debug.Assert( rs.bytesLeft <= span.Length );

			if( rs.offset < 0 )
			{
				// Deal with the stripped headers
				int firstHeaderByte = strippedHeaderBytes + rs.offset;

				if( span.Length > -rs.offset )
				{
					// The destination span has space for stripped bytes, plus some extra
					ReadOnlySpan<byte> src = strippedHeaders.AsSpan().Slice( firstHeaderByte );
					src.CopyTo( span );
					span = span.Slice( strippedHeaderBytes - firstHeaderByte );
					rs.offset += src.Length;
					rs.bytesLeft -= src.Length;
				}
				else
				{
					// The destination span only has space for stripped bytes
					ReadOnlySpan<byte> src = strippedHeaders.AsSpan().Slice( firstHeaderByte, span.Length );
					src.CopyTo( span );
					rs.offset += span.Length;
					rs.bytesLeft -= span.Length;
					return;
				}
			}

			// Read remaining bytes from the file. This assumes the caller has called seek() before read()
			if( laced.lacing != eLacingResult.Buffered )
				stream.read( span );
			else
			{
				var srcFrame = laced.currentFrame;
				var srcSpan = laced.buffer.AsSpan().Slice( srcFrame.position + rs.offset, span.Length );
				srcSpan.CopyTo( span );
			}
			rs.offset += span.Length;
			rs.bytesLeft -= span.Length;
		}
		#endregion

		protected MkvSeekPosition findPosition( TimeSpan ts )
		{
			MkvSeekPosition res = FindSeekPosition.find( clusters, ts, trackNumber );
			Logger.logDebug( "{0} findPosition: {1} -> {2} ", typeName, ts, res );
			return res;
		}

		protected void seekMedia( StreamPosition streamPosition )
		{
			MkvSeekPosition pos = (MkvSeekPosition)streamPosition;

			clusterIndex = pos.cluster;
			if( !loadCluster( clusterIndex, false ) )
				throw new ApplicationException( "Seek failed" );

			blobIndex = pos.blob;
			var block = blobs[ blobIndex ];
			lock( clusters.syncRoot )
				stream.unpackFrames( ref block, ref laced );
			updateTimestamp( block.timestamp );
		}
	}
}