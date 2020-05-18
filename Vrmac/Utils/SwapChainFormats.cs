#pragma warning disable CS1591  // CS1591: Missing XML comment for publicly visible type or member
using Diligent.Graphics;
using System;

namespace Vrmac
{
	/// <summary>Format of the swap chain’s buffers</summary>
	public struct SwapChainFormats
	{
		/// <summary>Color buffer format</summary>
		public readonly TextureFormat color;
		/// <summary>Depth buffer format</summary>
		public readonly TextureFormat depth;
		/// <summary>MSAA</summary>
		public readonly byte sampleCount;

		internal SwapChainFormats( TextureFormat color, TextureFormat depth, byte sampleCount )
		{
			this.color = color;
			this.depth = depth;
			this.sampleCount = sampleCount;
		}

		public override string ToString()
		{
			if( sampleCount == 1 )
				return $"RGB { color }, depth { depth }, 1 sample";
			else
				return $"RGB { color }, depth { depth }, { sampleCount } samples";
		}

		public override int GetHashCode()
		{
			return HashCode.Combine( color, depth, sampleCount );
		}
		public override bool Equals( object obj )
		{
			if( obj is SwapChainFormats scf )
				return Equals( scf );
			return false;
		}
		public bool Equals( SwapChainFormats p )
		{
			return color == p.color && depth == p.depth && sampleCount == p.sampleCount;
		}
		public static bool operator ==( SwapChainFormats lhs, SwapChainFormats rhs )
		{
			return lhs.Equals( rhs );
		}
		public static bool operator !=( SwapChainFormats lhs, SwapChainFormats rhs )
		{
			return !( lhs.Equals( rhs ) );
		}
	}
}