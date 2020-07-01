using System;
using VrmacVideo.IO.HEVC;

namespace VrmacVideo.Decoders
{
	class HEVC: IDisposable
	{
		DecoderDevice device;

		public HEVC()
		{
			device = new DecoderDevice( true );
		}

		public void Dispose()
		{
			device.finalize();
		}


	}
}