using System;
using System.Reflection;

namespace RenderSamples
{
	static class Program
	{
		static void dbgPrintResourceNames()
		{
			foreach( string n in Assembly.GetExecutingAssembly().GetManifestResourceNames() )
				Console.WriteLine( n );
		}

		static void mainImpl( string[] args )
		{
			// dbgPrintResourceNames();

			SampleBase sample;
			// sample = new HelloTriangle();
			// sample = new Tutorial02_Cube();
			// sample = new Tutorial03_Texturing();
			sample = new SpinningTeapot();
			// sample = new ShapesSample();
			// sample = new TigerSvgSample();
			// sample = new SpritesSample();
			// sample = new TextSample();

			SampleRenderer.runSample( sample );
		}

		static void Main( string[] args )
		{
			try
			{
				// Utils.Tests.TestMatrices.test(); return;
				// Utils.Tests.TestDispatcher.test();
				// Utils.Tests.TestUsbIDs.test();
				// Utils.Tests.TestUsbIDs.printAll();
				mainImpl( args );
				// Utils.Tests.TestRawInput.testMouse();
				// Utils.Tests.TestRawInput.testKeyboard();
				// Utils.Tests.TestQuaternions.test();
				// Utils.Tests.TestRiff.test();
			}
			catch( Exception ex )
			{
				Console.WriteLine( "Failed: {0}\n{1}", ex.Message, ex.ToString() );
			}
		}
	}
}