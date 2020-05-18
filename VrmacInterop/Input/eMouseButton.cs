using System;

namespace Vrmac.Input
{
	// https://raw.githubusercontent.com/dotnet/wpf/ae1790531c3b993b56eba8b1f0dd395a3ed7de75/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Input/MouseButton.cs

	/// <summary>The MouseButton enumeration describes the buttons available on the mouse device.</summary>
	public enum eMouseButton: byte
	{
		/// <summary>The left mouse button</summary>
		Left = 0,
		/// <summary>The middle mouse button</summary>
		Middle = 1,
		/// <summary>The right mouse button</summary>
		Right = 2,
		/// <summary>The fourth mouse button</summary>
		XButton1 = 3,
		/// <summary>The fifth mouse button</summary>
		XButton2 = 4,
	}

	/// <summary>State of the mouse buttons</summary>
	[Flags]
	public enum eMouseButtonsState: byte
	{
		/// <summary>All mouse buttons are released</summary>
		None = 0,
		/// <summary>Left mouse button is pressed</summary>
		Left = 1,
		/// <summary>The middle mouse button is pressed</summary>
		Middle = 2,
		/// <summary>The right mouse button is pressed</summary>
		Right = 4,
		/// <remarks>The fourth mouse button is pressed; Linux support for this is limited</remarks>
		XButton1 = 8,
		/// <remarks>The fifth mouse button is pressed; Linux support for this is limited</remarks>
		XButton2 = 0x10
	}
}