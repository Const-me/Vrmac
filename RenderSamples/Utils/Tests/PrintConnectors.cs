using System;
using Vrmac;
using Vrmac.ModeSet;

namespace RenderSamples.Utils.Tests
{
	static class PrintConnectors
	{
		public static void print( iGraphicsEngine engine )
		{
			using( var dispatcher = engine.dispatcher() )
			using( var gpuEnum = engine.createGpuEnumerator() )
			using( var gpu = gpuEnum.openFirstAdapter() )
			{
				int count = gpu.getInfo().numConnectors;
				for( int i = 0; i < count; i++ )
				{
					using( var c = gpu.openConnector( i ) )
						Console.WriteLine( c.getInfo().ToString() );
				}
			}
		}
	}
}