using System;
using System.Runtime.InteropServices;
using Vrmac.FreeType;

namespace Vrmac.Draw.Text
{
	sealed partial class Font
	{
		public sMeshDataSize renderConsole( Span<sVertexWithId> span, uint id, ReadOnlySpan<char> str, CPoint where, eTextRendering how, int width = 80 )
		{
			if( str.IsEmpty )
				return new sMeshDataSize();
			ref var resourses = ref getResources( how );
			Kompiler.ConsoleBlockDelegate kompiled;

			switch( how )
			{
				case eTextRendering.ClearTypeHorizontal:
					// Untransformed horizontal cleartype, the shader will read color texture by integer texture coordinates
					kompiled = compiledDelegates.consoleBlockCT;
					if( null == kompiled )
					{
						kompiled = Kompiler.consoleBlock( resourses.textureAtlas.layersCount, getAllGlyphs( how ) );
						compiledDelegates.consoleBlockCT = kompiled;
					}
					break;

				default:
					throw new NotSupportedException();
			}

			Span<sGlyphVertex> glyphsSpan = MemoryMarshal.Cast<sVertexWithId, sGlyphVertex>( span );
			sScaledMetrics metrics = scaledMetrics;
			ConsoleBlock context = new ConsoleBlock( glyphsSpan, where, ref metrics, id, width );
			kompiled( ref context, str );
			return context.actualMeshSize();
		}

		public CSize measureConsoleText( string text, int widthChars )
		{
			int maxWidth = 0, countLines = 0;
			int x = 0;
			Decoder dec = new Decoder( text );
			for( uint utf32 = dec.nextChar(); utf32 != uint.MaxValue; utf32 = dec.nextChar() )
			{
				if( countLines <= 0 )
					countLines = 1;
				switch( utf32 )
				{
					case '\n':
						countLines++;
						maxWidth = Math.Max( maxWidth, x );
						x = 0;
						break;
					case '\t':
						x = ( x + 4 ) & ( ~3 );
						break;
					default:
						if( x < widthChars )
						{
							x++;
							break;
						}
						countLines++;
						maxWidth = Math.Max( maxWidth, x );
						x = 1;
						break;
				}
			}
			maxWidth = Math.Max( maxWidth, x );

			sScaledMetrics metrics = scaledMetrics;
			return new CSize()
			{
				cx = metrics.maxAdvancePixels * ( maxWidth + 1 ),   // Because zero based
				cy = metrics.lineHeight * countLines,
			};
		}
	}
}