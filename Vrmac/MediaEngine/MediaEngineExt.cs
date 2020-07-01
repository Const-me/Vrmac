using ComLight;
using Diligent.Graphics;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Vrmac.Utils;

namespace Vrmac.MediaEngine
{
	/// <summary>Extension methods for iMediaEngine COM interface</summary>
	public static class MediaEngineExt
	{
		static Exception getExceptionForHResult( int hr )
		{
			// Windows version is in C++ (an OS component, really) and it defines quite a few custom error codes for various MediaEngine errors.
			string msg = NativeErrorMessages.customErrorMessage( hr );
			if( null != msg )
				return new COMException( msg, hr );
			msg = ErrorCodes.tryLookupCode( hr );
			if( null != msg )
				return new COMException( msg, hr );
			return Marshal.GetExceptionForHR( hr );
		}

		sealed class CompletionSource: iCompletionSource
		{
			readonly TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			public Task task => tcs.Task;

			void iCompletionSource.completed() => tcs.SetResult( true );

			void iCompletionSource.failed( int hr )
			{
				tcs.SetException( getExceptionForHResult( hr ) );
			}
		}

		/// <summary>Set URL of a media resource. Return a task which completes when it’s ready to be played, or fails if it was unable to do so.</summary>
		public static Task loadMedia( this iMediaEngine mediaEngine, string url )
		{
			if( mediaEngine is iLinuxMediaEngine linux )
			{
				// Linux implementation is in .NET, it exposes a task-based asynchronous API for loading videos
				return linux.loadMedia( url );
			}

			// Windows implementation is in C++, needs that helper iCompletionSource object to marshal the result back to .NET
			CompletionSource cs = new CompletionSource();
			mediaEngine.loadMedia( url, cs );
			return cs.task;
		}

		/// <summary>Creates an object which holds GPU resources necessary to render video frames.</summary>
		/// <remarks>Linux and Windows versions of that thing are substantially different, but both expose same API.</remarks>
		public static iVideoRenderState createRenderer( this iMediaEngine mediaEngine, Context context, IRenderDevice device, Vector4 borderColor )
		{
			if( mediaEngine is iLinuxMediaEngine linux )
			{
				// Linux implementation is in .NET, it exposes this from the engine.
				return linux.createRenderer( device, context.swapChainSize, context.swapChainFormats, borderColor );
			}

			Debug.Assert( RuntimeEnvironment.runningWindows );
			return new Render.WindowsRender( device, context.swapChainSize, context.swapChainFormats, borderColor, mediaEngine );
		}
	}
}