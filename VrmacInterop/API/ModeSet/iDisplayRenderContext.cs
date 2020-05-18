using ComLight;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Vrmac.ModeSet
{
	/// <summary>A rendering context created on top of hardware display, as opposed to a window</summary>
	[ComInterface( "92c8624b-4f7b-4104-9b2f-7b67ffae1dd6", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iDisplayRenderContext: IDisposable
	{
		/// <summary>Get the video mode: size, refresh rate, etc.</summary>
		[RetValIndex] sVideoMode getMode();

		/// <summary>Present a frame, waiting for vsync.</summary>
		void present();
	}

	/// <summary>Function pointer to receive GPU config XML data.</summary>
	/// <param name="xml">XML blob, probably UTF8 encoded</param>
	/// <param name="length">Length of the blob</param>
	[UnmanagedFunctionPointer( RuntimeClass.defaultCallingConvention )]
	public delegate void pfnHaveConfigXml( [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] byte[] xml, int length );

	/// <summary>A specialization of iDisplayRenderContext exposing features specific to OpenGL and GLES contexts.</summary>
	[ComInterface( "87cc8fe7-7041-4be4-b463-653a3866f27a", eMarshalDirection.ToManaged ), CustomConventions( typeof( Utils.NativeErrorMessages ) )]
	public interface iGlRenderContext: iDisplayRenderContext
	{
		/// <summary>Get the video mode: size, refresh rate, etc.</summary>
		[RetValIndex] new sVideoMode getMode();

		/// <summary>Present a frame, waiting for vsync.</summary>
		new void present();

		/// <summary>Call eglMakeCurrent. Newly created contexts are already current.</summary>
		/// <remarks>If you're driving a single display, you rarely need to call this, only if you want to try to resume rendering after <see cref="present()" /> failed with an exception.</remarks>
		void setCurrent();

		/// <summary>Call eglMakeCurrent passing null pointers, undoing what setCurrent() did. If you're driving a single display, you don't need to call this.</summary>
		void clearCurrent();

		/// <summary>Get version of the API.</summary>
		/// <remarks>At the time of writing, on Raspberry Pi4 I'm getting 3.1.0.0 value. May upgrade in future releases of that Linux.</remarks>
		[RetValIndex] sVersionInfo getGlVersion();

		/// <summary>Get a structure describing currently used GL config.</summary>
		[RetValIndex] sEglConfig getEglConfig();

		/// <summary>Get XML document with supported runtime options of the GPU driver.</summary>
		/// <remarks>Have no idea how to set these parameters.</remarks>
		/// <seealso href="https://www.khronos.org/registry/EGL/extensions/MESA/EGL_MESA_query_driver.txt" />
		void getConfigXml( pfnHaveConfigXml pfn );
	}

	/// <summary>Couple extension methods for <see cref="iGlRenderContext" /> COM interface.</summary>
	public static class GlRenderContextExt
	{
		/// <summary>An easier to use wrapper around <see cref="iGlRenderContext.getConfigXml(pfnHaveConfigXml)" /> which returns a read-only stream.</summary>
		public static MemoryStream getConfigXml( this iGlRenderContext context )
		{
			MemoryStream result = null;
			pfnHaveConfigXml pfn = ( byte[] xml, int length ) =>
			{
				result = new MemoryStream( xml, 0, length, false );
			};
			context.getConfigXml( pfn );
			return result;
		}
	}
}