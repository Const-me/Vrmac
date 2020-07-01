#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
#pragma warning disable CS0169  // field is never used
using System;

namespace VrmacVideo.Linux
{
	enum eTimeCodeType: uint
	{
		None = 0,
		/// <summary>24 frames per second, i. e. film.</summary>
		TT24 = 1,
		/// <summary>25 frames per second, i. e. PAL or SECAM video.</summary>
		TT25 = 2,
		/// <summary>30 frames per second, i. e. NTSC video.</summary>
		TT30 = 3,
		/// <summary>50 FPS</summary>
		TT50 = 4,
		/// <summary>60 FPS</summary>
		TT60 = 5,
	}

	[Flags]
	enum eTimeCodeFlags: uint
	{
		/// <summary>Unspecified format</summary>
		Unspecified = 0,
		/// <summary>Indicates “drop frame” semantics for counting frames in 29.97 fps material. When set, frame numbers 0 and 1 at the start of each minute, except minutes 0, 10, 20, 30, 40, 50 are omitted from the count.</summary>
		DropFrame = 1,
		/// <summary>The “color frame” flag.</summary>
		ColorFrame = 2,
		/// <summary>Field mask for the “binary group flags”.</summary>
		UserBitFields = 0xC,
		/// <summary>8-bit ISO characters.</summary>
		Characters = 8,
	}

	/// <summary>v4l2_timecode in C++</summary>
	/// <seealso href="https://www.kernel.org/doc/html/latest/media/uapi/v4l/buffer.html#c.v4l2_timecode" />
	unsafe struct sTimeCode
	{
		/// <summary>Frame rate the timecodes are based on</summary>
		public eTimeCodeType type;
		/// <summary>Timecode flags</summary>
		eTimeCodeFlags flags;
		/// <summary>Frame count, 0 … 23/24/29/49/59, depending on the type of timecode.</summary>
		byte frames;
		/// <summary>Seconds count, 0 … 59. This is a binary, not BCD number.</summary>
		byte seconds;
		/// <summary>Minutes count, 0 … 59. This is a binary, not BCD number.</summary>
		byte minutes;
		/// <summary>Hours count, 0 … 29. This is a binary, not BCD number.</summary>
		byte hours;
		/// <summary>The “user group” bits from the timecode.</summary>
		fixed byte userbits[ 4 ];

		public override string ToString()
		{
			if( type == eTimeCodeType.None )
				return $"type { type }";
			return $"type { type }, flags { flags }, frames { frames }, seconds { seconds }, minutes { minutes }, hours { hours }";
		}
	}
}