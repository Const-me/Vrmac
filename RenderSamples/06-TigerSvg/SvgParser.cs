using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml;
using Vrmac;
using Vrmac.Draw;
using Matrix = Vrmac.Draw.Matrix;

namespace RenderSamples
{
	interface iFigureBuilder: IDisposable
	{
		void moveTo( Vector2 pt );
		void cubicBezier( Vector2 c1, Vector2 c2, Vector2 end );
		void lineTo( Vector2 pt );
		void closePath();
	}

	interface iSvgSink
	{
		void viewbox( Rect rect );

		iFigureBuilder newFigure( float? strokeWidth, Vector4? strokeColor, Vector4? fillColor, Matrix transform );
	}

	// This SVG parser is only good to parse Tiger image from there: https://commons.wikimedia.org/wiki/File:Ghostscript_Tiger.svg
	// Nothing else is implemented.
	static class SvgParser
	{
		struct State
		{
			public float strokeWidth;
			public Vector4? strokeColor;
			public Vector4? fillColor;
			public Matrix transform;
		}

		public static void parse( Stream stream, iSvgSink sink )
		{
			using( XmlReader reader = XmlReader.Create( stream ) )
			{
				Stack<State> stack = new Stack<State>();
				while( reader.Read() )
				{
					switch( reader.NodeType )
					{
						case XmlNodeType.Element:
							switch( reader.Name )
							{
								case "g":
									pushGroup( stack, reader );
									break;
								case "path":
									State s = currentState( stack );
									string pathData = reader.GetAttribute( "d" );
									using( var figure = sink.newFigure( s.strokeWidth, s.strokeColor, s.fillColor, s.transform ) )
										SvgPathParser.parse( figure, pathData );
									break;
								case "svg":
									Rect? vb = parseViewbox( reader );
									if( vb.HasValue )
										sink.viewbox( vb.Value );
									break;
							}
							break;
						case XmlNodeType.EndElement:
							if( reader.Name == "g" )
								stack.Pop();
							break;
					}
				}
			}
		}

		static State currentState( Stack<State> stack )
		{
			if( stack.TryPeek( out var s ) )
				return s;
			s = new State();
			s.transform = Matrix.identity;
			return s;
		}

		static double parseNumber( string s )
		{
			return double.Parse( s, CultureInfo.InvariantCulture );
		}
		static Vector4? parseColor( string s )
		{
			if( s.Equals( "none", StringComparison.InvariantCultureIgnoreCase ) )
				return null;
			return Color.parse( s );
		}

		static readonly Regex reMatrix = new Regex( @"^\s*matrix\s*\(\s*(\S.*\S)\s*\)\s*$", RegexOptions.IgnoreCase );

		static string group( this Match m, int idx = 0 )
		{
			return m.Groups[ idx + 1 ].Value;
		}

		static Matrix parseTransform( string str )
		{
			var m = reMatrix.Match( str );
			if( !m.Success )
				throw new ArgumentException();
			string values = m.group( 0 );
			string[] arr = values.Split( " \t,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
			if( 6 != arr.Length )
				throw new ArgumentException();

			Span<float> floats = stackalloc float[ 6 ];
			for( int i = 0; i < 6; i++ )
				floats[ i ] = (float)parseNumber( arr[ i ] );

			Matrix r = new Matrix();
			r.m11 = floats[ 0 ];
			r.m12 = floats[ 2 ];
			r.m21 = floats[ 1 ];
			r.m22 = floats[ 3 ];
			r.dx = floats[ 4 ];
			r.dy = floats[ 5 ];
			return r;
		}

		static void pushGroup( Stack<State> stack, XmlReader reader )
		{
			State s = currentState( stack );

			string str = reader.GetAttribute( "stroke-width" );
			if( null != str )
			{
				if( "none" == str )
					s.strokeWidth = 0;
				else
					s.strokeWidth = (float)parseNumber( str );
			}

			str = reader.GetAttribute( "stroke" );
			if( null != str )
				s.strokeColor = parseColor( str );

			str = reader.GetAttribute( "fill" );
			if( null != str )
				s.fillColor = parseColor( str );

			str = reader.GetAttribute( "transform" );
			if( null != str )
				s.transform *= parseTransform( str );

			stack.Push( s );
		}

		static Rect makeRect( double x, double y, double w, double h )
		{
			return new Rect( (float)x, (float)y, (float)( x + w ), (float)( y + h ) );
		}

		static Rect? parseViewbox( XmlReader reader )
		{
			string vb = reader.GetAttribute( "viewBox" );
			string[] fields = vb.Split( " \t,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries );    // The numbers separated by whitespace and/or a comma :-(
			if( fields.Length != 4 )
				throw new ArgumentException();
			double[] numbers = fields.Select( parseNumber ).ToArray();
			return makeRect( numbers[ 0 ], numbers[ 1 ], numbers[ 2 ], numbers[ 3 ] );
		}
	}
}