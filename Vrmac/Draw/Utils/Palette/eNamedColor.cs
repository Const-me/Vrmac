namespace Vrmac.Draw
{
	enum eNamedColor: byte
	{
		// https://en.wikipedia.org/wiki/Web_colors#HTML_color_names
		// Same colors and same IDs: https://github.com/dotnet/runtime/blob/master/src/libraries/System.Console/src/System/ConsoleColor.cs
		// Same colors and same IDs: https://en.wikipedia.org/wiki/Color_Graphics_Adapter#With_an_RGBI_monitor

		// The following lines were copy-pasted from ConsoleColor.cs from .NET Core source code.
		Black = 0,
		DarkBlue = 1,
		DarkGreen = 2,
		DarkCyan = 3,
		DarkRed = 4,
		DarkMagenta = 5,
		DarkYellow = 6,
		Gray = 7,
		DarkGray = 8,
		Blue = 9,
		Green = 10,
		Cyan = 11,
		Red = 12,
		Magenta = 13,
		Yellow = 14,
		White = 15,

		// That's not on the list, but still useful at times.
		Transparent = 16,
	}
}