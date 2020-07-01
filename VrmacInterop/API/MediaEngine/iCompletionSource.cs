using ComLight;
using System.ComponentModel;

namespace Vrmac.MediaEngine
{
	/// <summary>Allows C++ code to complete C# tasks</summary>
	[ComInterface( "2722ef9d-5f4d-4e2c-b302-02fcc768407e", eMarshalDirection.ToNative ), EditorBrowsable( EditorBrowsableState.Never )]
	public interface iCompletionSource
	{
		/// <summary>Called if the asynchronous task completes successfully</summary>
		void completed();
		/// <summary>Called when the asynchronous task fails</summary>
		void failed( int hr );
	}
}