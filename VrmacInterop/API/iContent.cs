using ComLight;
using Diligent.Graphics;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Window position and show style.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sWindowPosition
	{
		/// <summary>Size of the client area. Will be empty if minimized.</summary>
		public CSize size;
		/// <summary>DPI scaling of the display</summary>
		public double dpiScaling;
		/// <summary>Specifies how the window is shown</summary>
		public eShowWindow show;

		/// <summary>A string for debugger</summary>
		public override string ToString()
		{
			return $"{ size }, DPI multiplier { dpiScaling }, { show }";
		}
	}

	/// <summary>Tells what to release in <see cref="iContent.releaseCachedResources" /> method.</summary>
	public enum eReleaseResources: byte
	{
		/// <summary>Release render targets and their textures, keep everything else.</summary>
		Buffers,
		/// <summary>In addition to the above, also release device context.</summary>
		Context,
		/// <summary>In addition to the above, also release swap chain.</summary>
		SwapChain
	}

	/// <summary>Implement this interface to 3D render your awesome stuff with Diligent Engine.</summary>
	/// <remarks>It's marked with <see cref="eMarshalDirection.BothWays" />, you can implement it in your custom C++ *.dll / *.so / *.dylib, to achieve the best performance possible while still using .NET for pieces where it makes sense for you.</remarks>
	[ComInterface( "f7ab5391-81ea-410a-88c6-7ed7babeccfe", eMarshalDirection.BothWays )]
	public interface iContent
	{
		/// <summary>Called when the library created a rendering window or some other surface.</summary>
		/// <remarks>If you'll return an implementation of iShaderCache, the library will use that object to cache shaders byte code, next time your app will launch slightly faster.</remarks>
		void sourceInitialized( out iShaderCache shaderCache );

		/// <summary>Called by the library to render and present your awesome 3D content.</summary>
		void render();

		/// <summary>Called when the swap chain is resized.</summary>
		void resized( [In] ref sWindowPosition position );

		/// <summary>User wants to close the window, e.g. pressed Alt+F4. Return true to close, or false to ignore user input.</summary>
		bool shouldClose();

		/// <summary>The window has been closed. Return true to quit the application and return from iDiligentNative.run(), return false to keep the process running.</summary>
		bool shouldExit();

		/// <summary>Release cached resources. Only called on Windows, when transitioning to/from true fullscreen state.</summary>
		void releaseCachedResources( eReleaseResources what );

		/// <summary>Cached new device and swap chain. Only called on Windows, when transitioning to true fullscreen state.</summary>
		void swapChainRecreated( iDiligentWindow window );
	}
}