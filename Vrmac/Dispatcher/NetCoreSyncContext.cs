using Diligent;
using System;
using System.Threading;

namespace Vrmac
{
	/// <summary>A specialization of SynchronizationContext from .NET for our 3D rendering use case.</summary>
	public abstract class NetCoreSyncContext: SynchronizationContext, IDisposable
	{
		// The only implementations of this class is NativeContext, implements the sync.context on top of iDispatcher COM interface.

		/// <summary>Shut down everything.</summary>
		/// <remarks>The pending callbacks are not dispatched, they are dropped silently.</remarks>
		public abstract void Dispose();

		/// <summary>Post a special message to the queue that, once delivered, causes the main loop to quit.</summary>
		/// <param name="hr">If you'll pass a negative value, <see cref="Dispatcher.run(Context)" /> or <see cref="Dispatcher.run()" /> will throw an exception. Otherwise, it will quit normally.</param>
		public abstract void postQuitMessage( int hr );

		/// <summary>Post a special message to the queue that, once delivered, causes the main loop to throw the passed exception.</summary>
		public abstract void postException( Exception ex );

		// It's internal, use Dispatcher.run() instead
		internal abstract void run( Context[] renderWhenIdle = null );

		internal abstract iDispatcher nativeDispatcher { get; }
	}
}