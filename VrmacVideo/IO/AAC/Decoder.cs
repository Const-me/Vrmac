using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.IO.AAC
{
	sealed class Decoder: IDisposable
	{
		const string dll = "libfdk-aac.so.1";

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern IntPtr aacDecoder_Open( eTransportType transportType, int countOfLayers );
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern void aacDecoder_Close( IntPtr decoder );
		IntPtr m_decoder;
		readonly int countOfLayers;

		public Decoder( eTransportType transportType, int countOfLayers )
		{
			m_decoder = aacDecoder_Open( transportType, countOfLayers );
			if( m_decoder == IntPtr.Zero )
				throw new ApplicationException( "Unable to create AAC decoder" );
			this.countOfLayers = countOfLayers;
		}

		public void Dispose()
		{
			if( IntPtr.Zero != m_decoder )
			{
				aacDecoder_Close( m_decoder );
				m_decoder = default;
			}
			GC.SuppressFinalize( this );
		}

		~Decoder()
		{
			if( IntPtr.Zero != m_decoder )
				aacDecoder_Close( m_decoder );
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern eAacStatus aacDecoder_ConfigRaw( IntPtr decoder, IntPtr ppBuffers, IntPtr pLengths );

		/// <summary>Explicitly configure the decoder by passing a raw AudioSpecificConfig (ASC) or a StreamMuxConfig (SMC), contained in a binary buffer.</summary>
		/// <remarks>This is required for MPEG-4 and Raw Packets file format bitstreams as well as for LATM bitstreams with no in-band SMC.
		/// If the transport format is LATM with or without LOAS, configuration is assumed to be an SMC, for all other file formats an ASC.</remarks>
		public void configRaw( ReadOnlySpan<byte> buffer )
		{
			if( 1 != countOfLayers )
				throw new ArgumentException( "This version of configRaw method is only compatible with single-layer decoding" );
			if( buffer.IsEmpty )
				throw new ArgumentException( "AAC configuration blob probably shouldn’t be empty." );
			if( m_decoder == default )
				throw new ApplicationException( "Uninitialized decoder" );

			unsafe
			{
				int length = buffer.Length;
				fixed ( byte* pointer = buffer )
				{
					aacDecoder_ConfigRaw( m_decoder, (IntPtr)( &pointer ), (IntPtr)( &length ) )
						.check( "aacDecoder_ConfigRaw" );
				}
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static extern eAacStatus aacDecoder_SetParam( IntPtr decoder, eParameter parameter, int value );

		/// <summary>Set a decoder parameter</summary>
		public void setParameter( eParameter parameter, int value )
		{
			if( m_decoder == default )
				throw new ApplicationException( "Uninitialized decoder" );
			aacDecoder_SetParam( m_decoder, parameter, value )
				.check( "aacDecoder_SetParam" );
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern eAacStatus aacDecoder_Fill( IntPtr decoder, byte** buffers, int* bufferLength, int* bytesValid );

		/// <summary>Fill AAC decoder's internal input buffer with bitstream data from the external input buffer.</summary>
		/// <remarks>Returns count of bytes consumed from the span. Use ReadOnlySpan<byte>.Slice to call it next time.</remarks>
		public int fillInputBuffer( ReadOnlySpan<byte> buffer )
		{
			if( 1 != countOfLayers )
				throw new ArgumentException( "fillInputBuffer is only compatible with single-layer decoding" );

			unsafe
			{
				int length = buffer.Length;
				int bytesValid = buffer.Length;
				fixed ( byte* pointer = buffer )
				{
					// The C++ code uses these values like this:
					// inputBuffer = &inputBuffer[bufferSize - *bytesValid];
					// Have no idea why, would be more reasonable to take pointer to first valid element + integer length of the data, like everyone else is doing.
					aacDecoder_Fill( m_decoder, &pointer, &length, &bytesValid )
						.check( "aacDecoder_Fill" );
					return buffer.Length - bytesValid;
				}
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern eAacStatus aacDecoder_DecodeFrame( IntPtr decoder, short* pcm, int length, eDecodeFrameFlags flags );

		public void decodeFrame( Span<short> decodedPcm, eDecodeFrameFlags flags = eDecodeFrameFlags.None )
		{
			unsafe
			{
				fixed ( short* pointer = decodedPcm )
					aacDecoder_DecodeFrame( m_decoder, pointer, decodedPcm.Length, flags )
						.check( "aacDecoder_DecodeFrame" );
			}
		}

		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern CStreamInfo* aacDecoder_GetStreamInfo( IntPtr decoder );

		/// <summary>Provides information about the currently decoded audio data.</summary>
		public CStreamInfo streamInfo
		{
			get
			{
				CStreamInfo si;
				unsafe
				{
					si = *aacDecoder_GetStreamInfo( m_decoder );
				}
				return si;
			}
		}

		/*
		[DllImport( dll, SetLastError = false, CallingConvention = CallingConvention.Cdecl )]
		static unsafe extern int aacDecoder_GetLibInfo( LibraryInfo* libInfo );

		public const int modulesCount = 32;

		public static void getLibraryInfo( Span<LibraryInfo> info )
		{
			if( info.Length < modulesCount )
				throw new ArgumentException( $"Buffer too small, must have { modulesCount } modules" );
			unsafe
			{
				fixed ( LibraryInfo* pointer = info )
				{
					int res = aacDecoder_GetLibInfo( pointer );
					if( 0 == res )
						return;
					throw new ApplicationException( $"aacDecoder_GetLibInfo returned status code { res }" );
				}
			}
		} */
	}
}