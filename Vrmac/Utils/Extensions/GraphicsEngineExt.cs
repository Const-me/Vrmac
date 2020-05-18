using Vrmac.Input.X11;

namespace Vrmac
{
	/// <summary>Implements couple extension methods on top of <see cref="iGraphicsEngine" /> COM interface.</summary>
	public static class GraphicsEngineExt
	{
		/// <summary>Uninitialize logging; any log messages produced after this call will go straight to /dev/null</summary>
		public static void clearLoggerSink( this iGraphicsEngine engine )
		{
			engine.setLoggerSink( null, eLogLevel.Error );
		}

		/// <summary>Get capability flags implemented by the currently loaded native library.</summary>
		public static eCapabilityFlags getCapabilityFlags( this iGraphicsEngine engine )
		{
			return engine.getLibraryInfo().capabilityFlags;
		}

		/// <summary>Create a .NET dispatcher for the current thread.</summary>
		/// <remarks>If the current thread already has one created, return the old one instead.</remarks>
		public static Dispatcher dispatcher( this iGraphicsEngine engine )
		{
			return Dispatcher.currentDispatcher ?? new Dispatcher( engine.createDispatcher() );
		}

		/// <summary>Before creating any windows, please call this. Will only do something on Linux.</summary>
		public static void loadKeySyms( this iGraphicsEngine engine )
		{
			if( RuntimeEnvironment.operatingSystem != eOperatingSystem.Linux )
				return;
			engine.uploadKeySymMap();
		}
	}
}