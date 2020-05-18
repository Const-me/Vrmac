using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Vrmac
{
	/// <summary>Utility class that implements weak event, i.e. an event that doesn’t prevent subscribing objects from being garbage collected.</summary>
	public class WeakEvent<TDelegate>: IEnumerable<TDelegate>
		where TDelegate : Delegate
	{
		readonly ConditionalWeakTable<object, TDelegate> table = new ConditionalWeakTable<object, TDelegate>();

		/// <summary>Add an event handler, with the life time linked to the specified object</summary>
		/// <remarks>This class holds at most 1 handler per owning object. If you’ll add another one, old one will be dropped.</remarks>
		public void add( object obj, TDelegate del )
		{
			Debug.Assert( !table.TryGetValue( obj, out var unused ) );
			table.AddOrUpdate( obj, del );
		}

		/// <summary>Manually remove the subscriber</summary>
		public bool remove( object obj )
		{
			return table.Remove( obj );
		}

		internal void clear()
		{
			table.Clear();
		}

		IEnumerator<TDelegate> IEnumerable<TDelegate>.GetEnumerator() => table.values().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => table.values().GetEnumerator();
	}
}