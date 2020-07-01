#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value 0 - the message is not true
#pragma warning disable CS0169  // field is never used
using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace VrmacVideo.Containers.MP4.Structures
{
	unsafe struct MovieHeaderCommon
	{
		int rate;
		short volume;
		ushort padding1;
		ulong padding2;
		fixed int matrix[ 9 ];
		fixed int undocumented[ 6 ];
		uint next_track_ID;

		public void parse( out double r, out float vol, out uint nt )
		{
			int rate = BinaryPrimitives.ReverseEndianness( this.rate );
			r = rate * ( 1.0 / 0x10000 );
			int volume = BinaryPrimitives.ReverseEndianness( this.volume );
			vol = volume * ( 1.0f / 0x100 );
			nt = BinaryPrimitives.ReverseEndianness( next_track_ID );
		}
	}

	interface iMovieHeader
	{
		void parseHeader( out DateTime creationTime, out DateTime modificationTime, out TimeSpan duration, out uint timescale );
		void parseCommon( out double r, out float vol, out uint nt );
	}

	struct MovieHeaderVersion0: iMovieHeader
	{
		uint creation_time, modification_time, scale, dur;
		MovieHeaderCommon common;

		public void parseHeader( out DateTime creationTime, out DateTime modificationTime, out TimeSpan duration, out uint timescale )
		{
			creationTime = Mp4Utils.time( creation_time );
			modificationTime = Mp4Utils.time( modification_time );
			timescale = BinaryPrimitives.ReverseEndianness( scale );
			double seconds = ( (double)BinaryPrimitives.ReverseEndianness( dur ) ) / timescale;
			duration = TimeSpan.FromSeconds( seconds );
		}

		public void parseCommon( out double r, out float vol, out uint nt ) =>
			common.parse( out r, out vol, out nt );
	}

	[StructLayout( LayoutKind.Sequential, Pack = 4 )]
	struct MovieHeaderVersion1: iMovieHeader
	{
		long creation_time, modification_time;
		uint scale;
		long dur;
		MovieHeaderCommon common;

		public void parseHeader( out DateTime creationTime, out DateTime modificationTime, out TimeSpan duration, out uint timescale )
		{
			creationTime = Mp4Utils.time( creation_time );
			modificationTime = Mp4Utils.time( modification_time );
			timescale = BinaryPrimitives.ReverseEndianness( scale );
			double seconds = ( (double)BinaryPrimitives.ReverseEndianness( dur ) ) / timescale;
			duration = TimeSpan.FromSeconds( seconds );
		}

		public void parseCommon( out double r, out float vol, out uint nt ) =>
			common.parse( out r, out vol, out nt );
	}
}