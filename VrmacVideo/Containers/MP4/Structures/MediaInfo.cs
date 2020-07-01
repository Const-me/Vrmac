#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace VrmacVideo.Containers.MP4.Structures
{
	struct MediaInfoV0
	{
		uint creation_time, modification_time, m_timescale, m_duration;
		ushort language;

		public DateTime creationTime => Mp4Utils.time( creation_time );
		public DateTime modificationTime => Mp4Utils.time( modification_time );
		public TimeSpan duration => Mp4Utils.duration( m_duration, m_timescale.endian() );
		public CultureInfo culture => Mp4Utils.culture( language );
		public uint timeScale => m_timescale.endian();
	}

	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	struct MediaInfoV1
	{
		long creation_time, modification_time;
		uint m_timescale;
		long m_duration;
		ushort language;

		public DateTime creationTime => Mp4Utils.time( creation_time );
		public DateTime modificationTime => Mp4Utils.time( modification_time );
		public TimeSpan duration => Mp4Utils.duration( m_duration, m_timescale.endian() );
		public CultureInfo culture => Mp4Utils.culture( language );
		public uint timeScale => m_timescale.endian();
	}
}