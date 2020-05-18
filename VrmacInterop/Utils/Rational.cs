// TODO: comment them
#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Non-negative rational number, stored as ( numerator / denominator ) where both are unsigned 32-bit integers</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Rational
	{
		public uint numerator, denominator;

		public bool isValid()
		{
			return 0 != denominator;
		}

		public double asDouble()
		{
			if( 0 == denominator )
				return 0 == numerator ? double.NaN : double.PositiveInfinity;

			return (double)numerator / denominator;
		}

		/// <summary>Print a value with the specified unit.</summary>
		public string print( string unit )
		{
			if( denominator == 1 )
				return $"{numerator} {unit}";   // It's an integer
			if( denominator == 0 )
			{
				return $"invalid ({ numerator }/{ denominator } {unit})";
			}
			return $"{ asDouble() } {unit} ({ numerator }/{ denominator })";
		}

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			return $"{ numerator }/{ denominator }";
		}
	}
}