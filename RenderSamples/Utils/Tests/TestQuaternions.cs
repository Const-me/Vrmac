using Diligent.Graphics;
using System;
using System.Diagnostics;

namespace RenderSamples.Utils.Tests
{
	static class TestQuaternions
	{
		static Quaternion rotateWithFormula( Quaternion q, Vector3 v, float t )
		{
			// 
			v *= ( 0.5f * t );
			return q + new Quaternion( v, 0 ) * q;
		}

		public static void test()
		{
			Vector3 axis = Vector3.UnitY;
			float angle = MathF.PI / 3;	// 60 degrees
			Vector3 vec = axis * angle;

			Quaternion q1 = Quaternion.Identity.rotate( vec, 6 );
			Quaternion q2 = rotateWithFormula( Quaternion.Identity, vec, 6 );
			Debugger.Break();
		}
	}
}
