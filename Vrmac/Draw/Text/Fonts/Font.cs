using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vrmac.FreeType;
using GlyphDictionary = System.Collections.Generic.Dictionary<uint, Vrmac.Draw.Text.sGlyphData>;

namespace Vrmac.Draw.Text
{
	sealed partial class Font: iFont
	{
		readonly FontFace fontFace;
		readonly uint size;
		readonly sScaledMetrics scaledMetrics;

		struct TextRenderResources
		{
			public readonly eLoadGlyphFlags loadFlags;

			// Keys in the dictionary are UTF32 values
			public readonly GlyphDictionary glyphs;

			public readonly TextureAtlas textureAtlas;

			public TextRenderResources( eLoadGlyphFlags loadFlags, GlyphDictionary glyphs, TextureAtlas textureAtlas )
			{
				this.loadFlags = loadFlags;
				this.glyphs = glyphs;
				this.textureAtlas = textureAtlas;
			}
		}

		readonly TextRenderResources[] styles;

		public Font( Textures textures, FontFace fontFace, uint sizePixels )
		{
			styles = new TextRenderResources[ 4 ];

			var glyphs = new Dictionary<uint, sGlyphData>( 64 );
			eLoadGlyphFlags loadFlags = eLoadGlyphFlags.Render | eLoadGlyphFlags.ForceAutohint | eLoadGlyphFlags.TargetLight;

			styles[ (byte)eTextRendering.GrayscaleExact ] = new TextRenderResources( loadFlags, glyphs, textures.grayscale );
			styles[ (byte)eTextRendering.GrayscaleTransformed ] = new TextRenderResources( loadFlags, glyphs, textures.grayscale );

			glyphs = new Dictionary<uint, sGlyphData>( 64 );
			loadFlags = eLoadGlyphFlags.Render | eLoadGlyphFlags.ForceAutohint | eLoadGlyphFlags.TargetClearTypeHorizontal;
			styles[ (byte)eTextRendering.ClearTypeHorizontal ] = new TextRenderResources( loadFlags, glyphs, textures.cleartype );

			glyphs = new Dictionary<uint, sGlyphData>( 64 );
			loadFlags = eLoadGlyphFlags.Render | eLoadGlyphFlags.ForceAutohint | eLoadGlyphFlags.TargetClearTypeVertical;
			styles[ (byte)eTextRendering.ClearTypeVertical ] = new TextRenderResources( loadFlags, glyphs, textures.cleartype );

			this.fontFace = fontFace;
			scaledMetrics = fontFace.getScaledMetrics( sizePixels );

			textures.grayscale.resized.add( this, onGrayAtlasResized );
			textures.cleartype.resized.add( this, onCleartypeAtlasResized );

			size = sizePixels;
		}

		ref TextRenderResources getResources( eTextRendering trs )
		{
			return ref styles[ (byte)trs ];
		}
		GlyphDictionary getGlyphs( eTextRendering trs )
		{
			return getResources( trs ).glyphs;
		}
		TextureAtlas getAtlas( eTextRendering trs )
		{
			return getResources( trs ).textureAtlas;
		}

		void onGrayAtlasResized()
		{
			compiledDelegates.dropGrayscale();
		}
		void onCleartypeAtlasResized()
		{
			compiledDelegates.dropCleartype();
		}

		public struct GlyphData
		{
			public uint utf32;
			public sGlyphData data;
			public GlyphSprite sprite;
		}

		IEnumerable<GlyphData> getAllGlyphs( eTextRendering how )
		{
			TextureAtlas atlas = getAtlas( how );
			GlyphData result = new GlyphData();
			foreach( var kvp in getGlyphs( how ) )
			{
				result.utf32 = kvp.Key;
				result.data = kvp.Value;
				if( result.data.hasSprite )
					result.sprite = atlas.getGlyph( result.data.spriteIndex );
				else
					result.sprite = default;

				yield return result;
			}
		}

		KompiledDelegates compiledDelegates = new KompiledDelegates();

		iFontFace iFont.fontFace => fontFace;
		uint iFont.sizePixels => size;
		int iFont.lineHeightPixels => scaledMetrics.lineHeight;

		/// <summary>Rasterize the glyphs and put them in the atlas. Returns upper estimate of mesh data size.</summary>
		public sMeshDataSize prepareGlyphs( ReadOnlySpan<char> str, eTextRendering how )
		{
			if( str.IsEmpty )
				return new sMeshDataSize();

			ref var resourses = ref getResources( how );
			eLoadGlyphFlags loadFlags = resourses.loadFlags;

			Decoder decoder = new Decoder( str );
			int countChars = 0;
			while( true )
			{
				uint utf32 = decoder.nextChar();
				if( utf32 == uint.MaxValue )
					return new sMeshDataSize( countChars * 4, countChars * 2 );

				if( resourses.glyphs.TryGetValue( utf32, out sGlyphData data ) )
				{
					// Have the glyph built already
					if( data.hasSprite )
					{
						// It even comes with a bitmap
						countChars++;
					}
					continue;
				}

				compiledDelegates = default;

				int index = fontFace.getGlyphIndex( utf32 );
				sGlyphInfo info = fontFace.font.loadGlyph( index, loadFlags, size );
				if( info.hasBitmap )
				{
					int idx = resourses.textureAtlas.addGlyph( fontFace.font );
					data = new sGlyphData( scaledMetrics.baselinePosition, ref info, idx );
					resourses.glyphs.Add( utf32, data );
					countChars++;
					continue;
				}

				data = new sGlyphData( scaledMetrics.baselinePosition, ref info );
				resourses.glyphs.Add( utf32, data );
			}
		}

		public sMeshDataSize renderLine( Span<sVertexWithId> span, uint id, ReadOnlySpan<char> str, CPoint start, eTextRendering how )
		{
			if( str.IsEmpty )
				return new sMeshDataSize();
			ref var resourses = ref getResources( how );

			Kompiler.RenderLineDelegate kompiled;
			switch( how )
			{
				case eTextRendering.GrayscaleExact:
					kompiled = compiledDelegates.renderLine;
					if( null == kompiled )
					{
						kompiled = Kompiler.renderLine( resourses.textureAtlas.layersCount, getAllGlyphs( how ) );
						compiledDelegates.renderLine = kompiled;
					}
					break;
				default:
					throw new NotImplementedException();

			}

			Span<sGlyphVertex> glyphsSpan = MemoryMarshal.Cast<sVertexWithId, sGlyphVertex>( span );
			SingleLineContext context = new SingleLineContext( glyphsSpan, start, id );
			compiledDelegates.renderLine( ref context, str );
			return context.actualMeshSize();
		}
	}
}