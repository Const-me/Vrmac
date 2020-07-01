using ComLight;
using Diligent.Graphics;
using Vrmac.Draw;

namespace Vrmac
{
	/// <summary>The main COM object of the library.</summary>
	/// <remarks>It's pointless to dispose, in C++ code this object is a singleton.</remarks>
	[ComInterface( "5dcc3930-15a6-449e-9c9e-4a6a93adf092", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iGraphicsEngine
	{
		/// <summary>Initialize logging.</summary>
		/// <remarks>It's your responsibility to make sure the logging delegate is not garbage collected until you reset it by calling setLoggerSink again, with null argument.</remarks>
		void setLoggerSink( pfnLogMessage sink, eLogLevel maxLevel );

		/// <summary>Get some information about the currently used native library.</summary>
		[RetValIndex]
		sNativeLibraryInfo getLibraryInfo();

		/// <summary>Create an object which enumerates GPUs, attached displays, supported video modes, and can create contexts for full-screen rendering.</summary>
		/// <remarks>Currently, this only implemented for Linux, and full-screen contexts require OS setup which does not run X windows or Wayland.</remarks>
		[RetValIndex]
		ModeSet.iGpuEnumerator createGpuEnumerator();

		/// <summary>Create Diligent 3D renderer on top of the full-screen rendering context.</summary>
		[RetValIndex]
		iRenderingContext createFullScreenContext( iContent content, ModeSet.iDisplayRenderContext ctx );

		/// <summary>Get native dispatcher for the current thread, or create a new one if the thread doesn't have one yet.</summary>
		[RetValIndex] iDispatcher createDispatcher();

		/// <summary>Create factory object to initialize 2D rendering</summary>
		[RetValIndex] iDrawFactory createDrawFactory();

		/// <summary>Utility object with a couple manually optimized SIMD routines</summary>
		void getSimdUtils( out Utils.iSimdUtils result );
		/// <summary>Utility object with a couple manually optimized SIMD routines</summary>
		Utils.iSimdUtils simdUtils { get; }

		/// <summary>Create a media engine.</summary>
		/// <remarks>Only works on Windows</remarks>
		[RetValIndex] MediaEngine.iMediaEngine createMediaEngine( IRenderDevice device, TextureFormat outputFormat );
	}
}