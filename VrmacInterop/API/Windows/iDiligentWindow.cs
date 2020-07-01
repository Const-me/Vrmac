using ComLight;
using Diligent.Graphics;
using System.Runtime.InteropServices;
using Vrmac.Input;
using Vrmac.ModeSet;

namespace Vrmac
{
	/// <summary>A window you can render to.</summary>
	[ComInterface( "31396e44-9292-44e6-8fc4-8c63b9257b29", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iDiligentWindow: iRenderingContext
	{
		// Override iRenderingContext methods

		/// <summary>Get render device interface</summary>
		new void getDevice( out IRenderDevice device );
		/// <summary>Get the device context</summary>
		new void getContext( out IDeviceContext context );
		/// <summary>Get the swap chain</summary>
		new void getSwapChain( out ISwapChain swapChain );
		/// <summary>Render and present a frame.</summary>
		new bool renderFrame();
		/// <summary>Wait for next vertical blank event</summary>
		new void waitForVBlank();

		/// <summary>Create an object to setup input handling</summary>
		void getInput( out iInput i );
		/// <summary>An interface to setup input handling</summary>
		iInput input { get; }

		/// <summary>Set the title of this window</summary>
		void setTitle( [NativeString] string newTitle );
		/// <summary>Title of this window</summary>
		[Property( "Title" )]
		string windowTitle { set; }

		/// <summary>Get current state of the window</summary>
		void getWindowState( out eShowWindow state );
		/// <summary>Current state of the window</summary>
		eShowWindow windowState { get; }

		/// <summary>Get window rectangle</summary>
		void getWindowRectangle( out CRect rect );
		/// <summary>Window rectangle; unless fullscreen or minimized, includes size of the title and borders.</summary>
		CRect windowRectangle { get; }

		/// <summary>Get current DPI scaling multiplier</summary>
		void getDpiScaling( out double dpiScaling );
		/// <summary>Current DPI scaling multiplier; the DPI value is 96.0 multiplied by this number.</summary>
		double dpiScaling { get; }

		/// <summary>Reposition the window.</summary>
		/// <remarks>The rectangle is only used when the new state is Normal. And even then, only if it’s not empty.
		/// User ain’t gonna resize these windows too often, simple API is more important here than performance.</remarks>
		void moveWindow( eShowWindow newState, [In] ref CRect rectangle );

		/// <summary>Switch to true fullscreen mode. Only supported on Windows.</summary>
		void fullScreen( iGpuConnector connector, [In] ref sVideoMode mode );
	}
}