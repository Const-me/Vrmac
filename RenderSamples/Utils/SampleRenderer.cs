using ComLight;
using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Vrmac;
using Vrmac.ModeSet;

namespace RenderSamples
{
	static class SampleRenderer
	{
		const eLogLevel consoleLoggingLevel = eLogLevel.Verbose;

		/// <summary>When running full screen, will switch to this resolution.</summary>
		/// <remarks>If it's not supported by GPU + display combination, will fail with exception.</remarks>
		static readonly CSize fullscreenResolution = new CSize( 1920, 1080 );

		/// <summary>When running full screen, will use this format if it's available.</summary>
		/// <remarks>If it's not supported by GPU + display combination, will pick another one.</remarks>
		const eDrmFormat idealDrmFormat = eDrmFormat.ARGB8888;

		const string dll = "Vrmac";
		[DllImport( dll, PreserveSig = false )]
		static extern void createEngine( [MarshalAs( UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof( Marshaler<iGraphicsEngine> ) )] out iGraphicsEngine engine );

		static void runFullScreen( iGraphicsEngine engine, SampleBase sample )
		{
			iVideoSetup videoSetup = new Utils.VideoSetup( idealDrmFormat );
			var dispatcher = engine.dispatcher();
			ThreadPool.QueueUserWorkItem( obj => { Console.ReadKey(); dispatcher.postQuitMessage( 0 ); } );

			engine.renderFullScreen( sample.context, fullscreenResolution, videoSetup );
		}

		static void runWindowed( iGraphicsEngine engine, SampleBase sample )
		{
			iWindowSetup setup = new Utils.WindowSetup();
			engine.renderWindowed( sample.context, null, setup );
		}

		public static void runSample( SampleBase sample )
		{
			createEngine( out var engine );
			engine.setConsoleLoggerSink( consoleLoggingLevel );

			// Utils.Tests.PrintConnectors.print( engine );
			try
			{
				eCapabilityFlags capabilityFlags = engine.getCapabilityFlags();

				if( capabilityFlags.HasFlag( eCapabilityFlags.GraphicsWindowed ) )
					runWindowed( engine, sample );
				else
				{
					if( !capabilityFlags.HasFlag( eCapabilityFlags.GraphicsFullscreen ) )
						throw new ApplicationException( "The native library doesn't implement any 3D rendering" );
					runFullScreen( engine, sample );
				}
			}
			catch( ShaderCompilerException sce )
			{
				sce.saveSourceCode();
				throw;
			}
			finally
			{
				engine.clearLoggerSink();
			}
		}
	}
}