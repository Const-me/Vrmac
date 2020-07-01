using System;
using System.Collections.Generic;
using System.Diagnostics;
using VrmacVideo.Containers.MP4.Structures;

namespace VrmacVideo.Containers.MP4
{
	public struct MovieHeader
	{
		public readonly DateTime creationTime, modificationTime;
		public readonly TimeSpan duration;
		public readonly uint timescale;

		public readonly double rate;
		public readonly float volume;
		public readonly uint nextTrackId;

		internal MovieHeader( Mp4Reader reader )
		{
			Debug.Assert( reader.currentBox == eBoxType.mvhd );
			uint versionAndFlags = reader.readUInt();
			switch( versionAndFlags & 0xFF )
			{
				case 0:
					var ver0 = reader.readStructure<MovieHeaderVersion0>();
					ver0.parseHeader( out creationTime, out modificationTime, out duration, out timescale );
					ver0.parseCommon( out rate, out volume, out nextTrackId );
					break;
				case 1:
					var ver1 = reader.readStructure<MovieHeaderVersion1>();
					ver1.parseHeader( out creationTime, out modificationTime, out duration, out timescale );
					ver1.parseCommon( out rate, out volume, out nextTrackId );
					break;
				default:
					throw new ArgumentException( "Unsupported track version" );
			}
		}

		IEnumerable<string> details()
		{
			yield return $"creationTime: { creationTime }";
			yield return $"modificationTime: { modificationTime }";
			yield return $"duration: { duration }";
			yield return $"timescale: { timescale }";
			yield return $"rate: { creationTime }";
			yield return $"volume: { volume }";
		}
		public override string ToString() => details().makeLines();
	}
}