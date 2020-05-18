using Diligent.Graphics;
using System;
using System.Text;

namespace Vrmac
{
	/// <summary>Utility class to print log messages into colored console.</summary>
	/// <remarks>You probably don't want to use this in your production code, but for testing and debugging it's OK.</remarks>
	public static class ConsoleLogger
	{
		static readonly ConsoleColor[] s_colors = new ConsoleColor[ 5 ]
		{
			ConsoleColor.Red,
			ConsoleColor.Yellow,
			ConsoleColor.Green,
			ConsoleColor.DarkGreen,
			ConsoleColor.Blue,
		};

		static readonly string[] s_components = new string[ 4 ]
		{
			"",
			"GPU",
			"NC",
			"ModeSet",
		};

		static readonly pfnLogMessage pfnLog = logMessage;
		static readonly object syncRoot = new object();

		static void logMessage( eLogLevel level, eLogComponent component, string message, string source )
		{
			if( level == eLogLevel.Error )
				Utils.NativeErrorMessages.setNativeErrorMessage( message );

			ConsoleColor ccMessage = s_colors[ (byte)level ];
			string componentString = s_components[ (byte)component ];

			lock( syncRoot )
			{
				ConsoleColor ccPrev = Console.ForegroundColor;
				Console.ForegroundColor = ccMessage;
				if( null == source )
					Console.WriteLine( "{0}\t{1}", componentString, message );
				else
					Console.WriteLine( "{0}\t{1}\t{2}", componentString, message, source );
				Console.ForegroundColor = ccPrev;
			}
		}

		/// <summary>Control the log level.</summary>
		/// <remarks>Native code ignores this value, it only uses what's passed in <see cref="iGraphicsEngine.setLoggerSink(pfnLogMessage, eLogLevel)" />, but some C# code also produces log messages.</remarks>
		public static eLogLevel logLevel = eLogLevel.Info;

		/// <summary>Wire up logging of Diligent native library into colored console</summary>
		public static void setConsoleLoggerSink( this iGraphicsEngine native, eLogLevel maxLevel = eLogLevel.Info )
		{
			logLevel = maxLevel;
			native.setLoggerSink( pfnLog, maxLevel );
			GraphicsUtils.engineCreated( native );
		}

		/// <summary>Log a message to console</summary>
		public static void writeLine( eLogLevel level, string format, params object[] args )
		{
			if( level > logLevel )
				return;

			ConsoleColor ccMessage = s_colors[ (byte)level ];
			StringBuilder message = new StringBuilder();
			message.Append( "C#\t" );
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
	}
}