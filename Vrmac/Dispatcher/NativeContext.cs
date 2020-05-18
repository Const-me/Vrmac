using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Vrmac.Utils
{
	/// <summary>A synchronization context backed by DiligentNative C++ library.</summary>
	sealed class NativeContext: SyncContextBase
	{
		public NativeContext( iDispatcher dispatcher )
		{
			nativeDispatcher = dispatcher;
			callback = this.nativeCallback;
		}

		internal override iDispatcher nativeDispatcher { get; }

		public override void Dispose()
		{
			nativeDispatcher?.Dispose();
		}
		public override void postQuitMessage( int hr )
		{
			nativeDispatcher.postQuitMessage( hr );
		}

		ExceptionDispatchInfo cachedException = null;

		public override void postException( Exception ex )
		{
			cachedException = ExceptionDispatchInfo.Capture( ex );
			base.postException( ex );
		}

		internal override void run( Context[] contexts )
		{
			cachedException = null;
			try
			{
				if( contexts.isEmpty() )
					nativeDispatcher.run( null, 0 );
				else
				{
					iRenderingContext[] nativeContexts = contexts.Select( c => c.renderContext ).ToArray();
					nativeDispatcher.run( nativeContexts, nativeContexts.Length );
				}
			}
			catch( Exception ex )
			{
				if( ex.HResult == cachedException?.SourceException.HResult )
					cachedException.Throw();
				throw;
			}
			finally
			{
				cachedException = null;
			}
		}

		public override void Post( SendOrPostCallback d, object state )
		{
			if( null == d )
				throw new ArgumentNullException();
			queue.Add( new Callback( d, state ) );
			nativeDispatcher.postCallback( callback, IntPtr.Zero );
		}

		public override void Send( SendOrPostCallback d, object state )
		{
			Post( d, state );
		}

		int nativeCallback( IntPtr state )
		{
			try
			{
				Callback cb = queue.Take();
				if( !dispatchMessage( cb ) )
					postQuitMessage( (int)cb.state );
				return 0;   // S_OK
			}
			catch( Exception ex )
			{
				cachedException = ExceptionDispatchInfo.Capture( ex );
				return ex.HResult;
			}
		}

		// Very important, keeping the delegate to protect it from garbage collector.
		// Otherwise, C++ will eventually crash calling into a function that's no longer there.
		readonly pfnDispatcherCallback callback;

		void captureException( Exception ex )
		{
			if( null == cachedException )
				cachedException = ExceptionDispatchInfo.Capture( ex );
		}

		internal static void cacheException( Exception ex )
		{
			NativeContext nc = Dispatcher.currentDispatcher?.synchronizationContext as NativeContext;
			nc?.captureException( ex );
		}

		// If HRESULT code matches cached one for the current thread, throw the cached one.
		internal static void throwCached( int hresult )
		{
			NativeContext nc = Dispatcher.currentDispatcher?.synchronizationContext as NativeContext;
			if( null == nc?.cachedException )
				return;

			if( nc.cachedException.SourceException.HResult == hresult )
			{
				var ce = nc.cachedException;
				nc.cachedException = null;
				ce.Throw();
			}
		}
	}
}