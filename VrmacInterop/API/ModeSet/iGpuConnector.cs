using ComLight;
using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.ModeSet
{
	/// <summary>Represents a GPU's output connector.</summary>
	[ComInterface( "906acdc8-dd91-408f-9532-a0375636f430", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iGpuConnector: IDisposable
	{
		/// <summary>Get the GPU of this connector</summary>
		[RetValIndex] iGpu getGpu();

		/// <summary>Get information about the connector, and optionally refresh that data by re-querying the display if it's connected</summary>
		[RetValIndex] sConnectorInfo getInfo( [MarshalAs( UnmanagedType.U1 )] bool refresh = false );

		/// <summary>Get a preferred video mode of the attached monitor.</summary>
		bool getPreferredMode( out sVideoMode mode );

		/// <summary>Get the current video mode of the attached monitor.</summary>
		bool getCurrentMode( out sVideoMode mode );
		/// <summary>Find the best video mode of the specified resolution.</summary>
		bool findVideoMode( out sVideoMode mode, [In] ref CSize sizePixels );

		/// <summary>Enumerate supported video modes and returns the count of these modes.</summary>
		[RetValIndex]
		int enumModes();

		/// <summary>Get all video modes supported by the display. You must call enumModes() first, to find out the required size of the array to pass here.</summary>
		void getAllModes( [Out, MarshalAs( UnmanagedType.LPArray )] sVideoMode[] videoModes );

		/// <summary>Find the best video mode of the specified resolution, switch to that mode and initialize rendering using some default frame buffer format, typically 8 bit/channel RGB.</summary>
		[RetValIndex]
		iDisplayRenderContext createContext( ref CSize resolution );

		/// <summary>Switch to the specified video mode, and initialize rendering.</summary>
		[RetValIndex]
		iDisplayRenderContext createContext( int index, iVideoSetup videoSetup = null );

		/// <summary>Set surface format to use when looking for video modes.</summary>
		/// <remarks>You must call this on Windows. On Linux it does nothing.</remarks>
		void setSurfaceFormat( TextureFormat surfaceFormat );
	}

	/// <summary>Extension methods for iGpuConnector</summary>
	public static class GpuConnectorExt
	{
		/// <summary>Get all video modes supported by the display, in a single call</summary>
		public static sVideoMode[] getAllModes( this iGpuConnector connector )
		{
			sVideoMode[] result = new sVideoMode[ connector.enumModes() ];
			connector.getAllModes( result );
			return result;
		}
	}
}