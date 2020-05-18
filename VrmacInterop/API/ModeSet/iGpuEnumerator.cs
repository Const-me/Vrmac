using ComLight;
using System.Runtime.InteropServices;
using System;

namespace Vrmac.ModeSet
{
	/// <summary>Interface to enumerate GPUs</summary>
	[ComInterface( "c2e7e53e-5d6e-4815-b2cb-20041accc682", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iGpuEnumerator: IDisposable
	{
		/// <summary>Get the count of GPUs.</summary>
		[RetValIndex] int getAdaptersCount( [MarshalAs( UnmanagedType.U1 )] bool refresh = false );

		/// <summary>Open a first GPU</summary>
		[RetValIndex] iGpu openDefaultAdapter();

		/// <summary>Open a GPU by index</summary>
		[RetValIndex( 1 )]
		iGpu openAdapter( int index );
	}

	/// <summary>Couple extension methods for <see cref="iGpuEnumerator" /> COM interface.</summary>
	public static class GpuEnumeratorExt
	{
		/// <summary>Open a first GPU.</summary>
		/// <remarks>For some reason my RPi4 have 2 of them, /dev/dri/card0 and card1, only card1 works, card0 fails to open.</remarks>
		public static iGpu openFirstAdapter( this iGpuEnumerator ge )
		{
			int count = ge.getAdaptersCount();
			if( count <= 0 )
				throw new ApplicationException( "No GPUs found" );

			for( int i = 0; i < count - 1; i++ )
			{
				try
				{
					return ge.openAdapter( i );
				}
				catch( Exception ){ }
			}

			return ge.openAdapter( count - 1 );
		}
	}
}