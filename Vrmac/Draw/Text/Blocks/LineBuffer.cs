using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vrmac.Draw.Text
{
	sealed class LineBuffer
	{
		public const int initialCapacity = 16;

		sGlyphVertex[] vertices;
		public int length { get; private set; }

		LineBuffer()
		{
			vertices = new sGlyphVertex[ initialCapacity ];
			length = 0;
		}

		public void clear()
		{
			length = 0;
		}

		public Span<sGlyphVertex> appendGlyph()
		{
			if( length + 4 > vertices.Length )
			{
				int newSize = ( length + 4 ).nextPowerOf2();
				Array.Resize( ref vertices, newSize );
			}
			Span<sGlyphVertex> res = vertices.AsSpan().Slice( length, 4 );
			length += 4;
			return res;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void copyTo( Span<sVertexWithId> dest )
		{
			ReadOnlySpan<sVertexWithId> source = MemoryMarshal.Cast<sGlyphVertex, sVertexWithId>( vertices.AsSpan() );
			source = source.Slice( 0, length );
			source.CopyTo( dest );
		}

		public void copyWithOffset( Span<sVertexWithId> dest, CPoint offset )
		{
			ReadOnlySpan<sGlyphVertex> source = vertices.AsSpan();
			source = source.Slice( 0, length );
			GraphicsUtils.offsetGlyphs( dest, source, offset );
		}

		static readonly LineBuffer s_instance = new LineBuffer();

		public static LineBuffer instance
		{
			get
			{
				s_instance.clear();
				return s_instance;
			}
		}
	}
}