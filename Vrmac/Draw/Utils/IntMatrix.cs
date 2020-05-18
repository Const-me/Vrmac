using System;
using System.Runtime.CompilerServices;
using Matrix2D = Vrmac.Draw.Matrix;
using Matrix3D = Diligent.Graphics.Matrix;
using Vector2 = Diligent.Graphics.Vector2;
using Vector4 = Diligent.Graphics.Vector4;

namespace Vrmac.Draw
{
	/// <summary>Represents rotations by multiples of 90 degrees, and/or mirror transforms. It only has 8 distinct values.</summary>
	public partial struct IntMatrix
	{
		/// <summary>Identity matrix</summary>
		public static IntMatrix identity => new IntMatrix( (byte)0 );
		/// <summary>Horizontal flip matrix</summary>
		public static IntMatrix horizontalFlip => new IntMatrix( 4 );
		/// <summary>Vertical flip matrix</summary>
		public static IntMatrix verticalFlip => new IntMatrix( 6 );

		/// <summary>Multiply two matrices</summary>
		public static IntMatrix operator *( IntMatrix a, IntMatrix b )
		{
			uint lhs = multiplyTable[ a.value ];
			uint index = ( lhs >> ( b.value * 3 ) ) & 7;
			return new IntMatrix( (byte)index );
		}

		/// <summary>Invert this matrix</summary>
		public IntMatrix invert()
		{
			// The lookup table is constexpr; this method doesn't reference memory at all, the lookup table becomes an immediate value encoded in the instruction.
			uint v = invertTable >> ( 3 * value );
			return new IntMatrix( (byte)( v & 7 ) );
		}

		/// <summary>Construct from ModeSet.eRotationMode enum; Unspecified results in identity matrix.</summary>
		public IntMatrix( ModeSet.eRotationMode rotation )
		{
			if( rotation == ModeSet.eRotationMode.Unspecified )
				value = 0;
			else
				value = (byte)( rotation - 1 );
		}

		/// <summary>Create a rotation matrix from angle in degrees. The angle must be a multiple of 90 degrees, otherwise this method will throw an exception.</summary>
		public static IntMatrix fromDegrees( int degrees )
		{
			// https://stackoverflow.com/q/1082917/126995
			if( 0 != degrees % 90 )
				throw new ArgumentException( "The angle must be a multiple of 90 degrees" );
			int index = ( degrees / 90 ) & 3;
			return new IntMatrix( (byte)index );
		}

		/// <summary>True if this matrix has a mirroring component.</summary>
		public bool hasMirrorComponent => 0 != ( value & 4 );

		/// <summary>Return a Vector4 with the components of the matrix. Useful for sending the matrix to vertex shaders.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public Vector4 asVector4()
		{
			MatrixValues mv = allMatrices[ value ];
			return new Vector4( mv.m00, mv.m01, mv.m10, mv.m11 );
		}

		/// <summary>Cast the matrix to 3D rotation matrix</summary>
		public static implicit operator Matrix3D( IntMatrix im )
		{
			MatrixValues mv = allMatrices[ im.value ];
			Matrix3D m = Matrix3D.Identity;
			m.M11 = mv.m00;
			m.M12 = mv.m01;
			m.M21 = mv.m10;
			m.M22 = mv.m11;
			return m;
		}

		/// <summary>Cast the matrix to 2D rotation matrix</summary>
		/// <remarks>The result of this operator may transform coordinates into negative half-spaces.</remarks>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static implicit operator Matrix2D( IntMatrix im )
		{
			MatrixValues mv = allMatrices[ im.value ];
			Matrix2D m = new Matrix2D();
			m.m11 = mv.m00;
			m.m12 = mv.m01;
			m.m21 = mv.m10;
			m.m22 = mv.m11;
			return m;
		}

