using System.Globalization;
using System.Numerics;

namespace Vrmac.MediaEngine.Render
{
	static class Utils
	{
		static readonly char[] floatMarkers = new char[] { '.', 'E', 'e' };

		// Stupid GLSL can't auto-cast integers to floats, not even literals, shaders instead fail to compile :-(
		static string ensureDot( string str )
		{
			if( str.IndexOfAny( floatMarkers ) < 0 )
				return str + ".0";
			return str;
		}

		static string print( this double f ) =>
			ensureDot( f.ToString( "G", CultureInfo.InvariantCulture ) );

		public static string printFloat2( double x, double y )
		{
			return $"{ x.print() }, { y.print() }";
		}

		static string print( this float f ) =>
			ensureDot( f.ToString( "F5", CultureInfo.InvariantCulture ) );

		public static string printFloat4( Vector4 vec )
		{
			return $"{ vec.X.print() }, { vec.Y.print() }, { vec.Z.print() }, { vec.W.print() }";
		}
	}
}