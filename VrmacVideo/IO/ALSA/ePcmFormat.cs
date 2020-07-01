namespace VrmacVideo.IO.ALSA
{
	enum ePcmFormat: int
	{
		/// <summary>Unknown</summary>
		Unknown = -1,
		/// <summary>Signed 8 bit</summary>
		S8 = 0,
		/// <summary>Unsigned 8 bit</summary>
		U8,
		/// <summary>Signed 16 bit Little Endian</summary>
		S16LE,
		/// <summary>Signed 16 bit Big Endian</summary>
		S16BE,
		/// <summary>Unsigned 16 bit Little Endian</summary>
		U16LE,
		/// <summary>Unsigned 16 bit Big Endian</summary>
		U16BE,
		/// <summary>Signed 24 bit Little Endian using low three bytes in 32-bit word</summary>
		S24LE,
		/// <summary>Signed 24 bit Big Endian using low three bytes in 32-bit word</summary>
		S24BE,
		/// <summary>Unsigned 24 bit Little Endian using low three bytes in 32-bit word</summary>
		U24LE,
		/// <summary>Unsigned 24 bit Big Endian using low three bytes in 32-bit word</summary>
		U24BE,
		/// <summary>Signed 32 bit Little Endian</summary>
		S32LE,
		/// <summary>Signed 32 bit Big Endian</summary>
		S32BE,
		/// <summary>Unsigned 32 bit Little Endian</summary>
		U32LE,
		/// <summary>Unsigned 32 bit Big Endian</summary>
		U32BE,
		/// <summary>Float 32 bit Little Endian, Range -1.0 to 1.0</summary>
		FloatLE,
		/// <summary>Float 32 bit Big Endian, Range -1.0 to 1.0</summary>
		FloatBE,
		/// <summary>Float 64 bit Little Endian, Range -1.0 to 1.0</summary>
		Float64LE,
		/// <summary>Float 64 bit Big Endian, Range -1.0 to 1.0</summary>
		Float64BE,
		/// <summary>IEC-958 Little Endian</summary>
		IEC958SubframeLE,
		/// <summary>IEC-958 Big Endian</summary>
		IEC958SubframeBE,
		/// <summary>Mu-Law</summary>
		MuLaw,
		/// <summary>A-Law</summary>
		ALAW,
		/// <summary>Ima-ADPCM</summary>
		ImaAdpcm,
		/// <summary>MPEG</summary>
		MPEG,
		/// <summary>GSM</summary>
		GSM,
		/// <summary>Signed 20bit Little Endian in 4bytes format, LSB justified</summary>
		S20LE,
		/// <summary>Signed 20bit Big Endian in 4bytes format, LSB justified</summary>
		S20BE,
		/// <summary>Unsigned 20bit Little Endian in 4bytes format, LSB justified</summary>
		U20LE,
		/// <summary>Unsigned 20bit Big Endian in 4bytes format, LSB justified</summary>
		U20BE,
		/// <summary>Special</summary>
		Special = 31,
		/// <summary>Signed 24bit Little Endian in 3bytes format</summary>
		S243LE = 32,
		/// <summary>Signed 24bit Big Endian in 3bytes format</summary>
		S243BE,
		/// <summary>Unsigned 24bit Little Endian in 3bytes format</summary>
		U243LE,
		/// <summary>Unsigned 24bit Big Endian in 3bytes format</summary>
		U243BE,
		/// <summary>Signed 20bit Little Endian in 3bytes format</summary>
		S203LE,
		/// <summary>Signed 20bit Big Endian in 3bytes format</summary>
		S203BE,
		/// <summary>Unsigned 20bit Little Endian in 3bytes format</summary>
		U203LE,
		/// <summary>Unsigned 20bit Big Endian in 3bytes format</summary>
		U203BE,
		/// <summary>Signed 18bit Little Endian in 3bytes format</summary>
		S183LE,
		/// <summary>Signed 18bit Big Endian in 3bytes format</summary>
		S183BE,
		/// <summary>Unsigned 18bit Little Endian in 3bytes format</summary>
		U183LE,
		/// <summary>Unsigned 18bit Big Endian in 3bytes format</summary>
		U183BE,

		/// <summary>Signed 16 bit CPU endian</summary>
		S16 = S16LE,
		/// <summary>Unsigned 16 bit CPU endian</summary>
		U16 = U16LE,
		/// <summary>Signed 24 bit CPU endian</summary>
		S24 = S24LE,
		/// <summary>Unsigned 24 bit CPU endian</summary>
		U24 = U24LE,
		/// <summary>Signed 32 bit CPU endian</summary>
		S32 = S32LE,
		/// <summary>Unsigned 32 bit CPU endian</summary>
		U32 = U32LE,
		/// <summary>Float 32 bit CPU endian</summary>
		Float = FloatLE,
		/// <summary>Float 64 bit CPU endian</summary>
		Float64 = Float64LE,
		/// <summary>IEC-958 CPU Endian</summary>
		IEC958Subframe = IEC958SubframeLE,
		/// <summary>Signed 20bit in 4bytes format, LSB justified, CPU Endian</summary>
		S20 = S20LE,
		/// <summary>Unsigned 20bit in 4bytes format, LSB justified, CPU Endian</summary>
		U20 = U20LE,
	}
}