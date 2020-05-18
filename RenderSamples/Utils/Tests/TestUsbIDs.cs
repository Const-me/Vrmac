using Vrmac.Input.Linux;
using System;

namespace RenderSamples.Utils.Tests
{
	/* static class TestUsbIDs
	{
		public static void test()
		{
#if DEBUG
			ushort id = 0x045e;
			string name = UsbVendors.print( id );
			Console.WriteLine( name );

			ushort dev = 0x02e6;
			// dev = 0x077f;
			name = UsbDevices.lookup( id, dev );
			Console.WriteLine( name );
#else
			throw new NotImplementedException();
#endif
		}

		public static void printAll()
		{
			foreach( var dev in RawDevice.list() )
			{
				Console.WriteLine( dev.printDetails() );
				Console.WriteLine();
			}
		}
	} */
}