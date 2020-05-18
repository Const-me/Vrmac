using System;

namespace Vrmac
{
	/// <summary>Couple utility functions related to COM lifetime management</summary>
	public static class ComUtils
	{
		/// <summary>Set COM variable to null, and if needed release the underlying C++ COM object.</summary>
		public static void clear<T>( ref T comPointer ) where T : class, IDisposable
		{
			comPointer?.Dispose();
			comPointer = null;
		}

		/// <summary>Assign COM variable to the new value, if it's changed, also release the old one.</summary>
		public static void assign<T>( ref T comPointer, T newValue ) where T : class, IDisposable
		{
			if( ReferenceEquals( comPointer, newValue ) )
				return;
			comPointer?.Dispose();
			comPointer = newValue;
		}
	}
}