using Diligent.Graphics;
using System.Runtime.InteropServices;

namespace Vrmac.Draw.Shaders
{
	/// <summary>Metadata for a slice of vertices produced by a single draw command.</summary>
	[StructLayout( LayoutKind.Sequential )]
	struct sDrawCallData
	{
		/// <summary>2x2 rotate/flip/scale matrix</summary>
		public Vector4 rotation;

		/// <summary>XY is translation column of the 3x2 matrix. Z is depth value. For fat strokes W is VAA multiplier. For text W is size of physical pixel in source mesh coordinate system.</summary>
		public Vector4 translationAndVaa;

		/// <summary>X is draw call information: mesh, brush, VAA type. Meaning of the rest of them depends on X.</summary>
		/// <remarks>For many draw commands including text runs, Y and Z are foreground and background colors, indices in the palette texture.
		/// For thin strokes YZ is FP16-packed stroke color.
		/// For sprites Y is layer of the texture atlas in lower byte, foreground color in the higher 3 bytes; ZW is texture rectangle packed into 1.16 fixed point.
		/// </remarks>
		public Int4 miscIntegers;
	}
}