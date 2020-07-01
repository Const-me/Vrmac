using System;
using System.Collections.Generic;
using System.IO;

namespace VrmacVideo.Containers.MKV
{
	/// <summary>Audio settings.</summary>
	public sealed partial class Audio
	{
		/// <summary>Sampling frequency in Hz.</summary>
		public readonly double samplingFrequency = 8000;
		/// <summary>Real output sampling frequency in Hz (used for SBR techniques).</summary>
		public readonly double? outputSamplingFrequency;
		/// <summary>Numbers of channels in the track.</summary>
		public readonly ulong channels = 1;
		/// <summary>Table of horizontal angles for each successive channel.</summary>
		public readonly Blob channelPositions;
		/// <summary>Bits per sample, mostly used for PCM.</summary>
		public readonly ulong bitDepth;

		internal Audio( Stream stream )
		{
			ElementReader reader = new ElementReader( stream );
			while( !reader.EOF )
			{
				eElement id = reader.readElementId();
				switch( id )
				{
					case eElement.SamplingFrequency:
						samplingFrequency = reader.readFloat( 8000 );
						break;
					case eElement.OutputSamplingFrequency:
						outputSamplingFrequency = reader.readFloat();
						break;
					case eElement.Channels:
						channels = reader.readUlong( 1 );
						break;
					case eElement.ChannelPositions:
						channelPositions = Blob.read( reader );
						break;
					case eElement.BitDepth:
						bitDepth = reader.readUlong();
						break;
					default:
						reader.skipElement();
						break;
				}
			}
		}
	}
}