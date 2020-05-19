using System;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace RenderSamples
{
	static class SvgPathParser
	{
		struct Context
		{
			readonly string path;
			int pos;
			public Vector2? current;
			public Vector2? prevCp;
			public readonly Vector2[] points;

			public Vector2 setCurrent( Vector2 pt )
			{
				current = pt;
				return pt;
			}

			public Context( string p )
			{
				path = p;
				pos = 0;
				current = null;
				prevCp = null;
				points = new Vector2[ 3 ];
			}
			void skipWhite()
			{
				while( pos < path.Length && char.IsWhiteSpace( path[ pos ] ) )
					pos++;
			}
			public bool eof => pos >= path.Length;

			static bool isNumber( char c )
			{
				if( char.IsDigit( c ) )
					return true;
				return ".+-eE".Contains( c );
			}

			const NumberStyles numberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign;

			public float parseNumber()
			{
				skipWhite();
				int ep = pos;
				// SVG makes it very hard to parse these numbers.
				// They have many bugs filed, still did not fixed their specs: https://github.com/w3c/svgwg/issues/331
				if( path[ ep ] == '-' )
					ep++;
				bool hadDot = false;
				while( ep < path.Length )
				{
					char c = path[ ep ];
					if( char.IsDigit( c ) )
					{
						ep++;
						continue;
					}
					if( c == '.' )
					{
						if( hadDot )
							break;
						hadDot = true;
						ep++;
						continue;
					}
					break;
				}
				if( ep == pos )
					throw new EndOfStreamException();
				ReadOnlySpan<char> span = path.AsSpan().Slice( pos, ep - pos );
				float val = float.Parse( span, numberStyles, CultureInfo.InvariantCulture );
				pos = ep;
				skipWhite();
				return val;
			}

			// Skip whitespaces and 1 optional comma
			public void skipOptionalDelimiters()
			{
				skipWhite();
				if( nextChar == ',' )
				{
					pos++;
					skipWhite();
				}
			}

			public Vector2 parsePoint()
			{
				Vector2 r = new Vector2();
				r.X = parseNumber();
				skipOptionalDelimiters();
				r.Y = parseNumber();
				return r;
			}

			public void parseAbsPoints( int count )
			{
				points[ 0 ] = parsePoint();
				for( int i = 1; i < count; i++ )
				{
					skipOptionalDelimiters();
					points[ i ] = parsePoint();
				}
				current = points[ count - 1 ];
			}

			public void parseRelPoints( int count )
			{
				Vector2 cp = current ?? Vector2.Zero;
				points[ 0 ] = parsePoint() + cp;
				for( int i = 1; i < count; i++ )
				{
					skipOptionalDelimiters();
					points[ i ] = parsePoint() + cp;
				}
				current = points[ count - 1 ];
			}

			public char nextChar
			{
				get
				{
					skipWhite();
					if( eof )
						return '\0';
					return path[ pos ];
				}
			}

			public void next()
			{
				pos++;
				skipWhite();
				if( eof )
					throw new EndOfStreamException();
			}

			public bool tryNext()
			{
				pos++;
				skipWhite();
				return !eof;
			}
		}

		static bool parseCommand( iFigureBuilder figure, ref Context context, char cmd )
		{
			Vector2 fcp;
			switch( cmd )
			{
				case 'M':
					context.parseAbsPoints( 1 );
					figure.moveTo( context.points[ 0 ] );
					context.prevCp = null;
					break;
				case 'm':
					context.parseRelPoints( 1 );
					figure.moveTo( context.points[ 0 ] );
					context.prevCp = null;
					break;
				case 'L':
					context.parseAbsPoints( 1 );
					figure.lineTo( context.points[ 0 ] );
					context.prevCp = null;
					break;
				case 'l':
					context.parseRelPoints( 1 );
					figure.lineTo( context.points[ 0 ] );
					context.prevCp = null;
					break;
				case 'V':
					fcp = context.current.Value;
					fcp.Y = context.parseNumber();
					context.setCurrent( fcp );
					figure.lineTo( fcp );
					break;
				case 'v':
					fcp = context.current.Value;
					fcp.Y += context.parseNumber();
					context.setCurrent( fcp );
					figure.lineTo( fcp );
					break;
				case 'C':
					context.parseAbsPoints( 3 );
					figure.cubicBezier( context.points[ 0 ], context.points[ 1 ], context.points[ 2 ] );
					context.prevCp = context.points[ 1 ];
					break;
				case 'c':
					context.parseRelPoints( 3 );
					figure.cubicBezier( context.points[ 0 ], context.points[ 1 ], context.points[ 2 ] );
					context.prevCp = context.points[ 1 ];
					break;
				case 'S':
					if( context.prevCp.HasValue )
						fcp = context.current.Value * 2.0f - context.prevCp.Value;
					else
						fcp = context.current.Value;
					context.parseAbsPoints( 2 );
					figure.cubicBezier( fcp, context.points[ 0 ], context.points[ 1 ] );
					context.prevCp = context.points[ 0 ];
					break;
				case 's':
					if( context.prevCp.HasValue )
						fcp = context.current.Value * 2.0f - context.prevCp.Value;
					else
						fcp = context.current.Value;
					context.parseRelPoints( 2 );
					figure.cubicBezier( fcp, context.points[ 0 ], context.points[ 1 ] );
					context.prevCp = context.points[ 0 ];
					break;
				case 'z':
				case 'Z':
					figure.closePath();
					return true;
				default:
					throw new NotImplementedException();
			}
			return false;
		}

		public static void parse( iFigureBuilder figure, string path )
		{
			Context context = new Context( path );

			char prevCommand = '\0';
			while( !context.eof )
			{
				context.skipOptionalDelimiters();
				char c = context.nextChar;
				if( '\0' == c )
					return;

				if( ( char.IsDigit( c ) || c == '-' ) && prevCommand != '\0' )
				{
					// https://www.w3.org/TR/SVG/paths.html#PathDataMovetoCommands
					// If a moveto is followed by multiple pairs of coordinates, the subsequent pairs are treated as implicit lineto commands.
					switch( prevCommand )
					{
						case 'm':
							prevCommand = 'l';
							break;
						case 'M':
							prevCommand = 'L';
							break;
					}
					if( parseCommand( figure, ref context, prevCommand ) )
						return;
					continue;
				}

				if( c == 'z' || c == 'Z' )
				{
					figure.closePath();
					if( !context.tryNext() )
						return;
					continue;
				}

				if( !context.tryNext() )
					return;

				if( parseCommand( figure, ref context, c ) )
					return;
				prevCommand = c;
			}
		}
	}
}