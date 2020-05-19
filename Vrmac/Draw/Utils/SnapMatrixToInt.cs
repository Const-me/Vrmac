using System.Collections.Generic;
using System.Numerics;

namespace Vrmac.Draw
{
	static class SnapMatrixToInt
	{
		const float scalingTolerance = 1.015625f;
		const float rotationTolerance = 0.015625f;

		static readonly Dictionary<(sbyte, sbyte, sbyte, sbyte), IntMatrix> lookup = new Dictionary<(sbyte, sbyte, sbyte, sbyte), IntMatrix>();

		static SnapMatrixToInt()
		{
			for( byte i = 0; i < 8; i++ )
			{
				IntMatrix im = new IntMatrix( i );
				var values = im.matrixValues;
				lookup[ (values.m00, values.m01, values.m10, values.m11) ] = im;
			}
		}

		static sbyte snapFloat( float f )
		{
			if( f < -0.5f )
				return -1;
			if( f < 0.5f )
				return 0;
			return 1;
		}

		public static IntMatrix? snapMatrixToInt( ref this Matrix3x2 tform )
		{
			Vector2 scaling = tform.getScaling();
			Vector2 scalingInc = scaling * scalingTolerance;
			if( scaling.X > scalingInc.Y || scaling.Y > scalingInc.X )
				return null;    // Detected non-uniform scaling, don't want that matrix.

			// Cancel the scaling
			Vector4 rotationValues = tform.rotationMatrix();
			Vector2 scalingInv = Vector2.One / scaling;
			Vector4 scalingInv4 = new Vector4( scalingInv.X, scalingInv.X, scalingInv.Y, scalingInv.Y );
			rotationValues *= scalingInv4;
			Matrix3x2 flipRotate = MathUtils.createRotation( ref rotationValues );

			// This one only contains rotation and flip.

			Vector2 vec = Vector2.Transform( Vector2.UnitX, flipRotate ).absolute();
			if( vec.X >= rotationTolerance && vec.Y >= rotationTolerance )
				return null;

			// OK, the matrix transformed X unit vector into something axis-aligned. Probably an integer matrix.
			var key = (snapFloat( flipRotate.M11 ), snapFloat( flipRotate.M12 ), snapFloat( flipRotate.M21 ), snapFloat( flipRotate.M22 ));
			if( lookup.TryGetValue( key, out var v ) )
				return v;
			return null;
		}
	}
}