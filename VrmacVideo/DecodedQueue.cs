using ComLight;
using Diligent.Graphics;
using Diligent.Graphics.Video;
using System;
using System.Diagnostics;
using VrmacVideo.IO;
using VrmacVideo.Linux;

namespace VrmacVideo
{
	/// <summary>V4L2 queue with decoded video frames</summary>
	sealed class DecodedQueue: QueueBase<DecodedBuffer>
	{
		static int allocateBuffers( VideoDevice videoDevice, int buffersCount )
		{
			// Allocate decoded buffers
			int c = videoDevice.allocateDecodedFrames( buffersCount );
			if( c == buffersCount )
				Logger.logVerbose( "allocateDecodedFrames created {0} frames", buffersCount );
			else
				Logger.logVerbose( "allocateDecodedFrames asked for {0} frames, allocated {1} instead", buffersCount, c );
			return c;
		}

		public DecodedQueue( VideoDevice videoDevice, int dbc ) :
			base( allocateBuffers( videoDevice, dbc ), videoDevice.file )
		{
			for( int i = 0; i < buffersCount; i++ )
			{
				var db = new DecodedBuffer( i, videoDevice );
				buffers[ i ] = db;
			}
		}

		public Nv12Texture[] textures { get; private set; }

		/// <summary>Export all buffers from V4L2, import them into GLES in Diligent Engine</summary>
		public void exportTextures( IRenderDevice renderDevice, VideoDevice device, ref sPixelFormatMP pixelFormat, ref ColorFormat color )
		{
			iGlesRenderDevice gles = ComLightCast.cast<iGlesRenderDevice>( renderDevice );
			textures = new Nv12Texture[ buffers.Length ];
			for( int i = 0; i < buffers.Length; i++ )
				textures[ i ] = buffers[ i ].exportNv12( gles, device, ref pixelFormat, ref color );
		}

		/* public SaveTgaFrames dbgMapOutput( VideoDevice device, ref sPixelFormatMP pixelFormat )
		{
			MappedOutput[] res = new MappedOutput[ buffers.Length ];
			for( int i = 0; i < buffers.Length; i++ )
				res[ i ] = buffers[ i ].dbgMapOutput( device, ref pixelFormat );
			return new SaveTgaFrames( res );
		} */

		public void Dispose()
		{
			if( null != textures )
			{
				foreach( var t in textures )
					t.finalize();
				textures = null;
			}
			DecodedBuffer.dispose( buffers );
			GC.SuppressFinalize( this );
		}

		public void enqueueAll()
		{
			while( true )
			{
				DecodedBuffer b = nextEnqueue;
				if( null == b )
					return;
				enqueue( b );
			}
		}

		public void enqueueByIndex( int idx )
		{
			DecodedBuffer b = buffers[ idx ];
			Debug.Assert( b.state == eBufferState.User );
			enqueue( b );
		}

		public TimeSpan getPresentationTime( int idx )
		{
			DecodedBuffer b = buffers[ idx ];
			Debug.Assert( b.state == eBufferState.User );
			return b.timestamp;
		}
	}
}