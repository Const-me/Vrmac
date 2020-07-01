using System;

namespace VrmacVideo.IO.HEVC
{
	[Flags]
	enum eDecoderStatus: byte
	{
		Interrupt = 1,
		Edge = 2,
		EN = 4,
		Status = 8,
	}

	/// <summary>Status of the decoder, 8 bits, 4 per phase</summary>
	struct sDecoderStatus
	{
		public readonly eDecoderStatus phase1, phase2;

		public sDecoderStatus( int state )
		{
			phase1 = (eDecoderStatus)( state & 0xF );
			phase2 = (eDecoderStatus)( ( state >> 4 ) & 0xF );
		}
	}
}