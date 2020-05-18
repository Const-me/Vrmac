using Diligent.Graphics;
using System.Runtime.InteropServices;

namespace Vrmac.ModeSet
{
	/// <summary>Supported pixel format of the display.</summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct sDrmFormat
	{
		/// <summary>Linux kernel format</summary>
		/// <seealso href="https://en.wikipedia.org/wiki/Direct_Rendering_Manager" />
		public eDrmFormat drm;

		/// <summary>Diligent's texture format. More often than not, the value is <see cref="TextureFormat.Unknown" />.</summary>
		/// <remarks>Majority of DRM native formats aren't supported by Diligent textures, doesn't mean it can't render to them, GLES may render into some other one, GPU will convert on the fly.</remarks>
		public TextureFormat diligent;

		/// <summary>Returns a string that represents the current object.</summary>
		public override string ToString()
		{
			if( diligent == TextureFormat.Unknown )
				return drm.ToString();
			return $"{ drm } ({ diligent })";
		}
	}
}