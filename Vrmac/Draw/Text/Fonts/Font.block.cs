using System;
using System.Runtime.InteropServices;

namespace Vrmac.Draw.Text
{
	sealed partial class Font: iFont
	{
		public sMeshDataSize renderBlock( Span<sVertexWithId> span, uint id, ReadOnlySpan<char> str, ref CRect rect, eTextRendering how )
		{
			if( str.IsEmpty )
				return new sMeshDataSize();
			ref var resourses = ref getResources( how );

			Kompiler.LeftAlignedBlockDelegate kompiled;

			switch( how )
			{
				case eTextRendering.GrayscaleTransformed:
					// Rotated text. ClearType will do no good, need padded grayscale glyphs with bilinear texture sampler in the shader
					kompiled = compiledDelegates.leftBlockTransformed;
					if( null == kompiled )
					{
						uint pixelUv = resourses.textureAtlas.pixelSizeInUvUnits;
						kompiled = Kompiler.leftAlignBlock( resourses.textureAtlas.layersCount, getAllGlyphs( how ), pixelUv );
						compiledDelegates.leftBlockTransformed = kompiled;
					}
					break;

				case eTextRendering.GrayscaleExact:
					// Untransformed grayscale AA text, need non-padded grayscale glyphs, the shader will read gray texture by integer texture coordinates
					kompiled = compiledDelegates.leftAlignBlock;
					if( null == kompiled )
					{
						kompiled = Kompiler.leftAlignBlock( resourses.textureAtlas.layersCount, getAllGlyphs( how ) );
						compiledDelegates.leftAlignBlock = kompiled;
					}
					break;

				case eTextRendering.ClearTypeHorizontal:
					// Untransformed horizontal cleartype, the shader will read color texture by integer texture coordinates
					kompiled = compiledDelegates.leftBlockCT;
					if( null == kompiled )
					{
						kompiled = Kompiler.leftAlignBlock( resourses.textureAtlas.layersCount, getAllGlyphs( how ) );
						compiledDelegates.leftBlockCT = kompiled;
					}
					break;

				default:
					throw new NotImplementedException();
			}

			Span<sGlyphVertex> glyphsSpan = MemoryMarshal.Cast<sVertexWithId, sGlyphVertex>( span );
			LeftAlignedBlock context = new LeftAlignedBlock( span, rect, scaledMetrics.lineHeight, id );
			kompiled( ref context, str );
			context.flush();
			return context.actualMeshSize();
		}

		public CSize measureText( ReadOnlySpan<char> str, int widthPixels, eTextRendering renderingMode )
		{
			if( str.IsEmpty )
				return default;
			prepareGlyphs( str, renderingMode );
			ref var resourses = ref getResources( renderingMode );

			Kompiler.MeasureBlockDelegate kompiled;

			switch( renderingMode )
			{
				case eTextRendering.GrayscaleTransformed:
				case eTextRendering.GrayscaleExact:
					// Untransformed grayscale AA text, need non-padded grayscale glyphs, the shader will read gray texture by integer texture coordinates
					kompiled = compiledDelegates.measureGray;
					if( null == kompiled )
					{
						kompiled = Kompiler.measureBlock( resourses.textureAtlas.layersCount, getAllGlyphs( renderingMode ) );
						compiledDelegates.measureGray = kompiled;
					}
					break;

				case eTextRendering.ClearTypeHorizontal:
					// Untransformed horizontal cleartype, the shader will read color texture by integer texture coordinates
					kompiled = compiledDelegates.measureCT;
					if( null == kompiled )
					{
						kompiled = Kompiler.measureBlock( resourses.textureAtlas.layersCount, getAllGlyphs( renderingMode ) );
						compiledDelegates.measureCT = kompiled;
					}
					break;

				default:
					throw new NotImplementedException();
			}

			MeasureBlock context = new MeasureBlock( widthPixels, scaledMetrics.lineHeight );
			kompiled( ref context, str );
			return context.getResult();
		}
	}
}