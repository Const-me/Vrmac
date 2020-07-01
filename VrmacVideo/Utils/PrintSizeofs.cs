using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VrmacVideo
{
#if false
	// Utility class to print sizeof of all structures in this assembly into /tmp/sizeof-cs.tsv
	static class PrintSizeofs
	{
		static void print( StreamWriter sw )
		{
			var ass = Assembly.GetExecutingAssembly();
			foreach( var t in ass.GetTypes() )
			{
				if( !t.IsValueType )
					continue;
				if( t.IsEnum )
					continue;
				try
				{
					int cb = Marshal.SizeOf( t );
					sw.WriteLine( "{0}\t{1}", t.FullName, cb );
				}
				catch( Exception ex )
				{
					Logger.logVerbose( "Error reflecting type {0}: {1}", t.FullName, ex.Message );
				}
			}
		}

		public static void print()
		{
			using( var f = File.CreateText( "/tmp/sizeof-cs.tsv" ) )
				print( f );
		}
	}
#endif
}