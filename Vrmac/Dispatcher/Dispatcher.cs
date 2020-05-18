using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vrmac.Utils;

namespace Vrmac
{
	/// <summary>Provides services for managing the queue of work items for a thread.</summary>
	/// <remarks>It's not as fancy as the one from WPF, e.g. it has no priorities, but it's simple, and does the job.</remarks>
	public class Dispatcher: IDisposable
	{
		/// <summary>The synchronization context created by this dispatcher.</summary>
		/// <remarks>That one does the actual heavy lifting, this class is merely a convenience wrapper around it.</remarks>
		public NetCoreSyncContext synchronizationContext { get; }

		/// <summary>If this sync.context has a native dispatcher, return that, otherwise null.</summary>
		public iDispatcher nativeDispatcher => synchronizationContext.nativeDispatcher;

		/// <summary>The ID of the thread associated with this Dispatcher</summary>
		public int idThread { get; }

		/// <summary>Shut down everything.</summary>
		/// <remarks>The pending callbacks are not dispatched, they are dropped silently.</remarks>
		public void Dispose()
		{
			SynchronizationContext.SetSynchronizationContext( null );
			s_current = null;
			synchronizationContext?.Dispose();
		}

		private Dispatcher( NetCoreSyncContext syncContext )
		{
			Debug.Assert( s_current == null );
			idThread = Thread.CurrentThread.ManagedThreadId;
			synchronizationContext = syncContext;
			SynchronizationContext.SetSynchronizationContext( syncContext );
			s_current = new WeakReference<Dispatcher>( this );
		}

		/// <summary>Create a dispatcher.</summary>
		internal Dispatcher( iDispatcher nativeDispatcher ) :
			this( new NativeContext( nativeDispatcher ) )
		{ }

		/// <summary>Ask the dispatcher to shut down, and return from the run() method. Can be called from any thread.</summary>
		public void postQuitMessage( int hr )
		{
			synchronizationContext.postQuitMessage( hr );
		}

		/// <summary>Post a special message to the queue that, once delivered, causes the run() method to throw the passed exception.</summary>
		public void postException( Exception ex )
		{
			synchronizationContext.postException( ex );
		}

		/// <summary>Run the message loop. When the dispatcher has nothing else to do, it will sleep, saving electricity.</summary>
		public void run()
		{
			synchronizationContext.run( null );
		}

		/// <summary>Run the message loop. When the dispatcher has nothing else to do, it will render frames of the specified content.</summary>
		public void run( Context context )
		{
			if( null == context )
				run();
			else
			{
				var arr = new Context[ 1 ] { context };
				synchronizationContext.run( arr );
			}
		}

		/// <summary>Determines whether the calling thread is the thread associated with this Dispatcher.</summary>
		public bool checkAccess() => idThread == Thread.CurrentThread.ManagedThreadId;

		[ThreadStatic]
		static WeakReference<Dispatcher> s_current;

		/// <summary>If the current thread has a dispatcher, return it, otherwise null.</summary>
		public static Dispatcher currentDispatcher
		{
			get
			{
				if( null == s_current )
					return null;
				if( s_current.TryGetTarget( out var dispatcher ) )
					return dispatcher;
				return null;
			}
		}

		static void yieldImpl( object obj )
		{
			TaskCompletionSource<bool> tcs = (TaskCompletionSource<bool>)obj;
			tcs.SetResult( true );
		}
		static readonly SendOrPostCallback yieldCallback = yieldImpl;

		/// <summary>Creates an awaitable object that asynchronously yields control back to the current dispatcher and provides an opportunity for the dispatcher to process other events.</summary>
		public Task yield()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			synchronizationContext.Post( yieldCallback, tcs );
			return tcs.Task;
		}

		static void runAction( object obj )
		{
			Action act = (Action)obj;
			act();
		}
		static readonly SendOrPostCallback runActionCallback = runAction;

		/// <summary>Post an action to the dispatcher; an exception in that action will cause the dispatcher to shut down with an exception.</summary>
		public void postAction( Action act )
		{
			synchronizationContext.Post( runActionCallback, act );
		}

		/// <summary>Post an action to the dispatcher, and marshal exceptions to the caller.</summary>
		public Task post( Action action )
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Action actPost = () =>
			{
				try
				{
					action();
					tcs.SetResult( true );
				}
				catch( Exception ex )
				{
					tcs.SetException( ex );
				}
			};
			postAction( actPost );
			return tcs.Task;
		}

		/// <summary>Post a function to the dispatcher, marshal both exceptions and return value to the caller.</summary>
		public Task<TResult> post<TResult>( Func<TResult> func )
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			Action actPost = () =>
			{
				try
				{
					tcs.SetResult( func() );
				}
				catch( Exception ex )
				{
					tcs.SetException( ex );
				}
			};
			postAction( actPost );
			return tcs.Task;
		}

		/// <summary>Post an async action to the dispatcher, marshal exceptions to the caller</summary>
		public Task postTask( Func<Task> asyncTask )
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			Action actPost = async () =>
			{
				try
				{
					Task task = asyncTask();
					await task;
					tcs.SetResult( true );
				}
				catch( Exception ex )
				{
					tcs.SetException( ex );
				}
			};
			postAction( actPost );
			return tcs.Task;
		}

		/// <summary>Post an async function to the dispatcher, marshal both exceptions and return value to the caller.</summary>
		public Task<TResult> postFunc<TResult>( Func<Task<TResult>> asyncFunc )
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			Action actPost = async () =>
			{
				try
				{
					Task<TResult> task = asyncFunc();
					tcs.SetResult( await task );
				}
				catch( Exception ex )
				{
					tcs.SetException( ex );
				}
			};
			postAction( actPost );
			return tcs.Task;
		}
	}
}