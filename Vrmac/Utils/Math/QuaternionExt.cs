using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Vrmac
{
	/// <summary>Implements operations on quaternions missing from the framework</summary>
	public static class QuaternionExt
	{
		/// <summary>Transform a 3D vector with this quaternion.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector3 transformVector( this Quaternion quaternion, Vector3 src )
		{
			// https://math.stackexchange.com/a/535223/467444
			Quaternion q = new Quaternion( src, 0 );
			q = quaternion * q * quaternion.conjugated();
			return q.vector3();
		}

		static Quaternion conjugated( this Quaternion q ) => new Quaternion( -q.X, -q.Y, -q.Z, q.W );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static Vector3 vector3( this Quaternion q )
		{
			return new Vector3( q.X, q.Y, q.Z );
		}

		/// <summary>Apply angular velocity to the quaternion</summary>
		/// <remarks>If you don’t know what’s angular velocity and how it’s a 3D vector, <see href="https://en.wikipedia.org/wiki/Angular_velocity#Particle_in_three_dimensions">read this article</see>.</remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Quaternion rotate( this Quaternion quat, Vector3 angularVelocity, float deltaTime )
		{
			Vector3 vec = angularVelocity * deltaTime;
			float length = vec.Length();
			if( length < 1E-6F )
				return quat;    // Otherwise we'll have division by zero when trying to normalize it later on

			// Convert the rotation vector to quaternion. The following 4 lines are very similar to CreateFromAxisAngle method.
			float half = length * 0.5f;
			MathHelper.sinCos( half, out float sin, out float cos );

			// Instead of normalizing the axis, we multiply W component by the length of it. This method normalizes result in the end.
			Quaternion q = new Quaternion( vec.X * sin, vec.Y * sin, vec.Z * sin, length * cos );
			q = q * quat;
			q = Quaternion.Normalize( q );
#if DEBUG
			// The following line is not required, only useful for people. Computers are fine with 2 different quaternion representations of each possible rotation.
			if( q.W < 0 ) q = Quaternion.Negate( q );
#endif
			return q;
		}
	}
}