		/// <summary>Same as operator Vrmac.Draw.Matrix, but rotates/flips around the center of the rectangle, instead of [ 0, 0 ].</summary>
		public Matrix2D as2d( Vector2 completeSize )
		{
			Matrix2D rotation = this;
			Vector2 transformedSize = this * completeSize;
			rotation.dx = MathF.Max( 0, -transformedSize.X );
			rotation.dy = MathF.Max( 0, -transformedSize.Y );
			return rotation;
		}

		/// <summary>Transform 2D vector with this matrix</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Vector2 operator *( IntMatrix im, Vector2 vec )
		{
			MatrixValues mv = allMatrices[ im.value ];
			float x = vec.X * mv.m00 + vec.Y * mv.m01;
			float y = vec.X * mv.m10 + vec.Y * mv.m11;
			return new Vector2( x, y );
		}

		/// <summary>Transform integer size with this matrix</summary>
		public static CSize operator *( IntMatrix im, CSize size ) =>
			( 0 == ( im.value & 1 ) ) ? size : new CSize( size.cy, size.cx );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static CPoint transformPoint( MatrixValues mv, CPoint pt )
		{
			int x = pt.x * mv.m00 + pt.y * mv.m01;
			int y = pt.x * mv.m10 + pt.y * mv.m11;
			return new CPoint( x, y );
		}

		/// <summary>Transform integer rectangle with this matrix</summary>
		/// <remarks>The rectangle is assumed to be within [ 0, 0, outerSize.cx, outerSize.cy ] outer rectangle.
		/// The output coordinates are relative to the transformed outer rectangle.
		/// Mirroring component of the matrix is ignored, on output you'll always get a good rectangle with left &lt; right and top &lt; bottom.</remarks>
		public CRect transformRect( CRect rect, CSize outerSize )
		{
			MatrixValues mv = allMatrices[ value ];

			CPoint topLeft = transformPoint( mv, rect.topLeft );
			CPoint bottomRight = transformPoint( mv, rect.bottomRight );
			CPoint transformedSize = transformPoint( mv, new CPoint( outerSize.cx, outerSize.cy ) );

			int minX = Math.Min( 0, transformedSize.x );
			int minY = Math.Min( 0, transformedSize.y );

			return new CRect
				(
					Math.Min( topLeft.x, bottomRight.x ) - minX,
					Math.Min( topLeft.y, bottomRight.y ) - minY,
					Math.Max( topLeft.x, bottomRight.x ) - minX,
					Math.Max( topLeft.y, bottomRight.y ) - minY
				);
		}

		// ==== C# boilerplate ==== 

		/// <summary>Returns a value indicating whether this instance is equal to a specified object</summary>
		public override bool Equals( object obj )
		{
			if( obj is IntMatrix im )
				return value == im.value;
			return false;
		}

		/// <summary>Returns a value indicating whether this instance and a specified IntMatrix object represent the same value</summary>
		public bool Equals( IntMatrix im ) => value == im.value;

		/// <summary>Returns the hash code for this instance</summary>
		public override int GetHashCode() => value.GetHashCode();

		/// <summary>Compare for equality</summary>
		public static bool operator ==( IntMatrix lhs, IntMatrix rhs ) => lhs.value == rhs.value;

		/// <summary>Compare for inequality</summary>
		public static bool operator !=( IntMatrix lhs, IntMatrix rhs ) =>lhs.value != rhs.value;

		internal MatrixValues matrixValues => allMatrices[ value ];

		// ==== Implementation ====

		// The range of the value is [ 0 .. 7 ]: 4 rotations, and 4 mirrors of them
		readonly byte value;

		internal IntMatrix( byte v )
		{
			value = v;
		}

		internal struct MatrixValues
		{
			public readonly sbyte m00, m01, m10, m11;

			public MatrixValues( sbyte a00, sbyte a01, sbyte a10, sbyte a11 )
			{
				m00 = a00;
				m01 = a01;
				m10 = a10;
				m11 = a11;
			}
		}

		/// <summary>Human-friendly name of this matrix</summary>
		public override string ToString()
		{
			return matrixNames[ value ];
		}

		// The rest of the implementation is T4 generated code; note this struct is partial.
	}
}