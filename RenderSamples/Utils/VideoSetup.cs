using Vrmac.ModeSet;
using System;

namespace RenderSamples.Utils
{
	class VideoSetup: iVideoSetup
	{
		readonly eDrmFormat? format;

		public VideoSetup( eDrmFormat? format = null )
		{
			this.format = format;
		}

		int iVideoSetup.pickDrmFormat( sDrmFormat[] available, int availableCount )
		{
			Console.WriteLine( "iVideoSetup.pickRgbFormat, following is available: {0}", string.Join( ", ", available ) );
			return available.findIndex( df => df.drm == format );
		}

		int iVideoSetup.pickEglConfig( sEglConfig[] configs, int configsCount )
		{
			Console.WriteLine( "iVideoSetup.pickEglConfig, following is available:\n\t{0}",
				string.Join( ";\n\t", configs ) );
			return -1;
		}
	}
}