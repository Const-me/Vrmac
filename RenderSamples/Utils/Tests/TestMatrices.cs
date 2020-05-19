using Diligent.Graphics;
using System;
using System.Numerics;
using Vrmac;
using Matrix = Vrmac.Draw.Matrix;

namespace RenderSamples.Utils.Tests
{
	static class TestMatrices
	{
		public static void test()
		{
			Matrix trans = Matrix.createTranslation( 10, 20 );
			Console.WriteLine( "trans = translate [10, 20]: {0}", trans );

			Matrix rotZero = Matrix.createRotation( MathUtils.radians( 30 ) );
			Console.WriteLine( "rotZero = rotate 30 around center: {0}", rotZero );

			Matrix rotNonZero = Matrix.createRotation( MathUtils.radians( -200 ), 3, 4 );
			Console.WriteLine( "rotNonZero = rotate -200 degrees around [3,4]: {0}", rotNonZero );

			Console.WriteLine( "trans * rotZero: {0}", trans * rotZero );
			Console.WriteLine( "rotZero * trans: {0}", rotZero * trans );
			Matrix mm = trans * rotNonZero;
			Console.WriteLine( "trans * rotNonZero: {0}", mm );

			Vector2 p = mm.transformPoint( new Vector2( 11, 12 ) );
			Console.WriteLine( "transformed point: {0}", p );
		}
	}
}