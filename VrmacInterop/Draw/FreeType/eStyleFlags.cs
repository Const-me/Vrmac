using System;
using Vrmac.FreeType;

namespace Vrmac.Draw
{
	/// <summary>A list of bit flags to indicate the style of a given face. These are used in the <see cref="sFontInfo.styleFlags" /> field of <see cref="sFontInfo" />.</summary>
	/// <remarks>The style information as provided by FreeType is very basic.
	/// More details are beyond the scope and should be done on a higher level (for example, by analyzing various fields of the 'OS/2' table in SFNT based fonts).</remarks>
	[Flags]
	public enum eFontStyleFlags: byte
	{
		/// <summary>None of the below</summary>
		Normal = 0,
		/// <summary>The face style is italic or oblique.</summary>
		Italic = 1,
		/// <summary>The face is bold.</summary>
		Bold = 2
	}
}