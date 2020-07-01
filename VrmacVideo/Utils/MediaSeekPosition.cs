namespace VrmacVideo
{
	struct MediaSeekPosition
	{
		public readonly StreamPosition video, audio;

		public MediaSeekPosition( StreamPosition video, StreamPosition audio )
		{
			this.video = video;
			this.audio = audio;
		}

		public override string ToString() => $"video { video }, audio { audio }";
	}
}