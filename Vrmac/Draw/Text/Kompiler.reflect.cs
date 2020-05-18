using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vrmac.Draw.Text
{
	static partial class Kompiler
	{
		static IEnumerable<Type> makeEmitGlyphArgs()
		{
			yield return typeof( int );     // int advance
			yield return typeof( short );   // short lsbDelta
			yield return typeof( short );   // short rsbDelta
			yield return typeof( short );   // short spriteLeft
			yield return typeof( short );   // short spriteTop
			yield return typeof( ushort );  // ushort sx
			yield return typeof( ushort );  // ushort sy
			yield return typeof( uint );    // uint uvTopLeft
			yield return typeof( uint );    // uint uvBottomRight
			yield return typeof( byte );    // byte layer
		}
		static readonly Type[] emitGlyphArgTypes = makeEmitGlyphArgs().ToArray();

		static IEnumerable<Type> makeSkipGlyphArgs()
		{
			yield return typeof( int );     // int advance
			yield return typeof( short );   // short lsbDelta
			yield return typeof( short );   // short rsbDelta
		}

		static readonly Type[] skipGlyphArgTypes = makeSkipGlyphArgs().ToArray();

		struct ContextReflection
		{
			public readonly ParameterExpression paramExpression;
			public readonly MethodInfo miEmit, miSkip;
			public readonly MethodInfo miNewLine, miNbsp, miTab;

			public ContextReflection( Type type )
			{
				miEmit = type.GetMethod( "emitGlyph", emitGlyphArgTypes );
				if( null == miEmit )
					throw new ArgumentException( "The context type doesn’t implement the required `emitGlyph` method" );

				miSkip = type.GetMethod( "skipGlyph", skipGlyphArgTypes );
				if( null == miSkip )
					throw new ArgumentException( "The context type doesn’t implement the required `skipGlyph` method" );

				// The methods below are optional. Some context types implement them and get them called, others don't.
				miNewLine = type.GetMethod( "newline", Type.EmptyTypes );
				miNbsp = type.GetMethod( "nonBreakingSpace", skipGlyphArgTypes );
				miTab = type.GetMethod( "tabulation", Type.EmptyTypes );

				paramExpression = Expression.Parameter( type.MakeByRefType(), "context" );
			}
		}

		// Caching stuff which depends on the context type, but not font or glyphs.
		static readonly Dictionary<Type, ContextReflection> contextTypes = new Dictionary<Type, ContextReflection>();

		static ContextReflection getContextReflection( Type t )
		{
			if( contextTypes.TryGetValue( t, out var r ) )
				return r;
			r = new ContextReflection( t );
			contextTypes.Add( t, r );
			return r;
		}
	}
}