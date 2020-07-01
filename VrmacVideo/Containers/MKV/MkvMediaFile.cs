using System;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	class MkvMediaFile: iMediaFile
	{
		public readonly Stream stream;
		public readonly Segment segment;
		readonly TimeSpan duration;
		public readonly ClustersCache clusters;

		public MkvMediaFile( Stream stream )
		{
			this.stream = stream;
			segment = Segment.read( stream );

			videoTrack = new VideoTrack( this, segment.findVideoTrack() );
			audioTrack = new AudioTrack( this, segment.findAudioTrack() );

			var in4 = segment.info[ 0 ];
			if( in4.duration.HasValue )
			{
				double nano = in4.duration.Value * in4.timestampScale;
				long ticks = (long)( nano * 0.01 );
				duration = TimeSpan.FromTicks( ticks );
			}
			else
				throw new ArgumentException( "THe MKV lacks the duration field" );

			clusters = new ClustersCache( this );
		}

		public void Dispose()
		{
			stream?.Dispose();
		}

		readonly VideoTrack videoTrack;
		iVideoTrack iMediaFile.videoTrack => videoTrack;

		readonly AudioTrack audioTrack;
		iAudioTrack iMediaFile.audioTrack => audioTrack;

		TimeSpan iMediaFile.duration => duration;

		public void loadCluster( int idx, ReusableCluster dest )
		{
			if( idx < 0 || idx >= segment.cluster.Length )
				throw new IndexOutOfRangeException();
			segment.cluster[ idx ].load( stream, segment.position, dest );
		}
	}
}