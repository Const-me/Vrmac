using System;

namespace VrmacVideo.Containers.MP4
{
	sealed class Mp4StreamPosition: StreamPosition
	{
		public readonly int sample;

		public Mp4StreamPosition( int sample, TimeSpan time ) :
			base( time )
		{
			this.sample = sample;
		}
		public override string ToString() => $"Sample { sample }, time { time }";
	}
}