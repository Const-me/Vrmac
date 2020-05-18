using Diligent.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Vrmac.Draw
{
	/// <summary>3x2 2D transfomr matrix</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Matrix: IEquatable<Matrix>
	{
		/// <summary>Components of 2x2 rotation + scale matrix</summary>
		public float m11, m12, m21, m22;
		/// <summary>Translation component of the matrix</summary>
		public float dx, dy;

		static Matrix buildIdentity()
		{
			Matrix m = new Matrix();
			m.m11 = 1;
			m.m22 = 1;
			return m;
		}
		/// <summary>Identity matrix</summary>
		public static Matrix identity { get; } = buildIdentity();

		/// <summary>Multiply two matrices</summary>
		public static Matrix operator *( Matrix a, Matrix b )
		{
			Matrix r = new Matrix();
			r.m11 = a.m11 * b.m11 + a.m12 * b.m21;
			r.m12 = a.m11 * b.m12 + a.m12 * b.m22;
			r.m21 = a.m21 * b.m11 + a.m22 * b.m21;
			r.m22 = a.m21 * b.m12 + a.m22 * b.m22;
			r.dx = a.dx * b.m11 + a.dy * b.m21 + b.dx;
			r.dy = a.dx * b.m12 + a.dy * b.m22 + b.dy;
			return r;
		}

		/// <summary>Create a translation-only matrix</summary>
		public static Matrix createTranslation( Vector2 vec )
		{
			Matrix r = identity;
			r.dx = vec.X;
			r.dy = vec.Y;
			return r;
		}

		/// <summary>Create a translation-only matrix</summary>
		public static Matrix createTranslation( float x, float y )
		{
			Matrix r = identity;
			r.dx = x;
			r.dy = y;
			return r;
		}

		/// <summary>Create scaling matrix</summary>
		public static Matrix createScale( float mul )
		{
			Matrix r = new Matrix();
			r.m11 = r.m22 = mul;
			return r;
		}

		/// <summary>Determines whether two object instances are equal</summary>
		public override bool Equals( object obj )
		{
			if( obj is sStrokeStyle ss )
				return Equals( ss );
			return false;
		}
		/// <summary>Determines whether two instances are equal</summary>
		public bool Equals( Matrix p )
		{
			return m11 == p.m11 && m12 == p.m12 && m21 == p.m21 && m22 == p.m22 && dx == p.dx && dy == p.dy;
		}
		/// <summary>Compute hash code</summary>
		public override int GetHashCode()
		{
			return HashCode.Combine( m11, m12, m21, m22, dx, dy );
		}
		/// <summary>Compare for equality</summary>
		public static bool operator ==( Matrix lhs, Matrix rhs )
		{
			return lhs.Equals( rhs );
		}
		/// <summary>Compare for inequality</summary>
		public static bool operator !=( Matrix lhs, Matrix rhs )
		{
			return !( lhs.Equals( rhs ) );
		}

		/// <summary>Create rotation matrix around [ 0, 0 ]</summary>
		public static Matrix createRotation( float angle )
		{
			MathHelper.sinCos( angle, out float sin, out float cos );
			Matrix m = new Matrix();
			m.m11 = cos;
			m.m12 = sin;
			m.m21 = -sin;
			m.m22 = cos;
			return m;
		}

		/// <summary>Create rotation matrix around specified center of rotation</summary>
		public static Matrix createRotation( float angle, float x, float y )
		{
			MathHelper.sinCos( angle, out float sin, out float cos );
			Matrix m = new Matrix();
			// https://math.stackexchange.com/q/2093314/467444 is incorrect.
			// The formula below matches the output of D2D1MakeRotateMatrix WinAPI.
			// Likely reason for that, left versus right hand coordinate systems.
			// Using same conventions as Windows.
			m.m11 = cos;
			m.m12 = sin;
			m.m21 = -sin;
			m.m22 = cos;
			m.dx = -x * cos + y * sin + x;
			m.dy = -x * sin - y * cos + y;
			return m;
		}

		/// <summary>Create rotation matrix around specified center of rotation</summary>
		public static Matrix createRotation( float angle, Vector2 center )
		{
			return createRotation( angle, center.X, center.Y );
		}

		/// <summary>Create rotation matrix directly from values in a vector</summary>
		public static Matrix createRotation( ref Vector4 rotationValues )
		{
			Matrix m = new Matrix();
			m.m11 = rotationValues.X;
			m.m12 = rotationValues.Y;
			m.m21 = rotationValues.Z;
			m.m22 = rotationValues.W;
			return m;
		}

		/// <summary></summary>
		public override string ToString()
		{
			if( this == identity )
				return "[identity]";
			return $"rotation [ { m11 }, { m12 }, { m21 }, { m22 } ], translation [ { dx }, { dy } ]";
		}

		/// <summary>Transform a point with this matrix</summary>
		public Vector2 transformPoint( Vector2 v )
		{
			float x = v.X * m11 + v.Y * m21 + dx;
			float y = v.X * m12 + v.Y * m22 + dy;
			return new Vector2( x, y );
		}

		/// <summary>Transform a vector with this matrix</summary>
		/// <remarks>Same as <see cref="transformPoint" /> but doesn’t add translation.</remarks>
		public Vector2 transformVector( Vector2 v )
		{
			float x = v.X * m11 + v.Y * m21;
			float y = v.X * m12 + v.Y * m22;
			return new Vector2( x, y );
		}

		/// <summary>Transform an axis-aligned rectangle with this matrix, return bounding box of the transformed rectangle.</summary>
		public Rect transformRectangle( ref Rect rect )
		{
			Span<Vector2> vertices = stackalloc Vector2[ 4 ];
			rect.listVertices( vertices );
			for( int i = 0; i < 4; i++ )
				vertices[ i ] = transformPoint( vertices[ i ] );
			return Rect.createBoundingBox( vertices );
		}

		/// <summary>Extract scaling component of this matrix</summary>
		public Vector2 getScaling()
		{
			Vector2 x = new Vector2( m11, m21 );
			Vector2 y = new Vector2( m12, m22 );
			return new Vector2( x.Length(), y.Length() );
		}

		/// <summary>Rotation, flip and scale components of the matrix, as a Vector4 structure</summary>
		public Vector4 rotationMatrix => new Vector4( m11, m12, m21, m22 );

		/// <summary>Get or reset the translation component of this matrix</summary>
		public Vector2 translation
		{
			get { return new Vector2( dx, dy ); }
			set { dx = value.X; dy = value.Y; }
		}

		/// <summary>Built a matrix that scales + translates the content to the inside of the outer rectangle</summary>
		public static Matrix createViewbox( Rect outer, Rect content )
		{
			Vector2 sizeContent = content.size;
			Vector2 sizeViewport = outer.size;
			float scaling;
			if( sizeContent.X * sizeViewport.Y >= sizeContent.Y * sizeViewport.X )
			{
				// left-right to fit the VP, center vertically
				scaling = sizeViewport.X / sizeContent.X;
			}
			else
			{
				// top-bottom to fit the VP, center horizontally
				scaling = sizeViewport.Y / sizeContent.Y;
			}

			Matrix m = createScale( scaling );
			Vector2 scaledCenter = m.transformPoint( content.center );
			m.translation = outer.center - scaledCenter;
			return m;
		}

		/// <summary>Built a matrix that scales + translates the content to the inside of the rectangle that starts at [ 0, 0 ] and has the specified size</summary>
		public static Matrix createViewbox( Vector2 sizeViewport, Rect content )
		{
			return createViewbox( new Rect( Vector2.Zero, sizeViewport ), content );
		}

		/// <summary>Determinant of the rotation + scaling portion of the matrix.</summary>
		/// <remarks>By the way, the sign of this value tells whether or not the matrix has flipping component.</remarks>
		public float determinant => m11 * m22 - m12 * m21;

		/// <summary>Funfact: mathematicians think only square matrices can be inverted.</summary>
		public Matrix invert()
		{
			// https://www.chilimath.com/lessons/advanced-algebra/inverse-of-a-2x2-matrix/
			float mul = 1.0f / determinant;
			Matrix res = new Matrix();
			res.m11 = m22 * mul;
			res.m22 = m11 * mul;
			res.m21 = -m21 * mul;
			res.m12 = -m12 * mul;

			// The original matrix translated [ 0, 0 ] into [ this.dx, this.dy ]
			// The inverted one should transform [ this.dx, this.dy ] into [ 0, 0 ], that's why `-`
			res.translation = -res.transformPoint( translation );
			return res;
		}
	}
}