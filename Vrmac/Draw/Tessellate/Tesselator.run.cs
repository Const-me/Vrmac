using System;
using System.Diagnostics;
using System.Threading;

namespace Vrmac.Draw.Tessellate
{
	sealed partial class Tesselator
	{
		static int backgroundThreadsCount()
		{
#if DEBUG
			return 1;
#else
			if( RuntimeEnvironment.runningLinux )
				return 2;

			int ideal = Environment.ProcessorCount / 3;
			if( ideal < 1 )
				return 1;
			if( ideal > 4 )
				return 4;
			return ideal;
#endif
		}

		static readonly int countThreads = backgroundThreadsCount();

		volatile int threadsRunning = 0;

		// Caching the delegate to reduce dynamic memory allocations
		readonly WaitCallback postJobCallback;

		void threadMain2()
		{
			DequedJob job;

			iPolylinePath tempPoly = TempPolylines.getTemp( factory );

			Rect clipRect;
			lock( queues.syncRoot )
			{
				clipRect = getClippingRect();
				job = queues.dequeue();
				if( !job )
				{
					threadFinished();
					return;
				}
			}

			bool updated = job.tessellate( ref clipRect, tempPoly );

			while( true )
			{
				lock( queues.syncRoot )
				{
					if( job.isSameOptions() )
					{
						queues.completed( ref job, updated );
						updated = false;

						job = queues.dequeue();
						if( !job )
						{
							threadFinished();
							return;
						}
					}
				}
				bool u = job.tessellate( ref clipRect, tempPoly );
				updated = updated || u;
			}
		}

		void threadMain( object unused )
		{
			try
			{
				threadMain2();
			}
			catch( Exception ex )
			{
				// Never saw that happening, but still, better be safe.
				ex.logError( "Tessellator thread crashed" );
			}
		}

		void threadFinished()
		{
			threadsRunning--;
			if( threadsRunning > 0 )
				return;
			Monitor.Pulse( queues.syncRoot );
		}
	}
}