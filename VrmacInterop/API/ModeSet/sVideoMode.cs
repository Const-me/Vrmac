using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.ModeSet
{
	/// <summary>Video mode scaling</summary>
	public enum eScalingMode: byte
	{
		/// Unspecified scaling.
		/// D3D Counterpart: DXGI_MODE_SCALING_UNSPECIFIED.
		Unspecified = 0,

		/// Specifies no scaling. The image is centered on the display. 
		/// This flag is typically used for a fixed-dot-pitch display (such as an LED display).
		/// D3D Counterpart: DXGI_MODE_SCALING_CENTERED.
		Centered = 1,

		/// Specifies stretched scaling.
		/// D3D Counterpart: DXGI_MODE_SCALING_STRETCHED.
		Stretched = 2
	};

	/// <summary>Information about a video mode.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sVideoMode
	{
		/// <summary>Resolution in pixels.</summary>
		public CSize sizePixels;
		/// <summary>Refresh rate in Hertz (Hz)</summary>
		public Rational refreshRate;
		/// <summary>Texture format for the front buffer. Very likely to be <see cref="TextureFormat.Unknown" /> because many native formats are not directly supported by GPUs.</summary>
		public TextureFormat format;
		/// <summary>Video mode scaling</summary>
		public eScalingMode scaling;

		IntPtr m_name;
		/// <summary>Name of the mode, as reported by the OS</summary>
		public string name => MiscUtils.stringFromPointer( m_name );
		/// <summary>Index of the mode, you need this value if you want to switch to this mode.</summary>
		public int index;

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			string rate = refreshRate.print( "Hz" );
			string str = name;
			if( null != str )
				return $"{ sizePixels } { rate } \"{ str }\" #{ index }";
			else
				return $"{ sizePixels } { rate } #{ index }";
		}
	}
}