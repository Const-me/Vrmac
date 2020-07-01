using System;

namespace VrmacVideo.Containers.MKV
{
	sealed class MkvSeekPosition: StreamPosition
	{
		public readonly int cluster;
		public readonly int blob;

		public MkvSeekPosition( TimeSpan time, int cluster, int blob ) :
			base( time )
		{
			this.cluster = cluster;
			this.blob = blob;
		}

		public override string ToString() => $"Cluster { cluster }, blob { blob }, time { time }";
	}
}