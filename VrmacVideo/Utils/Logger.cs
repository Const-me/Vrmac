using System;
using System.Text;
using Vrmac;

namespace VrmacVideo
{
	// TODO: maybe pass a delegate to reuse ConsoleLogger class from VrmacGraphics DLL.
	static class Logger
	{
		const eLogLevel logLevel = eLogLevel.Verbose;
		static readonly object syncRoot = new object();

		static readonly ConsoleColor[] s_colors = new ConsoleColor[ 5 ]
		{
			ConsoleColor.Red,
			ConsoleColor.Yellow,
			ConsoleColor.Green,
			ConsoleColor.DarkGreen,
			ConsoleColor.Blue,
		};

		public static void writeLine( eLogLevel level, string format, params object[] args )
		{
			if( level > logLevel )
				return;

			ConsoleColor ccMessage = s_colors[ (byte)level ];
			StringBuilder message = new StringBuilder();
			message.Append( "Video\t" );
			message.AppendFormat( format, args );
			string str = message.ToString();

			lock( syncRoot )
			{
				ConsoleColor ccPrev = Console.ForegroundColor;
				Console.ForegroundColor = ccMessage;
				Console.WriteLine( str );
				Console.ForegroundColor = ccPrev;
			}
		}

		/// <summary>Log a verbose message</summary>
		public static void logVerbose( string format, params object[] args )
		{
			writeLine( eLogLevel.Verbose, format, args );
		}
		/// <summary>Log a debug message</summary>
		public static void logDebug( string format, params object[] args )
		{
			writeLine( eLogLevel.Debug, format, args );
		}
		/// <summary>Log an informational message</summary>
		public static void logInfo( string format, params object[] args )
		{
			writeLine( eLogLevel.Info, format, args );
		}
		/// <summary>Log a warning message</summary>
		public static void logWarning( string format, params object[] args )
		{
			writeLine( eLogLevel.Warning, format, args );
		}
		/// <summary>Log an error message</summary>
		public static void logError( this Exception ex, string what )
		{
			writeLine( eLogLevel.Error, "{0}: {1}", what, ex.Message );
		}
		/// <summary>Log an error message</summary>
		public static void logError( string format, params object[] args )
		{
			writeLine( eLogLevel.Error, format, args );
		}
	}
}