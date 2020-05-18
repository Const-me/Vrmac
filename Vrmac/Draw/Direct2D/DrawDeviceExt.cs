using Vrmac.Draw;
using Vec4 = Diligent.Graphics.Vector4;

namespace Vrmac.Direct2D
{
	/// <summary>Extension methods for iDrawDevice</summary>
	public static class DrawDeviceExt
	{
		/// <summary>Create solid color brush from premultiplied RGBA values</summary>
		public static iSolidColorBrush createSolidBrush( this iDrawDevice device, Vec4 color )
		{
			return device.createSolidColorBrush( ref color );
		}

		/// <summary>Create solid color brush from color string like "blue" or "#ffccff44"</summary>
		public static iSolidColorBrush createSolidBrush( this iDrawDevice device, string colorString )
		{
			Vec4 color = Color.parseNonPremultiplied( colorString );

			return device.createSolidColorBrush( ref color );
		}

		/// <summary>Upload path data to VRAM</summary>
		public static iPathGeometry createPathGeometry( this iDrawDevice device, iPathData data )
		{
			return data.createPathGeometry( device );
		}
	}
}