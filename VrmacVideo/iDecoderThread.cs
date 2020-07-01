using System;

namespace VrmacVideo
{
	interface iDecoderThread
	{
		/// <summary>If the decoding thread is good does nothing, otherwise marshals the exception from that thread.</summary>
		void marshalException();

		MediaSeekPosition findStreamsPosition( TimeSpan where );

		void seek( ref MediaSeekPosition msp );

		void setPresentationClock( PresentationClock clock );
	}
}