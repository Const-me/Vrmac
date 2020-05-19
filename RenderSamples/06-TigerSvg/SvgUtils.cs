using System.Numerics;

namespace RenderSamples
{
	static class SvgUtils
	{
		public static Vector4? fixFill( Vector4? c )
		{
			if( !c.HasValue )
				return null;
			if( c.Value.W < 1.0f / 255.0f )
				return null;
			return c;
		}

		public static float fixStroke( float? width, ref Vector4? c )
		{
			if( !width.HasValue || width.Value <= 0 )
			{
				c = null;
				return 0;
			}
			c = fixFill( c );
			if( !c.HasValue )
				return 0;
			return width.Value;
		}
	}
}