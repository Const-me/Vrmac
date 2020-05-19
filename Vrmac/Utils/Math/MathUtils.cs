using System;
using System.Numerics;

namespace Vrmac
{
	/// <summary>Miscellaneous math-related utility functions</summary>
	public static class MathUtils
	{
		/// <summary>Transpose a 4x4 matrix.</summary>
		/// <remarks>On CPUs, matrices are traditionally row-major. On GPUs however column-major matrices are faster, due to coalesced memory access.</remarks>
		public static Matrix4x4 transposed( this ref Matrix4x4 m )
		{
			return new Matrix4x4(
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
		public static Matrix4x4 transformToUnitCube( this BoundingBox bbox )
		{
			float size = bbox.size.maxCoordinate();
			return Matrix4x4.CreateTranslation( -bbox.center ) * Matrix4x4.CreateScale( 2.0f / size );
		}

		/// <summary>Returns a vector whose elements are the absolute values of each of the specified vector&#39;s elements.</summary>
		public static Vector2 absolute( this Vector2 vec ) => Vector2.Abs( vec );

		/// <summary>Returns a vector whose elements are the absolute values of each of the specified vector&#39;s elements.</summary>
		public static Vector4 absolute( this Vector4 vec ) => Vector4.Abs( vec );

		/// <summary>Maximum of the coordinates</summary>
		public static float maxCoordinate( this Vector2 vec ) => MathF.Max( vec.X, vec.Y );
		/// <summary>Minimum of the coordinates</summary>
		public static float minCoordinate( this Vector2 vec ) => MathF.Min( vec.X, vec.Y );

		/// <summary>Maximum of the coordinates</summary>
		public static float maxCoordinate( this Vector4 vec ) => MathF.Max( MathF.Max( vec.X, vec.Y ), MathF.Max( vec.Z, vec.W ) );
		/// <summary>Minimum of the coordinates</summary>
		public static float minCoordinate( this Vector4 vec ) => MathF.Min( MathF.Min( vec.X, vec.Y ), MathF.Min( vec.Z, vec.W ) );

		/// <summary>Round to nearest integers</summary>
		public static CPoint roundToInt( this Vector2 vec )
		{
			int x = (int)MathF.Round( vec.X );
			int y = (int)MathF.Round( vec.Y );
			return new CPoint( x, y );
		}

		/// <summary>Smoothstep is a family of sigmoid-like interpolation and clamping functions commonly used in computer graphics and video game engines.</summary>
		/// <seealso href="https://en.wikipedia.org/wiki/Smoothstep" />
		public static float smoothStep( float edge0, float edge1, float x )
		{
			float relative = ( x - edge0 ) / ( edge1 - edge0 );
			// Modern CPUs, ARM included, have dedicated instructions for min/max, pretty fast and available for decades now.
			float clamped = MathF.Min( MathF.Max( relative, 0 ), 1 );
			return clamped * clamped * ( 3 - 2 * clamped );
		}

		/// <summary>Same as <see cref="MathHelper.Lerp" /> but applies smoothspep to the amount argument</summary>
		public static float smoothInterpolate( float a, float b, float amount )
		{
			float clamped = MathF.Min( MathF.Max( amount, 0 ), 1 );
			float interpolationValue = clamped * clamped * ( 3 - 2 * clamped );
			return MathHelper.Lerp( a, b, interpolationValue );
		}

		/// <summary>Normalize the vector and write the result back</summary>
		public static void Normalize( this ref Vector2 vec )
		{
			vec = Vector2.Normalize( vec );
		}
		/// <summary>Normalize the vector and return the result in another one</summary>
		public static Vector2 normalized( this Vector2 vec )
		{
			return Vector2.Normalize( vec );
		}
		/// <summary>Normalize the vector and write the result back</summary>
		public static void Normalize( this ref Vector3 vec )
		{
			vec = Vector3.Normalize( vec );
		}

		const float mulRadFromDeg = MathF.PI / 180.0f;
		/// <summary>Convert degrees into radians</summary>
		public static float radians( float degrees )
		{
			return degrees * mulRadFromDeg;
		}
	}
}