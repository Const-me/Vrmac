using Diligent.Graphics;
using System.Runtime.InteropServices;

namespace Vrmac
{
	/// <summary>Screen information</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sScreen
	{
		/// <summary>Screen resolution, probably in pixels</summary>
		public CSize sizePixels;
		/// <summary>Screen size in millimeters</summary>
		public Vector2 sizeMillimeters;

		/// <summary>String representation of this object</summary>
		public override string ToString()
		{
			return $"{ sizePixels.cx } × { sizePixels.cy } pixels, { sizeMillimeters.X } × { sizeMillimeters.Y } mm";
		}
	};
}