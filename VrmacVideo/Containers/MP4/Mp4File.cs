using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using VrmacVideo.Audio;
using VrmacVideo.Linux;

namespace VrmacVideo.Containers.MP4
{
	/// <summary>Parses the content of MP4 files</summary>
	public class Mp4File: iMediaFile
	{
		public void Dispose()
		{
			reader?.Dispose();
		}

		internal readonly Mp4Reader reader;

		public readonly eFileType[] fileType;
		public readonly int minorVersion;
		public readonly Metadata metadata;

		public TrackMetadata videoTrack { get; private set; }
		public VideoSampleEntry videoTrackSample { get; private set; }

		public TrackMetadata audioTrack { get; private set; }
		public AudioSampleEntry audioTrackSample { get; private set; }

		public Mp4File( Stream stm ) :
			this( new Mp4Reader( stm ) )
		{ }

		public Mp4File( string sourcePath ) :
			this( new Mp4Reader( File.OpenRead( sourcePath ) ) )
		{ }

		public Mp4File( Mp4Reader reader )
		{
			this.reader = reader;

			reader.moveToBox( eBoxType.ftyp );

			int cb = checked((int)reader.remainingBytes);
			Span<byte> data = stackalloc byte[ cb ];
			reader.read( data );

			List<eFileType> types = new List<eFileType>();
			var values = data.cast<uint>();
			types.Add( (eFileType)values[ 0 ] );
			minorVersion = (int)BinaryPrimitives.ReverseEndianness( values[ 1 ] );
			for( int i = 2; i < values.Length; i++ )
			{
				uint v = values[ i ];
				if( v == values[ 0 ] )
					continue;
				if( Enum.IsDefined( typeof( eFileType ), v ) )
					types.Add( (eFileType)v );
			}
			fileType = types.ToArray();

			metadata = new Metadata( reader );

			findDefaultTracks();

			// Fast forward to the movie data; that box has the actual content we gonna play.
			// if( reader.currentBox != eBoxType.mdat )
			//	reader.moveToBox( eBoxType.mdat );

			m_videoTrack = new VideoTrack( this );
			m_audio = new Audio( this );
		}

		IEnumerable<string> details()
		{
			yield return $"File info: { string.Join( ", ", fileType ) }; minor version { minorVersion }";
			yield return metadata.ToString();
		}

		public override string ToString() => details().makeLines();

		/// <summary>Loop through enabled tracks, find first enabled audio &amp; video tracks.</summary>
		void findDefaultTracks()
		{
			foreach( var t in metadata.tracks )
			{
				if( !t.header.flags.HasFlag( eTrackFlags.Enabled ) )
					continue;

				if( t.info.mediaHandler.mediaHandler == eMediaHandler.vide && null == videoTrack )
				{
					videoTrack = t;
					videoTrackSample = (VideoSampleEntry)t.info.mediaInformation.sampleTable.firstEntry;
				}

				if( t.info.mediaHandler.mediaHandler == eMediaHandler.soun && null == audioTrack )
				{
					audioTrack = t;
					audioTrackSample = (AudioSampleEntry)t.info.mediaInformation.sampleTable.firstEntry;
				}
			}
		}

		TimeSpan getDuration()
		{
			TimeSpan v = TimeSpan.Zero, a = TimeSpan.Zero;
			if( null != videoTrack )
				v = videoTrack.header.duration;
			if( null != audioTrack )
				a = audioTrack.header.duration;
			return TimeSpan.FromTicks( Math.Max( v.Ticks, a.Ticks ) );
		}
		TimeSpan iMediaFile.duration => getDuration();

		sealed class VideoTrack: iVideoTrack
		{
			readonly Mp4File mp4;
			public VideoTrack( Mp4File mp4 )
			{
				this.mp4 = mp4;
				maxBytesInFrame = Math.Max( mp4.videoTrackSample.maxBytesInFrame,
					mp4.videoTrack.info.mediaInformation.sampleTable.sampleSize.maxSampleSize );
			}

			eVideoCodec iVideoTrack.codec => eVideoCodec.h264;
			eChromaFormat iVideoTrack.chromaFormat => ( (AVC1SampleEntry)mp4.videoTrackSample ).chromaFormat;

			sDecodedVideoSize iVideoTrack.decodedSize => mp4.videoTrackSample.getDecodedSize();

			readonly int maxBytesInFrame;
			int iVideoTrack.maxBytesInFrame => maxBytesInFrame;

			sPixelFormatMP iVideoTrack.getEncodedFormat() => mp4.videoTrackSample.getEncodedFormat();

			sPixelFormatMP iVideoTrack.getDecodedFormat() => mp4.videoTrackSample.getDecodedFormat();

			iVideoTrackReader iVideoTrack.createReader( EncodedQueue queue )
			{
				VideoSampleReader reader = VideoSampleReader.create( mp4 );
				var videoSample = mp4.videoTrackSample;

				// Decrypt the SPS and initialize sample reader with it.
				// Some slice header fields are present or missing based on the data in SPS, for this reason reader needs decrypted SPS.
				var sps = videoSample.parseSps();
				reader.setSps( ref sps );

#if DEBUG
				if( null == queue ) return reader;
#endif

				// Enqueue SPS and PPS NALUs
				var b = queue.nextEnqueue;
				videoSample.writeParameters( b, eParameterSet.SPS );
				queue.enqueue( b );

				b = queue.nextEnqueue;
				videoSample.writeParameters( b, eParameterSet.PPS );
				queue.enqueue( b );

				return reader;
			}
		};
		readonly VideoTrack m_videoTrack;
		iVideoTrack iMediaFile.videoTrack => m_videoTrack;

		sealed class Audio: iAudioTrack
		{
			readonly Mp4File mp4;
			public Audio( Mp4File mp4 ) { this.mp4 = mp4; }

			iAudioTrackReader iAudioTrack.createReader() => new VrmacVideo.Audio.Reader( mp4 );
			TrackInfo iAudioTrack.info => new TrackInfo( mp4 );
		}
		readonly Audio m_audio;
		iAudioTrack iMediaFile.audioTrack => m_audio;
	}
}