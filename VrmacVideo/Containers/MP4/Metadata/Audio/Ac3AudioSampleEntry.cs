using System;

namespace VrmacVideo.Containers.MP4
{
	class Ac3AudioSampleEntry: AudioSampleEntry
	{
		// byte[] configBlob;
		public override byte[] audioSpecificConfig => null;
		public Ac3AudioSampleEntry( Mp4Reader mp4, int bytesLeft ) :
			base( mp4, ref bytesLeft )
		{
			// configBlob = new byte[ bytesLeft ];
			// mp4.read( configBlob.AsSpan() );
			mp4.skipCurrentBox();
		}
	}
}