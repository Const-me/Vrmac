using System;
using System.Numerics;

namespace Vrmac
{
	/// <summary>Diligent Engine needs slightly unconventional matrices. This static class contains couple methods which differ from what’s in the .NET runtime.</summary>
	public static class DiligentMatrices
	{
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

		/// <summary>Creates a view matrix.</summary>
		/// <param name="cameraPosition">The position of the camera.</param>
		/// <param name="cameraTarget">The target towards which the camera is pointing.</param>
		/// <param name="cameraUpVector">The direction that is "up" from the camera's point of view.</param>
		/// <returns>The view matrix.</returns>
		public static Matrix4x4 createLookAt( Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector )
		{
			Vector3 zaxis = Vector3.Normalize( cameraPosition - cameraTarget );
			Vector3 xaxis = Vector3.Normalize( Vector3.Cross( cameraUpVector, zaxis ) );
			Vector3 yaxis = Vector3.Cross( zaxis, xaxis );

			Matrix4x4 result;

			result.M11 = xaxis.X;
			result.M12 = yaxis.X;
			result.M13 = zaxis.X;
			result.M14 = 0.0f;
			result.M21 = xaxis.Y;
			result.M22 = yaxis.Y;
			result.M23 = zaxis.Y;
			result.M24 = 0.0f;
			result.M31 = xaxis.Z;
			result.M32 = yaxis.Z;
			result.M33 = zaxis.Z;
			result.M34 = 0.0f;

			// Original code from .NET: https://source.dot.net/#System.Private.CoreLib/Matrix4x4.cs,b82966e485b5a306
			// result.M41 = -Vector3.Dot( xaxis, cameraPosition );
			// result.M42 = -Vector3.Dot( yaxis, cameraPosition );
			// result.M43 = -Vector3.Dot( zaxis, cameraPosition );

			// I have no idea why, but this change is required for this to be compatible with Diligent.
			result.M41 = Vector3.Dot( xaxis, cameraPosition );
			result.M42 = Vector3.Dot( yaxis, cameraPosition );
			result.M43 = Vector3.Dot( zaxis, cameraPosition );

			result.M44 = 1.0f;
			return result;
		}
	}
}