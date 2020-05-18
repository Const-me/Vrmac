using ComLight;

namespace Vrmac.Input.Linux
{
	/// <summary>Implement this interface to receive raw input on Linux</summary>
	[ComInterface( "418b7523-b85f-468e-93f9-4cd41f1959ba", eMarshalDirection.ToNative )]
	public interface iRawInputSink
	{
		/// <summary>Handle an incoming event.</summary>
		/// <remarks>Events are delivered to the dispatcher's thread, you can directly manipulate GPU resources from the handler.</remarks>
		/// <param name="timestamp">Time is in nanoseconds, since Unit epoch, UTC</param>
		/// <param name="eventType">Event type</param>
		/// <param name="code">The meaning depends on the type. For <see cref="eEventType.Key" />, the value is either eKey or eButton enum.</param>
		/// <param name="value">The meaning depends on the type, for keys and buttons it’s <see cref="eKeyValue" /></param>
		void handle( ulong timestamp, eEventType eventType, ushort code, int value );

		/// <summary>Called immediately after the dispatcher has sent all the pending events to handle() method.</summary>
		/// <remarks>The reason for this separate call, to have the library produce diagonal mouse movement messages, as opposed to 2 distinct events for X and Y.</remarks>
		void updated();

		/// <summary>The input device has failed and will no longer receive events.</summary>
		/// <remarks>One likely reason for that, user has unplugged that device from USB.</remarks>
		void failed( int hResult );
	}
}