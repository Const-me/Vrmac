using ComLight;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.Input
{
	/// <summary>Interface to setup input handling on Windows, and on Linux when using X11</summary>
	[ComInterface( "e206ce22-0c57-445c-b0e5-9a3b716e2513", eMarshalDirection.ToManaged )]
	public interface iInput: IDisposable
	{
		/// <summary>Subscribe for mouse events</summary>
		void mouseInputHandler( iMouseHandler handler );

		/// <summary>If the argument is true, capture mouse events.</summary>
		void mouseCapture( [MarshalAs( UnmanagedType.U1 )] bool capture );

		/// <summary>Subscribe for keyboard events</summary>
		void keyboardInputHandler( iKeyboardHandler handler );

		/// <summary>Time in milliseconds when the current input message was generated</summary>
		/// <remarks>On Windows it calls GetMessageTime, on X11 returns xcb_timestamp_t of the last received message. Fortunately, both are in milliseconds.</remarks>
		void getLastMessageTime( out uint milliseconds );

		/// <summary>Time in milliseconds when the current input message was generated</summary>
		uint lastMessageTime { get; }
	}
}