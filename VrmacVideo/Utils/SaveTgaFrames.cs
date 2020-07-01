using System;
using System.IO;

namespace VrmacVideo
{
	sealed class SaveTgaFrames: IDisposable
	{
		MappedOutput[] mapped;

		public SaveTgaFrames( MappedOutput[] mapped )
		{
			this.mapped = mapped;

			DirectoryInfo di = new DirectoryInfo( destFolder );
			foreach( FileInfo file in di.GetFiles() )
				file.Delete();
		}

		int nextFrame = 0;
		static readonly string destFolder = @"/home/pi/z/Temp/Frames";
		string framePath()
		{
			int sn = nextFrame++;
			string fileName = string.Format( "frame-{0:D4}.tga", sn );
			string path = Path.Combine( destFolder, fileName );
			return path;
		}

		public void saveLuma( int bufferIndex )
		{
			mapped[ bufferIndex ].saveLuma( framePath() );
		}

		public void Dispose()
		{
			if( null == mapped )
				return;
			foreach( var m in mapped )
				m.finalize();
			mapped = null;
		}
	}
}