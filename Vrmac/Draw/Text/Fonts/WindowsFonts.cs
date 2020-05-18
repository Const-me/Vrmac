using System;
using System.Collections.Generic;
using System.IO;
using Vrmac.Draw.Main;

namespace Vrmac.Draw.Text
{
	sealed class WindowsFonts: FontCollectionBase
	{
		static readonly string folder = Environment.GetFolderPath( Environment.SpecialFolder.Fonts );

		public WindowsFonts( DrawDevice drawDevice ) :
			base( drawDevice )
		{
			addDefaults( defaultFonts, eDefaultFont.Mono, "consola.ttf", "consolab.ttf", "consolai.ttf", "consolaz.ttf" );
			addDefaults( defaultFonts, eDefaultFont.Sans, "calibri.ttf", "calibrib.ttf", "calibrii.ttf", "calibriz.ttf" );
			addDefaults( defaultFonts, eDefaultFont.Serif, "cambria.ttc", "cambriab.ttf", "cambriai.ttf", "cambriaz.ttf" );
			addDefaults( defaultFonts, eDefaultFont.ComicSans, "comic.ttf", "comicbd.ttf", "comici.ttf", "comicz.ttf" );
		}

		protected override string defaultFontPath( (eDefaultFont, eFontStyleFlags) key )
		{
			if( !defaultFonts.TryGetValue( key, out string value ) )
				throw new KeyNotFoundException();
			return Path.Combine( folder, value );
		}

		readonly Dictionary<(eDefaultFont, eFontStyleFlags), string> defaultFonts = new Dictionary<(eDefaultFont, eFontStyleFlags), string>( 16 );
	}
}