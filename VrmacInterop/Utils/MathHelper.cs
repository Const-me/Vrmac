using System.Runtime.CompilerServices;

namespace Vrmac
{
	/// <summary>Globally available math routines</summary>
	public static class MathHelper
	{
		const float XM_1DIV2PI = 0.159154943f;
		const float XM_2PI = 6.283185307f;
		const float XM_PIDIV2 = 1.570796327f;
		const float XM_PI = 3.141592654f;

		/// <summary>Computes both the sine and cosine of a radian angle</summary>
		/// <seealso href="https://github.com/microsoft/DirectXMath/blob/83634c742a85d1027765af53fbe79506fd72e0c3/Inc/DirectXMathMisc.inl#L2237-L2284" />
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void sinCos( float Value, out float sin, out float cos )
		{
			// Map Value to y in [-pi,pi], x = 2*pi*quotient + remainder.
			float quotient = XM_1DIV2PI * Value;
			if( Value >= 0.0f )
				quotient = (int)( quotient + 0.5f );
			else
				quotient = (int)( quotient - 0.5f );

			float y = Value - XM_2PI * quotient;

			// Map y to [-pi/2,pi/2] with sin(y) = sin(Value).
			float sign;
			if( y > XM_PIDIV2 )
			{
				y = XM_PI - y;
				sign = -1.0f;
			}
			else if( y < -XM_PIDIV2 )
			{
				y = -XM_PI - y;
				sign = -1.0f;
			}
			else
				sign = +1.0f;

			float y2 = y * y;

			// 11-degree minimax approximation
			sin = ( ( ( ( ( -2.3889859e-08f * y2 + 2.7525562e-06f ) * y2 - 0.00019840874f ) * y2 + 0.0083333310f ) * y2 - 0.16666667f ) * y2 + 1.0f ) * y;

			// 10-degree minimax approximation
			float p = ( ( ( ( -2.6051615e-07f * y2 + 2.4760495e-05f ) * y2 - 0.0013888378f ) * y2 + 0.041666638f ) * y2 - 0.5f ) * y2 + 1.0f;
			cos = sign * p;
		}

		/// <summary>Linearly interpolates between two values.</summary>
		/// <param name="value1">Source value.</param>
		/// <param name="value2">Destination value.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
		/// <returns>Interpolated value.</returns> 
		/// <remarks>This method performs the linear interpolation based on the following formula:
		/// <code>value1 + (value2 - value1) * amount</code>.
		/// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
		/// See <see cref="LerpPrecise"/> for a less efficient version with more precision around edge cases.
		/// </remarks>
		public static float Lerp( float value1, float value2, float amount )
		{
			return value1 + ( value2 - value1 ) * amount;
		}

		/// <summary>Linearly interpolates between two values.
		/// This method is a less efficient, more precise version of <see cref="Lerp"/>.
		/// See remarks for more info.</summary>
		/// <param name="value1">Source value.</param>
		/// <param name="value2">Destination value.</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
		/// <returns>Interpolated value.</returns>
		/// <remarks>This method performs the linear interpolation based on the following formula:
		/// <code>((1 - amount) * value1) + (value2 * amount)</code>.
		/// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
		/// This method does not have the floating point precision issue that <see cref="Lerp"/> has.
		/// i.e. If there is a big gap between value1 and value2 in magnitude (e.g. value1=10000000000000000, value2=1),
		/// right at the edge of the interpolation range (amount=1), <see cref="Lerp"/> will return 0 (whereas it should return 1).
		/// This also holds for value1=10^17, value2=10; value1=10^18,value2=10^2... so on.
		/// For an in depth explanation of the issue, see below references:
		/// Relevant Wikipedia Article: https://en.wikipedia.org/wiki/Linear_interpolation#Programming_language_support
		/// Relevant StackOverflow Answer: http://stackoverflow.com/questions/4353525/floating-point-linear-interpolation#answer-23716956
		/// </remarks>
		public static float LerpPrecise( float value1, float value2, float amount )
		{
			return ( ( 1 - amount ) * value1 ) + ( value2 * amount );
		}
	}
}