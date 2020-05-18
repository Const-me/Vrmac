// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Diligent.Graphics
{
	/// <summary>An efficient mathematical representation for three dimensional rotations.</summary>
	[StructLayout( LayoutKind.Sequential ), DataContract, DebuggerDisplay( "{DebugDisplayString,nq}" )]
	public struct Quaternion: IEquatable<Quaternion>
	{

		/// <summary>The x coordinate of this <see cref="Quaternion"/>.</summary>
		[DataMember]
		public float X;

		/// <summary>The y coordinate of this <see cref="Quaternion"/>.</summary>
		[DataMember]
		public float Y;

		/// <summary>The z coordinate of this <see cref="Quaternion"/>.</summary>
		[DataMember]
		public float Z;

		/// <summary>The rotation component of this <see cref="Quaternion"/>.</summary>
		[DataMember]
		public float W;

		/// <summary>Constructs a quaternion with X, Y, Z and W from four values.</summary>
		/// <param name="x">The x coordinate in 3d-space.</param>
		/// <param name="y">The y coordinate in 3d-space.</param>
		/// <param name="z">The z coordinate in 3d-space.</param>
		/// <param name="w">The rotation component.</param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Quaternion( float x, float y, float z, float w )
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		/// <summary>Constructs a quaternion with X, Y, Z from <see cref="Vector3"/> and rotation component from a scalar.</summary>
		/// <param name="value">The x, y, z coordinates in 3d-space.</param>
		/// <param name="w">The rotation component.</param>
		public Quaternion( Vector3 value, float w )
		{
			X = value.X;
			Y = value.Y;
			Z = value.Z;
			W = w;
		}

		/// <summary>Constructs a quaternion from <see cref="Vector4"/>.</summary>
		/// <param name="value">The x, y, z coordinates in 3d-space and the rotation component.</param>
		public Quaternion( Vector4 value )
		{
			X = value.X;
			Y = value.Y;
			Z = value.Z;
			W = value.W;
		}

		/// <summary>Returns a quaternion representing no rotation.</summary>
		public static Quaternion Identity { get; } = new Quaternion( 0, 0, 0, 1 );

		internal string DebugDisplayString
		{
			get
			{
				if( this == Quaternion.Identity )
				{
					return "Identity";
				}

				return string.Concat(
					X.ToString(), " ",
					Y.ToString(), " ",
					Z.ToString(), " ",
					W.ToString()
				);
			}
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains the sum of two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <returns>The result of the quaternion addition.</returns>
		public static Quaternion Add( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X + quaternion2.X;
			quaternion.Y = quaternion1.Y + quaternion2.Y;
			quaternion.Z = quaternion1.Z + quaternion2.Z;
			quaternion.W = quaternion1.W + quaternion2.W;
			return quaternion;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains the sum of two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <param name="result">The result of the quaternion addition as an output parameter.</param>
		public static void Add( ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result )
		{
			result.X = quaternion1.X + quaternion2.X;
			result.Y = quaternion1.Y + quaternion2.Y;
			result.Z = quaternion1.Z + quaternion2.Z;
			result.W = quaternion1.W + quaternion2.W;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains concatenation between two quaternion.</summary>
		/// <param name="value1">The first <see cref="Quaternion"/> to concatenate.</param>
		/// <param name="value2">The second <see cref="Quaternion"/> to concatenate.</param>
		/// <returns>The result of rotation of <paramref name="value1"/> followed by <paramref name="value2"/> rotation.</returns>
		public static Quaternion Concatenate( Quaternion value1, Quaternion value2 )
		{
			Quaternion quaternion;

			float x1 = value1.X;
			float y1 = value1.Y;
			float z1 = value1.Z;
			float w1 = value1.W;

			float x2 = value2.X;
			float y2 = value2.Y;
			float z2 = value2.Z;
			float w2 = value2.W;

			quaternion.X = ( ( x2 * w1 ) + ( x1 * w2 ) ) + ( ( y2 * z1 ) - ( z2 * y1 ) );
			quaternion.Y = ( ( y2 * w1 ) + ( y1 * w2 ) ) + ( ( z2 * x1 ) - ( x2 * z1 ) );
			quaternion.Z = ( ( z2 * w1 ) + ( z1 * w2 ) ) + ( ( x2 * y1 ) - ( y2 * x1 ) );
			quaternion.W = ( w2 * w1 ) - ( ( ( x2 * x1 ) + ( y2 * y1 ) ) + ( z2 * z1 ) );

			return quaternion;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains concatenation between two quaternion.</summary>
		/// <param name="value1">The first <see cref="Quaternion"/> to concatenate.</param>
		/// <param name="value2">The second <see cref="Quaternion"/> to concatenate.</param>
		/// <param name="result">The result of rotation of <paramref name="value1"/> followed by <paramref name="value2"/> rotation as an output parameter.</param>
		public static void Concatenate( ref Quaternion value1, ref Quaternion value2, out Quaternion result )
		{
			float x1 = value1.X;
			float y1 = value1.Y;
			float z1 = value1.Z;
			float w1 = value1.W;

			float x2 = value2.X;
			float y2 = value2.Y;
			float z2 = value2.Z;
			float w2 = value2.W;

			result.X = ( ( x2 * w1 ) + ( x1 * w2 ) ) + ( ( y2 * z1 ) - ( z2 * y1 ) );
			result.Y = ( ( y2 * w1 ) + ( y1 * w2 ) ) + ( ( z2 * x1 ) - ( x2 * z1 ) );
			result.Z = ( ( z2 * w1 ) + ( z1 * w2 ) ) + ( ( x2 * y1 ) - ( y2 * x1 ) );
			result.W = ( w2 * w1 ) - ( ( ( x2 * x1 ) + ( y2 * y1 ) ) + ( z2 * z1 ) );
		}

		/// <summary>Transforms this quaternion into its conjugated version.</summary>
		public void Conjugate()
		{
			X = -X;
			Y = -Y;
			Z = -Z;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains conjugated version of the specified quaternion.</summary>
		/// <param name="value">The quaternion which values will be used to create the conjugated version.</param>
		/// <returns>The conjugate version of the specified quaternion.</returns>
		public static Quaternion Conjugate( Quaternion value )
		{
			return new Quaternion( -value.X, -value.Y, -value.Z, value.W );
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains conjugated version of the specified quaternion.</summary>
		/// <param name="value">The quaternion which values will be used to create the conjugated version.</param>
		/// <param name="result">The conjugated version of the specified quaternion as an output parameter.</param>
		public static void Conjugate( ref Quaternion value, out Quaternion result )
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			result.W = value.W;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> from the specified axis and angle.</summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle in radians.</param>
		/// <returns>The new quaternion builded from axis and angle.</returns>
		public static Quaternion CreateFromAxisAngle( Vector3 axis, float angle )
		{
			float half = angle * 0.5f;
			float sin = MathF.Sin( half );
			float cos = MathF.Cos( half );
			return new Quaternion( axis.X * sin, axis.Y * sin, axis.Z * sin, cos );
		}

		/// <summary>Creates a new <see cref="Quaternion"/> from the specified axis and angle.</summary>
		/// <param name="axis">The axis of rotation.</param>
		/// <param name="angle">The angle in radians.</param>
		/// <param name="result">The new quaternion builded from axis and angle as an output parameter.</param>
		public static void CreateFromAxisAngle( ref Vector3 axis, float angle, out Quaternion result )
		{
			float half = angle * 0.5f;
			float sin = MathF.Sin( half );
			float cos = MathF.Cos( half );
			result.X = axis.X * sin;
			result.Y = axis.Y * sin;
			result.Z = axis.Z * sin;
			result.W = cos;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> from the specified <see cref="Matrix"/>.</summary>
		/// <param name="matrix">The rotation matrix.</param>
		/// <returns>A quaternion composed from the rotation part of the matrix.</returns>
		public static Quaternion CreateFromRotationMatrix( Matrix matrix )
		{
			Quaternion quaternion;
			float sqrt;
			float half;
			float scale = matrix.M11 + matrix.M22 + matrix.M33;

			if( scale > 0.0f )
			{
				sqrt = MathF.Sqrt( scale + 1.0f );
				quaternion.W = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;

				quaternion.X = ( matrix.M23 - matrix.M32 ) * sqrt;
				quaternion.Y = ( matrix.M31 - matrix.M13 ) * sqrt;
				quaternion.Z = ( matrix.M12 - matrix.M21 ) * sqrt;

				return quaternion;
			}
			if( ( matrix.M11 >= matrix.M22 ) && ( matrix.M11 >= matrix.M33 ) )
			{
				sqrt = MathF.Sqrt( 1.0f + matrix.M11 - matrix.M22 - matrix.M33 );
				half = 0.5f / sqrt;

				quaternion.X = 0.5f * sqrt;
				quaternion.Y = ( matrix.M12 + matrix.M21 ) * half;
				quaternion.Z = ( matrix.M13 + matrix.M31 ) * half;
				quaternion.W = ( matrix.M23 - matrix.M32 ) * half;

				return quaternion;
			}
			if( matrix.M22 > matrix.M33 )
			{
				sqrt = MathF.Sqrt( 1.0f + matrix.M22 - matrix.M11 - matrix.M33 );
				half = 0.5f / sqrt;

				quaternion.X = ( matrix.M21 + matrix.M12 ) * half;
				quaternion.Y = 0.5f * sqrt;
				quaternion.Z = ( matrix.M32 + matrix.M23 ) * half;
				quaternion.W = ( matrix.M31 - matrix.M13 ) * half;

				return quaternion;
			}
			sqrt = MathF.Sqrt( 1.0f + matrix.M33 - matrix.M11 - matrix.M22 );
			half = 0.5f / sqrt;

			quaternion.X = ( matrix.M31 + matrix.M13 ) * half;
			quaternion.Y = ( matrix.M32 + matrix.M23 ) * half;
			quaternion.Z = 0.5f * sqrt;
			quaternion.W = ( matrix.M12 - matrix.M21 ) * half;

			return quaternion;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> from the specified <see cref="Matrix"/>.</summary>
		/// <param name="matrix">The rotation matrix.</param>
		/// <param name="result">A quaternion composed from the rotation part of the matrix as an output parameter.</param>
		public static void CreateFromRotationMatrix( ref Matrix matrix, out Quaternion result )
		{
			float sqrt;
			float half;
			float scale = matrix.M11 + matrix.M22 + matrix.M33;

			if( scale > 0.0f )
			{
				sqrt = MathF.Sqrt( scale + 1.0f );
				result.W = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;

				result.X = ( matrix.M23 - matrix.M32 ) * sqrt;
				result.Y = ( matrix.M31 - matrix.M13 ) * sqrt;
				result.Z = ( matrix.M12 - matrix.M21 ) * sqrt;
			}
			else
			if( ( matrix.M11 >= matrix.M22 ) && ( matrix.M11 >= matrix.M33 ) )
			{
				sqrt = MathF.Sqrt( 1.0f + matrix.M11 - matrix.M22 - matrix.M33 );
				half = 0.5f / sqrt;

				result.X = 0.5f * sqrt;
				result.Y = ( matrix.M12 + matrix.M21 ) * half;
				result.Z = ( matrix.M13 + matrix.M31 ) * half;
				result.W = ( matrix.M23 - matrix.M32 ) * half;
			}
			else if( matrix.M22 > matrix.M33 )
			{
				sqrt = MathF.Sqrt( 1.0f + matrix.M22 - matrix.M11 - matrix.M33 );
				half = 0.5f / sqrt;

				result.X = ( matrix.M21 + matrix.M12 ) * half;
				result.Y = 0.5f * sqrt;
				result.Z = ( matrix.M32 + matrix.M23 ) * half;
				result.W = ( matrix.M31 - matrix.M13 ) * half;
			}
			else
			{
				sqrt = MathF.Sqrt( 1.0f + matrix.M33 - matrix.M11 - matrix.M22 );
				half = 0.5f / sqrt;

				result.X = ( matrix.M31 + matrix.M13 ) * half;
				result.Y = ( matrix.M32 + matrix.M23 ) * half;
				result.Z = 0.5f * sqrt;
				result.W = ( matrix.M12 - matrix.M21 ) * half;
			}
		}

		/// <summary>Creates a new <see cref="Quaternion"/> from the specified yaw, pitch and roll angles.</summary>
		/// <param name="yaw">Yaw around the y axis in radians.</param>
		/// <param name="pitch">Pitch around the x axis in radians.</param>
		/// <param name="roll">Roll around the z axis in radians.</param>
		/// <returns>A new quaternion from the concatenated yaw, pitch, and roll angles.</returns>
		public static Quaternion CreateFromYawPitchRoll( float yaw, float pitch, float roll )
		{
			float halfRoll = roll * 0.5f;
			float halfPitch = pitch * 0.5f;
			float halfYaw = yaw * 0.5f;

			float sinRoll = MathF.Sin( halfRoll );
			float cosRoll = MathF.Cos( halfRoll );
			float sinPitch = MathF.Sin( halfPitch );
			float cosPitch = MathF.Cos( halfPitch );
			float sinYaw = MathF.Sin( halfYaw );
			float cosYaw = MathF.Cos( halfYaw );

			return new Quaternion( ( cosYaw * sinPitch * cosRoll ) + ( sinYaw * cosPitch * sinRoll ),
								  ( sinYaw * cosPitch * cosRoll ) - ( cosYaw * sinPitch * sinRoll ),
								  ( cosYaw * cosPitch * sinRoll ) - ( sinYaw * sinPitch * cosRoll ),
								  ( cosYaw * cosPitch * cosRoll ) + ( sinYaw * sinPitch * sinRoll ) );
		}

		/// <summary>Creates a new <see cref="Quaternion"/> from the specified yaw, pitch and roll angles.</summary>
		/// <param name="yaw">Yaw around the y axis in radians.</param>
		/// <param name="pitch">Pitch around the x axis in radians.</param>
		/// <param name="roll">Roll around the z axis in radians.</param>
		/// <param name="result">A new quaternion from the concatenated yaw, pitch, and roll angles as an output parameter.</param>
		public static void CreateFromYawPitchRoll( float yaw, float pitch, float roll, out Quaternion result )
		{
			float halfRoll = roll * 0.5f;
			float halfPitch = pitch * 0.5f;
			float halfYaw = yaw * 0.5f;

			float sinRoll = MathF.Sin( halfRoll );
			float cosRoll = MathF.Cos( halfRoll );
			float sinPitch = MathF.Sin( halfPitch );
			float cosPitch = MathF.Cos( halfPitch );
			float sinYaw = MathF.Sin( halfYaw );
			float cosYaw = MathF.Cos( halfYaw );

			result.X = ( cosYaw * sinPitch * cosRoll ) + ( sinYaw * cosPitch * sinRoll );
			result.Y = ( sinYaw * cosPitch * cosRoll ) - ( cosYaw * sinPitch * sinRoll );
			result.Z = ( cosYaw * cosPitch * sinRoll ) - ( sinYaw * sinPitch * cosRoll );
			result.W = ( cosYaw * cosPitch * cosRoll ) + ( sinYaw * sinPitch * sinRoll );
		}

		/// <summary>Divides a <see cref="Quaternion"/> by the other <see cref="Quaternion"/>.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Divisor <see cref="Quaternion"/>.</param>
		/// <returns>The result of dividing the quaternions.</returns>
		public static Quaternion Divide( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num14 = ( ( ( quaternion2.X * quaternion2.X ) + ( quaternion2.Y * quaternion2.Y ) ) + ( quaternion2.Z * quaternion2.Z ) ) + ( quaternion2.W * quaternion2.W );
			float num5 = 1f / num14;
			float num4 = -quaternion2.X * num5;
			float num3 = -quaternion2.Y * num5;
			float num2 = -quaternion2.Z * num5;
			float num = quaternion2.W * num5;
			float num13 = ( y * num2 ) - ( z * num3 );
			float num12 = ( z * num4 ) - ( x * num2 );
			float num11 = ( x * num3 ) - ( y * num4 );
			float num10 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			quaternion.X = ( ( x * num ) + ( num4 * w ) ) + num13;
			quaternion.Y = ( ( y * num ) + ( num3 * w ) ) + num12;
			quaternion.Z = ( ( z * num ) + ( num2 * w ) ) + num11;
			quaternion.W = ( w * num ) - num10;
			return quaternion;
		}

		/// <summary>Divides a <see cref="Quaternion"/> by the other <see cref="Quaternion"/>.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Divisor <see cref="Quaternion"/>.</param>
		/// <param name="result">The result of dividing the quaternions as an output parameter.</param>
		public static void Divide( ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result )
		{
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num14 = ( ( ( quaternion2.X * quaternion2.X ) + ( quaternion2.Y * quaternion2.Y ) ) + ( quaternion2.Z * quaternion2.Z ) ) + ( quaternion2.W * quaternion2.W );
			float num5 = 1f / num14;
			float num4 = -quaternion2.X * num5;
			float num3 = -quaternion2.Y * num5;
			float num2 = -quaternion2.Z * num5;
			float num = quaternion2.W * num5;
			float num13 = ( y * num2 ) - ( z * num3 );
			float num12 = ( z * num4 ) - ( x * num2 );
			float num11 = ( x * num3 ) - ( y * num4 );
			float num10 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			result.X = ( ( x * num ) + ( num4 * w ) ) + num13;
			result.Y = ( ( y * num ) + ( num3 * w ) ) + num12;
			result.Z = ( ( z * num ) + ( num2 * w ) ) + num11;
			result.W = ( w * num ) - num10;
		}

		/// <summary>Returns a dot product of two quaternions.</summary>
		/// <param name="quaternion1">The first quaternion.</param>
		/// <param name="quaternion2">The second quaternion.</param>
		/// <returns>The dot product of two quaternions.</returns>
		public static float Dot( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return ( ( ( ( quaternion1.X * quaternion2.X ) + ( quaternion1.Y * quaternion2.Y ) ) + ( quaternion1.Z * quaternion2.Z ) ) + ( quaternion1.W * quaternion2.W ) );
		}

		/// <summary>Returns a dot product of two quaternions.</summary>
		/// <param name="quaternion1">The first quaternion.</param>
		/// <param name="quaternion2">The second quaternion.</param>
		/// <param name="result">The dot product of two quaternions as an output parameter.</param>
		public static void Dot( ref Quaternion quaternion1, ref Quaternion quaternion2, out float result )
		{
			result = ( ( ( quaternion1.X * quaternion2.X ) + ( quaternion1.Y * quaternion2.Y ) ) + ( quaternion1.Z * quaternion2.Z ) ) + ( quaternion1.W * quaternion2.W );
		}

		/// <summary>Compares whether current instance is equal to specified <see cref="Object"/>.</summary>
		/// <param name="obj">The <see cref="Object"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public override bool Equals( object obj )
		{
			if( obj is Quaternion )
				return Equals( (Quaternion)obj );
			return false;
		}

		/// <summary>Compares whether current instance is equal to specified <see cref="Quaternion"/>.</summary>
		/// <param name="other">The <see cref="Quaternion"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public bool Equals( Quaternion other )
		{
			return X == other.X &&
				   Y == other.Y &&
				   Z == other.Z &&
				   W == other.W;
		}

		/// <summary>Gets the hash code of this <see cref="Quaternion"/>.</summary>
		/// <returns>Hash code of this <see cref="Quaternion"/>.</returns>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode() + W.GetHashCode();
		}

		/// <summary>Returns the inverse quaternion which represents the opposite rotation.</summary>
		/// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
		/// <returns>The inverse quaternion.</returns>
		public static Quaternion Inverse( Quaternion quaternion )
		{
			Quaternion quaternion2;
			float num2 = ( ( ( quaternion.X * quaternion.X ) + ( quaternion.Y * quaternion.Y ) ) + ( quaternion.Z * quaternion.Z ) ) + ( quaternion.W * quaternion.W );
			float num = 1f / num2;
			quaternion2.X = -quaternion.X * num;
			quaternion2.Y = -quaternion.Y * num;
			quaternion2.Z = -quaternion.Z * num;
			quaternion2.W = quaternion.W * num;
			return quaternion2;
		}

		/// <summary>Returns the inverse quaternion which represents the opposite rotation.</summary>
		/// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
		/// <param name="result">The inverse quaternion as an output parameter.</param>
		public static void Inverse( ref Quaternion quaternion, out Quaternion result )
		{
			float num2 = ( ( ( quaternion.X * quaternion.X ) + ( quaternion.Y * quaternion.Y ) ) + ( quaternion.Z * quaternion.Z ) ) + ( quaternion.W * quaternion.W );
			float num = 1f / num2;
			result.X = -quaternion.X * num;
			result.Y = -quaternion.Y * num;
			result.Z = -quaternion.Z * num;
			result.W = quaternion.W * num;
		}

		/// <summary>Returns the magnitude of the quaternion components.</summary>
		/// <returns>The magnitude of the quaternion components.</returns>
		public float Length()
		{
			return MathF.Sqrt( ( X * X ) + ( Y * Y ) + ( Z * Z ) + ( W * W ) );
		}

		/// <summary>Returns the squared magnitude of the quaternion components.</summary>
		/// <returns>The squared magnitude of the quaternion components.</returns>
		public float LengthSquared()
		{
			return ( X * X ) + ( Y * Y ) + ( Z * Z ) + ( W * W );
		}

		/// <summary>Performs a linear blend between two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <returns>The result of linear blending between two quaternions.</returns>
		public static Quaternion Lerp( Quaternion quaternion1, Quaternion quaternion2, float amount )
		{
			float num = amount;
			float num2 = 1f - num;
			Quaternion quaternion = new Quaternion();
			float num5 = ( ( ( quaternion1.X * quaternion2.X ) + ( quaternion1.Y * quaternion2.Y ) ) + ( quaternion1.Z * quaternion2.Z ) ) + ( quaternion1.W * quaternion2.W );
			if( num5 >= 0f )
			{
				quaternion.X = ( num2 * quaternion1.X ) + ( num * quaternion2.X );
				quaternion.Y = ( num2 * quaternion1.Y ) + ( num * quaternion2.Y );
				quaternion.Z = ( num2 * quaternion1.Z ) + ( num * quaternion2.Z );
				quaternion.W = ( num2 * quaternion1.W ) + ( num * quaternion2.W );
			}
			else
			{
				quaternion.X = ( num2 * quaternion1.X ) - ( num * quaternion2.X );
				quaternion.Y = ( num2 * quaternion1.Y ) - ( num * quaternion2.Y );
				quaternion.Z = ( num2 * quaternion1.Z ) - ( num * quaternion2.Z );
				quaternion.W = ( num2 * quaternion1.W ) - ( num * quaternion2.W );
			}
			float num4 = ( ( ( quaternion.X * quaternion.X ) + ( quaternion.Y * quaternion.Y ) ) + ( quaternion.Z * quaternion.Z ) ) + ( quaternion.W * quaternion.W );
			float num3 = 1.0f / MathF.Sqrt( num4 );
			quaternion.X *= num3;
			quaternion.Y *= num3;
			quaternion.Z *= num3;
			quaternion.W *= num3;
			return quaternion;
		}

		/// <summary>Performs a linear blend between two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <param name="result">The result of linear blending between two quaternions as an output parameter.</param>
		public static void Lerp( ref Quaternion quaternion1, ref Quaternion quaternion2, float amount, out Quaternion result )
		{
			float num = amount;
			float num2 = 1f - num;
			float num5 = ( ( ( quaternion1.X * quaternion2.X ) + ( quaternion1.Y * quaternion2.Y ) ) + ( quaternion1.Z * quaternion2.Z ) ) + ( quaternion1.W * quaternion2.W );
			if( num5 >= 0f )
			{
				result.X = ( num2 * quaternion1.X ) + ( num * quaternion2.X );
				result.Y = ( num2 * quaternion1.Y ) + ( num * quaternion2.Y );
				result.Z = ( num2 * quaternion1.Z ) + ( num * quaternion2.Z );
				result.W = ( num2 * quaternion1.W ) + ( num * quaternion2.W );
			}
			else
			{
				result.X = ( num2 * quaternion1.X ) - ( num * quaternion2.X );
				result.Y = ( num2 * quaternion1.Y ) - ( num * quaternion2.Y );
				result.Z = ( num2 * quaternion1.Z ) - ( num * quaternion2.Z );
				result.W = ( num2 * quaternion1.W ) - ( num * quaternion2.W );
			}
			float num4 = ( ( ( result.X * result.X ) + ( result.Y * result.Y ) ) + ( result.Z * result.Z ) ) + ( result.W * result.W );
			float num3 = 1.0f / MathF.Sqrt( num4 );
			result.X *= num3;
			result.Y *= num3;
			result.Z *= num3;
			result.W *= num3;

		}

		/// <summary>Performs a spherical linear blend between two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <returns>The result of spherical linear blending between two quaternions.</returns>
		public static Quaternion Slerp( Quaternion quaternion1, Quaternion quaternion2, float amount )
		{
			float num2;
			float num3;
			Quaternion quaternion;
			float num = amount;
			float num4 = ( ( ( quaternion1.X * quaternion2.X ) + ( quaternion1.Y * quaternion2.Y ) ) + ( quaternion1.Z * quaternion2.Z ) ) + ( quaternion1.W * quaternion2.W );
			bool flag = false;
			if( num4 < 0f )
			{
				flag = true;
				num4 = -num4;
			}
			if( num4 > 0.999999f )
			{
				num3 = 1f - num;
				num2 = flag ? -num : num;
			}
			else
			{
				float num5 = MathF.Acos( num4 );
				float num6 = 1.0f / MathF.Sin( num5 );
				num3 = ( MathF.Sin( ( ( 1f - num ) * num5 ) ) ) * num6;
				num2 = flag ? ( ( -MathF.Sin( ( num * num5 ) ) ) * num6 ) : ( ( MathF.Sin( ( num * num5 ) ) ) * num6 );
			}
			quaternion.X = ( num3 * quaternion1.X ) + ( num2 * quaternion2.X );
			quaternion.Y = ( num3 * quaternion1.Y ) + ( num2 * quaternion2.Y );
			quaternion.Z = ( num3 * quaternion1.Z ) + ( num2 * quaternion2.Z );
			quaternion.W = ( num3 * quaternion1.W ) + ( num2 * quaternion2.W );
			return quaternion;
		}

		/// <summary>Performs a spherical linear blend between two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <param name="amount">The blend amount where 0 returns <paramref name="quaternion1"/> and 1 <paramref name="quaternion2"/>.</param>
		/// <param name="result">The result of spherical linear blending between two quaternions as an output parameter.</param>
		public static void Slerp( ref Quaternion quaternion1, ref Quaternion quaternion2, float amount, out Quaternion result )
		{
			float num2;
			float num3;
			float num = amount;
			float num4 = ( ( ( quaternion1.X * quaternion2.X ) + ( quaternion1.Y * quaternion2.Y ) ) + ( quaternion1.Z * quaternion2.Z ) ) + ( quaternion1.W * quaternion2.W );
			bool flag = false;
			if( num4 < 0f )
			{
				flag = true;
				num4 = -num4;
			}
			if( num4 > 0.999999f )
			{
				num3 = 1f - num;
				num2 = flag ? -num : num;
			}
			else
			{
				float num5 = MathF.Acos( num4 );
				float num6 = 1.0f / MathF.Sin( num5 );
				num3 = MathF.Sin( ( 1f - num ) * num5 ) * num6;
				num2 = flag ? ( ( -MathF.Sin( num * num5 ) ) * num6 ) : ( ( MathF.Sin( num * num5 ) ) ) * num6;
			}
			result.X = ( num3 * quaternion1.X ) + ( num2 * quaternion2.X );
			result.Y = ( num3 * quaternion1.Y ) + ( num2 * quaternion2.Y );
			result.Z = ( num3 * quaternion1.Z ) + ( num2 * quaternion2.Z );
			result.W = ( num3 * quaternion1.W ) + ( num2 * quaternion2.W );
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains subtraction of one <see cref="Quaternion"/> from another.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <returns>The result of the quaternion subtraction.</returns>
		public static Quaternion Subtract( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X - quaternion2.X;
			quaternion.Y = quaternion1.Y - quaternion2.Y;
			quaternion.Z = quaternion1.Z - quaternion2.Z;
			quaternion.W = quaternion1.W - quaternion2.W;
			return quaternion;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains subtraction of one <see cref="Quaternion"/> from another.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <param name="result">The result of the quaternion subtraction as an output parameter.</param>
		public static void Subtract( ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result )
		{
			result.X = quaternion1.X - quaternion2.X;
			result.Y = quaternion1.Y - quaternion2.Y;
			result.Z = quaternion1.Z - quaternion2.Z;
			result.W = quaternion1.W - quaternion2.W;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains a multiplication of two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <returns>The result of the quaternion multiplication.</returns>
		public static Quaternion Multiply( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num4 = quaternion2.X;
			float num3 = quaternion2.Y;
			float num2 = quaternion2.Z;
			float num = quaternion2.W;
			float num12 = ( y * num2 ) - ( z * num3 );
			float num11 = ( z * num4 ) - ( x * num2 );
			float num10 = ( x * num3 ) - ( y * num4 );
			float num9 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			quaternion.X = ( ( x * num ) + ( num4 * w ) ) + num12;
			quaternion.Y = ( ( y * num ) + ( num3 * w ) ) + num11;
			quaternion.Z = ( ( z * num ) + ( num2 * w ) ) + num10;
			quaternion.W = ( w * num ) - num9;
			return quaternion;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains a multiplication of <see cref="Quaternion"/> and a scalar.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <returns>The result of the quaternion multiplication with a scalar.</returns>
		public static Quaternion Multiply( Quaternion quaternion1, float scaleFactor )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X * scaleFactor;
			quaternion.Y = quaternion1.Y * scaleFactor;
			quaternion.Z = quaternion1.Z * scaleFactor;
			quaternion.W = quaternion1.W * scaleFactor;
			return quaternion;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains a multiplication of <see cref="Quaternion"/> and a scalar.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="scaleFactor">Scalar value.</param>
		/// <param name="result">The result of the quaternion multiplication with a scalar as an output parameter.</param>
		public static void Multiply( ref Quaternion quaternion1, float scaleFactor, out Quaternion result )
		{
			result.X = quaternion1.X * scaleFactor;
			result.Y = quaternion1.Y * scaleFactor;
			result.Z = quaternion1.Z * scaleFactor;
			result.W = quaternion1.W * scaleFactor;
		}

		/// <summary>Creates a new <see cref="Quaternion"/> that contains a multiplication of two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/>.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/>.</param>
		/// <param name="result">The result of the quaternion multiplication as an output parameter.</param>
		public static void Multiply( ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result )
		{
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num4 = quaternion2.X;
			float num3 = quaternion2.Y;
			float num2 = quaternion2.Z;
			float num = quaternion2.W;
			float num12 = ( y * num2 ) - ( z * num3 );
			float num11 = ( z * num4 ) - ( x * num2 );
			float num10 = ( x * num3 ) - ( y * num4 );
			float num9 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			result.X = ( ( x * num ) + ( num4 * w ) ) + num12;
			result.Y = ( ( y * num ) + ( num3 * w ) ) + num11;
			result.Z = ( ( z * num ) + ( num2 * w ) ) + num10;
			result.W = ( w * num ) - num9;
		}

		/// <summary>Flips the sign of the all the quaternion components.</summary>
		/// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
		/// <returns>The result of the quaternion negation.</returns>
		public static Quaternion Negate( Quaternion quaternion )
		{
			return new Quaternion( -quaternion.X, -quaternion.Y, -quaternion.Z, -quaternion.W );
		}

		/// <summary>Flips the sign of the all the quaternion components.</summary>
		/// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
		/// <param name="result">The result of the quaternion negation as an output parameter.</param>
		public static void Negate( ref Quaternion quaternion, out Quaternion result )
		{
			result.X = -quaternion.X;
			result.Y = -quaternion.Y;
			result.Z = -quaternion.Z;
			result.W = -quaternion.W;
		}

		/// <summary>Scales the quaternion magnitude to unit length.</summary>
		public void Normalize()
		{
			float num = 1f / ( MathF.Sqrt( ( X * X ) + ( Y * Y ) + ( Z * Z ) + ( W * W ) ) );
			X *= num;
			Y *= num;
			Z *= num;
			W *= num;
		}

		/// <summary>Scales the quaternion magnitude to unit length.</summary>
		/// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
		/// <returns>The unit length quaternion.</returns>
		public static Quaternion Normalize( Quaternion quaternion )
		{
			Quaternion result;
			float num = 1.0f / ( MathF.Sqrt( ( quaternion.X * quaternion.X ) + ( quaternion.Y * quaternion.Y ) + ( quaternion.Z * quaternion.Z ) + ( quaternion.W * quaternion.W ) ) );
			result.X = quaternion.X * num;
			result.Y = quaternion.Y * num;
			result.Z = quaternion.Z * num;
			result.W = quaternion.W * num;
			return result;
		}

		/// <summary>Scales the quaternion magnitude to unit length.</summary>
		/// <param name="quaternion">Source <see cref="Quaternion"/>.</param>
		/// <param name="result">The unit length quaternion an output parameter.</param>
		public static void Normalize( ref Quaternion quaternion, out Quaternion result )
		{
			float num = 1.0f / ( MathF.Sqrt( ( quaternion.X * quaternion.X ) + ( quaternion.Y * quaternion.Y ) + ( quaternion.Z * quaternion.Z ) + ( quaternion.W * quaternion.W ) ) );
			result.X = quaternion.X * num;
			result.Y = quaternion.Y * num;
			result.Z = quaternion.Z * num;
			result.W = quaternion.W * num;
		}

		/// <summary>Returns a <see cref="String"/> representation of this <see cref="Quaternion"/> in the format:
		/// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>] W:[<see cref="W"/>]}</summary>
		/// <returns>A <see cref="String"/> representation of this <see cref="Quaternion"/>.</returns>
		public override string ToString()
		{
			return "{X:" + X + " Y:" + Y + " Z:" + Z + " W:" + W + "}";
		}

		/// <summary>Gets a <see cref="Vector4"/> representation for this object.</summary>
		/// <returns>A <see cref="Vector4"/> representation for this object.</returns>
		public Vector4 ToVector4()
		{
			return new Vector4( X, Y, Z, W );
		}

		/// <summary>Deconstruction method for <see cref="Quaternion" />.</summary>
		public void Deconstruct( out float x, out float y, out float z, out float w )
		{
			x = X;
			y = Y;
			z = Z;
			w = W;
		}

		/// <summary>Adds two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/> on the left of the add sign.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/> on the right of the add sign.</param>
		/// <returns>Sum of the vectors.</returns>
		public static Quaternion operator +( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X + quaternion2.X;
			quaternion.Y = quaternion1.Y + quaternion2.Y;
			quaternion.Z = quaternion1.Z + quaternion2.Z;
			quaternion.W = quaternion1.W + quaternion2.W;
			return quaternion;
		}

		/// <summary>Divides a <see cref="Quaternion"/> by the other <see cref="Quaternion"/>.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/> on the left of the div sign.</param>
		/// <param name="quaternion2">Divisor <see cref="Quaternion"/> on the right of the div sign.</param>
		/// <returns>The result of dividing the quaternions.</returns>
		public static Quaternion operator /( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num14 = ( ( ( quaternion2.X * quaternion2.X ) + ( quaternion2.Y * quaternion2.Y ) ) + ( quaternion2.Z * quaternion2.Z ) ) + ( quaternion2.W * quaternion2.W );
			float num5 = 1f / num14;
			float num4 = -quaternion2.X * num5;
			float num3 = -quaternion2.Y * num5;
			float num2 = -quaternion2.Z * num5;
			float num = quaternion2.W * num5;
			float num13 = ( y * num2 ) - ( z * num3 );
			float num12 = ( z * num4 ) - ( x * num2 );
			float num11 = ( x * num3 ) - ( y * num4 );
			float num10 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			quaternion.X = ( ( x * num ) + ( num4 * w ) ) + num13;
			quaternion.Y = ( ( y * num ) + ( num3 * w ) ) + num12;
			quaternion.Z = ( ( z * num ) + ( num2 * w ) ) + num11;
			quaternion.W = ( w * num ) - num10;
			return quaternion;
		}

		/// <summary>Compares whether two <see cref="Quaternion"/> instances are equal.</summary>
		/// <param name="quaternion1"><see cref="Quaternion"/> instance on the left of the equal sign.</param>
		/// <param name="quaternion2"><see cref="Quaternion"/> instance on the right of the equal sign.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public static bool operator ==( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return ( ( ( ( quaternion1.X == quaternion2.X ) && ( quaternion1.Y == quaternion2.Y ) ) && ( quaternion1.Z == quaternion2.Z ) ) && ( quaternion1.W == quaternion2.W ) );
		}

		/// <summary>Compares whether two <see cref="Quaternion"/> instances are not equal.</summary>
		/// <param name="quaternion1"><see cref="Quaternion"/> instance on the left of the not equal sign.</param>
		/// <param name="quaternion2"><see cref="Quaternion"/> instance on the right of the not equal sign.</param>
		/// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
		public static bool operator !=( Quaternion quaternion1, Quaternion quaternion2 )
		{
			if( ( ( quaternion1.X == quaternion2.X ) && ( quaternion1.Y == quaternion2.Y ) ) && ( quaternion1.Z == quaternion2.Z ) )
			{
				return ( quaternion1.W != quaternion2.W );
			}
			return true;
		}

		/// <summary>Multiplies two quaternions.</summary>
		/// <param name="quaternion1">Source <see cref="Quaternion"/> on the left of the mul sign.</param>
		/// <param name="quaternion2">Source <see cref="Quaternion"/> on the right of the mul sign.</param>
		/// <returns>Result of the quaternions multiplication.</returns>
		public static Quaternion operator *( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num4 = quaternion2.X;
			float num3 = quaternion2.Y;
			float num2 = quaternion2.Z;
			float num = quaternion2.W;
			float num12 = ( y * num2 ) - ( z * num3 );
			float num11 = ( z * num4 ) - ( x * num2 );
			float num10 = ( x * num3 ) - ( y * num4 );
			float num9 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			quaternion.X = ( ( x * num ) + ( num4 * w ) ) + num12;
			quaternion.Y = ( ( y * num ) + ( num3 * w ) ) + num11;
			quaternion.Z = ( ( z * num ) + ( num2 * w ) ) + num10;
			quaternion.W = ( w * num ) - num9;
			return quaternion;
		}

		/// <summary>Multiplies the components of quaternion by a scalar.</summary>
		/// <param name="quaternion1">Source <see cref="Vector3"/> on the left of the mul sign.</param>
		/// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
		/// <returns>Result of the quaternion multiplication with a scalar.</returns>
		public static Quaternion operator *( Quaternion quaternion1, float scaleFactor )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X * scaleFactor;
			quaternion.Y = quaternion1.Y * scaleFactor;
			quaternion.Z = quaternion1.Z * scaleFactor;
			quaternion.W = quaternion1.W * scaleFactor;
			return quaternion;
		}

		/// <summary>Subtracts a <see cref="Quaternion"/> from a <see cref="Quaternion"/>.</summary>
		/// <param name="quaternion1">Source <see cref="Vector3"/> on the left of the sub sign.</param>
		/// <param name="quaternion2">Source <see cref="Vector3"/> on the right of the sub sign.</param>
		/// <returns>Result of the quaternion subtraction.</returns>
		public static Quaternion operator -( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X - quaternion2.X;
			quaternion.Y = quaternion1.Y - quaternion2.Y;
			quaternion.Z = quaternion1.Z - quaternion2.Z;
			quaternion.W = quaternion1.W - quaternion2.W;
			return quaternion;

		}

		/// <summary>Flips the sign of the all the quaternion components.</summary>
		/// <param name="quaternion">Source <see cref="Quaternion"/> on the right of the sub sign.</param>
		/// <returns>The result of the quaternion negation.</returns>
		public static Quaternion operator -( Quaternion quaternion )
		{
			Quaternion quaternion2;
			quaternion2.X = -quaternion.X;
			quaternion2.Y = -quaternion.Y;
			quaternion2.Z = -quaternion.Z;
			quaternion2.W = -quaternion.W;
			return quaternion2;
		}

		/// <summary>Apply angular velocity to the quaternion</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Quaternion rotate( Vector3 angularVelocity, float deltaTime )
		{
			Vector3 vec = angularVelocity * deltaTime;
			float length = vec.Length();
			if( length < 1E-6F )
				return this;    // Otherwise we'll have division by zero when trying to normalize it later on

			// Convert the rotation vector to quaternion. The following 4 lines are very similar to CreateFromAxisAngle method.
			float half = length * 0.5f;
			float sin = MathF.Sin( half );
			float cos = MathF.Cos( half );
			// Instead of normalizing the axis, we multiply W component by the length of it. This method normalizes result in the end.
			Quaternion q = new Quaternion( vec.X * sin, vec.Y * sin, vec.Z * sin, length * cos );
			q = Multiply( q, this );
			q.Normalize();
#if DEBUG
			// The following line is not required, only useful for people. Computers are fine with 2 different quaternion representations of each possible rotation.
			if( q.W < 0 ) q = Negate( q );
#endif
			return q;
		}

		/// <summary>Conjugated version of self</summary>
		public Quaternion conjugated => new Quaternion( -X, -Y, -Z, W );

		/// <summary>Discard W, return XYX components of this quaternion as Vector3</summary>
		public Vector3 vector3 => new Vector3( X, Y, Z );

		/// <summary>Transform a 3D vector with this quaternion.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Vector3 transformVector( Vector3 src )
		{
			// https://math.stackexchange.com/a/535223/467444
			Quaternion q = new Quaternion( src, 0 );
			return ( this * q * conjugated ).vector3;
		}
	}
}