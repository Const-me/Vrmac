using ComLight;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.ModeSet
{
	/// <summary>Represents a GPU found in this PC</summary>
	/// <see href="https://en.wikipedia.org/wiki/Graphics_processing_unit" />
	[ComInterface( "cec875d7-efea-4a5c-b620-42cf7f7a551c", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iGpu: IDisposable
	{
		/// <summary>Get information about the GPU</summary>
		[RetValIndex] sGpuInfo getInfo();

		/// <summary>Open a connector by index.</summary>
		/// <remarks>This method only fails if you'll specify index outside of the valid range, [ 0 .. <see cref="sGpuInfo.numConnectors" /> - 1 ].
		/// If the index is good but nothing is connected to that connector, you'll receive a result,
		/// where <see cref="sConnectorInfo.flags" /> fields lacks the <see cref="eConnectorFlags.Connected" /> bit.</remarks>
		[RetValIndex] iGpuConnector openConnector( int index, [MarshalAs( UnmanagedType.U1 )] bool refresh = false );

		/// <summary>Open a default GPU connector.</summary>
		[RetValIndex] iGpuConnector openDefaultConnector( [MarshalAs( UnmanagedType.U1 )] bool refresh = false );
	}
}