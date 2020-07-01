#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
#pragma warning disable CS0169  // field is never used
using System;
using System.Buffers.Binary;
using System.Numerics;

namespace VrmacVideo.Containers.MP4.Structures
{
	unsafe struct TrackHeaderCommon
	{
		fixed int reserved[ 2 ];
		short layer, alternate_group, volume, reserved2;
		fixed int matrix[ 9 ];
		int width, height;

		public void parse( out short layer, out short alternate_group, out float volume, out Vector2 size )
		{
			layer = BinaryPrimitives.ReverseEndianness( this.layer );
			alternate_group = BinaryPrimitives.ReverseEndianness( this.alternate_group );
			volume = BinaryPrimitives.ReverseEndianness( this.volume ) * ( 1.0f / 0x100 );
			float sizeMul = 1.0f / 0x10000;
			size = new Vector2( BinaryPrimitives.ReverseEndianness( width ) * sizeMul, BinaryPrimitives.ReverseEndianness( height ) * sizeMul );
		}
	}

	interface iTrackHeader
	{
		void parseHeader( uint timescale, out DateTime creationTime, out DateTime modificationTime, out uint id, out TimeSpan duration );
		void parseCommon( out short layer, out short alternate_group, out float volume, out Vector2 size );
	}

	struct TrackHeaderVersion0: iTrackHeader
	{
		uint creation_time, modification_time, track_ID, reserved, dur;
		TrackHeaderCommon common;

		public void parseHeader( uint timescale, out DateTime creationTime, out DateTime modificationTime, out uint id, out TimeSpan duration )
		{
			creationTime = Mp4Utils.time( creation_time );
			modificationTime = Mp4Utils.time( modification_time );
			id = BinaryPrimitives.ReverseEndianness( track_ID );

			double seconds = ( (double)BinaryPrimitives.ReverseEndianness( dur ) ) / timescale;
			duration = TimeSpan.FromSeconds( seconds );
		}

		public void parseCommon( out short layer, out short alternate_group, out float volume, out Vector2 size ) =>
			common.parse( out layer, out alternate_group, out volume, out size );
	}

	struct TrackHeaderVersion1: iTrackHeader
	{
		long creation_time, modification_time;
		uint track_ID, reserved;
		long dur;
		TrackHeaderCommon common;

		public void parseHeader( uint timescale, out DateTime creationTime, out DateTime modificationTime, out uint id, out TimeSpan duration )
		{
			creationTime = Mp4Utils.time( creation_time );
			modificationTime = Mp4Utils.time( modification_time );
			id = BinaryPrimitives.ReverseEndianness( track_ID );

			double seconds = ( (double)BinaryPrimitives.ReverseEndianness( dur ) ) / timescale;
			duration = TimeSpan.FromSeconds( seconds );
		}

		public void parseCommon( out short layer, out short alternate_group, out float volume, out Vector2 size ) =>
			common.parse( out layer, out alternate_group, out volume, out size );
	}
}