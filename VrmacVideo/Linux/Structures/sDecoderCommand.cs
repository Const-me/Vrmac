using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.Linux
{
	public enum eDecoderCommand: uint
	{
		/// <summary>Start the decoder. When the decoder is already running or paused, this command will just change the playback speed. </summary>
		Start = 0,

		/// <summary>Stop the decoder. When the decoder is already stopped, this command does nothing</summary>
		Stop = 1,

		/// <summary>Pause the decoder. When the decoder has not been started yet, the driver will return an EPERM error code.
		/// When the decoder is already paused, this command does nothing.</summary>
		Pause = 2,

		/// <summary>Resume decoding after <see cref="Pause" />. When the decoder has not been started yet, the driver will return an EPERM error code.
		/// When the decoder is already running, this command does nothing.</summary>
		Resume = 3,

		/// <summary>Flush any held capture buffers. Only valid for stateless decoders.</summary>
		Flush = 4
	}

	[Flags]
	public enum eDecoderCommandFlags: uint
	{
		None = 0,

		/// <summary>Audio will be muted when playing back at a non-standard speed.</summary>
		StartMuted = 1,

		/// <summary>The decoder output to black when paused</summary>
		PauseToBlack = 1,

		/// <summary>The decoder will set the picture to black after it stopped decoding</summary>
		StopToBlack = 1,

		/// <summary>The decoder stops immediately (ignoring the stopPts value), otherwise it will keep decoding until timestamp ≥ stopPts or until the last of the pending data from its internal buffers was decoded.</summary>
		StopImmidiately = 2,
	}

	[Flags]
	public enum eDecoderStartFormat: uint
	{
		/// <summary>The decoder has no special format requirements</summary>
		None = 0,

		/// <summary>The decoder operates on full GOPs (Group Of Pictures).</summary>
		/// <remarks>This is usually the case for reverse playback: the decoder needs full GOPs, which it can then play in reverse order.
		/// So to implement reverse playback the application must feed the decoder the last GOP in the video file, then the GOP before that, etc. etc.</remarks>
		FullGOPs = 1,
	}

	// C++ type v4l2_decoder_cmd: https://www.kernel.org/doc/html/latest/media/uapi/v4l/vidioc-decoder-cmd.html#c.v4l2_decoder_cmd
	public unsafe struct sDecoderCommand
	{
		public eDecoderCommand command;
		public eDecoderCommandFlags flags;

		[StructLayout( LayoutKind.Explicit, Size = 16 * 4 )]
		public struct Union
		{
			[FieldOffset( 0 )] public ulong stopPts;

			/// <summary>0 or 1000 - normal speed, 1 - forward single stepping, -1 - backward single stepping, ≥2 - playback at speed/1000 of the normal speed, ≤-2 - reverse playback at (-startSpeed/1000) of the normal speed</summary>
			[FieldOffset( 0 )] int startSpeed;
			/// <summary>Format restrictions. This field is set by the driver, not the application.</summary>
			[FieldOffset( 4 )] eDecoderStartFormat startFormat;
		}
		public Union union;
	}
}