using ComLight;
using Vrmac;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace RenderSamples.Utils.Tests
{
	static class TestDispatcher
	{
		const string dll = "DiligentNative";
		[DllImport( dll, PreserveSig = false )]
		static extern void createEngine( [MarshalAs( UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof( Marshaler<iGraphicsEngine> ) )] out iGraphicsEngine engine );

		static Dispatcher mainDispatcher;

		static int id => Thread.CurrentThread.ManagedThreadId;

		static async Task fail()
		{
			Console.WriteLine( "{0} TestDispatcher.fail", id );
			await Task.Delay( 500 );
			Console.WriteLine( "{0} Throwing an exception", id );
			throw new ApplicationException( "Failed deliberately" );
			// mainThreadContext.nativeDispatcher.postQuitMessage( 0 );
		}

		static async void readKey( object state )
		{
			Console.ReadKey();
			Console.WriteLine( "{0} Got a key", id );
			await Task.Delay( 500 );
			Console.WriteLine( "{0} Posting a function", id );
			try
			{
				await mainDispatcher.postTask( fail );
			}
			catch( Exception ex )
			{
				Console.WriteLine( "{0} got the exception: {1}", id, ex.Message );
				mainDispatcher.postQuitMessage( ex.HResult );
			}
		}

		public static void test()
		{
			createEngine( out var engine );
			engine.setConsoleLoggerSink( eLogLevel.Debug );

			using( mainDispatcher = engine.dispatcher() )
			{
				ThreadPool.QueueUserWorkItem( readKey );

				Console.WriteLine( "{0} Created the dispatcher", id );
				mainDispatcher.run();
			}

			Console.WriteLine( "{0} The dispatcher quit normally", id );
		}
	}
}