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

		/// <summary>Create a perspective projection matrix using field of view angle and aspect ratio.</summary>
		/// <remarks>The implementation is slightly non-standard, note the result depends on whether that's for OpenGL (ES included) or not</remarks>
		public static Matrix4x4 createPerspectiveFieldOfView( float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, bool openGlVariant )
		{
			Matrix4x4 result;

			// Patched to match what Diligent Engine expects.
			// The original C++ source code for that is in DiligentCore\Common\interface\BasicMath.hpp, in Matrix4x4<float> class, in Projection(..) and SetNearFarClipPlanes(..) methods
			// Hopefully, the rest of the Matrix4x4 code there is conventional, i.e. compatible with DirectX, and therefore with MonoGame

			if( ( fieldOfView <= 0f ) || ( fieldOfView >= 3.141593f ) )
			{
				throw new ArgumentException( "fieldOfView <= 0 or >= PI" );
			}
			if( nearPlaneDistance <= 0f )
			{
				throw new ArgumentException( "nearPlaneDistance <= 0" );
			}
			if( farPlaneDistance <= 0f )
			{
				throw new ArgumentException( "farPlaneDistance <= 0" );
			}
			if( nearPlaneDistance >= farPlaneDistance )
			{
				throw new ArgumentException( "nearPlaneDistance >= farPlaneDistance" );
			}
			float yScale = 1.0f / ( MathF.Tan( fieldOfView * 0.5f ) );
			float xScale = yScale / aspectRatio;
			result.M11 = xScale;
			result.M12 = result.M13 = result.M14 = result.M21 = 0;
			result.M22 = yScale;
			result.M23 = result.M24 = result.M31 = result.M32 = 0;
			if( openGlVariant )
			{
				// https://www.opengl.org/sdk/docs/man2/xhtml/gluPerspective.xml
				// http://www.terathon.com/gdc07_lengyel.pdf
				// Note that OpenGL uses right-handed coordinate system, where
				// camera is looking in negative z direction:
				//   OO
				//  |__|<--------------------
				//         -z             +z
				// Consequently, OpenGL projection matrix given by these two
				// references inverts z axis.

				// We do not need to do this, because we use DX coordinate
				// system for the camera space. Thus we need to invert the
				// sign of the values in the third column in the matrix
				// from the references:
				result.M33 = ( farPlaneDistance + nearPlaneDistance ) / ( farPlaneDistance - nearPlaneDistance );
				result.M34 = 1;
				result.M41 = result.M42 = 0;
				result.M43 = -2.0f * nearPlaneDistance * farPlaneDistance / ( farPlaneDistance - nearPlaneDistance );
				result.M44 = 0;
			}
			else
			{
				result.M33 = farPlaneDistance / ( farPlaneDistance - nearPlaneDistance );
				result.M34 = 1;
				result.M41 = result.M42 = 0;
				result.M43 = ( nearPlaneDistance * farPlaneDistance ) / ( nearPlaneDistance - farPlaneDistance );
				result.M44 = 0;
			}

			return result;
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