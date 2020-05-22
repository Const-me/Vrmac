using System;
using System.Runtime.CompilerServices;

namespace Vrmac.Draw
{
	enum eBrushType: byte
	{
		Null,
		Opaque,
		Transparent,
	};

	struct SolidColorData: IEquatable<SolidColorData>
	{
		public readonly int paletteIndex;
		public readonly eBrushType brushType;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public SolidColorData( int paletteIndex, eBrushType brushType )
		{
			this.paletteIndex = paletteIndex;
			this.brushType = brushType;
		}

		/// <summary>Compare for equality</summary>
		public bool Equals( SolidColorData other )
		{
			return paletteIndex == other.paletteIndex;
		}

		/// <summary>Compare for equality</summary>
		public override bool Equals( object other )
		{
			return other is SolidColorData scd && Equals( scd );
		}

		/// <summary>Compute hash code</summary>
		public override int GetHashCode()
		{
			return HashCode.Combine( paletteIndex, typeof( SolidColorData ) );
		}

		/// <summary>Compare for equality</summary>
		public static bool operator ==( SolidColorData lhs, SolidColorData rhs )
		{
			return lhs.Equals( rhs );
		}

		/// <summary>Compare for inequality</summary>
		public static bool operator !=( SolidColorData lhs, SolidColorData rhs )
		{
			return !( lhs.Equals( rhs ) );
		}
	};
}