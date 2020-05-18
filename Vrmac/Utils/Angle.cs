using System;
using System.Runtime.CompilerServices;

namespace Vrmac
{
	/// <summary>Utility structure to keep and increment/decrement angle in radians.</summary>
	public struct Angle
	{
		float value;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		Angle( float value )
		{
			this.value = value;
			normalize();
		}

		/// <summary>Construct from radians</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Angle radians( float r )
		{
			return new Angle( r );
		}

		const float mulToRad = MathF.PI / 180;

		/// <summary>Construct from degrees</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Angle degrees( float r )
		{
			return new Angle( r * mulToRad );
		}

		/// <summary>Implicit cast to float radians</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static implicit operator float( Angle a ) => a.value;

		const float Pi2 = MathF.PI * 2;
		const float Pi2Inv = 1.0f / Pi2;

		/// <summary>Add angles</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Angle operator +( Angle a, Angle b ) => new Angle( a.value + b.value );

		/// <summary>Subtract angles</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Angle operator -( Angle a, Angle b ) => new Angle( a.value - b.value );

		/// <summary>Apply rotation</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public void rotate( float angularVelocity, float deltaTime )
		{
			value += angularVelocity * deltaTime;
			normalize();
		}

		// This is really why we need this structure - to prevent glitches caused by 32-bit float precision issues
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		void normalize()
		{
			if( MathF.Abs( value ) > Pi2 )
			{
				float rotations = MathF.Truncate( value * Pi2Inv );
				value -= rotations * Pi2;
			}
		}
	}
}