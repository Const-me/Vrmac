using System;
using System.Diagnostics;

namespace Diligent.Graphics
{
	/// <summary>Utility class to compute FPS, rolling averages across a few frames.</summary>
	public class FramesPerSecond
	{
		readonly Stopwatch stopwatch = Stopwatch.StartNew();

		const int capacity = 16;
		readonly TimeSpan[] times = new TimeSpan[ capacity ];
		int lastIndex = 0;

		/// <summary>Mark time when a frame is rendered</summary>
		public void rendered()
		{
			TimeSpan now = stopwatch.Elapsed;
			lastIndex = ( lastIndex + 1 ) % capacity;
			times[ lastIndex ] = now;
		}

		const float secondsMul = (float)( (double)( capacity - 1 ) * (double)TimeSpan.TicksPerSecond );

		float? computeFps()
		{
			TimeSpan last = times[ lastIndex ];
			TimeSpan first = times[ ( lastIndex + 1 ) % capacity ];
			if( first == default )
				return null;
			long ticks = last.Ticks - first.Ticks;
			return secondsMul / (float)( ticks );
		}

		/// <summary>Current FPS, average over the most recent 15 frames</summary>
		public float? framesPerSecond => computeFps();
	}
}