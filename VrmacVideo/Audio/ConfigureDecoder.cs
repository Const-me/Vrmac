using VrmacVideo.IO.AAC;

namespace VrmacVideo.Audio
{
	static class ConfigureDecoder
	{
		public static void configureAacDecoder( Decoder decoder )
		{
			// Setup the decoder to duplicate mono to stereo, and mixdown 5.1 - 7.1 inputs to stereo.
			// If you somehow connects a better USB sound card to the Pi, or using HDMI audio output, you'll want to adjust the PcmMaxOutputChannels value to higher one.
			// You'll also probably need to adjust ALSA parts.
			decoder.setParameter( eParameter.PcmMinOutputChannels, 2 );
			decoder.setParameter( eParameter.PcmMaxOutputChannels, 2 );
			Logger.logVerbose( "Configured AAC decoder to output stereo sound." );
		}
	}
}