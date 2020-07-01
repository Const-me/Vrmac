#pragma warning disable CS0649
using System.Runtime.InteropServices;

namespace VrmacVideo.Linux
{
	/// <summary>Payload structure for <see cref="eControlCode.DQEVENT" /> code, C++ type is v4l2_event</summary>
	/// <seealso href="https://www.kernel.org/doc/html/latest/media/uapi/v4l/vidioc-dqevent.html#c.v4l2_event" />
	public unsafe struct sEvent
	{
		public readonly eEventType type;

		/// <summary>Undocumented padding because the union has int64_t somewhere</summary>
		uint padding;

		[StructLayout( LayoutKind.Explicit, Size = 64 )]
		public struct Union
		{
			/// <summary>When the type is VSync, the upcoming field</summary>
			[FieldOffset( 0 )] public readonly eField vsyncField;

			/// <summary>When the type is FrameSync, the sequence number of the frame being received</summary>
			[FieldOffset( 0 )] public readonly uint frameSyncSequence;

			/// <summary>When the type is SourceChange, a bitmask that tells what has changed</summary>
			[FieldOffset( 0 )] public readonly eSourceChanges sourceChanges;
		}

		public readonly Union u;

		/// <summary>Number of pending events excluding this one.</summary>
		public readonly uint pending;

		/// <summary>Event sequence number. The sequence number is incremented for every subscribed event that takes place. If sequence numbers are not contiguous it means that events have been lost.</summary>
		public readonly uint sequence;

		/// <summary>Event timestamp. The timestamp has been taken from the CLOCK_MONOTONIC clock. To access the same clock outside V4L2, use clock_gettime().</summary>
		public readonly sTimeNano timestamp;

		/// <summary>The ID associated with the event source. If the event does not have an associated ID (this depends on the event type), then this is 0.</summary>
		public readonly uint id;

		/// <summary>Reserved for future extensions</summary>
		/// <remarks>Linux spec says "8" but that's not true, due to the 8-alignment compiler adds one extra.</remarks>
		fixed uint reserved[ 9 ];

		/// <summary>A string for debugger</summary>
		public override string ToString()
		{
			switch( type )
			{
				case eEventType.SourceChange:
					return $"type { type }, sourceChanges { u.sourceChanges }";
				case eEventType.VSync:
					return $"type { type }, field { u.vsyncField }";
				case eEventType.FrameSync:
					return $"type { type }, sequence { u.frameSyncSequence }";
			}
			return $"type { type }";
		}
	}
}