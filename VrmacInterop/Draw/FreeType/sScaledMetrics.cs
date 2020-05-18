using System.Runtime.InteropServices;

namespace Vrmac.FreeType
{
	/// <summary></summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sScaledMetrics
	{
		/// <summary>Line height in pixels</summary>
		public readonly int lineHeight;
		/// <summary>Baseline position in pixels</summary>
		public readonly int baselinePosition;
		/// <summary>Maximum horizontal advance, in these FreeType's fixed point 26.6 pixels</summary>
		public readonly int maxAdvance;

		/// <summary>Maximum horizontal advance in pixels</summary>
		public int maxAdvancePixels => ( maxAdvance + 63 ) / 64;
	}
}