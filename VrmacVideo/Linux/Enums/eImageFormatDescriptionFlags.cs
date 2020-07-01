using System;

namespace VrmacVideo.Linux
{
	[Flags]
	public enum eImageFormatDescriptionFlags: int
	{
		None = 0,

		/// <summary>This is a compressed format</summary>
		Compressed = 0x0001,

		/// <summary>This format is not native to the device but emulated through software (usually libv4l2), where possible try to use a native format instead for better performance.</summary>
		Emulated = 0x0002,

		/// <summary>The hardware decoder for this compressed bytestream format is capable of parsing a continuous bytestream.</summary>
		/// <remarks>
		/// <para>Applications do not need to parse the bytestream themselves to find the boundaries between frames / fields.</para>
		/// <para>This flag can only be used in combination with <see cref="Compressed"/>, since this applies to compressed formats only.</para>
		/// <para>This flag is valid for stateful decoders only.</para>
		/// </remarks>
		ContinuousByteStream = 0x0004,

		/// <summary>Dynamic resolution switching is supported by the device for this compressed bytestream format</summary>
		/// <remarks>
		/// <para>It will notify the user via the event V4L2_EVENT_SOURCE_CHANGE when changes in the video parameters are detected.</para>
		/// <para>This flag can only be used in combination with <see cref="Compressed"/>, since this applies to compressed formats only.</para>
		/// <para>This flag is valid for stateful decoders only.</para>
		/// </remarks>
		DynamicResolution = 0x0008,
	}
}