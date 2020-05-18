using System;
using System.Diagnostics;

namespace Vrmac.Animation
{
	/// <summary>Animation timers. All of them update exactly once per frame.</summary>
	public sealed class Timers
	{
		/// <summary>Elapsed time of the specified timer</summary>
		public TimeSpan this[ eAnimationTimer timer ]
		{
			get
			{
				return readings[ (byte)timer ];
			}
		}

		/// <summary>Delta time of the specified timer, in seconds since the previous frame rendered</summary>
		/// <remarks>For absolute and especially for the wallclock timer, be prepared for large values.
		/// If the window is left minimized for prolonged period of time, you’ll get hours or even days from this method.</remarks>
		public float delta( eAnimationTimer timer )
		{
			return deltas[ (byte)timer ];
		}

		readonly TimeSpan[] readings;
		readonly float[] deltas;
		readonly TimeSpan[] previousReadings;
		readonly Stopwatch[] stopwatches;

		internal Timers()
		{
			stopwatches = new Stopwatch[ 3 ];
			for( int i = 0; i < 3; i++ )
				stopwatches[ i ] = new Stopwatch();
			// Arrays in .NET are zero initialized.
			readings = new TimeSpan[ 3 ];
			previousReadings = new TimeSpan[ 3 ];
			deltas = new float[ 3 ];
		}

		internal void start()
		{
			for( int i = 0; i < 3; i++ )
				stopwatches[ i ].Restart();
		}

		internal void update()
		{
			for( int i = 0; i < 3; i++ )
			{
				TimeSpan now = stopwatches[ i ].Elapsed;
				TimeSpan prev = readings[ i ];
				readings[ i ] = now;
				if( previousReadings[ i ] != default )
				{
					previousReadings[ i ] = prev;
					deltas[ i ] = (float)( ( now - prev ).TotalSeconds );
				}
				else
				{
					previousReadings[ i ] = now;
					deltas[ i ] = 0;
				}
			}
		}

		internal void pause()
		{
			stopwatches[ 0 ].Stop();
		}

		internal void resume()
		{
			stopwatches[ 0 ].Start();
			previousReadings[ 0 ] = default;
		}

		class HardPause: IDisposable
		{
			readonly Timers timers;
			readonly bool relativeRunning;

			public HardPause( Timers timers )
			{
				this.timers = timers;
				relativeRunning = timers.stopwatches[ 0 ].IsRunning;
				if( relativeRunning )
					timers.pause();
				timers.stopwatches[ 1 ].Stop();
			}

			void IDisposable.Dispose()
			{
				if( relativeRunning )
					timers.resume();
				timers.stopwatches[ 1 ].Start();
			}
		}

		internal IDisposable hardPause()
		{
			return new HardPause( this );
		}
	}
}