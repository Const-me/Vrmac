using System.Collections.Generic;
using System.IO;
using Vrmac.Draw.Main;
using Vrmac.FreeType;

namespace Vrmac.Draw.Text
{
	abstract class FontCollectionBase: iFontCollection
	{
		public readonly iFreeType factory;
		readonly DrawDevice drawDevice;

		public FontCollectionBase( DrawDevice drawDevice )
		{
			this.drawDevice = drawDevice;
			factory = drawDevice.factory.loadFreeType();
		}

		iFontFace iFontCollection.defaultMono( eFontStyleFlags styleFlags ) =>
			getDefaultFont( (eDefaultFont.Mono, styleFlags) );
		iFontFace iFontCollection.defaultSanSerif( eFontStyleFlags styleFlags ) =>
			getDefaultFont( (eDefaultFont.Sans, styleFlags) );
		iFontFace iFontCollection.defaultSerif( eFontStyleFlags styleFlags ) =>
			getDefaultFont( (eDefaultFont.Serif, styleFlags) );
		iFontFace iFontCollection.comicSans( eFontStyleFlags styleFlags ) =>
			getDefaultFont( (eDefaultFont.ComicSans, styleFlags) );

		iFontFace getDefaultFont( (eDefaultFont, eFontStyleFlags) key )
		{
			if( defaultFonts.TryGetValue( key, out var font ) )
				return font;
			string path = defaultFontPath( key );
			font = new FontFace( drawDevice, factory, File.OpenRead( path ) );
			defaultFonts.Add( key, font );
			return font;
		}

		protected enum eDefaultFont: byte
		{
			Mono,
			Sans,
			Serif,
			ComicSans,
		}

		protected abstract string defaultFontPath( (eDefaultFont, eFontStyleFlags) styleFlags );

		readonly Dictionary<(eDefaultFont, eFontStyleFlags), iFontFace> defaultFonts = new Dictionary<(eDefaultFont, eFontStyleFlags), iFontFace>();

		protected static void addDefaults( Dictionary<(eDefaultFont, eFontStyleFlags), string> dict, eDefaultFont df, string normal, string bold, string italic, string italicBold )
		{
			dict.Add( (df, eFontStyleFlags.Normal), normal );
			dict.Add( (df, eFontStyleFlags.Bold), bold );
			dict.Add( (df, eFontStyleFlags.Italic), italic );
			dict.Add( (df, eFontStyleFlags.Italic | eFontStyleFlags.Bold), italicBold );
		}
	}
}