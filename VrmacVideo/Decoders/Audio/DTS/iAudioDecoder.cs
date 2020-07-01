using ComLight;
using System;
using System.Runtime.InteropServices;

namespace VrmacVideo.Decoders.Audio.DTS
{
	[ComInterface( "6ef46e96-22c3-4100-9cee-c093723c0b33" )]
	public interface iAudioDecoder: IDisposable
	{
		unsafe int syncAndDecode( byte* buffer, byte volume );
		int blocksLeft();
		unsafe int decodeBlock( short* pcm );

		void clearHistory();

		int sampleRate();
		int channelsCount();
		int blockSize();
		bool copiesCompressedData();
	}

	static class AudioDecoder
	{
		const string dll = "DtsDecoder";

		[DllImport( dll, CallingConvention = RuntimeClass.defaultCallingConvention, PreserveSig = false )]
		public static extern void createDcaDecoder( [MarshalAs( UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof( Marshaler<iAudioDecoder> ) )] out iAudioDecoder decoder, byte channels, int sampleRate );

		[DllImport( dll, CallingConvention = RuntimeClass.defaultCallingConvention, PreserveSig = true )]
		public static extern IntPtr formatMessage( int code );
	}
}