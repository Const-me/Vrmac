using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Vrmac.Utils
{
	abstract class SyncContextBase: NetCoreSyncContext
	{
		const int queueSize = 256;

		protected struct Callback
		{
			public readonly SendOrPostCallback callback;
			public readonly object state;
			public Callback( SendOrPostCallback callback, object state )
			{
				this.callback = callback;
				this.state = state;
			}
		}

		protected readonly BlockingCollection<Callback> queue = new BlockingCollection<Callback>( queueSize );

		protected bool dispatchMessage( Callback cb )
		{
			// There's probably a bug in .NET, when async-await is combined with native interop on Linux, current synchronization context is sometimes lost.
			// Resetting manually on each callback, it's just a single field assignment, hopefully OK performance-wise:
			// https://github.com/dotnet/runtime/blob/4f9ae42d861fcb4be2fcd5d3d55d5f227d30e723/src/libraries/System.Private.CoreLib/src/System/Threading/SynchronizationContext.cs#L58
			SetSynchronizationContext( this );

			if( cb.callback != null )
			{
				cb.callback( cb.state );
				return true;
			}

			switch( cb.state )
			{
				case Exception ex:
					throw ex;
				default:
					throw new ApplicationException( "Unexpected callback" );
			}
		}

		public override void postException( Exception ex )
		{
			queue.Add( new Callback( null, ex ) );
		}
	}
}