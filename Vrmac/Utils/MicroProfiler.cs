using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vrmac
{
	sealed class MicroProfiler
	{
		readonly Stopwatch sw = Stopwatch.StartNew();
		readonly TimeSpan reportFrequency = TimeSpan.FromSeconds( 3 );
		TimeSpan nextReport;

		MicroProfiler()
		{
			nextReport = reportFrequency;
		}

		sealed class Measure
		{
			TimeSpan ts = TimeSpan.Zero;
			int count = 0;
			public Measure( TimeSpan t )
			{
				ts = t;
				count = 1;
			}
			public void add( TimeSpan t )
			{
				ts += t;
				count++;
			}
			public override string ToString()
			{
				double ms = TimeSpan.FromTicks( ts.Ticks / count ).TotalMilliseconds;
				return string.Format( "{0:G3} ms", ms );
			}
		}

		readonly Dictionary<string, Measure> dict = new Dictionary<string, Measure>();

		void update( string key, TimeSpan value )
		{
			if( dict.TryGetValue( key, out var m ) )
				m.add( value );
			else
				dict.Add( key, new Measure( value ) );
		}

		bool finishedOnce = false;
		TimeSpan last;
		public static void start()
		{
			instance.last = instance.sw.Elapsed;
		}
		void finishImpl()
		{
			var e = sw.Elapsed;
			update( "present", e - last );
			if( e >= nextReport )
			{
				logReport();
				nextReport = e + reportFrequency;
			}
		}
		public static void finish()
		{
			if( instance.finishedOnce )
				instance.finishImpl();
			else
				instance.finishedOnce = true;
		}

		void add( string what )
		{
			if( !finishedOnce )
				return;
			var e = sw.Elapsed;
			update( what, e - last );
			last = e;
		}
		public static void key( string k )
		{
			instance.add( k );
		}
		public static void methodBegin( [CallerMemberName] string memberName = null )
		{
			instance.add( memberName + " started" );
		}
		public static void methodEnd( [CallerMemberName] string memberName = null )
		{
			instance.add( memberName + " finished" );
		}
		void logReport()
		{
			string msg = string.Join( "; ",
				dict.Select( kvp => $"{kvp.Key}: {kvp.Value}" ) );
			ConsoleLogger.logInfo( "MicroProfiler report: {0}", msg );
		}

		static readonly MicroProfiler instance = new MicroProfiler();
	}
}