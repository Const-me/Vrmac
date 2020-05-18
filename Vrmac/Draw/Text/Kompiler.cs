using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vrmac.Draw.Text
{
	/// <summary>This utility class compiles character metrics and glyph UV coordinates into .NET code.</summary>
	/// <remarks>
	/// <para>Even very optimized C++ hash tables are still slower than immediate values in the instruction stream, because memory latency.</para>
	/// <para>If you wonder why K in the name - to disambiguate from the rest of the compilers used in this software.
	/// We have quite a few of them here: C++, C#, HLSL, GLSL.</para>
	/// </remarks>
	static partial class Kompiler
	{
		// Caching whatever pieces of the expression tree don't depend on font, glyphs or context types.
		// This is to reduce GC allocations.

		static readonly ParameterExpression decoder = Expression.Variable( typeof( Decoder ), "decoder" );
		static readonly ParameterExpression utf32 = Expression.Variable( typeof( uint ), "utf32" );

		// return target
		static readonly LabelExpression returnLabel = Expression.Label( Expression.Label( "returnTarget" ) );
		// `return;`
		static readonly GotoExpression returnExpression = Expression.Return( returnLabel.Target );

		// `break;`
		static readonly GotoExpression switchBreak = Expression.Break( Expression.Label( "breakLabel" ) );
		// Break label
		static readonly LabelExpression switchBreakLabel = Expression.Label( switchBreak.Target );

		// decoder = new Decoder( utf16 );
		static readonly Expression createDecoder;
		// utf32 = decoder.nextChar();
		static readonly Expression decodeCharacter;

		static readonly List<ConstantExpression> layerConstants = new List<ConstantExpression>();

		// `ReadOnlySpan<char> utf16` parameter
		static readonly ParameterExpression paramString = Expression.Parameter( typeof( ReadOnlySpan<char> ), "utf16" );

		static readonly ParameterExpression[] localVariables;
		static readonly List<ParameterExpression> parameters = new List<ParameterExpression>( 3 );
		static readonly List<Expression> loopBody = new List<Expression>( 3 );
		static readonly List<Expression> functionBody = new List<Expression>( 3 );

		// case uint.MaxValue: return;
		static readonly SwitchCase endOfStringCase;

		static Kompiler()
		{
			Type[] decoderCtorParams = new Type[ 1 ] { typeof( ReadOnlySpan<char> ) };
			var ci = typeof( Decoder ).GetConstructor( decoderCtorParams );
			createDecoder = Expression.Assign( decoder, Expression.New( ci, paramString ) );

			MethodInfo miNextChar = typeof( Decoder ).GetMethod( "nextChar", BindingFlags.Instance | BindingFlags.Public );
			Expression callNextChar = Expression.Call( decoder, miNextChar );
			decodeCharacter = Expression.Assign( utf32, callNextChar );

			layerConstants.Add( Expression.Constant( (byte)0, typeof( byte ) ) );

			localVariables = new ParameterExpression[ 2 ]
			{
				decoder,
				utf32
			};

			endOfStringCase = Expression.SwitchCase
				(
				Expression.Return( returnLabel.Target ),
				Expression.Constant( uint.MaxValue, typeof( uint ) )
				);
		}

		static readonly SortedDictionary<uint, SwitchCase> map = new SortedDictionary<uint, SwitchCase>();

		static void addSwitchCase( uint utf32, ParameterExpression instance, MethodInfo mi, params Expression[] arguments )
		{
			var call = Expression.Call( instance, mi, arguments );
			var u32 = Expression.Constant( utf32, typeof( uint ) );
			Expression block = Expression.Block( call, switchBreak );
			SwitchCase sc = Expression.SwitchCase( block, u32 );
			map.Add( utf32, sc );
		}

		const char nbsp = '\u00A0';

		// Build SwitchCase expression for one UTF32 character
		static void buildGlyph( ref ContextReflection reflection, Font.GlyphData glyph, byte paddingPixel, uint paddingUV )
		{
			if( glyph.utf32 == '\n' && null != reflection.miNewLine )
			{
				addSwitchCase( '\n', reflection.paramExpression, reflection.miNewLine );
				return;
			}
			if( glyph.utf32 == '\t' && null != reflection.miTab )
			{
				addSwitchCase( '\t', reflection.paramExpression, reflection.miTab );
				return;
			}

			Expression advance = Expression.Constant( glyph.data.advance );
			Expression lsbDelta = Expression.Constant( glyph.data.lsbDelta );
			Expression rsbDelta = Expression.Constant( glyph.data.rsbDelta );

			if( glyph.utf32 == nbsp && null != reflection.miNbsp )
			{
				addSwitchCase( nbsp, reflection.paramExpression, reflection.miNbsp,
					advance, lsbDelta, rsbDelta );
				return;
			}

			if( glyph.data.hasSprite )
			{
				// The glyph has a bitmap on the atlas
				short left = glyph.data.left;
				short top = glyph.data.top;
				left -= paddingPixel;
				top -= paddingPixel;

				CSize size = glyph.sprite.size;
				size.cx += 2 * paddingPixel;
				size.cy += 2 * paddingPixel;

				Expression spriteLeft = Expression.Constant( left, typeof( short ) );
				Expression spriteTop = Expression.Constant( top, typeof( short ) );
				Expression sx = Expression.Constant( (ushort)size.cx, typeof( ushort ) );
				Expression sy = Expression.Constant( (ushort)size.cy, typeof( ushort ) );

				ulong uv = glyph.sprite.uv;
				uint uvTL = (uint)( uv & uint.MaxValue );
				uint uvBR = (uint)( uv >> 32 );
				uvTL -= paddingUV;
				uvBR += paddingUV;

				Expression uvTopLeft = Expression.Constant( uvTL, typeof( uint ) );
				Expression uvBottomRight = Expression.Constant( uvBR, typeof( uint ) );
				Expression layer = layerConstants[ glyph.sprite.layer ];

				addSwitchCase( glyph.utf32, reflection.paramExpression, reflection.miEmit,
					advance, lsbDelta, rsbDelta,
					spriteLeft, spriteTop, sx, sy,
					uvTopLeft, uvBottomRight, layer );
				return;
			}

			// The glyph does not have a bitmap. The most likely reason for that, it's a space character.
			addSwitchCase( glyph.utf32, reflection.paramExpression, reflection.miSkip,
				advance, lsbDelta, rsbDelta );
		}

		/// <summary>Compile all glyphs of the font into .NET codez</summary>
		static TDelegate compile<TDelegate>( int atlasLayersCount, IEnumerable<Font.GlyphData> glyphs, Type tContext, byte paddingPixel, uint paddingUV )
			where TDelegate : Delegate
		{
			// If needed, create moar layer index constants
			while( atlasLayersCount > layerConstants.Count )
			{
				byte layer = (byte)layerConstants.Count;
				layerConstants.Add( Expression.Constant( layer, typeof( byte ) ) );
			}

			ContextReflection reflection = getContextReflection( tContext );

			// Build sorted map of these case expressions
			map.Clear();
			foreach( Font.GlyphData glyph in glyphs )
				buildGlyph( ref reflection, glyph, paddingPixel, paddingUV );
			// Add end of string switch-case, which tests for UINT_MAX and returns
			map.Add( uint.MaxValue, endOfStringCase );

			// Implement body of the loop
			loopBody.Clear();
			loopBody.Add( decodeCharacter );
			loopBody.Add( Expression.Switch( utf32, map.Values.ToArray() ) );
			loopBody.Add( switchBreakLabel );

			// Implement body of the function
			functionBody.Clear();
			functionBody.Add( createDecoder );
			functionBody.Add( Expression.Loop( Expression.Block( loopBody ) ) );
			functionBody.Add( returnLabel );

			// Gather delegate parameters into a list
			parameters.Clear();
			parameters.Add( reflection.paramExpression );
			parameters.Add( paramString );

			// Finally, compile all that stuff into CIL. JIT compiler will do the rest.
			var lambda = Expression.Lambda<TDelegate>( Expression.Block( localVariables, functionBody ), parameters );
			TDelegate result = lambda.Compile();

			// Cleanup temporary stuff. Hopefully, all these no longer needed objects will be garbage collected right away, while still at generation 0.
			map.Clear();
			loopBody.Clear();
			functionBody.Clear();
			parameters.Clear();

			return result;
		}

		// Apparently, Microsoft thinks copy-pasting is underrated: https://stackoverflow.com/q/50871135/126995

		// Single-line version, no longer used
		public delegate void RenderLineDelegate( ref SingleLineContext context, ReadOnlySpan<char> utf16 );
		public static RenderLineDelegate renderLine( int atlasLayersCount, IEnumerable<Font.GlyphData> glyphs ) =>
			compile<RenderLineDelegate>( atlasLayersCount, glyphs, typeof( SingleLineContext ), 0, 0 );

		// Left aligned blocks, with and without extra UV padding used by transformed text
		public delegate void LeftAlignedBlockDelegate( ref LeftAlignedBlock context, ReadOnlySpan<char> utf16 );
		public static LeftAlignedBlockDelegate leftAlignBlock( int atlasLayersCount, IEnumerable<Font.GlyphData> glyphs ) =>
			compile<LeftAlignedBlockDelegate>( atlasLayersCount, glyphs, typeof( LeftAlignedBlock ), 0, 0 );
		public static LeftAlignedBlockDelegate leftAlignBlock( int atlasLayersCount, IEnumerable<Font.GlyphData> glyphs, uint paddingUV ) =>
			compile<LeftAlignedBlockDelegate>( atlasLayersCount, glyphs, typeof( LeftAlignedBlock ), 1, paddingUV );

		// Console-like line breaking version. Only supports untransformed text.
		public delegate void ConsoleBlockDelegate( ref ConsoleBlock context, ReadOnlySpan<char> utf16 );
		public static ConsoleBlockDelegate consoleBlock( int atlasLayersCount, IEnumerable<Font.GlyphData> glyphs ) =>
			compile<ConsoleBlockDelegate>( atlasLayersCount, glyphs, typeof( ConsoleBlock ), 0, 0 );

		// Measuring size of text, it doesn't care whether the text transformed or not.
		public delegate void MeasureBlockDelegate( ref MeasureBlock context, ReadOnlySpan<char> utf16 );
		public static MeasureBlockDelegate measureBlock( int atlasLayersCount, IEnumerable<Font.GlyphData> glyphs ) =>
			compile<MeasureBlockDelegate>( atlasLayersCount, glyphs, typeof( MeasureBlock ), 0, 0 );
	}
}