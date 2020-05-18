using ComLight;
using ComLight.IO;
using System;

namespace Diligent.Graphics.Buffers
{
	[ComInterface( "79f4e602-9af1-4bc9-8652-ac6c6202b85f", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	interface iBufferUploadStream: IDisposable
	{
		/// <summary>write a sequence of bytes to the current stream and advance the current position within this stream by the number of bytes written.</summary>
		void write( IntPtr lpBuffer, int nNumberOfBytesToWrite );

		/// <summary>Set the position within the current stream.</summary>
		void seek( uint offset, eSeekOrigin origin );
		/// <summary>Get the position within the current stream.</summary>
		void getPosition( out uint length );
		/// <summary>Get the length in bytes of the stream.</summary>
		void getLength( out uint length );
	}
}