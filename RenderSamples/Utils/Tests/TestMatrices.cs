using System;
using System.Numerics;
using Vrmac;

namespace RenderSamples.Utils.Tests
{
	static class TestMatrices
	{
		public static void test()
		{
			Matrix3x2 trans = Matrix3x2.CreateTranslation( 10, 20 );
			Console.WriteLine( "trans = translate [10, 20]: {0}", trans );

			Matrix3x2 rotZero = Matrix3x2.CreateRotation( MathUtils.radians( 30 ) );
			Console.WriteLine( "rotZero = rotate 30 around center: {0}", rotZero );

			Matrix3x2 rotNonZero = Matrix3x2.CreateRotation( MathUtils.radians( -200 ), new Vector2( 3, 4 ) );
			Console.WriteLine( "rotNonZero = rotate -200 degrees around [3,4]: {0}", rotNonZero );

			Console.WriteLine( "trans * rotZero: {0}", trans * rotZero );
			Console.WriteLine( "rotZero * trans: {0}", rotZero * trans );
			Matrix3x2 mm = trans * rotNonZero;
			Console.WriteLine( "trans * rotNonZero: {0}", mm );

			Vector2 p = Vector2.Transform( new Vector2( 11, 12 ), mm );
			Console.WriteLine( "transformed point: {0}", p );
		}
	}
}