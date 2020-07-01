using System;
using System.IO;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	/// <summary>V4L2 queue with encoded video frames</summary>
	sealed class EncodedQueue: QueueBase<EncodedBuffer>, IDisposable
	{
		public readonly int bufferCapacity;

		/// <summary>Round up max.sample length for potential NALU start codes. Also round up to the whole number of memory pages.</summary>
		public static int encodedVideoBufferSize( iVideoTrack videoTrack )
		{
			int maxSample = videoTrack.maxBytesInFrame;
			// Add a few bytes for safely. Might need the space for NALU start codes, if the video has 2 bytes NALU lengths.
			// Potentially, might also need couple bytes of space for emulation prevention bytes, if we'll find out the file doesn't have them but the decoder requires them.
			int res = maxSample + 64;
			// Round up by 4kb; `getconf PAGESIZE` console command printed "4096" on my Pi4
			return ( res + 0xFFF ) & ~0xFFF;
		}

		public void Dispose()
		{
			EncodedBuffer.dispose( buffers );
			GC.SuppressFinalize( this );
		}

		/// <summary>Allocate encoded buffers</summary>
		static int allocateBuffers( VideoDevice device, int encodedBuffersCount )
		{
			int buffersCount = device.allocateEncodedBuffers( encodedBuffersCount );
			if( buffersCount == encodedBuffersCount )
				Logger.logVerbose( "eControlCode.REQBUFS completed, created {0} encoded buffers", buffersCount );
			else
				Logger.logInfo( "eControlCode.REQBUFS: asked for {0} encoded buffer, GPU driver created {1} instead", buffersCount, buffersCount );
			return buffersCount;
		}

		public EncodedQueue( VideoDevice device, int encodedBuffersCount, ref sPixelFormatMP encodedFormat ) :
			base( allocateBuffers( device, encodedBuffersCount ), device.file )
		{
			bufferCapacity = encodedFormat.getPlaneFormat( 0 ).sizeImage;

			// Create encoded buffers, this does the memory mapping
			for( int i = 0; i < buffersCount; i++ )
			{
				var eb = new EncodedBuffer( device.file, i );
				buffers[ i ] = eb;
			}
		}

		/// <summary>Create sample reader and enqueue initial batch of NALUs, starting from SPS and PPS.</summary>
		/// <remarks>This enqueues all buffers in the queue.</remarks>
		public iVideoTrackReader enqueueInitial( iVideoTrack track )
		{
			iVideoTrackReader reader = track.createReader( this );

			// Enqueue moar NALUs from the video to saturate the encoded buffers
			while( true )
			{
				var b = nextEnqueue;
				if( null != b )
				{
					var act = reader.writeNextNalu( b );
					if( act == eNaluAction.EOF )
						throw new EndOfStreamException( "Too few samples in the video" );
					if( act == eNaluAction.Decode )
						enqueue( b );
				}
				else
					return reader;
			}
		}
	}
}