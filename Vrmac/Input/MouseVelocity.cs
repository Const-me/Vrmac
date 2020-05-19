using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Vrmac.Input
{
	/// <summary>Utility class to compute mouse velocity.</summary>
	/// <seealso href="https://en.wikipedia.org/wiki/Ordinary_least_squares" />
	public class MouseVelocity
	{
		const int capacity = 4;

		// It only has integers there because we save a few nanoseconds by not computing the sum, instead subtracting replaced values.
		// The trick only works for integers, for floats the precision issues kick in: (A+B)-B == A true for integers, not true for floats.
		struct DataPoint
		{
			public int x, y;
			public long time;

			public DataPoint( CPoint pt, long time )
			{
				x = pt.x;
				y = pt.y;
				this.time = time;
			}

			DataPoint( int x, int y, long time )
			{
				this.x = x;
				this.y = y;
				this.time = time;
			}

			public static DataPoint operator +( DataPoint a, DataPoint b )
			{
				return new DataPoint( a.x + b.x, a.y + b.y, a.time + b.time );
			}
			public static DataPoint operator -( DataPoint a, DataPoint b )
			{
				return new DataPoint( a.x - b.x, a.y - b.y, a.time - b.time );
			}
		}

		readonly DataPoint[] points = new DataPoint[ capacity ];
		DataPoint sum;
		int countValues = 0;
		int nextIndex = 0;

		/// <summary>Remove all data from this class</summary>
		public void reset()
		{
			countValues = nextIndex = 0;
			sum = new DataPoint();
		}

		/// <summary>Add a data point</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void add( CPoint pt, TimeSpan time )
		{
			if( countValues >= capacity )
				sum -= points[ nextIndex ];
			else
				countValues++;
			// BTW, the range of TimeSpan is enough to add ~500 measures if each one is from 1970 to now. We only have up to 4 measures in this class, so we're good.

			DataPoint np = new DataPoint( pt, time.Ticks );
			points[ nextIndex ] = np;
			sum += np;
			nextIndex = ( nextIndex + 1 ) % capacity;
		}

		/// <summary>Compute mouse velocity in pixels/second.</summary>
		public Vector2? compute()
		{
			if( countValues < 2 )
				return null;

			// https://en.wikipedia.org/wiki/Simple_linear_regression#Fitting_the_regression_line
			// https://stackoverflow.com/a/18974171/126995

			double invCount = 1.0 / countValues;
			double averageX = invCount * sum.x;
			double averageY = invCount * sum.y;
			long averageTime = sum.time / countValues;

			double nx = 0, ny = 0, den = 0;
			for( int i = 0; i < countValues; i++ )
			{
				DataPoint dp = points[ i ];

				double devX = dp.x - averageX;
				double devY = dp.y - averageY;
				double devTime = ( dp.time - averageTime );

				nx += devTime * devX;
				ny += devTime * devY;
				den += devTime * devTime;
			}

			if( 0 == den )
				return null;	// This means all data points have the same time stamp.

			// So far, the formula would gives the speed in pixel / tick where the tick = 100 nanoseconds.
			// We want pixels/second.
			double mul = TimeSpan.TicksPerSecond / den;
			return new Vector2( (float)( nx * mul ), (float)( ny * mul ) );
		}
	}
}