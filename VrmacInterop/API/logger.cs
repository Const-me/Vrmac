using ComLight;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Log level of the messages produced by the library</summary>
	public enum eLogLevel: byte
	{
		/// <summary>Really bad</summary>
		Error = 0,
		/// <summary>Bad but likely to recover</summary>
		Warning = 1,
		/// <summary>Informational events or messages that are not errors</summary>
		Info = 2,
		/// <summary>Informational events only interesting to developers.</summary>
		Debug = 3,
		/// <summary>A lot of messages safe to ignore</summary>
		Verbose=4,
	};

	/// <summary>Component which produced the message</summary>
	public enum eLogComponent: byte
	{
		/// <summary>Diligent messages, the main thing calling these GPUs.</summary>
		Diligent = 1,
		/// <summary>NetCore implements windows handling, some other OS integration, and a few less important things like shaders cache.</summary>
		NetCore = 2,
		/// <summary>ModeSet parts: enumerating GPUs, connectors, video modes, and creating full-screen contexts.</summary>
		ModeSet = 3,
	};

	/// <summary>Native function pointer to receive log messages from native code.</summary>
	[UnmanagedFunctionPointer( RuntimeClass.defaultCallingConvention )]
	public delegate void pfnLogMessage( eLogLevel level, eLogComponent component,
		[MarshalAs( UnmanagedType.LPUTF8Str )] string message,
		[MarshalAs( UnmanagedType.LPUTF8Str )] string source );
}