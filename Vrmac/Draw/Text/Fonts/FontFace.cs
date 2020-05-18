using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vrmac.Draw.Main;
using Vrmac.FreeType;

namespace Vrmac.Draw.Text
{
	sealed class FontFace: iFontFace
	{
		readonly DrawDevice drawDevice;
		internal readonly FreeType.iFont font;
		public readonly sFontInfo info;

		public FontFace( DrawDevice drawDevice, iFreeType factory, System.IO.Stream stream, string name = null, int faceIndex = 0 )
		{
			this.drawDevice = drawDevice;
			font = factory.createFont( stream, name, faceIndex );
			font.getInfo( out info );
		}

		public sScaledMetrics getScaledMetrics( uint size ) => font.getScaledMetrics( size );

		readonly Dictionary<uint, int> glyphIndices = new Dictionary<uint, int>();

		string iFontFace.familyName => info.familyName;
		eFontStyleFlags iFontFace.styleFlags => info.styleFlags;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int getGlyphIndex( uint utf32 )
		{
			if( glyphIndices.TryGetValue( utf32, out int idx ) )
				return idx;
			idx = font.getGlyphIndex( utf32 );
			glyphIndices.Add( utf32, idx );
			return idx;
		}

		void IDisposable.Dispose()
		{
			font?.Dispose();
		}

		readonly Dictionary<uint, iFont> cache = new Dictionary<uint, iFont>();

		iFont iFontFace.createFont( float fontSizePt, float dpiScaling )
		{
			uint sizePixels = Utils.computeFontSize( fontSizePt, dpiScaling );
			if( cache.TryGetValue( sizePixels, out var font ) )
				return font;

			font = new Font( drawDevice.fontTextures, this, sizePixels );
			cache.Add( sizePixels, font );
			return font;
		}
	}
}