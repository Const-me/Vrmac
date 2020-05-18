using System.Collections.Generic;
using System.IO;
using Vrmac.Draw.Main;

namespace Vrmac.Draw.Text
{
	sealed class LinuxFonts: FontCollectionBase
	{
		const string folder = @"/usr/share/fonts/truetype";

		public LinuxFonts( DrawDevice drawDevice ) :
			base( drawDevice )
		{
			addDefaults( defaultFonts, eDefaultFont.Mono,
				"liberation2/LiberationMono-Regular.ttf", "liberation2/LiberationMono-Bold.ttf", "liberation2/LiberationMono-Italic.ttf", "liberation2/LiberationMono-BoldItalic.ttf" );
			addDefaults( defaultFonts, eDefaultFont.Sans,
				"liberation2/LiberationSans-Regular.ttf", "liberation2/LiberationSans-Bold.ttf", "liberation2/LiberationSans-Italic.ttf", "liberation2/LiberationSans-BoldItalic.ttf" );
			addDefaults( defaultFonts, eDefaultFont.Serif,
				"liberation2/LiberationSerif-Regular.ttf", "liberation2/LiberationSerif-Bold.ttf", "liberation2/LiberationSerif-Italic.ttf", "liberation2/LiberationSerif-BoldItalic.ttf" );

			defaultFonts.Add( (eDefaultFont.ComicSans, eFontStyleFlags.Normal), "quicksand/Quicksand-Medium.ttf" );
			defaultFonts.Add( (eDefaultFont.ComicSans, eFontStyleFlags.Bold), "quicksand/Quicksand-Bold.ttf" );
		}

		protected override string defaultFontPath( (eDefaultFont, eFontStyleFlags) key )
		{
			if( !defaultFonts.TryGetValue( key, out string value ) )
				throw new KeyNotFoundException();
			return Path.Combine( folder, value );
		}

		readonly Dictionary<(eDefaultFont, eFontStyleFlags), string> defaultFonts = new Dictionary<(eDefaultFont, eFontStyleFlags), string>( 14 );
	}
}