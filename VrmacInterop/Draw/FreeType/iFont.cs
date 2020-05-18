using ComLight;
using System;
using Vrmac.Draw;

namespace Vrmac.FreeType
{
	/// <summary>Represents a FreeType2 font face</summary>
	[ComInterface( "be38d945-5386-4bb3-8857-a7f397d6e87d", eMarshalDirection.ToManaged ), CustomConventions( typeof( Vrmac.Utils.NativeErrorMessages ) )]
	public interface iFont: IDisposable
	{
		/// <summary>Get information about the font</summary>
		void getInfo( out sFontInfo fi );

		/// <summary></summary>
		[RetValIndex]
		sScaledMetrics getScaledMetrics( uint size );

		/// <summary>Get glyph index for UTF32 character</summary>
		int getGlyphIndex( uint utf32 );

		/// <summary>Load a glyph</summary>
		[RetValIndex]
		sGlyphInfo loadGlyph( int index, eLoadGlyphFlags flags, uint size );

		/// <summary>Copy bitmap into the atlas texture array</summary>
		int buildBitmap( iTextureAtlas atlas );
	}
}