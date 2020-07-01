using System;
using VrmacVideo.IO;

namespace VrmacVideo
{
	// The thread calls this interface when stuff happens
	interface iDecoderEvents
	{
		void onFrameDecoded( DecodedBuffer buffer );

		void onEndOfStream();

		void onDynamicResolutionChange();

		void discardDecoded();
	}
}