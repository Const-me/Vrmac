using ComLight;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using Vrmac;
using Vrmac.Input.Linux;

namespace RenderSamples.Utils.Tests
{
	static class TestRawInput
	{
		const eLogLevel consoleLoggingLevel = eLogLevel.Verbose;
		const string dll = "DiligentNative";
		[DllImport( dll, PreserveSig = false )]
		static extern void createEngine( [MarshalAs( UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof( Marshaler<iGraphicsEngine> ) )] out iGraphicsEngine engine );

		class EventPrinter: iRawInputSink
		{
			void iRawInputSink.failed( int hResult )
			{
				Console.WriteLine( "EventPrinter: failed" );
			}
			void iRawInputSink.handle( ulong timestamp, eEventType eventType, ushort code, int value )
			{
				Console.WriteLine( "EventPrinter: {0} {1} {2} {3}", eventType, code, value, timestamp.timeFromUnixNano() );
			}
			void iRawInputSink.updated()
			{
				Console.WriteLine( "EventPrinter: updated" );
			}
		}

		public static void test()
		{
			createEngine( out var engine );
			engine.setConsoleLoggerSink( consoleLoggingLevel );

			var devices = RawDevice.list().ToArray();

			using( var disp = engine.dispatcher() )
			{
				Console.WriteLine( "Created a dispatcher" );
				using( var linuxDisp = ComLightCast.cast<iLinuxDispatcher>( disp.nativeDispatcher ) )
				{
					// Console.WriteLine( "Casted to iLinuxDispatcher" );
					string path = devices[ 0 ].eventInterface;
					linuxDisp.openInputDevice( path, new EventPrinter() );
					disp.run();
				}
			}
		}

		public static void testMouse()
		{
			createEngine( out var engine );
			engine.setConsoleLoggerSink( consoleLoggingLevel );
			using( var disp = engine.dispatcher() )
			{
				disp.openRawMouse( new MousePrinter() );
				disp.run();
			}
		}

		public static void testKeyboard()
		{
			iRawInputSink sink;
			// sink = new RawEventsLogger();
			sink = new LayoutTest();

			createEngine( out var engine );
			engine.setConsoleLoggerSink( consoleLoggingLevel );
			using( var disp = engine.dispatcher() )
			{
				// string device = RawDevice.list().FirstOrDefault( RawInput.isQwertyKeyboard ).eventInterface;
				string device = @"/dev/input/event4";

				using( iLinuxDispatcher linuxDispatcher = ComLightCast.cast<iLinuxDispatcher>( disp.nativeDispatcher ) )
					linuxDispatcher.openInputDevice( device, sink );
				disp.run();
			}
		}
	}
}