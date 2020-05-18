using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Vrmac.Draw.Tessellate
{
	sealed class Queues
	{
		public readonly object syncRoot = new object();

		/// <summary>The paths with tri.meshes built, waiting to flip these buffers.</summary>
		readonly HashSet<Meshes> ready = new HashSet<Meshes>();
		/// <summary>Normal priority pending tasks</summary>
		readonly HashSet<Meshes> pending = new HashSet<Meshes>();

		public void enqueue( Meshes meshes )
		{
			Debug.Assert( Monitor.IsEntered( syncRoot ) );

			if( meshes.state == eState.Pending )
				return;

			switch( meshes.state )
			{
				case eState.Idle:
					pending.Add( meshes );
					meshes.state = eState.Pending;
					return;
				case eState.Pending:
					pending.Remove( meshes );
					meshes.state = eState.Pending;
					return;
				case eState.Running:
					return;
				case eState.Ready:
					if( ready.Remove( meshes ) )
						meshes.flipBuffers();
					goto case eState.Idle;
			}
		}

		// Dequeue a job
		public DequedJob dequeue()
		{
			Debug.Assert( Monitor.IsEntered( syncRoot ) );

			var e = pending.GetEnumerator();
			if( e.MoveNext() )
			{
				Meshes res = e.Current;
				e.Dispose();
				pending.Remove( res );
				res.state = eState.Running;
				return new DequedJob( res );
			}
			e.Dispose();
			return new DequedJob();
		}

		// Mark job as completed
		public void completed( ref DequedJob job, bool isReady )
		{
			Debug.Assert( Monitor.IsEntered( syncRoot ) );

			if( isReady )
			{
				job.meshes.state = eState.Ready;
				ready.Add( job.meshes );
			}
			else
				job.meshes.state = eState.Idle;
		}

		// Flip buffers for all jobs in the ready set, then clear that set
		public void present()
		{
			Debug.Assert( Monitor.IsEntered( syncRoot ) );

			foreach( var m in ready )
				m.flipBuffers();
			ready.Clear();
		}

		public void dropPending()
		{
			Debug.Assert( Monitor.IsEntered( syncRoot ) );
			pending.Clear();
		}

		public void dropReady()
		{
			Debug.Assert( Monitor.IsEntered( syncRoot ) );
			ready.Clear();
		}
	}
}