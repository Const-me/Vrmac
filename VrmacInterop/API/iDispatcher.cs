using ComLight;
using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Native function pointer for callbacks posted to the dispatcher.</summary>
	[UnmanagedFunctionPointer( RuntimeClass.defaultCallingConvention )]
	public delegate int pfnDispatcherCallback( IntPtr state );

	/// <summary>Execution policy of the dispatcher</summary>
	public enum eDispatcherRunPolicy: byte
	{
		/// <summary>Conserves electricity and thermal budget by only rendering frames on demand, and sleeping as much as possible.</summary>
		/// <remarks>This mode is set by default when you call iDispatcher.run without supplying any contexts.</remarks>
		EnvironmentFriendly,
		/// <summary>Ignores electricity consumption and thermal budget, render frames at the VSync frequency, like videogames do.</summary>
		/// <remarks>This mode is set by default when you call iDispatcher.run and pass one or more contexts to render.</remarks>
		GameStyle
	};

	/// <summary>An abstraction over platform-specific message loop.</summary>
	[ComInterface( "0b6fc298-63b0-4379-acee-03efb229a3a4", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iDispatcher: IDisposable
	{
		/// <summary>Run the message loop.</summary>
		void run( [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] iRenderingContext[] contexts, int contextsCount );

		/// <summary>Render and present a frame on the specified context.</summary>
		/// <returns>True if the thread was good and the frame was rendered right off the bat, or false if the request was posted to the queue for the dispatcher’s thread.</returns>
		/// <remarks>Can be called from any thread. If you call this from dispatcher’s one, will render synchronously, unless postToQueue argument is true.
		/// If called from some other thread, will always post to the queue.</remarks>
		bool renderFrame( iRenderingContext context, [MarshalAs( UnmanagedType.U1 )] bool postToQueue );

		/// <summary>Ask the dispatcher to shut down, and return from the run() method. Can be called from any thread.</summary>
		void postQuitMessage( int hr );

		/// <summary>Post a callback to run on the dispatcher. Can be called from any thread.</summary>
		/// <remarks>
		/// <para>Even if you call this from the same thread running the dispatcher, the callback will run as soon as dispatcher has dispatched all previously posted messages.</para>
		/// <para>On Linux, the default message queue size for un-priveledged processes is just 10.
		/// If you’ll hit that limit by posting messages much faster than the dispatcher thread handles them, this method will block. Potentially, might even cause deadlocks in some edge cases. </para>
		/// <seealso href="http://man7.org/linux/man-pages/man7/mq_overview.7.html" />
		/// </remarks>
		void postCallback( pfnDispatcherCallback pfn, IntPtr state );

		/// <summary>Get current run policy of the dispatcher</summary>
		void getRunPolicy( out eDispatcherRunPolicy policy );
		/// <summary>Change the current run policy of the dispatcher</summary>
		void setRunPolicy( eDispatcherRunPolicy setting );
		/// <summary>Current run policy of the dispatcher</summary>
		eDispatcherRunPolicy runPolicy { get; set; }

		/// <summary>Create a new 3D-rendered window.</summary>
		[RetValIndex]
		iDiligentWindow createWindow( iContent content, iWindowSetup setup = null, RenderDeviceType deviceType = RenderDeviceType.Undefined );
	}

	/// <summary>Linux-specific features of the dispatcher.</summary>
	[ComInterface( "d9e94b86-79b1-4f7a-a61f-d4b7f0ed3e46", eMarshalDirection.ToManaged )]
	public interface iLinuxDispatcher: IDisposable
	{
		/// <summary>Open raw Linux input device, e.g. "/dev/input/event0" is a USB mouse on my system.</summary>
		/// <remarks>The library has a limit of 32 opened input devices, hopefully you don’t need that many of them.</remarks>
		void openInputDevice( [NativeString] string device, Input.Linux.iRawInputSink sink );
	}
}