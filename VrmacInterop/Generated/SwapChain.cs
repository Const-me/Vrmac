// This source file was automatically generated from "SwapChain.h" C++ header by a custom tool.
// The tool is based on CppSharp https://github.com/mono/CppSharp which is based on clang AST https://clang.llvm.org/
#pragma warning disable CS1591	// CS1591: Missing XML comment for publicly visible type or member
using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Numerics;

namespace Diligent.Graphics
{
	/// <summary>The swap chain is created by a platform-dependent function</summary>
	[ComInterface( "1c703b77-6607-4eec-b1fe-15c82d3b4130", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface ISwapChain: IDisposable
	{
		/// <summary>Presents a rendered image to the user</summary>
		void Present( int SyncInterval );

		/// <summary>Returns the swap chain desctription</summary>
		[RetValIndex] SwapChainDesc GetDesc();

		/// <summary>Changes the swap chain's back buffer size</summary>
		/// <param name="NewWidth">New swap chain width, in pixels</param>
		/// <param name="NewHeight">New swap chain height, in pixels</param>
		/// <remarks>When resizing non-primary swap chains, the engine unbinds the swap chain buffers from the output.</remarks>
		void Resize( int NewWidth, int NewHeight );

		/// <summary>Sets fullscreen mode (only supported on Win32 platform)</summary>
		void SetFullscreenMode( [In] ref DisplayModeAttribs DisplayMode );

		/// <summary>Sets windowed mode (only supported on Win32 platform)</summary>
		void SetWindowedMode();

		/// <summary>Get index of the current back buffer in the swap chain</summary>
		int GetCurrentBackBufferIndex();

		/// <summary>Returns render target view of the current back buffer in the swap chain</summary>
		/// <remarks>
		/// <para>For Direct3D12 and Vulkan backends, the function returns different pointer for every offscreen buffer in the swap chain (flipped by every call to ISwapChain::Present()). For Direct3D11 backend it always returns the
		/// same pointer. For OpenGL/GLES backends the method returns null.</para>
		/// <para>The method does *NOT* call AddRef() on the returned interface, so Release() must not be called.</para>
		/// </remarks>
		[RetValIndex] ITextureView GetCurrentBackBufferRTV();

		/// <summary>Returns depth-stencil view of the depth buffer</summary>
		/// <remarks>The method does *NOT* call AddRef() on the returned interface, so Release() must not be called.</remarks>
		[RetValIndex] ITextureView GetDepthBufferDSV();
	}
}
