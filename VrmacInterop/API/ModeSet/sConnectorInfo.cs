using System;
using System.Runtime.InteropServices;

namespace Vrmac.ModeSet
{
	/// <summary>Flags for GPU connector</summary>
	[Flags]
	public enum eConnectorFlags: byte
	{
		/// <summary>No flags set, it typically means no display is connected to the output, or the display is powered off.</summary>
		None = 0,
		/// <summary>When set, the display is connected. When not set, sDisplayInfo represents available GPU connector, without any display attached.</summary>
		Connected = 1,
		/// <summary>The display is already showing something. The OS may or may not allow you to replace that image with your own renderings.</summary>
		Active = 2,

		/// <summary>On Linux it means the display is connected, and the EDID of the display has at least 1 video mode marked with "preferred mode" bit.
		/// On Windows however, it means the display is attached to the desktop.</summary>
		HasPreferredMode = 4,
	}

	/// <summary>Display output technology</summary>
	public enum eOutputTechnology: byte
	{
		/// <summary>We don't know the technology</summary>
		Unknown = 0,
		/// <summary>Some other technology, externally connected</summary>
		OtherExternal,
		/// <summary>Some other technology, built-in</summary>
		OtherInternal,

		/// <summary>Composite analog video</summary>
		Composite,
		/// <summary>Component analog video</summary>
		Component,
		/// <summary>Analog S-video</summary>
		SVideo,
		/// <summary>Analog video connector for Japanese market, supports up to 1080p. Only supported on Windows.</summary>
		DTanshi,

		/// <summary>VGA D-Sub analog RGB connector</summary>
		VGA,
		/// <summary>Digital Video Interface</summary>
		DVI,
		/// <summary>High-Definition Multimedia Interface</summary>
		HDMI,

		/// <summary>External display port, which is a display port that connects externally to a display device.</summary>
		DisplayPortExt,
		/// <summary>Embedded display port that connects internally to a display device.</summary>
		DisplayPortInt,

		/// <summary>Miracast or an equivalent</summary>
		Networked,

		/// <summary>Low Voltage Differential Signaling</summary>
		LVDS,
		/// <summary>Display Serial Interface by MIPI alliance. Only supported on Linux.</summary>
		DSI,
	}

	/// <summary>Flags that indicate how the back buffers should be rotated to fit the physical rotation of a monitor.</summary>
	public enum eRotationMode: byte
	{
		/// <summary>Unspecified rotation.</summary>
		Unspecified = 0,
		/// <summary>Specifies no rotation.</summary>
		Identity = 1,
		/// <summary>Specifies 90 degrees of rotation.</summary>
		Rotate90 = 2,
		/// <summary>Specifies 180 degrees of rotation.</summary>
		Rotate180 = 3,
		/// <summary>Specifies 270 degrees of rotation.</summary>
		Rotate270 = 4,
	}

	/// <summary>Layout of the subpixels.</summary>
	/// <remarks>Unfortunately, Raspberry Pi 4 returns `Unknown` for my BenQ PD2700U.</remarks>
	public enum eSubpixelLayout: byte
	{
		/// <summary>No data is available</summary>
		Unknown = 0,
		/// <summary>There're no subpixels</summary>
		None = 1,
		/// <summary>Horizontal order, leftmost is red, green is in the middle, the rightmost is blue.</summary>
		HorizontalRGB = 2,
		/// <summary>Horizontal order, leftmost is blue, green is in the middle, the rightmost is red.</summary>
		HorizontalBGR = 3,
		/// <summary>Vertical order, the topmost is red, green is in the middle, the lowest one is blue.</summary>
		VerticalRGB = 4,
		/// <summary>Vertical order, the topmost is blue, green is in the middle, the lowest one is red.</summary>
		VerticalBGR = 5,
	}

	/// <summary>Information about GPU connector and possible about the display attached to it.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sConnectorInfo
	{
		/// <summary>Flags for GPU connector.</summary>
		public eConnectorFlags flags;
		/// <summary>On Linux either 0 or 1. On Windows can be more, when "duplicate output" check box if checked in display preferences.</summary>
		public byte countDisplays;
		/// <summary>Display output technology. On Windows with duplicate output, will return "OtherExternal".</summary>
		public eOutputTechnology technology;
		/// <summary>Flags that indicate how the back buffers should be rotated to fit the physical rotation of a monitor.</summary>
		public eRotationMode rotation;
		/// <summary>Layout of the subpixels. Windows doesn't expose that information, always "Unknown".</summary>
		public eSubpixelLayout subpixelLayout;

		IntPtr m_displayName;
		/// <summary>Name of the display</summary>
		public string displayName => MiscUtils.stringFromPointer( m_displayName );

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			return $"\"{ displayName }\", status { flags }, tech { technology }, subpixel { subpixelLayout }";
		}
	}
}