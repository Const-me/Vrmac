using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using VrmacVideo.Containers.MP4.Structures;

namespace VrmacVideo.Containers.MP4
{
	[Flags]
	public enum eTrackFlags: int
	{
		/// <summary>The track is enabled. A disabled track is treated as if it were not present.</summary>
		Enabled = 0x000001,
		/// <summary>The track is used in the presentation</summary>
		Movie = 0x000002,
		/// <summary>The track is used when previewing the presentation</summary>
		Preview = 0x000004,
		/// <summary>Indicates that the width and height fields are not expressed in pixel units. The values have the same units but these units are not specified. The values are only an indication of the desired aspect ratio.</summary>
		SizeIsAspectRatio = 0x000008
	}

	public struct TrackHeader
	{
		public readonly uint id;
		public readonly DateTime creationTime, modificationTime;
		public readonly TimeSpan duration;

		public readonly eTrackFlags flags;
		/// <summary>front‐to‐back ordering of video tracks; tracks with lower numbers are closer to the viewer. 0 is the normal value, and ‐1 would be in front of track 0, and so on.</summary>
		public readonly short layer;
		/// <summary>A group or collection of tracks. If this field is not 0, it should be the same for tracks that contain alternate data for one another and different for tracks belonging to different such groups.</summary>
		public readonly short alternateGroup;
		public readonly float volume;
		public readonly Vector2 size;

		internal TrackHeader( Mp4Reader reader, uint timescale )
		{
			Debug.Assert( reader.currentBox == eBoxType.tkhd );
			uint versionAndFlags = reader.readUInt();
			flags = (eTrackFlags)BinaryPrimitives.ReverseEndianness( versionAndFlags & 0xFFFFFF00u );

			switch( versionAndFlags & 0xFF )
			{
				case 0:
					var ver0 = reader.readStructure<TrackHeaderVersion0>();
					ver0.parseHeader( timescale, out creationTime, out modificationTime, out id, out duration );
					ver0.parseCommon( out layer, out alternateGroup, out volume, out size );
					break;
				case 1:
					var ver1 = reader.readStructure<TrackHeaderVersion1>();
					ver1.parseHeader( timescale, out creationTime, out modificationTime, out id, out duration );
					ver1.parseCommon( out layer, out alternateGroup, out volume, out size );
					break;
				default:
					throw new ArgumentException( "Unsupported track version" );
			}
		}
	}
}