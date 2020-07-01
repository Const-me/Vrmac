using System;
using System.IO;
using System.Linq;
using Vrmac;

namespace RenderSamples
{
	/// <summary>Utility class to find the media file to play.</summary>
	/// <remarks>First it checks command-line arguments.
	/// Then, if running on a desktop (Win32 or Linux) it opens an OS-implemented file picker dialog.
	/// If running on bare metal Linux, command-line argument is the only option to specify the media file.</remarks>
	static class MediaFileName
	{
		public static string getPath( string[] args )
		{
			string p;
			if( args.Any() )
			{
				foreach( string a in args )
				{
					p = a;
					if( !Path.IsPathRooted( p ) )
						p = Path.Combine( Environment.CurrentDirectory, p );
					if( File.Exists( p ) )
						return p;
				}
			}

			p = null;

			try
			{
				if( RuntimeEnvironment.runningWindows )
					p = Win32.OpenFileName.getFileName();
				else
				{
					if( null == Environment.GetEnvironmentVariable( "DISPLAY" ) )
						Environment.SetEnvironmentVariable( "DISPLAY", ":0", EnvironmentVariableTarget.Process );
					p = GTK.OpenFileName.getFileName();
				}
			}
			catch( Exception ex )
			{
				Console.WriteLine( ex.Message );
			}

			if( null == p )
				Console.WriteLine( "No media file specified" );
			return p;
		}
	}
}