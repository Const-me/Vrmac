using Diligent.Graphics;
using System;
using Vrmac.Draw.Main;
using Vrmac.Draw.Palette;

namespace Vrmac.Draw
{
	/// <summary>Extension methods for iDrawDevice interface</summary>
	public static class iDrawDeviceExt
	{
		/// <summary>Parse brush color from string</summary>
		public static Vector4 parseBrushColor( this iDrawDevice device, string colorString )
		{
			return device.premultipliedAlphaBrushes ? Color.parse( colorString ) : Color.parseNonPremultiplied( colorString ); ;
		}

		/// <summary>Parse string to color, and create a new solid color brush of that color.</summary>
		public static iBrush createSolidBrush( this iDrawDevice device, string colorString )
		{
			return device.createSolidColorBrush( device.parseBrushColor( colorString ) );
		}

		static Vector4[] consoleColors = null;

		/// <summary>Create brush for console color.</summary>
		/// <remarks>Most named values are very different from createSolidBrush, the console colors are from CGA.
		/// For example, parsing "Green" string will get you #008000, ConsoleColor.Green returns #00FF00.</remarks>
		/// <seealso href="https://en.wikipedia.org/wiki/Color_Graphics_Adapter#With_an_RGBI_monitor" />
		/// <seealso href="https://docs.microsoft.com/en-us/dotnet/api/system.drawing.color?view=netcore-2.1" />
		public static iBrush createSolidBrush( this iDrawDevice device, ConsoleColor color )
		{
			if( device is DrawDevice vrmac )
				return new SolidColorBrush( color );

			if( null == consoleColors )
				consoleColors = PredefinedPaletteEntries.readPalette();
			Vector4 vals = consoleColors[ (int)color ];
			return device.createSolidColorBrush( vals );
		}
	}
}