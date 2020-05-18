using System;

namespace Diligent.Graphics
{
	/// <summary>Utility functions for MonoGame</summary>
	public static class MonoGameUtils
	{
		/// <summary>Transpose a 4x4 matrix.</summary>
		/// <remarks>On CPUs, matrices are traditionally row-major. On GPUs however column-major matrices are faster, due to coalesced memory access.</remarks>
		public static Matrix transposed( this ref Matrix m )
		{
			return new Matrix(
				m.M11, m.M21, m.M31, m.M41,
				m.M12, m.M22, m.M32, m.M42,
				m.M13, m.M23, m.M33, m.M43,
				m.M14, m.M24, m.M34, m.M44
			);
		}

		/// <summary>Maximum coordinate of the vector</summary>
		public static float maxCoordinate( this Vector3 v3 )
		{
			return MathF.Max( MathF.Max( v3.X, v3.Y ), v3.Z );
		}

		/// <summary>Make a transformation matrix that translates + scales the box into the center of [ -1 .. +1 ] cube</summary>
		public static Matrix transformToUnitCube( this BoundingBox bbox )
		{
			float size = bbox.size.maxCoordinate();
			return Matrix.CreateTranslation( -bbox.center ) * Matrix.CreateScale( 2.0f / size );
		}
	}
}