using Vrmac.Utils;
using System;
using System.IO;
using Vrmac.Utils.Cursor.Load;

namespace RenderSamples.Utils.Tests
{
	static class TestRiff
	{
		const string path = @"C:\Temp\2remove\Win7Cursors\aero_working.ani";

		static void consumeChunk( iRiffChunk chunk )
		{
			Console.WriteLine( chunk );
		}
		static void testRiff()
		{
			using( var f = File.OpenRead( path ) )
				throw new NotImplementedException();
				// RiffParser.parse( f, consumeChunk );
		}
		static void testAni()
		{
			AniFile file;
			using( var f = File.OpenRead( path ) )
				file = new AniFile( f );

			Console.WriteLine( file );

			CursorTexture texture;
			using( var f = File.OpenRead( path ) )
				texture = file.load( null, f, null );
		}

		public static void test()
		{
			// testRiff();
			testAni();
		}
	}
}