using Diligent.Graphics;
using System.IO;
using Vrmac.Utils;

namespace Vrmac.MediaEngine
{
	sealed class SavePngFrames
	{
		int nextFrame = 0;
		static readonly string destFolder = @"/home/pi/z/Temp/Frames";

		string framePath()
		{
			int sn = nextFrame++;
			string fileName = string.Format( "frame-{0:D4}.png", sn );
			string path = Path.Combine( destFolder, fileName );
			return path;
		}

		public void saveFrame( IRenderDevice device, IDeviceContext context, ITexture texture )
		{
			ScreenGrabber.saveTexture( device, context, texture, framePath() );
		}
	}
}