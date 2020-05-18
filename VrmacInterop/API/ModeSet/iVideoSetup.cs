using ComLight;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Vrmac.ModeSet
{
	/// <summary>The caveats for the frame buffer configuration.</summary>
	public enum eEglCaveat: byte
	{
		/// <summary>Corresponds to EGL_NONE value</summary>
		None,
		/// <summary>Corresponds to EGL_SLOW_CONFIG value</summary>
		Slow,
	}

	/// <summary>Flags for a config</summary>
	[Flags]
	public enum eConfigFlags: byte
	{
		/// <summary>No flags are set</summary>
		None = 0,
		/// <summary>The color buffers can be bound to an RGB texture</summary>
		BindToRgbTexture = 1,
		/// <summary>The color buffers can be bound to an RGBA texture</summary>
		BindToRgbaTexture = 2,
		/// <summary>Native rendering APIs can render into the surface</summary>
		NativeRenderable = 4,
	}

	/// <summary>EGL frame buffer configuration</summary>
	/// <seealso href="https://www.khronos.org/registry/EGL/sdk/docs/man/html/eglGetConfigAttrib.xhtml" />
	[StructLayout( LayoutKind.Sequential )]
	public struct sEglConfig
	{
		/// <summary>Number of bits in the RGBA channels</summary>
		public byte red, green, blue, alpha;
		/// <summary>Number of bits in the depth and stencil buffer</summary>
		public byte depth, stencil;
		/// <summary>The caveats for the frame buffer configuration</summary>
		public eEglCaveat caveat;
		/// <summary>Flags for a config</summary>
		public eConfigFlags flags;
		/// <summary>Maximum size of a pixel buffer surface in pixels. </summary>
		public CSize maxSize;
		/// <summary>Number of samples per pixel</summary>
		public byte samples;
		/// <summary>Number of bits in the alpha mask buffer</summary>
		public byte alphaMask;

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat( "RGBA {0}/{1}/{2}/{3}", red, green, blue, alpha );
			if( depth > 0 || stencil > 0 )
				sb.AppendFormat( ", depth/stencil {0}/{1}", depth, stencil );
			else
				sb.Append( ", no depth or stencil" );
			if( caveat != eEglCaveat.None )
				sb.AppendFormat( ", caveat: {0}", caveat );
			if( flags != eConfigFlags.None )
				sb.AppendFormat( ", {0}", flags );
			if( maxSize.cx > 0 && maxSize.cy > 0 )
				sb.AppendFormat( ", up to {0}", maxSize );
			if( samples > 0 )
				sb.AppendFormat( ", {0} samples", samples );

			return sb.ToString();
		}
	}

	/// <summary>Implement this interface if you want fine control over frame buffer and EGL setup</summary>
	[ComInterface( "07aea253-7519-437f-83e0-df6268b41dbb", eMarshalDirection.ToNative )]
	public interface iVideoSetup
	{
		/// <summary>Select GLES configuration of the front buffer. Used by full-screen Linux contexts.</summary>
		/// <param name="configs">Configurations supported by the display and GPU driver</param>
		/// <param name="configsCount">Count of available configurations</param>
		/// <returns>Zero-based index of the config to use, or -1 to use built-in heuristics instead.</returns>
		int pickEglConfig( [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] sEglConfig[] configs,
			int configsCount );

		/// <summary>Select pixel format of the front buffer. Used by full-screen Linux contexts.</summary>
		/// <param name="available">Pixel formats supported by the display</param>
		/// <param name="availableCount">Count of available formats</param>
		/// <returns>Zero-based index of the format to use, or -1 to use built-in heuristics instead.</returns>
		int pickDrmFormat( [In, MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] sDrmFormat[] available,
			int availableCount );
	}
}